using System;
using System.Collections.Generic;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Sigils: permanent upgrades bought with banked void energy, a level at a time; below, what opens over time.
/// </summary>
public partial class SigilsScreen : StationScreen
{
	private const int Cols = 4, TileW = 151, TileH = 54, Gap = 4;

	/// <inheritdoc/>
	protected override string Title => "SIGILS";

	/// <inheritdoc/>
	protected override void Build()
	{
		var ids = D.Sigils.Ids;
		for (int i = 0; i < ids.Count; i++)
		{
			string id = ids[i];
			var sg = D.Sigils[id];
			int lv = R.Meta.Upg.GetValueOrDefault(id), levels = sg.Costs.Length;
			bool locked = R.SigilLocked(id);
			int? cost = R.SigilCost(id);
			var tile = Ui.Tile(Body, i % Cols * (TileW + Gap), i / Cols * (TileH + Gap), TileW, TileH, () => Buy(id), colour: lv > 0 ? Pal.Gold : Pal.Line);
			tile.Paint = t =>
			{
				for (int k = 0; k < levels; k++) t.DrawTexture(PixelIcons.Pip(k < lv ? Pal.Gold : Pal.Line), new Vector2(TileW - 8 - (levels - k) * 5 + 2, 7));
			};
			Ui.Text(tile, sg.Name, 6, 3, lv > 0 ? Pal.GoldHi : Pal.Bright);
			int right = TileW - 8 - levels * 5;
			string price = locked ? "LOCKED" : cost is { } c ? $"{c}" : "MAX";
			var priceColour = cost == null ? (locked ? Pal.Dim : Pal.Gold) : R.Meta.Bank >= cost ? Pal.Void : Pal.Dim;
			Ui.Text(tile, price, right - 46, 3, priceColour, width: 44, align: HorizontalAlignment.Right);
			Ui.Text(tile, sg.Says, 6, 19, locked ? Pal.Dim : Pal.Soft, width: TileW - 12);
		}

		Ui.Text(Body, "UNLOCKS", 0, 238, Pal.Bright);
		Ui.Text(Body, $"VOID {R.Meta.Bank}", 300, 238, Pal.Void, width: 316, align: HorizontalAlignment.Right);
		var unlocks = D.Unlocks.Ids;
		for (int i = 0; i < unlocks.Count; i++)
		{
			bool ok = R.Unlocked(unlocks[i]);
			int x = i % 2 * 308, y = 256 + i / 2 * 16;
			Body.AddChild(new ColorRect { Position = new Vector2(x, y + 3), Size = new Vector2(6, 6), Color = ok ? Pal.Mint : Pal.Line, MouseFilter = MouseFilterEnum.Ignore });
			Ui.Text(Body, D.Unlocks[unlocks[i]].Name, x + 12, y, ok ? Pal.Mint : Pal.Mid);
		}
	}

	private void Buy(string id)
	{
		var (message, bought) = R.BuySigil(id);
		Say(message, bought ? Pal.GoldHi : Pal.Warn);
		if (bought) Commit();
	}
}
