using System.Collections.Generic;
using System.Linq;
using Godot;
using Voidwalker.Core;
using Voidwalker.Ui;

namespace Voidwalker.Hub;

/// <summary>
/// The Rift Altar hub. Drawn in a 640x360 stage (the browser hub's layout at half scale), centred in a viewport that
/// may be wider on phones. Deep space is a shader with fixed blinking stars; the stations are code-drawn pixel
/// sprites; void flames ride a tide round the altar; the Rite's rift is a baked pixelfx loop. Touch-first: a tap on a
/// station lights it and opens its screen. The lobby shows the save: cards in the pool, sigils bought (stars lit),
/// relic slots open, the chosen rift's colour, the bank and what is known.
/// </summary>
public partial class Hub : Node2D
{
	/// <summary>Raised when a station's screen opens.</summary>
	[Signal] public delegate void StationOpenedEventHandler(string id);

	/// <summary>The hub's tuning (tide, trails, sky); defaults apply when unset.</summary>
	[Export] public HubConfig? Config { get; set; }

	private const int StageW = 640, StageH = 360;
	private static readonly Vector2 BlobCentre = new(320, 245); // where the form floats before the lectern
	private string _blobForm = "";
	private static readonly Vector2[] Constellation =
	{
		new(115, 65), new(131, 55), new(150, 62), new(165, 52), new(176, 70), new(159, 85),
		new(141, 91), new(123, 88), new(113, 105), new(135, 107), new(157, 103), new(175, 98),
	};

	private readonly List<Station> _stations = new();
	private readonly Dictionary<string, Station> _byId = new();
	private readonly List<(Sprite2D sprite, int i)> _rocks = new();
	private readonly List<Sprite2D> _cards = new(), _stars = new(), _pedestals = new(), _relicIcons = new();
	private const float PedestalX0 = 465, PedestalStep = 30, RelicIconY = 83; // the relic floats above its pedestal
	private Node2D _stage = null!;
	private ColorRect _sky = null!;
	private ShaderMaterial _swirl = null!;
	private Station? _hovered;
	private Sprite2D? _tome, _blob;
	private Label _bank = null!, _counts = null!;
	private Label _saveNotice = null!;
	private CanvasLayer _ui = null!;
	private StationScreen? _open;
	private LobbyArt _art = null!;
	private Shader _highlight = null!;
	private double _t;
	private HubConfig _cfg = null!;
	private Session _session = null!;

