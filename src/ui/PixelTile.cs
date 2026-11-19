using System;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// A hard-edged pixel frame that can be tapped: a one-pixel border with notched corners round a flat fill, an
/// optional inner line two pixels in, all in exact palette colours and on whole pixels. The building block of the
/// station screens (tiles, buttons, rows). A tap counts when the finger lifts inside the frame.
/// <example><code>var tile = new PixelTile { Position = new Vector2(0, 0), Size = new Vector2(56, 64), Border = new Color("#ff4b3a") };
/// tile.Tapped += () => GD.Print("tapped");
/// body.AddChild(tile);</code></example>
/// </summary>
public partial class PixelTile : Control
{
	/// <summary>Raised when a tap lands and lifts inside the frame while enabled.</summary>
	public event Action? Tapped;

	/// <summary>The fill.</summary>
	public Color Fill { get => _fill; set { _fill = value; QueueRedraw(); } }
	/// <summary>The border line.</summary>
	public Color Border { get => _border; set { _border = value; QueueRedraw(); } }
	/// <summary>A second line two pixels inside the border, or none.</summary>
	public Color? Inner { get => _inner; set { _inner = value; QueueRedraw(); } }
	/// <summary>Whether taps count (a disabled tile still swallows them).</summary>
	public bool Enabled { get; set; } = true;
	/// <summary>Extra drawing on top of the frame (icons and pips), in the tile's own pixels.</summary>
	public Action<PixelTile>? Paint { get; set; }

	private Color _fill = Pal.Ink, _border = Pal.Line;
	private Color? _inner;
	private bool _down, _hover;

	/// <inheritdoc/>
	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;
		MouseEntered += () => { _hover = true; QueueRedraw(); };
		MouseExited += () => { _hover = false; _down = false; QueueRedraw(); };
	}

	/// <inheritdoc/>
	public override void _GuiInput(InputEvent e)
	{
		if (e is not InputEventMouseButton { ButtonIndex: MouseButton.Left } b) return;
		AcceptEvent();
		if (b.Pressed) { _down = true; QueueRedraw(); return; }
		bool inside = new Rect2(Vector2.Zero, Size).HasPoint(b.Position);
		bool fire = _down && inside && Enabled;
		_down = false; QueueRedraw();
		if (fire) Tapped?.Invoke();
	}

	/// <inheritdoc/>
	public override void _Draw()
	{
		int w = (int)Size.X, h = (int)Size.Y;
		var fill = _down && Enabled ? Pal.Deep : Fill;
		var border = _hover && Enabled && Tapped != null ? Lighter(Border) : Border;
		Frame(this, new Rect2I(0, 0, w, h), fill, border, Inner);
		Paint?.Invoke(this);
	}

	/// <summary>Draws a notched frame: fill inside, border with its four corner pixels left out, optional inner line.</summary>
	public static void Frame(CanvasItem on, Rect2I r, Color fill, Color border, Color? inner = null)
	{
		int x = r.Position.X, y = r.Position.Y, w = r.Size.X, h = r.Size.Y;
		if (w < 3 || h < 3) return;
		on.DrawRect(new Rect2(x + 1, y + 1, w - 2, h - 2), fill);
		Notched(on, x, y, w, h, border);
		if (inner is { } c && w > 7 && h > 7) Notched(on, x + 2, y + 2, w - 4, h - 4, c);
	}

	private static void Notched(CanvasItem on, int x, int y, int w, int h, Color c)
	{
		on.DrawRect(new Rect2(x + 1, y, w - 2, 1), c);
		on.DrawRect(new Rect2(x + 1, y + h - 1, w - 2, 1), c);
		on.DrawRect(new Rect2(x, y + 1, 1, h - 2), c);
		on.DrawRect(new Rect2(x + w - 1, y + 1, 1, h - 2), c);
	}

	// Hover brightens a border one step along the palette's grey-violet ramp; other colours stay (they already stand out).
	private static Color Lighter(Color c)
	{
		if (c == Pal.Line) return Pal.Dim;
		if (c == Pal.Dim) return Pal.Mid;
		if (c == Pal.Mid) return Pal.Soft;
		return c;
	}
}
