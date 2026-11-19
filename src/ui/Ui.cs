using System;
using Godot;

namespace Voidwalker.Ui;

/// <summary>The station screens' colours: every one is in the game's palette (src/data/game_data.json "palette").</summary>
public static class Pal
{
	/// <summary>Deepest background.</summary>
	public static readonly Color Abyss = new("#07040d");
	/// <summary>Panel fill.</summary>
	public static readonly Color Panel = new("#0e0a18");
	/// <summary>Tile fill.</summary>
	public static readonly Color Ink = new("#1a1229");
	/// <summary>Pressed or chosen fill; inner frame line.</summary>
	public static readonly Color Deep = new("#241a3a");
	/// <summary>Selected fill.</summary>
	public static readonly Color Chosen = new("#2a1636");
	/// <summary>Quiet lines; unknown things.</summary>
	public static readonly Color Line = new("#3b3350");
	/// <summary>Disabled text.</summary>
	public static readonly Color Dim = new("#6d6488");
	/// <summary>Secondary text.</summary>
	public static readonly Color Mid = new("#9d92bb");
	/// <summary>Body text.</summary>
	public static readonly Color Soft = new("#cfc6e6");
	/// <summary>Headings.</summary>
	public static readonly Color Bright = new("#efe6ff");
	/// <summary>The panel's outer frame.</summary>
	public static readonly Color Frame = new("#4a3a7a");
	/// <summary>Void lavender.</summary>
	public static readonly Color Void = new("#d8c8ff");
	/// <summary>Progress fill.</summary>
	public static readonly Color Violet = new("#7b5bdf");
	/// <summary>Gold: rewards, levels, mythics.</summary>
	public static readonly Color Gold = new("#ffd27a");
	/// <summary>Pale gold.</summary>
	public static readonly Color GoldHi = new("#fff0b0");
	/// <summary>Good news; cards in the pool.</summary>
	public static readonly Color Mint = new("#c8ffe8");
	/// <summary>Warnings.</summary>
	public static readonly Color Warn = new("#ff8a8a");
	/// <summary>Rare cards.</summary>
	public static readonly Color Rare = new("#9ad7ff");
	/// <summary>A log's paper.</summary>
	public static readonly Color Paper = new("#e8e6c0");
	/// <summary>Ink on paper.</summary>
	public static readonly Color PaperInk = new("#2c2520");
	/// <summary>Faded ink on paper.</summary>
	public static readonly Color PaperDim = new("#6e6156");
	/// <summary>Logs found.</summary>
	public static readonly Color Teal = new("#5ac8b9");
}

/// <summary>
/// Pixel-font text and frames for the station screens. Spleen at its native sizes only (6x12 at 12, 8x16 at 16),
/// with no smoothing, hinting or subpixel placement, so every glyph pixel lands on a game pixel.
/// <example><code>Ui.Text(body, "POOL", 0, 0, Pal.Bright, big: true);
/// Ui.Text(body, longText, 0, 20, Pal.Mid, width: 200); // wraps at 200 px</code></example>
/// </summary>
public static class Ui
{
	/// <summary>Spleen 6x12's native size.</summary>
	public const int SmallSize = 12;
	/// <summary>Spleen 8x16's native size.</summary>
	public const int BigSize = 16;

	private static FontFile? _small, _big;

	/// <summary>Spleen 6x12.</summary>
	public static FontFile Small => _small ??= LoadFont("res://assets/fonts/spleen-6x12.otf");
	/// <summary>Spleen 8x16.</summary>
	public static FontFile Big => _big ??= LoadFont("res://assets/fonts/spleen-8x16.otf");

	private static FontFile LoadFont(string path)
	{
		var font = GD.Load<FontFile>(path);
		font.Antialiasing = TextServer.FontAntialiasing.None;
		font.Hinting = TextServer.Hinting.None;
		font.SubpixelPositioning = TextServer.SubpixelPositioning.Disabled;
		font.GenerateMipmaps = false;
		return font;
	}

	/// <summary>A label in a pixel font; with a width it wraps (or aligns) within it.</summary>
	public static Label Text(Control parent, string text, float x, float y, Color colour, bool big = false, float width = 0, HorizontalAlignment align = HorizontalAlignment.Left)
	{
		// Order matters: a label measures its text when the text is set, so the font and wrapping come first and the
		// text last; otherwise it sizes itself to the text in Godot's default font and never shrinks back to wrap.
		var label = new Label { MouseFilter = Control.MouseFilterEnum.Ignore, HorizontalAlignment = align, Position = new Vector2(x, y) };
		label.AddThemeFontOverride("font", big ? Big : Small);
		label.AddThemeFontSizeOverride("font_size", big ? BigSize : SmallSize);
		label.AddThemeColorOverride("font_color", colour);
		label.AddThemeConstantOverride("line_spacing", 0);
		if (width > 0) { label.AutowrapMode = TextServer.AutowrapMode.WordSmart; label.CustomMinimumSize = new Vector2(width, 0); }
		parent.AddChild(label);
		label.Text = text;
		label.Size = new Vector2(width > 0 ? width : 0, 0); // shrink to the minimum: the width asked for, the lines it needs
		return label;
	}

	/// <summary>Cuts text to fit <paramref name="chars"/> characters.</summary>
	public static string Fit(string text, int chars) => text.Length <= chars ? text : text[..chars];

	/// <summary>A tappable frame at a place, with an optional centred caption.</summary>
	public static PixelTile Tile(Control parent, float x, float y, float w, float h, Action? tapped = null, string caption = "", Color? colour = null, bool enabled = true, bool big = false)
	{
		var tile = new PixelTile { Position = new Vector2(x, y), Size = new Vector2(w, h), Enabled = enabled };
		if (tapped != null) tile.Tapped += tapped;
		if (colour is { } c) tile.Border = enabled ? c : Pal.Line;
		parent.AddChild(tile);
		if (caption.Length > 0)
		{
			int lineH = big ? BigSize : SmallSize;
			Text(tile, caption, 0, Mathf.Floor((h - lineH) / 2), enabled ? (colour ?? Pal.Bright) : Pal.Dim, big, w, HorizontalAlignment.Center);
		}
		return tile;
	}

	/// <summary>A one-pixel-high progress bar.</summary>
	public static void Bar(Control parent, float x, float y, float w, float h, double fraction, Color fill)
	{
		parent.AddChild(new ColorRect { Position = new Vector2(x, y), Size = new Vector2(w, h), Color = Pal.Deep, MouseFilter = Control.MouseFilterEnum.Ignore });
		int fw = (int)Math.Floor(w * Math.Clamp(fraction, 0, 1));
		if (fw > 0) parent.AddChild(new ColorRect { Position = new Vector2(x, y), Size = new Vector2(fw, h), Color = fill, MouseFilter = Control.MouseFilterEnum.Ignore });
	}

	/// <summary>An icon at 1x (never scaled).</summary>
	public static TextureRect Icon(Control parent, Texture2D texture, float x, float y)
	{
		var icon = new TextureRect { Texture = texture, Position = new Vector2(x, y), MouseFilter = Control.MouseFilterEnum.Ignore, StretchMode = TextureRect.StretchModeEnum.Keep };
		parent.AddChild(icon);
		return icon;
	}
}