	/// <inheritdoc/>
	public override void _Ready()
	{
		var cfg = _cfg = Config ?? new HubConfig(); // resolved once: no allocation per frame
		var data = GameData.Parse(FileAccess.GetFileAsString(Session.DataPath));
		_session = Session.Load(data, ProjectSettings.GlobalizePath(Session.SavePath));
		var art = _art = new LobbyArt();
		var highlight = _highlight = GD.Load<Shader>("res://src/shaders/canvas_ui_station_highlight.gdshader");

		// deep space fills the whole viewport, behind the stage
		_sky = new ColorRect { Material = SkyMaterial(cfg), MouseFilter = Control.MouseFilterEnum.Ignore };
		AddChild(_sky);
		_stage = new Node2D { Name = "Stage" };
		AddChild(_stage);

		for (int i = 0; i < 5; i++) // far asteroids drifting behind everything
		{
			var rock = art.Make($"asteroid-{i % 3}", Vector2.Zero); _stage.AddChild(rock); _rocks.Add((rock, i));
		}

		_stage.AddChild(art.Make("altar", new Vector2(320, 257)));

		// the Deck: a lectern with the pool's cards fanned above it, and the blob standing before it
		var deck = AddStation("deck", new Vector2(320, 159), 43);
		deck.AddArt(art.Make("lectern", new Vector2(320, 220) - deck.Position), highlight);
		_blob = art.Make("form-blob", BlobCentre); _stage.AddChild(_blob); // RefreshState gives it the pool's form

		// the Void Rite: a ring of standing stones round a narrow void rift
		var rite = AddStation("rite", new Vector2(150, 215), 43);
		var ring = new List<(float depth, Vector2 at)>();
		for (int i = 0; i < 9; i++)
		{
			float a = i / 9f * Mathf.Tau; ring.Add((Mathf.Sin(a), new Vector2(150 + Mathf.Cos(a) * 35, 235 + Mathf.Sin(a) * 10)));
		}
		ring.Sort((p, q) => p.depth.CompareTo(q.depth));
		foreach (var (depth, at) in ring) if (depth < 0) rite.AddArt(art.Make("standing-stone-far", at - rite.Position), highlight);
		rite.AddArt(SheetPlayer.Load("res://assets/fx/", "rift", new Vector2(150, 239) - rite.Position), highlight);
		foreach (var (depth, at) in ring) if (depth >= 0) rite.AddArt(art.Make("standing-stone-lit", at - rite.Position), highlight);

		// the Rift Gate: two stone horns round a swirl tinted by the chosen rift's prevailing nature
		var gate = AddStation("gate", new Vector2(492, 205), 48);
		var swirl = new ColorRect { Size = new Vector2(56, 104), Position = new Vector2(492 - 28, 205 - 52) - gate.Position, MouseFilter = Control.MouseFilterEnum.Ignore };
		_swirl = new ShaderMaterial { Shader = GD.Load<Shader>("res://src/shaders/canvas_env_gate_swirl.gdshader") };
		_swirl.SetShaderParameter("rect_size", swirl.Size);
		swirl.Material = _swirl; gate.AddChild(swirl);
		gate.AddArt(art.Make("gate-arch", new Vector2(437, 144) - gate.Position), highlight);
		gate.AddArt(art.Make("gate-arch", new Vector2(548, 144) - gate.Position, flipH: true), highlight);

		// the Sigils: a constellation, a star lit for each sigil level bought
		var sigils = AddStation("sigils", new Vector2(145, 82), 40);
		var lines = new Line2D { Width = 1, DefaultColor = new Color("#3b3350"), Antialiased = false };
		foreach (var p in Constellation) lines.AddPoint(p - sigils.Position);
		sigils.AddChild(lines);

		// the Relics: three pedestals, open as far as the slots go
		AddStation("relics", new Vector2(495, 86), 40);

		// the Journal: an open tome, floating
		var journal = AddStation("journal", new Vector2(320, 59), 33);
		_tome = journal.AddArt(art.Make("tome", Vector2.Zero), highlight);

		// void energy flowing: flames riding the tide round the altar
		for (int i = 0; i < cfg.FlowCount; i++)
		{
			var flow = new VoidFlow { Name = $"Flow{i}" }; flow.Setup(art, cfg, i, cfg.FlowCount); _stage.AddChild(flow);
		}

		// the top bar: the bank, and how much is known
		_stage.AddChild(art.Make("void-icon", new Vector2(14, 12)));
		_bank = MakeLabel(true, new Color("#d8c8ff"));
		_bank.Position = new Vector2(24, 3);
		_stage.AddChild(_bank);
		_counts = MakeLabel(false, new Color("#9d92bb"));
		_counts.HorizontalAlignment = HorizontalAlignment.Right;
		_counts.Size = new Vector2(480, 14); _counts.Position = new Vector2(StageW - 8 - 480, 6);
		_stage.AddChild(_counts);
		var version = MakeLabel(false, new Color("#3b3350")); // which build this is, so a phone install can be checked
		version.Text = BuildVersion(); version.Position = new Vector2(8, StageH - 14);
		_stage.AddChild(version);
		_saveNotice = MakeLabel(false, Pal.Warn);
		_saveNotice.HorizontalAlignment = HorizontalAlignment.Right;
		_saveNotice.Size = new Vector2(372, 14);
		_saveNotice.Position = new Vector2(StageW - 380, StageH - 14);
		_stage.AddChild(_saveNotice);

		// station screens draw above everything
		_ui = new CanvasLayer { Layer = 10, Name = "Screens" };
		AddChild(_ui);

		RefreshState();
		GetViewport().SizeChanged += Layout;
		Layout();
		foreach (var arg in OS.GetCmdlineUserArgs()) // checks and screenshots: `-- --open=gate` opens a station's screen at start
			if (arg.StartsWith("--open=")) Callable.From(() => Open(arg["--open=".Length..])).CallDeferred();
	}

	private Station AddStation(string id, Vector2 centre, float radius)
	{
		var s = new Station(); s.Setup(id, centre, radius); _stage.AddChild(s); _stations.Add(s); _byId[id] = s;
		return s;
	}

