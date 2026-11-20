using System;
using System.Collections.Generic;
using Godot;
using Voidwalker.Core;
using Voidwalker.Hub;
using Voidwalker.Ui;
using UiKit = Voidwalker.Ui.Ui;

namespace Voidwalker.Adventure;

/// <summary>
/// The adventure on screen: it steps <see cref="RiftRun"/> (the rules, in the browser's 1280x720 units) and draws it
/// in whole game pixels, scaled to fit the game area (k is 0.5 in a 640x360 area, less on a phone's shorter one).
/// Phones: drag anywhere to move (a floating stick under the thumb) while the deck fires on its own at the nearest
/// threat. PC: WASD or the arrows to move, the mouse to aim, the left button to fire, Q for auto-fire. Escape, P or
/// Space pauses; the pause screen can leave the rift (a fall). The run banks its void energy into the save when it
/// ends, and a tap returns to the Rift Altar.
/// Every figure and effect is a baked pixelfx sheet (tools/art/bake_rift_fx.py, sources in tools/art/fx/): Ybasogs,
/// rocks, the slab, bolts (painted per card by canvas_fx_bolt_tint), globs, hit sparks, bursts and breaks.
/// Checks: <c>-- --shot=out.png</c> saves the frame after <c>--shot-at=</c> seconds (default 4; <c>end</c>: on the end
/// screen; <c>pick</c>: once the first pick is open, which autoplay then leaves open) and quits; <c>-- --return-at-end</c> goes back to the Altar after the end (the hub then takes the shot);
/// <c>-- --autoplay</c> plays with auto-aim and a drifting blob; <c>-- --speed=6</c> runs time faster.
/// <example><code>GetTree().ChangeSceneToFile(RiftScene.ScenePath); // from the Gate, after saving</code></example>
/// </summary>
public partial class RiftScene : Node2D
{
	/// <summary>The scene the Gate opens.</summary>
	public const string ScenePath = "res://src/adventure/Rift.tscn";
	private const string HubPath = "res://src/hub/Hub.tscn";

	/// <summary>Screen and control tuning, and which kinds are migrated.</summary>
	[Export] public RiftConfig? Config { get; set; }
	/// <summary>The hub's tuning, for the sky's stars.</summary>
	[Export] public HubConfig? Sky { get; set; }

	private RiftConfig _cfg = null!;
	private Session _session = null!;
	private RiftRun _run = null!;
	private LobbyArt _art = null!;
	private RiftSheets _sheets = null!;
	private GameData _data = null!;
	private Texture2D _voidBall = null!;
	private Vector2 _voidBallAnchor;
	private string _formSprite = "form-blob";
	private ColorRect _sky = null!;
	private float _k = 0.5f;
	private Vector2 _o, _area;
	private double _t;

	// input
	private bool _touch, _mouseFire, _autoFire, _paused, _saved;
	private int _stickIdx = -1;
	private Vector2 _stickO, _stickAt, _mouse;
	private bool _autoplay;
	private string? _shotPath;
	private double _shotAt = 4, _endT;
	private bool _shotAtEnd, _returnAtEnd, _shotAtPick;
	private double _pickT = -1;

	// presentation only (never gameplay): floating words, in a fixed pool
	private struct Floater { public float X, Y, Life, Max; public string S; public Color C; public bool Big; }
	private readonly Floater[] _floats = new Floater[40];
	private struct OneShot { public string Sheet; public int Row; public float X, Y; public double T; }
	private readonly OneShot[] _shots = new OneShot[48]; // the baked pixelfx one-shots playing: hits, splashes, bursts, breaks
	private int _shotN;
	private BoltLayer _boltLayer = null!;
	private Vector2 _shake;
	private CanvasLayer? _pickLayer; // the pick panel, while the pick is open
	private int _floatN;
	private readonly Rng _fx = new(911);
	private readonly Dictionary<string, Color> _colors = new();
	private Vector2[] _wallL = Array.Empty<Vector2>(), _wallR = Array.Empty<Vector2>();

