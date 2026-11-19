using System;
using Godot;
using Voidwalker.Core;
using Voidwalker.Hub;

namespace Voidwalker.Ui;

/// <summary>
/// A station's screen over the hub: the hub dimmed by a one-in-two pixel dither (exact palette, no translucency),
/// a framed panel with a title, a message line and a close button, and a body the screen lays out in whole pixels.
/// The panel fills the game area less a 2-pixel margin, so the body is <see cref="BodyW"/> x <see cref="BodyH"/>:
/// 677x276 on a 2772x1280 phone, 624x316 at 640x360, never less than 517x256; screens lay out from those
/// sizes and page what does not fit. A resize lays the body out again. A tap on the dimmed hub, the X, the Android
/// back button or Escape closes it. Screens change the save only through <see cref="AltarRules"/>, then call
/// <see cref="Commit"/>, which saves and redraws the body.
/// <example><code>var screen = new PoolScreen();
/// screen.Open(session);
/// screen.Closed += hub.RefreshState;
/// uiLayer.AddChild(screen);</code></example>
/// </summary>
public abstract partial class StationScreen : Control
{
	private const int Margin = 2, TitleBarH = 30, CloseW = 30;

	/// <summary>The body's width in game pixels (the panel's less its frame).</summary>
	protected int BodyW { get; private set; }
	/// <summary>The body's height in game pixels (the panel's less its title bar and frame).</summary>
	protected int BodyH { get; private set; }

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
	private PixelTile _close = null!;
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

		_panel = new PanelFrame { MouseFilter = MouseFilterEnum.Stop, TitleBarH = TitleBarH };
		AddChild(_panel);
		Ui.Text(_panel, Title, 8, 3, Pal.Bright, big: true);
		_msg = Ui.Text(_panel, "", MessageX, 7, Pal.Gold, width: 1, align: HorizontalAlignment.Right);
		_close = Ui.Tile(_panel, 0, 3, CloseW, 24, Close, "X", Pal.Mid, big: true);
		Body = new Control { Position = new Vector2(6, TitleBarH + 4), MouseFilter = MouseFilterEnum.Pass };
		_panel.AddChild(Body);

		GetViewport().SizeChanged += Relayout;
		Layout();
		Build();
		if (S.SaveNotice.Length > 0) Say(S.SaveNotice, Pal.Warn);
	}

	private int MessageX => 8 + Title.Length * Ui.BigW + 12;

	/// <summary>Sizes the panel to the game area and the body to the panel.</summary>
	private void Layout()
	{
		var area = GetViewportRect().Size;
		var panel = new Vector2I((int)area.X - 2 * Margin, (int)area.Y - 2 * Margin);
		_panel.Size = panel;
		_panel.Position = new Vector2(Margin, Margin);
		BodyW = panel.X - 12; BodyH = panel.Y - TitleBarH - 10;
		Body.Size = new Vector2(BodyW, BodyH);
		_close.Position = new Vector2(panel.X - CloseW - 4, 3);
		_msg.CustomMinimumSize = new Vector2(Math.Max(1, panel.X - MessageX - CloseW - 12), 0);
		_msg.Size = new Vector2(_msg.CustomMinimumSize.X, 0);
		_panel.QueueRedraw();
	}

	private void Relayout() { Layout(); Rebuild(); }

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
		GetViewport().SizeChanged -= Relayout;
		Closed?.Invoke();
		QueueFree();
	}

	/// <summary>The panel's frame: the outer line and an inner line two pixels in, as the browser build draws it, and a rule under the title bar.</summary>
	private sealed partial class PanelFrame : Control
	{
		public int TitleBarH { get; init; }

		public override void _Draw()
		{
			PixelTile.Frame(this, new Rect2I(0, 0, (int)Size.X, (int)Size.Y), Pal.Panel, Pal.Frame, Pal.Deep);
			DrawRect(new Rect2(4, TitleBarH + 1, Size.X - 8, 1), Pal.Deep);
		}
	}
}