	/// <summary>Redraws what the lobby shows from the save: the blob's form, the card fan, lit stars, open pedestals, the swirl's colour and the top bar.</summary>
	public void RefreshState()
	{
		var rules = _session.Rules; var meta = rules.Meta; var data = _session.Data;

		var deck = _byId["deck"];
		foreach (var c in _cards) deck.RemoveArt(c);
		_cards.Clear();
		int fan = Mathf.Clamp(rules.DeckCount(), 1, 5);
		for (int i = 0; i < fan; i++) _cards.Add(deck.AddArt(_art.Make("card-back", Vector2.Zero), _highlight));

		var form = rules.DominantForm(); // the blob takes the shape of the pool's main nature
		if (_blob != null && form != _blobForm)
		{
			var shaped = _art.Make("form-" + form, BlobCentre);
			_blob.Texture = shaped.Texture; _blob.Offset = shaped.Offset; _blobForm = form;
			shaped.Free();
		}

		var sigils = _byId["sigils"];
		foreach (var s in _stars) sigils.RemoveArt(s);
		_stars.Clear();
		int lit = meta.Upg.Values.Sum();
		for (int i = 0; i < Constellation.Length; i++)
			_stars.Add(sigils.AddArt(_art.Make(i < lit ? "sigil-star-lit" : "sigil-star-dark", Constellation[i] - sigils.Position), _highlight));

		var relics = _byId["relics"];
		foreach (var p in _pedestals) relics.RemoveArt(p);
		_pedestals.Clear();
		int slots = rules.RelicSlots();
		for (int i = 0; i < 3; i++)
			_pedestals.Add(relics.AddArt(_art.Make(i < slots ? "pedestal-open" : "pedestal-locked", new Vector2(PedestalX0 + PedestalStep * i, 116) - relics.Position), _highlight));
		foreach (var r in _relicIcons) relics.RemoveArt(r);
		_relicIcons.Clear();
		for (int i = 0; i < Mathf.Min(slots, meta.Equipped.Count); i++) // each equipped relic glows above its open pedestal
		{
			var tex = PixelIcons.Relic(meta.Equipped[i]);
			var icon = new Sprite2D { Texture = tex, Centered = false, Offset = -new Vector2(tex.GetWidth() / 2, tex.GetHeight() / 2) };
			_relicIcons.Add(relics.AddArt(icon, _highlight));
		}

		_swirl.SetShaderParameter("tint", meta.Prevail != null ? new Color(data.Natures[meta.Prevail].Color) : new Color("#b9a6ff"));
		_bank.Text = meta.Bank.ToString();
		_saveNotice.Text = _session.SaveNotice;
		_counts.Text = $"NATURES {meta.Discovered.Count}/{data.Natures.Count}  FUSIONS {meta.Fusions.Count}/{data.FusionKeys.Count}  YBASOGS {rules.YbasogsMet()}/{data.Ybasogs.Count}  RELICS {meta.Relics.Count}/{data.Relics.Count}";
	}

	/// <summary>The build's version: written by tools/build/android_export.sh, or the project's version marked dev.</summary>
	private static string BuildVersion()
	{
		const string path = "res://src/data/build_info.json";
		if (FileAccess.FileExists(path) && Json.ParseString(FileAccess.GetFileAsString(path)).AsGodotDictionary() is { } info && info.ContainsKey("name"))
			return $"v{info["name"]}";
		return $"v{ProjectSettings.GetSetting("application/config/version")} dev";
	}

	private static Label MakeLabel(bool big, Color colour)
	{
		var label = new Label { MouseFilter = Control.MouseFilterEnum.Ignore };
		label.AddThemeFontOverride("font", big ? Ui.Ui.Big : Ui.Ui.Small);
		label.AddThemeFontSizeOverride("font_size", big ? Ui.Ui.BigSize : Ui.Ui.SmallSize);
		label.AddThemeColorOverride("font_color", colour);
		return label;
	}