	// buttons, in game pixels (laid out on resize)
	private Rect2 _pauseBtn, _resumeBtn, _leaveBtn;

	private static readonly Color Edge = new("#7b5bdf"), Outside = new("#07040d");

	/// <inheritdoc/>
	public override void _Ready()
	{
		_cfg = Config ?? new RiftConfig();
		var data = _data = GameData.Parse(FileAccess.GetFileAsString(Session.DataPath));
		_session = Session.Load(data, ProjectSettings.GlobalizePath(Session.SavePath));
		_art = new LobbyArt(LobbyArt.RiftDir);
		var lobby = new LobbyArt(); // the collectible void energy is the hub's own void ball
		_voidBall = lobby.Texture("void-ball"); _voidBallAnchor = lobby.Anchor("void-ball");
		_sheets = new RiftSheets();
		_run = new RiftRun(data, _session.Rules, _cfg.Settings());
		if (_run.Form != null && _art.Has("form-" + data.Natures[_run.Form].Form)) _formSprite = "form-" + data.Natures[_run.Form].Form;

		var back = new CanvasLayer { Layer = -10, Name = "Sky" };
		AddChild(back);
		_sky = new ColorRect { Material = Voidwalker.Hub.Hub.SkyMaterial(Sky ?? new HubConfig()), MouseFilter = Control.MouseFilterEnum.Ignore };
		back.AddChild(_sky);

		foreach (var arg in OS.GetCmdlineUserArgs())
		{
			if (arg.StartsWith("--shot=")) _shotPath = arg["--shot=".Length..];
			else if (arg == "--shot-at=end") _shotAtEnd = true;
			else if (arg == "--shot-at=pick") _shotAtPick = true;
			else if (arg.StartsWith("--shot-at=") && double.TryParse(arg["--shot-at=".Length..], System.Globalization.CultureInfo.InvariantCulture, out var at)) _shotAt = at;
			else if (arg == "--autoplay") _autoplay = true;
			else if (arg == "--return-at-end") _returnAtEnd = true;
			else if (arg.StartsWith("--speed=") && double.TryParse(arg["--speed=".Length..], System.Globalization.CultureInfo.InvariantCulture, out var sp)) Engine.TimeScale = sp;
		}
		_boltLayer = new BoltLayer { Scene = this, Material = new ShaderMaterial { Shader = GD.Load<Shader>("res://src/shaders/canvas_fx_bolt_tint.gdshader") } };
		AddChild(_boltLayer);
		GetViewport().SizeChanged += Layout;
		Layout();
	}

	private void Layout()
	{
		_area = GetViewportRect().Size;
		_k = Mathf.Min(_area.Y / (float)RiftRun.H, _area.X / (float)(RiftRun.RR - RiftRun.RL));
		float fw = (float)(RiftRun.RR - RiftRun.RL) * _k;
		_o = new Vector2(Mathf.Floor((_area.X - fw) / 2 - (float)RiftRun.RL * _k), Mathf.Floor((_area.Y - (float)RiftRun.H * _k) / 2));
		_sky.Size = _area;
		int rows = (int)_area.Y + 1;
		_wallL = new Vector2[rows]; _wallR = new Vector2[rows];
		_pauseBtn = new Rect2(_area.X - 24, 4, 20, 18);
		_resumeBtn = new Rect2(_area.X / 2 - 70, _area.Y / 2 - 4, 140, 26);
		_leaveBtn = new Rect2(_area.X / 2 - 70, _area.Y / 2 + 30, 140, 26);
	}

	private Vector2 ToScreen(double x, double y) => new(Mathf.Round((float)x * _k + _o.X), Mathf.Round((float)y * _k + _o.Y));
	private Color Col(string hex) => _colors.TryGetValue(hex, out var c) ? c : _colors[hex] = new Color(hex);

