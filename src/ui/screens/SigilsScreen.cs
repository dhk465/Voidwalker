using System;
using System.Collections.Generic;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Sigils: permanent upgrades bought with banked void energy, a level at a time, paged to fit the body; a second
/// tab lists what opens over time.
/// </summary>
public partial class SigilsScreen : StationScreen
{
	private const int MinTileW = 160, TileH = 58, Gap = 4, Top = 28;
	private bool _unlocks;
	private int _page;

	/// <inheritdoc/>
	protected override string Title => "SIGILS";

	/// <inheritdoc/>
	protected override void Build()
	{
		for (int i = 0; i < 2; i++)
		{
			bool tab = i == 1, on = _unlocks == tab;
			var t = Ui.Tile(Body, i * 104, 0, 100, 22, () => { _unlocks = tab; Rebuild(); }, tab ? "UNLOCKS" : "SIGILS", on ? Pal.Void : Pal.Mid);
			t.Fill = on ? Pal.Deep : Pal.Panel;
		}
		Ui.Text(Body, $"VOID {R.Meta.Bank}", 212, 3, Pal.Void, width: BodyW - 212, align: HorizontalAlignment.Right);
		if (_unlocks) Unlocks(); else Sigils();
	}

	private void Sigils()
	{
		var ids = D.Sigils.Ids;
		int cols = Ui.Fits(BodyW, MinTileW, Gap), tileW = (BodyW - (cols - 1) * Gap) / cols;
		int room = BodyH - Top, rows = Math.Max(1, (room + Gap) / (TileH + Gap));
		if (cols * rows < ids.Count) rows = Math.Max(1, (room - Ui.PagerH - 4 + Gap) / (TileH + Gap));
		int per = cols * rows, pages = Math.Max(1, (ids.Count + per - 1) / per);
		_page = Math.Min(_page, pages - 1);
		for (int j = 0; j < per && _page * per + j < ids.Count; j++)
		{
			string id = ids[_page * per + j];
			var sg = D.Sigils[id];
			int lv = R.Meta.Upg.GetValueOrDefault(id), levels = sg.Costs.Length;
			bool locked = R.SigilLocked(id);
			int? cost = R.SigilCost(id);
			var tile = Ui.Tile(Body, j % cols * (tileW + Gap), Top + j / cols * (TileH + Gap), tileW, TileH, () => Buy(id), colour: lv > 0 ? Pal.Gold : Pal.Line);
			tile.Paint = t =>
			{
				for (int k = 0; k < levels; k++) t.DrawTexture(PixelIcons.Pip(k < lv ? Pal.Gold : Pal.Line), new Vector2(tileW - 8 - (levels - k) * 5 + 2, 9));
			};
			Ui.Text(tile, sg.Name, 6, 3, lv > 0 ? Pal.GoldHi : Pal.Bright);
			int right = tileW - 10 - levels * 5;
			string price = locked ? "LOCKED" : cost is { } c ? $"{c}" : "MAX";
			var priceColour = cost == null ? (locked ? Pal.Dim : Pal.Gold) : R.Meta.Bank >= cost ? Pal.Void : Pal.Dim;
			Ui.Text(tile, price, right - 52, 3, priceColour, width: 50, align: HorizontalAlignment.Right);
			Ui.Text(tile, sg.Says, 6, 21, locked ? Pal.Dim : Pal.Soft, width: tileW - 12);
		}
		Ui.Pager(Body, 0, BodyH - Ui.PagerH, BodyW, _page, pages, p => { _page = p; Rebuild(); });
	}

	private void Unlocks()
	{
		var unlocks = D.Unlocks.Ids;
		int cols = BodyW >= 560 ? 2 : 1, colW = BodyW / cols;
		for (int i = 0; i < unlocks.Count; i++)
		{
			bool ok = R.Unlocked(unlocks[i]);
			int x = i % cols * colW, y = Top + 4 + i / cols * 22;
			Body.AddChild(new ColorRect { Position = new Vector2(x, y + 4), Size = new Vector2(8, 8), Color = ok ? Pal.Mint : Pal.Line, MouseFilter = MouseFilterEnum.Ignore });
			Ui.Text(Body, Ui.FitPx(D.Unlocks[unlocks[i]].Name, colW - 20), x + 14, y, ok ? Pal.Mint : Pal.Mid);
		}
	}

	private void Buy(string id)
	{
		var (message, bought) = R.BuySigil(id);
		Say(message, bought ? Pal.GoldHi : Pal.Warn);
		if (bought) Commit();
	}
}