	private static ShaderMaterial SkyMaterial(HubConfig cfg)
	{
		var m = new ShaderMaterial { Shader = GD.Load<Shader>("res://src/shaders/canvas_env_deep_space.gdshader") };
		m.SetShaderParameter("violet", new[] { new Color("#07040d"), new Color("#0e0a18"), new Color("#1a1229"), new Color("#2a1636"), new Color("#3a1746"), new Color("#4a3a7a") });
		m.SetShaderParameter("teal", new[] { new Color("#07040d"), new Color("#16302e"), new Color("#2e5a55") });
		m.SetShaderParameter("star_ramp", new[] { new Color("#1a1229"), new Color("#241a3a"), new Color("#3b3350"), new Color("#6d6488"), new Color("#9d92bb"), new Color("#cfc6e6"), new Color("#efe6ff") });
		m.SetShaderParameter("star_density", cfg.StarDensity);
		m.SetShaderParameter("blink_min", cfg.StarBlinkMin);
		m.SetShaderParameter("blink_max", cfg.StarBlinkMax);
		return m;
	}

	/// <summary>Centres the 640x360 stage in the viewport (whole pixels) and stretches the sky over all of it.</summary>
	private void Layout()
	{
		var vp = GetViewportRect().Size;
		_stage.Position = new Vector2(Mathf.Floor((vp.X - StageW) / 2), Mathf.Floor((vp.Y - StageH) / 2));
		_sky.Size = vp;
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_t += delta;
		var cfg = _cfg;
		float span = GetViewportRect().Size.X + 100;
		foreach (var (rock, i) in _rocks) // far rocks drift across slowly; they never turn (pixels stay square)
			rock.Position = new Vector2(Mathf.Floor((float)((i * 145 + _t * cfg.AsteroidDrift) % span) - 50 - _stage.Position.X), 40 + (i * 68.5f) % 280).Round();
		for (int i = 0; i < _cards.Count; i++) // the fan of cards breathes over the lectern
		{
			float k = i - (_cards.Count - 1) / 2f;
			_cards[i].Position = new Vector2(k * 17, 6 + Mathf.Abs(k) * 4 + Mathf.Sin((float)_t * 2 + i) * 1.5f).Round();
		}
		var relicsAt = _byId["relics"].Position;
		for (int i = 0; i < _relicIcons.Count; i++) // equipped relics bob over their pedestals, each in its own phase
			_relicIcons[i].Position = (new Vector2(PedestalX0 + PedestalStep * i, RelicIconY + Mathf.Sin((float)_t * 2 + i) * 2) - relicsAt).Round();
		if (_tome != null) _tome.Position = new Vector2(0, Mathf.Sin((float)_t * 1.5f) * 2).Round();
		if (_blob != null) _blob.Position = (BlobCentre + new Vector2(0, Mathf.Sin((float)_t * 2) * 2)).Round();
	}

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent e)
	{
		if (e.IsActionPressed("ui_cancel")) { _open?.Close(); return; }
		if (_open != null) return;
		if (e is InputEventMouseMotion motion) // PC: hovering lights a station; phones have no hover
		{
			_hovered = StationAt(motion.Position); Relight(null);
		}
		else if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } press) // a tap (touch arrives as this too)
		{
			if (StationAt(press.Position) is { } hit) Open(hit);
		}
	}

	/// <inheritdoc/>
	public override void _Notification(int what)
	{
		if (what == NotificationWMGoBackRequest) _open?.Close(); // Android's back button closes a screen (quit_on_go_back is off)
	}

	/// <summary>Opens a station's screen by id (deck, rite, gate, sigils, relics, journal).</summary>
	public void Open(string id) { if (_byId.TryGetValue(id, out var s)) Open(s); }

	private void Open(Station station)
	{
		if (_open != null) return;
		StationScreen screen = station.Id switch
		{
			"deck" => new PoolScreen(),
			"rite" => new RiteScreen(),
			"gate" => new GateScreen(),
			"sigils" => new SigilsScreen(),
			"relics" => new RelicsScreen(),
			_ => new JournalScreen(),
		};
		screen.Open(_session);
		screen.Closed += () => { _open = null; _hovered = null; Relight(null); RefreshState(); };
		_open = screen;
		Relight(station);
		_ui.AddChild(screen);
		EmitSignal(SignalName.StationOpened, station.Id);
	}

	private Station? StationAt(Vector2 viewportPoint)
	{
		var p = viewportPoint - _stage.Position;
		Station? best = null; float bd = float.MaxValue;
		foreach (var s in _stations) { float d = p.DistanceTo(s.Position); if (s.Hits(p) && d < bd) { bd = d; best = s; } }
		return best;
	}

	private void Relight(Station? opened)
	{
		foreach (var s in _stations) s.SetLit(s == opened || s == _hovered);
	}
}
