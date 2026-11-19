using System.Collections.Generic;
using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// The Rift Altar hub, the first slice of the Godot port. Drawn in a 640x360 stage (the browser hub's layout at
/// half scale), centred in a viewport that may be wider on phones. Deep space is a shader with fixed blinking stars;
/// the stations are code-drawn pixel sprites; void flames ride a tide round the altar; the Rite's rift is a baked
/// pixelfx loop. Touch-first: tap a station to light it, tap it again to open it. Stations' screens come later.
/// </summary>
public partial class Hub : Node2D
{
	/// <summary>Raised when a station is opened (tapped while selected).</summary>
	[Signal] public delegate void StationOpenedEventHandler(string id);

	/// <summary>The hub's tuning (tide, trails, sky); defaults apply when unset.</summary>
	[Export] public HubConfig? Config { get; set; }
	/// <summary>The state the hub shows until the save system is ported: a collection part-way through.</summary>
	[Export] public HubDemoState? State { get; set; }

	private const int StageW = 640, StageH = 360;
	private static readonly Vector2[] Constellation =
	{
		new(115, 65), new(131, 55), new(150, 62), new(165, 52), new(176, 70), new(159, 85),
		new(141, 91), new(123, 88), new(113, 105), new(135, 107), new(157, 103), new(175, 98),
	};

	private readonly List<Station> _stations = new();
	private readonly List<(Sprite2D sprite, int i)> _rocks = new();
	private Node2D _stage = null!;
	private ColorRect _sky = null!;
	private Station? _selected, _hovered;
	private Sprite2D? _tome, _blob;
	private readonly List<Sprite2D> _cards = new();
	private Label _bank = null!, _counts = null!;
	private double _t;

	/// <inheritdoc/>
	public override void _Ready()
	{
		var cfg = Config ?? new HubConfig();
		var state = State ?? new HubDemoState();
		var art = new LobbyArt();
		var highlight = GD.Load<Shader>("res://src/shaders/canvas_ui_station_highlight.gdshader");

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
		int fan = Mathf.Clamp(state.PoolCards, 1, 5);
		for (int i = 0; i < fan; i++)
		{
			var card = deck.AddArt(art.Make("card-back", Vector2.Zero), highlight); _cards.Add(card);
		}
		_blob = art.Make("blob", new Vector2(320, 252)); _stage.AddChild(_blob);

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
		var swirlMat = new ShaderMaterial { Shader = GD.Load<Shader>("res://src/shaders/canvas_env_gate_swirl.gdshader") };
		swirlMat.SetShaderParameter("tint", state.RiftTint);
		swirlMat.SetShaderParameter("rect_size", swirl.Size);
		swirl.Material = swirlMat; gate.AddChild(swirl);
		gate.AddArt(art.Make("gate-arch", new Vector2(437, 144) - gate.Position), highlight);
		gate.AddArt(art.Make("gate-arch", new Vector2(548, 144) - gate.Position, flipH: true), highlight);

		// the Sigils: a constellation, a star lit for each sigil bought
		var sigils = AddStation("sigils", new Vector2(145, 82), 40);
		var lines = new Line2D { Width = 1, DefaultColor = new Color("#3b3350"), Antialiased = false };
		foreach (var p in Constellation) lines.AddPoint(p - sigils.Position);
		sigils.AddChild(lines);
		for (int i = 0; i < Constellation.Length; i++)
			sigils.AddArt(art.Make(i < state.SigilsBought ? "sigil-star-lit" : "sigil-star-dark", Constellation[i] - sigils.Position), highlight);

		// the Relics: three pedestals, open as far as the slots go
		var relics = AddStation("relics", new Vector2(495, 86), 40);
		for (int i = 0; i < 3; i++)
			relics.AddArt(art.Make(i < state.RelicSlots ? "pedestal-open" : "pedestal-locked", new Vector2(465 + 30 * i, 116) - relics.Position), highlight);

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
		_bank = MakeLabel("res://assets/fonts/spleen-8x16.otf", 16, new Color("#d8c8ff"));
		_bank.Position = new Vector2(24, 3); _bank.Text = state.Bank.ToString();
		_stage.AddChild(_bank);
		_counts = MakeLabel("res://assets/fonts/spleen-6x12.otf", 12, new Color("#9d92bb"));
		_counts.Text = $"NATURES {state.Natures}/14  FUSIONS {state.Fusions}/84  YBASOGS {state.Ybasogs}/58  RELICS {state.Relics}/16";
		_counts.HorizontalAlignment = HorizontalAlignment.Right;
		_counts.Size = new Vector2(480, 14); _counts.Position = new Vector2(StageW - 8 - 480, 6);
		_stage.AddChild(_counts);

		GetViewport().SizeChanged += Layout;
		Layout();
	}

	private Station AddStation(string id, Vector2 centre, float radius)
	{
		var s = new Station(); s.Setup(id, centre, radius); _stage.AddChild(s); _stations.Add(s);
		return s;
	}

	private static Label MakeLabel(string fontPath, int size, Color colour)
	{
		var font = GD.Load<FontFile>(fontPath);
		font.Antialiasing = TextServer.FontAntialiasing.None; // the pixel font's own pixels: no smoothing, no hinting
		font.Hinting = TextServer.Hinting.None;
		font.SubpixelPositioning = TextServer.SubpixelPositioning.Disabled;
		font.GenerateMipmaps = false;
		var label = new Label { MouseFilter = Control.MouseFilterEnum.Ignore };
		label.AddThemeFontOverride("font", font);
		label.AddThemeFontSizeOverride("font_size", size);
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
		var cfg = Config ?? new HubConfig();
		float span = GetViewportRect().Size.X + 100;
		foreach (var (rock, i) in _rocks) // far rocks drift across slowly; they never turn (pixels stay square)
			rock.Position = new Vector2(Mathf.Floor((float)((i * 145 + _t * cfg.AsteroidDrift) % span) - 50 - _stage.Position.X), 40 + (i * 68.5f) % 280).Round();
		for (int i = 0; i < _cards.Count; i++) // the fan of cards breathes over the lectern
		{
			float k = i - (_cards.Count - 1) / 2f;
			_cards[i].Position = new Vector2(k * 17, 6 + Mathf.Abs(k) * 4 + Mathf.Sin((float)_t * 2 + i) * 1.5f).Round();
		}
		if (_tome != null) _tome.Position = new Vector2(0, Mathf.Sin((float)_t * 1.5f) * 2).Round();
		if (_blob != null) _blob.Position = new Vector2(320, 252 + Mathf.Sin((float)_t * 2) * 2).Round();
	}

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent e)
	{
		if (e is InputEventMouseMotion motion) // PC: hovering lights a station too; phones have no hover
		{
			_hovered = StationAt(motion.Position); Relight();
		}
		else if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } press) // a tap (touch arrives as this too)
		{
			var hit = StationAt(press.Position);
			if (hit != null && hit == _selected) { EmitSignal(SignalName.StationOpened, hit.Id); GD.Print($"open {hit.Id}"); }
			_selected = hit; Relight();
		}
	}

	private Station? StationAt(Vector2 viewportPoint)
	{
		var p = viewportPoint - _stage.Position;
		Station? best = null; float bd = float.MaxValue;
		foreach (var s in _stations) { float d = p.DistanceTo(s.Position); if (s.Hits(p) && d < bd) { bd = d; best = s; } }
		return best;
	}

	private void Relight()
	{
		foreach (var s in _stations) s.SetLit(s == _selected || s == _hovered);
	}
}
