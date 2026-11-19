using System;
using Godot;
using Voidwalker.Core;
using Voidwalker.Hub;

namespace Voidwalker.Ui;

/// <summary>
/// A station's screen over the hub: the hub dimmed by a one-in-two pixel dither (exact palette, no translucency),
/// a framed panel with a title, a message line and a close button, and a 616x330 body the screen lays out in whole
/// pixels. It fits the 640x360 base and stays centred on wider phones. A tap on the dimmed hub, the X, the Android
/// back button or Escape closes it. Screens change the save only through <see cref="AltarRules"/>, then call
/// <see cref="Commit"/>, which saves and redraws the body.
/// <example><code>var screen = new PoolScreen();
/// screen.Open(session);
/// screen.Closed += hub.RefreshState;
/// uiLayer.AddChild(screen);</code></example>
/// </summary>
public abstract partial class StationScreen : Control
{
	/// <summary>The body's size in game pixels.</summary>
	public const int BodyW = 616, BodyH = 330;
	private static readonly Vector2I PanelSize = new(BodyW + 12, BodyH + 30);

	/// <summary>Raised once the screen has closed.</summary>
	public event Action? Closed;

	/// <summary>The shared session.</summary>
	protected Session S { get; private set; } = null!;
	/// <summary>The rules over the save.</summary>
	protected AltarRules R => S.Rules;
	/// <summary>The data tables.</summary>
	protected GameData D => S.Data;
	/// <summary>Where the screen lays itself out (616x330).</summary>
	protected Control Body { get; private set; } = null!;
	/// <summary>The panel's title.</summary>
	protected abstract string Title { get; }

	private Control _panel = null!;
	private Label _msg = null!;
	private bool _closing;

	/// <summary>Gives the screen its session; call before adding it to the tree.</summary>
	public void Open(Session session) => S = session;

	/// <inheritdoc/>
	public override void _Ready()
	{
		SetAnchorsPreset(LayoutPreset.FullRect);
		MouseFilter = MouseFilterEnum.Stop;
		var dim = new ColorRect { MouseFilter = MouseFilterEnum.Stop, Material = new ShaderMaterial { Shader = GD.Load<Shader>("res://src/shaders/canvas_ui_dither_dim.gdshader") } };
		dim.SetAnchorsPreset(LayoutPreset.FullRect);
		dim.GuiInput += e => { if (e is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }) Close(); };
		AddChild(dim);

		_panel = new PanelFrame { Size = PanelSize, MouseFilter = MouseFilterEnum.Stop };
		AddChild(_panel);
		Ui.Text(_panel, Title, 8, 5, Pal.Bright, big: true);
		_msg = Ui.Text(_panel, "", 160, 7, Pal.Gold, width: PanelSize.X - 160 - 34, align: HorizontalAlignment.Right);
		Ui.Tile(_panel, PanelSize.X - 28, 4, 22, 18, Close, "X", Pal.Mid);
		Body = new Control { Position = new Vector2(6, 26), Size = new Vector2(BodyW, BodyH), MouseFilter = MouseFilterEnum.Pass };
		_panel.AddChild(Body);

		GetViewport().SizeChanged += Layout;
		Layout();
		Build();
		if (S.SaveNotice.Length > 0) Say(S.SaveNotice, Pal.Warn);
	}

	private void Layout() => _panel.Position = ((GetViewportRect().Size - (Vector2)PanelSize) / 2).Floor().Max(Vector2.Zero);

	/// <summary>Lays the body out from the save. Called on opening and after every change.</summary>
	protected abstract void Build();

	/// <summary>Clears the body and builds it again.</summary>
	protected void Rebuild()
	{
		foreach (var child in Body.GetChildren()) { Body.RemoveChild(child); child.QueueFree(); }
		Build();
	}

	/// <summary>Saves the change and redraws.</summary>
	protected void Commit()
	{
		if (!S.Save()) Say(S.SaveNotice, Pal.Warn);
		Rebuild();
	}

	/// <summary>Shows a short message in the panel's title bar ("" clears it).</summary>
	protected void Say(string message, Color? colour = null)
	{
		_msg.Text = message;
		_msg.AddThemeColorOverride("font_color", colour ?? Pal.Gold);
	}

	/// <summary>Closes the screen.</summary>
	public void Close()
	{
		if (_closing) return;
		_closing = true;
		GetViewport().SizeChanged -= Layout;
		Closed?.Invoke();
		QueueFree();
	}

	/// <summary>The panel's frame: the outer line and an inner line two pixels in, as the browser build draws it.</summary>
	private sealed partial class PanelFrame : Control
	{
		public override void _Draw()
		{
			PixelTile.Frame(this, new Rect2I(0, 0, (int)Size.X, (int)Size.Y), Pal.Panel, Pal.Frame, Pal.Deep);
			DrawRect(new Rect2(4, 23, Size.X - 8, 1), Pal.Deep);
		}
	}
}
