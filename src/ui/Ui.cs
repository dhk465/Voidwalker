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
/// Pixel-font text and frames for the station screens and the HUD. Spleen at its native sizes only (8x16 at 16 for
/// body text, 12x24 at 24 for titles), with no smoothing, hinting or subpixel placement, so every glyph pixel lands
/// on a game pixel. Sized for phones: at the 4x zoom a 2772x1280 phone gets, body text is 64 screen pixels tall.
/// <example><code>Ui.Text(body, "POOL", 0, 0, Pal.Bright, big: true);
/// Ui.Text(body, longText, 0, 20, Pal.Mid, width: 200); // wraps at 200 px</code></example>
/// </summary>
public static class Ui
{
	/// <summary>Body text's line height: Spleen 8x16's native size.</summary>
	public const int SmallSize = 16;
	/// <summary>Title text's line height: Spleen 12x24's native size.</summary>
	public const int BigSize = 24;
	/// <summary>Body text's character width (monospaced).</summary>
	public const int SmallW = 8;
	/// <summary>Title text's character width (monospaced).</summary>
	public const int BigW = 12;

	private static FontFile? _small, _big;

	/// <summary>Spleen 8x16, the body font.</summary>
	public static FontFile Small => _small ??= LoadFont("res://assets/fonts/spleen-8x16.otf");
	/// <summary>Spleen 12x24, the title font.</summary>
	public static FontFile Big => _big ??= LoadFont("res://assets/fonts/spleen-12x24.otf");

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
	public static string Fit(string text, int chars) => text.Length <= chars ? text : text[..System.Math.Max(0, chars)];

	/// <summary>Cuts text to fit <paramref name="width"/> pixels in the body (or title) font.</summary>
	public static string FitPx(string text, float width, bool big = false) => Fit(text, (int)width / (big ? BigW : SmallW));

	/// <summary>How many tiles of <paramref name="size"/> with <paramref name="gap"/> between them fit in <paramref name="room"/> (at least 1).</summary>
	public static int Fits(float room, float size, float gap) => System.Math.Max(1, (int)((room + gap) / (size + gap)));

	/// <summary>Page buttons and a "2/5" count across <paramref name="w"/> pixels; draws nothing for a single page.</summary>
	public static void Pager(Control parent, float x, float y, float w, int page, int pages, Action<int> go)
	{
		if (pages <= 1) return;
		Tile(parent, x, y, 32, PagerH, () => go(page - 1), "<", Pal.Void, page > 0);
		Text(parent, $"{page + 1}/{pages}", x + 32, y + 3, Pal.Mid, width: w - 64, align: HorizontalAlignment.Center);
		Tile(parent, x + w - 32, y, 32, PagerH, () => go(page + 1), ">", Pal.Void, page < pages - 1);
	}

	/// <summary>The height of <see cref="Pager"/>'s buttons.</summary>
	public const int PagerH = 22;

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