	// ---- input --------------------------------------------------------------------------------------------

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent e)
	{
		switch (e)
		{
			case InputEventScreenTouch t:
				_touch = true;
				if (t.Pressed)
				{
					if (Tap(t.Position)) return;
					if (_stickIdx < 0) { _stickIdx = t.Index; _stickO = _stickAt = t.Position; }
				}
				else if (t.Index == _stickIdx) _stickIdx = -1;
				break;
			case InputEventScreenDrag d when d.Index == _stickIdx:
				_stickAt = d.Position;
				break;
			case InputEventMouse m when m.Device == InputEvent.DeviceIdEmulation:
				break; // a touch already handled above, seen again as a mouse
			case InputEventMouseMotion mm:
				_mouse = mm.Position; _touch = false;
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.Left } mb:
				_mouse = mb.Position; _touch = false;
				if (mb.Pressed && Tap(mb.Position)) return;
				_mouseFire = mb.Pressed;
				break;
			case InputEventKey { Pressed: true, Echo: false } k:
				if (_run.Outcome != RiftOutcome.None) { if (k.Keycode is Key.Enter or Key.Space or Key.Escape) ReturnToHub(); }
				else if (_run.Phase == RiftPhase.Pick) { } // the pick panel takes the screen
				else if (k.Keycode is Key.Escape or Key.P or Key.Space) _paused = !_paused;
				else if (k.Keycode == Key.Q) { _autoFire = !_autoFire; Say(_autoFire ? "AUTO" : "MANUAL", _run.Player.X, _run.Player.Y - 50, Pal.Bright, false); }
				break;
		}
	}

	// A tap or click on a button, the pause screen or the end screen. Returns whether it was used.
	private bool Tap(Vector2 at)
	{
		if (_run.Outcome != RiftOutcome.None) { ReturnToHub(); return true; }
		if (_paused)
		{
			if (_resumeBtn.HasPoint(at)) _paused = false;
			else if (_leaveBtn.HasPoint(at)) { _paused = false; _run.Abandon(); }
			return true;
		}
		if (_pauseBtn.Grow(4).HasPoint(at)) { _paused = true; _stickIdx = -1; _mouseFire = false; return true; }
		return false;
	}

	private void ReturnToHub()
	{
		if (!_saved) { _session.Save(); _saved = true; }
		GetTree().ChangeSceneToFile(HubPath);
	}

	private RiftInput ReadInput()
	{
		var p = _run.Player;
		double mx = 0, my = 0;
		if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) mx -= 1;
		if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) mx += 1;
		if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) my -= 1;
		if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) my += 1;
		if (_stickIdx >= 0) // the floating stick: the origin follows a thumb that drags past its reach
		{
			var v = _stickAt - _stickO; float len = v.Length();
			if (len > _cfg.StickRadius) { _stickO = _stickAt - v / len * _cfg.StickRadius; len = _cfg.StickRadius; v = _stickAt - _stickO; }
			if (len > _cfg.StickDeadzone) { mx += v.X / _cfg.StickRadius; my += v.Y / _cfg.StickRadius; }
		}
		if (_autoplay) { mx = Math.Clamp((RiftRun.RCX + Math.Sin(_t * 0.5) * 380 - p.X) / 150, -1, 1); my = Math.Clamp((610 - p.Y) / 150, -1, 1); } // sway along the lower rift
		if (_touch || _autoplay) { var aim = _run.AimAtNearest(); return new RiftInput(mx, my, aim, aim != null); } // auto-aim, auto-fire
		double toMouse = Math.Atan2((_mouse.Y - _o.Y) / _k - p.Y, (_mouse.X - _o.X) / _k - p.X);
		return new RiftInput(mx, my, toMouse, _mouseFire || _autoFire);
	}

	// ---- the frame ------------------------------------------------------------------------------------------

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_t += delta;
		if (_run.Outcome == RiftOutcome.None && !_paused)
		{
			// in steps of at most 1/30 s, so a slow frame (or a sped-up check) never jumps a bolt through a Ybasog
			for (double left = Math.Min(delta, 0.5); left > 1e-6 && _run.Outcome == RiftOutcome.None; left -= 1 / 30.0)
			{
				_run.Step(Math.Min(left, 1 / 30.0), ReadInput());
				foreach (var ev in _run.Events) Show(ev);
				_run.Events.Clear();
			}
			if (_run.Phase == RiftPhase.Pick && _autoplay && !_shotAtPick) // checks: buy what scrap affords, then take the first offer
			{
				foreach (var b in _run.Settings.Boosts) _run.Buy(b.Id);
				_run.TakeOffer(0);
			}
			if (_run.Phase == RiftPhase.Pick && _pickLayer == null)
			{
				if (_shotAtPick) _run.Scrap = Math.Max(_run.Scrap, 24); // checks: some scrap to show prices against
				_stickIdx = -1; _mouseFire = false;
				_pickLayer = new CanvasLayer { Layer = 6, Name = "Pick" };
				_pickLayer.AddChild(new RiftPickPanel(_run, _session.Rules, () => { _pickLayer?.QueueFree(); _pickLayer = null; }));
				AddChild(_pickLayer);
			}
			if (_run.Outcome != RiftOutcome.None)
			{
				if (!_saved) { _session.Save(); _saved = true; }
				_endT = _t;
				GD.Print($"rift over: {_run.Outcome}, stage {_run.Stage} wave {_run.WaveN}, {_run.Kills} kills, void {_run.VoidE}, banked {_run.Banked}, at {_run.T:0.0} s");
			}
		}
		if (!_paused) Animate((float)delta);
		QueueRedraw();
		if (_returnAtEnd && _endT > 0 && _t - _endT > 1.5 * Engine.TimeScale) { Engine.TimeScale = 1; _returnAtEnd = false; ReturnToHub(); return; }
		if (_shotAtPick && _run.Phase == RiftPhase.Pick && _pickT < 0) _pickT = _t;
		bool shotNow = !_returnAtEnd && (_shotAtPick ? _pickT >= 0 && _t - _pickT > 0.6 * Engine.TimeScale : _shotAtEnd ? _endT > 0 && _t - _endT > 1.5 * Engine.TimeScale : _t >= _shotAt);
		if (_shotPath != null && shotNow) { var img = GetViewport().GetTexture().GetImage(); var err = img.SavePng(_shotPath); GD.Print($"shot {img.GetWidth()}x{img.GetHeight()} -> {_shotPath} ({err})"); GetTree().Quit(); _shotPath = null; }
	}

	private void Show(RiftEvent ev)
	{
		switch (ev.Kind)
		{
			case RiftEventKind.Text: Say(ev.Text, ev.X, ev.Y, Col(ev.Color), ev.Size >= 24); break;
			case RiftEventKind.Hit: Play("hit-spark", EighthOf(ev.Size), ev.X, ev.Y); break; // aimed along the blow
			case RiftEventKind.Splash: Play("glob-splash", EighthOf(ev.Size), ev.X, ev.Y); break;
			case RiftEventKind.Hatch: Play("pod-burst", 0, ev.X, ev.Y); break; // the pod splits and a grazer stands in it
			case RiftEventKind.Kill: // Ybasogs burst in torn flesh and bile; rock and hull break apart
				Play(ev.Text switch { "asteroid" => "rock-break", "slab" => "slab-break", _ => "pod-burst" }, 0, ev.X, ev.Y);
				break;
		}
	}

	private void Say(string s, double x, double y, Color c, bool big)
	{
		if (_floatN >= _floats.Length) { Array.Copy(_floats, 1, _floats, 0, _floats.Length - 1); _floatN--; }
		_floats[_floatN++] = new Floater { X = (float)x, Y = (float)y, S = s, C = c, Big = big, Life = big ? 1.8f : 0.9f, Max = big ? 1.8f : 0.9f };
	}

	// Starts a baked one-shot (pixelfx, via tools/art/bake_rift_fx.py) at a rift position.
	private void Play(string sheet, int row, double x, double y)
	{
		if (_shotN < _shots.Length) _shots[_shotN++] = new OneShot { Sheet = sheet, Row = row, X = (float)x, Y = (float)y, T = 0 };
	}

	// Which of eight 45-degree rows points along an angle (screen angle, y down).
	private static int EighthOf(double angle) => Mathf.PosMod((int)Math.Round(angle / (Math.PI / 4)), 8);

	private void Animate(float dt)
	{
		for (int i = 0; i < _floatN; i++)
		{
			ref var f = ref _floats[i];
			f.Life -= dt; f.Y -= 18 * dt;
			if (f.Life <= 0) { Array.Copy(_floats, i + 1, _floats, i, _floatN - i - 1); _floatN--; i--; }
		}
		for (int i = 0; i < _shotN; i++)
		{
			_shots[i].T += dt;
			if (_shots[i].T >= _sheets.Length(_shots[i].Sheet)) _shots[i--] = _shots[--_shotN];
		}
	}

	// ---- drawing ---------------------------------------------------------------------------------------------

	/// <inheritdoc/>
	public override void _Draw()
	{
		_shake = Vector2.Zero;
		if (_run.Shake > 0.5 && !_paused) _shake = new Vector2(Mathf.Round((float)((_fx.Next() - 0.5) * _run.Shake * _k)), Mathf.Round((float)((_fx.Next() - 0.5) * _run.Shake * _k)));
		DrawSetTransform(_shake);
		DrawWalls();
		foreach (var m in _run.Matter) DrawMatter(m);
		foreach (var e in _run.Foes) DrawFoe(e);
		foreach (var g in _run.Globs) DrawGlob(g);
		DrawPlayer();
		foreach (var m in _run.Motes) DrawMote(m);
		foreach (var b in _run.ScrapBits) _sheets.DrawAt(this, "scrap", _t + b.Ph, ToScreen(b.X, b.Y), Colors.White); // pixelfx's tumbling scrap
		for (int i = 0; i < _shotN; i++) // the one-shots, over the figures (bolts are drawn on top by the bolt layer)
		{
			var sh = _shots[i];
			if (_sheets[sh.Sheet].Rows > 1 && _sheets[sh.Sheet].Cols == _sheets[sh.Sheet].Frames) _sheets.DrawRow(this, sh.Sheet, sh.Row, sh.T, ToScreen(sh.X, sh.Y), Colors.White);
			else _sheets.DrawAt(this, sh.Sheet, sh.T, ToScreen(sh.X, sh.Y), Colors.White);
		}
		for (int i = 0; i < _floatN; i++)
		{
			var f = _floats[i]; var font = f.Big ? UiKit.Big : UiKit.Small; int size = f.Big ? 24 : 16;
			float w = font.GetStringSize(f.S, HorizontalAlignment.Left, -1, size).X;
			var p = ToScreen(f.X, f.Y) - new Vector2(Mathf.Round(w / 2), 0);
			DrawString(font, p, f.S, HorizontalAlignment.Left, -1, size, f.C with { A = Mathf.Clamp(f.Life / f.Max * 2, 0, 1) });
		}
		DrawSetTransform(Vector2.Zero);
		DrawHud();
		if (_stickIdx >= 0)
		{
			DrawArc(_stickO.Round(), _cfg.StickRadius, 0, Mathf.Tau, 24, Pal.Line, -1, false);
			var knob = _stickO + (_stickAt - _stickO).LimitLength(_cfg.StickRadius);
			DrawRect(new Rect2(knob.Round() - new Vector2(2, 2), 5, 5), Pal.Mid);
		}
		if (_paused) DrawPause();
		if (_run.Outcome != RiftOutcome.None) DrawEnd();
	}

	private static readonly Color FlashWhite = new(2.5f, 2.5f, 2.5f);

	// Falling matter: a rock picks the frame of its turn; a slab's dead lights flicker.
	private void DrawMatter(RiftMatter m)
	{
		var at = ToScreen(m.X, m.Y); var tint = m.Flash > 0 ? FlashWhite : Colors.White;
		if (m.Rect) { _sheets.DrawAt(this, "slab", _t + m.X * 0.01, at, tint); return; }
		string name = m.Variant switch { 1 => "rock-1", 2 => "rock-2", _ => "rock-0" };
		int turn = (int)Math.Floor(Mathf.PosMod((float)m.Rot, Mathf.Tau) / Mathf.Tau * _sheets[name].Cols);
		int row = (m.CarriesVoid ? 1 : 0) + (m.CarriesScrap ? 2 : 0); // rows: plain, void crystal, scrap metal, both
		_sheets.Draw(this, name, row, turn, at, tint);
	}

	// A Ybasog: each kind animates its own way, and a grazer faces where it is going.
	private void DrawFoe(RiftFoe e)
	{
		var at = ToScreen(e.X, e.Y); var tint = e.Flash > 0 ? FlashWhite : Colors.White;
		switch (e.Fam)
		{
			case "grazer":
			{
				const int Dirs = 16; // the sheet's rows: heading d (row d * 2), then the same with red dive eyes (+1)
				int dir = Mathf.PosMod((int)Math.Round((e.Heading - Math.PI / 2) / (Math.Tau / Dirs)), Dirs); // dir 0 faces down the rift
				double pace = e.Turning ? 0.5 : e.Dive ? 1.7 : 1; // legs scrabble on the turn, race on the charge
				_sheets.DrawRow(this, "grazer", dir * 2 + (e.Dive ? 1 : 0), _t * pace + e.Ph, at, tint);
				break;
			}
			case "feeder":
			{
				bool gape = e.T1 < 0.35 && e.Y > 20; // the maw opens wide just before it spits
				_sheets.DrawRow(this, "feeder", gape ? 1 : 0, _t + e.Ph, at, tint);
				break;
			}
			default: // a pod: pixelfx's breathing egg sac, then the cracking one as it nears hatching
			{
				bool cracking = e.Age > _data.Tune("pod_hatch") * 0.6;
				_sheets.DrawAt(this, cracking ? "pod-crack" : "pod", _t + e.Ph, at, tint);
				break;
			}
		}
	}

	// A glob of bile: pixelfx's wobbling drop, its trail of drips behind it (rows: eight headings, dir 0 down the rift).
	private void DrawGlob(RiftGlob g)
	{
		int dir = Mathf.PosMod((int)Math.Round((Math.Atan2(g.Vy, g.Vx) - Math.PI / 2) / (Math.Tau / 8)), 8);
		_sheets.DrawRow(this, "glob", dir, _t + g.X * 0.01, ToScreen(g.X, g.Y), Colors.White);
	}

	/// <summary>
	/// Draws the bolts on their own canvas item, whose shader paints the bolt sheet's three grey steps in each card's
	/// colour (canvas_fx_bolt_tint). It sits above the rift's figures and is hidden while paused or over.
	/// </summary>
	private sealed partial class BoltLayer : Node2D
	{
		/// <summary>The rift scene whose bolts it draws.</summary>
		public RiftScene Scene = null!;

		/// <inheritdoc/>
		public override void _Process(double delta) => QueueRedraw();

		/// <inheritdoc/>
		public override void _Draw()
		{
			var r = Scene;
			if (r._paused || r._run.Outcome != RiftOutcome.None) return;
			DrawSetTransform(r._shake);
			const int Dirs = 16; // rows: heading d (row d * 2), a fused card's rings (+1)
			foreach (var b in r._run.Bolts)
			{
				int dir = Mathf.PosMod((int)Math.Round((Math.Atan2(b.Vy, b.Vx) - Math.PI / 2) / (Math.Tau / Dirs)), Dirs);
				r._sheets.DrawRow(this, "bolt", dir * 2 + (b.Natures.Length >= 2 ? 1 : 0), r._t + b.Age, r.ToScreen(b.X, b.Y), r.Col(b.Color));
			}
		}
	}

	// A mote of void energy: the hub's void ball (a dark heart in a pale crackling ring), without its trail.
	private void DrawMote(RiftMote m)
	{
		DrawTexture(_voidBall, ToScreen(m.X, m.Y) - _voidBallAnchor);
	}

	// The rift's walls: the dark beyond them, and an edge that wavers a pixel either way.
	private void DrawWalls()
	{
		float l = ToScreen(RiftRun.RL, 0).X, r = ToScreen(RiftRun.RR, 0).X;
		DrawRect(new Rect2(-8, -8, l + 8, _area.Y + 16), Outside);
		DrawRect(new Rect2(r, -8, _area.X - r + 8, _area.Y + 16), Outside);
		for (int y = 0; y < _wallL.Length; y++)
		{
			float wob = Mathf.Round(Mathf.Sin(y * 0.09f + (float)_t * 1.3f) + Mathf.Sin(y * 0.031f - (float)_t * 0.7f) * 0.6f);
			_wallL[y] = new Vector2(l + wob, y); _wallR[y] = new Vector2(r - wob, y);
		}
		DrawPolyline(_wallL, Edge, -1, false); DrawPolyline(_wallR, Edge, -1, false);
	}

	private void DrawPlayer()
	{
		var p = _run.Player;
		if (p.Iframe > 0 && (int)(_t * 14) % 2 == 0) return; // blinking while safe after a hit
		var tex = _art.Texture(_formSprite); var anchor = (Vector2)_art.Anchor(_formSprite);
		float bob = p.Still ? (Math.Sin(_t * 3) > 0 ? -1 : 0) : (Math.Sin(_t * 12) > 0 ? -1 : 0); // breathing, or bounding as it moves
		var pos = ToScreen(p.X, p.Y) - anchor + new Vector2(0, bob);
		DrawTexture(tex, pos); // never flipped: the form keeps its drawn facing whichever way it moves
		if (_touch && _run.AimAtNearest() == null) return;
		var at = ToScreen(p.X, p.Y);
		for (int i = 0; i < 3; i++) // the aim: three dots
		{
			float d = 13 + i * 4;
			DrawRect(new Rect2((at + new Vector2(Mathf.Cos((float)p.Aim), Mathf.Sin((float)p.Aim)) * d).Round(), 1, 1), Pal.Void with { A = 0.9f - i * 0.25f });
		}
	}

	private void Text(string s, Vector2 at, Color c, bool big = false, bool centre = false)
	{
		var font = big ? UiKit.Big : UiKit.Small; int size = big ? 24 : 16;
		if (centre) at.X -= Mathf.Round(font.GetStringSize(s, HorizontalAlignment.Left, -1, size).X / 2);
		DrawString(font, at, s, HorizontalAlignment.Left, -1, size, c);
	}

	private void Diamond(Vector2 at, Color c, bool filled)
	{
		DrawRect(new Rect2(at + new Vector2(-1, 0), 3, 1), c); DrawRect(new Rect2(at + new Vector2(0, -1), 1, 3), c);
		if (!filled) DrawRect(new Rect2(at, 1, 1), Pal.Abyss);
	}

	private void DrawHud()
	{
		// energy: the bar alone (the player reads its length, never a number)
		float bw = 96, frac = (float)Math.Clamp(_run.Energy / _run.MaxEnergy, 0, 1);
		DrawRect(new Rect2(6, 6, bw + 2, 7), Pal.Ink); DrawRect(new Rect2(7, 7, bw, 5), Pal.Deep);
		DrawRect(new Rect2(7, 7, Mathf.Round(bw * frac), 5), frac > 0.3f ? Edge : Pal.Warn);
		for (int i = 0; i < _run.Ward; i++) Diamond(new Vector2(9 + i * 6, 18), Pal.GoldHi, true);
		// void energy gathered
		Diamond(new Vector2(10, 30), Pal.Void, true);
		Text($"{_run.VoidE}", new Vector2(16, 36), Pal.Void);
		_sheets.Draw(this, "scrap", 0, 0, new Vector2(10, 46), Colors.White); // scrap, for this adventure's boosts
		Text($"{_run.Scrap}", new Vector2(16, 52), Pal.Soft);
		// the adventure's level, and a thin bar of experience under the energy bar
		float xf = (float)Math.Clamp(_run.Xp / _run.Settings.XpToNext(_run.Level), 0, 1);
		DrawRect(new Rect2(7, 14, bw, 1), Pal.Deep); DrawRect(new Rect2(7, 14, Mathf.Round(bw * xf), 1), Pal.Gold);
		Text($"LV {_run.Level}", new Vector2(bw + 12, 16), Pal.Gold);
		// stage and wave pips, top right, left of the pause button
		for (int s = 0; s < 3; s++)
			for (int w = 0; w < 3; w++)
			{
				int idx = s * 3 + w, cur = (_run.Stage - 1) * 3 + (_run.WaveN - 1);
				var at = new Vector2(_area.X - 112 + s * 26 + w * 6, 12);
				Diamond(at, idx < cur ? Pal.Mid : idx == cur ? Pal.Bright : Pal.Line, idx <= cur);
			}
		// the pause button
		DrawRect(_pauseBtn, Pal.Ink); DrawRect(new Rect2(_pauseBtn.Position + new Vector2(7, 4), 2, 10), Pal.Mid); DrawRect(new Rect2(_pauseBtn.Position + new Vector2(11, 4), 2, 10), Pal.Mid);
		// the next bolts, bottom left: one diamond per card in draw order
		int n = 0; float y = _area.Y - 10;
		foreach (var c in _run.NextCards)
		{
			if (n >= _cfg.NextShown) break;
			Diamond(new Vector2(10 + n * 8, y), Col(_session.Rules.CardColor(c.Natures)), true); n++;
		}
		if (n == 0 && _run.ReshuffleT > 0) Text("...", new Vector2(8, y + 4), Pal.Dim);
		Text($"{_run.Deck.Count}", new Vector2(14 + Math.Max(n, 1) * 8, y + 5), Pal.Dim);
		if (_autoFire && !_touch) Text("AUTO", new Vector2(_area.X - 40, _area.Y - 6), Pal.Mid);
	}

	private void Button(Rect2 r, string s, Color c)
	{
		DrawRect(r, Pal.Ink); DrawRect(r, Pal.Frame, false, -1);
		Text(s, new Vector2(r.Position.X + r.Size.X / 2, r.Position.Y + 17), c, false, true);
	}

	private void DrawPause()
	{
		DrawRect(new Rect2(Vector2.Zero, _area), Pal.Abyss with { A = 0.8f });
		Text("PAUSED", new Vector2(_area.X / 2, _area.Y / 2 - 24), Pal.Bright, true, true);
		Button(_resumeBtn, "RESUME", Pal.Mint);
		Button(_leaveBtn, "LEAVE THE RIFT", Pal.Warn);
		Text("leaving counts as a fall", new Vector2(_area.X / 2, _leaveBtn.End.Y + 16), Pal.Dim, false, true);
	}

	private void DrawEnd()
	{
		DrawRect(new Rect2(Vector2.Zero, _area), Pal.Abyss with { A = 0.85f });
		float cy = _area.Y / 2 - 40;
		var (title, line, colour) = _run.Outcome switch
		{
			RiftOutcome.Won => ("THE MOTHER FALLS", "the rift falls silent", Pal.Gold),
			RiftOutcome.StageCleared => ("STAGE CLEARED", "the way deeper is not open yet", Pal.Mint),
			_ => ("FALLEN", "the void takes back part of what you gathered", Pal.Warn),
		};
		Text(title, new Vector2(_area.X / 2, cy), colour, true, true);
		Text(line, new Vector2(_area.X / 2, cy + 22), Pal.Mid, false, true);
		Text($"VOID BANKED +{_run.Banked}", new Vector2(_area.X / 2, cy + 52), Pal.Void, true, true);
		Text($"{_run.Kills} YBASOGS SLAIN", new Vector2(_area.X / 2, cy + 74), Pal.Soft, false, true);
		if ((int)(_t * 2) % 2 == 0) Text(_touch ? "TAP TO RETURN" : "CLICK OR PRESS ENTER TO RETURN", new Vector2(_area.X / 2, cy + 104), Pal.Bright, false, true);
	}
}
