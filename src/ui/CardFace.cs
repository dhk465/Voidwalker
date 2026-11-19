using System;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Ui;

/// <summary>
/// A card as a tile: framed in its recipe's colour (a fusion gets a second inner line), its nature sigils at 1x
/// (each in its own nature's colour), its name, and a pip per level.
/// <example><code>CardFace.Make(body, rules, new[] { "wrath" }, 2, 0, 0, 88, 64, () => GD.Print("tapped")); // 10 letters of name fit 88 px</code></example>
/// </summary>
public static class CardFace
{
	/// <summary>Puts a card tile on <paramref name="parent"/>; <paramref name="count"/> above 0 shows the copies owned.</summary>
	public static PixelTile Make(Control parent, AltarRules rules, string[] natures, int level, float x, float y, float w, float h,
		Action? tapped = null, bool chosen = false, int count = 0)
	{
		var colour = new Color(rules.CardColor(natures));
		var tile = Ui.Tile(parent, x, y, w, h, tapped, colour: colour);
		tile.Fill = chosen ? Pal.Chosen : Pal.Panel;
		if (natures.Length > 1) tile.Inner = colour;
		var data = rules.Data;
		int iconY = (int)Math.Floor(h * 0.2f); // sigils, then the name (16 rows) below them, pips along the bottom
		tile.Paint = t =>
		{
			int s = PixelIcons.SigilSize, total = natures.Length == 0 ? s : natures.Length * (s + 2) - 2;
			int x0 = ((int)w - total) / 2;
			if (natures.Length == 0) t.DrawTexture(PixelIcons.Sigil("", colour), new Vector2(x0, iconY));
			for (int i = 0; i < natures.Length; i++)
				t.DrawTexture(PixelIcons.Sigil(natures[i], new Color(data.Natures[natures[i]].Color)), new Vector2(x0 + i * (s + 2), iconY));
			int pipsW = level * 5 - 2, px = ((int)w - pipsW) / 2;
			for (int i = 0; i < level; i++) t.DrawTexture(PixelIcons.Pip(colour), new Vector2(px + i * 5, (int)h - 8));
		};
		string name = rules.RecipeOf(natures)?.Name ?? "Unstable";
		Ui.Text(tile, Ui.FitPx(name, w - 4), 0, iconY + 12, colour, width: w, align: HorizontalAlignment.Center);
		if (count > 0) Ui.Text(tile, $"{count}", 0, 1, Pal.Mid, width: w - 4, align: HorizontalAlignment.Right);
		return tile;
	}
}
