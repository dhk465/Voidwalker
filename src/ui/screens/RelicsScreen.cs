using System;
using System.Collections.Generic;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Relics: the slots at the top, every relic below (paged to fit the body). A tap on a relic found equips it (a
/// full set drops the oldest) or takes it off, and shows what it does and where it came from along the bottom.
/// </summary>
public partial class RelicsScreen : StationScreen
{
	private const int TileW = 84, TileH = 66, Gap = 4, GridY = 36, SlotSize = 30, DetailH = 52;
	private string? _sel;
	private int _page;

	/// <inheritdoc/>
	protected override string Title => "RELICS";

	/// <inheritdoc/>
	protected override void Build()
	{
		int slots = R.RelicSlots();
		Ui.Text(Body, $"SLOTS {slots}", 0, 7, Pal.Mid);
		for (int i = 0; i < 3; i++)
		{
			bool open = i < slots;
			string? id = i < R.Meta.Equipped.Count ? R.Meta.Equipped[i] : null;
			var slot = Ui.Tile(Body, 72 + i * (SlotSize + 6), 0, SlotSize, SlotSize, id != null ? () => Toggle(id) : null,
				colour: open ? (id != null ? Pal.Gold : Pal.Frame) : Pal.Deep);
			slot.Fill = open ? Pal.Ink : Pal.Abyss;
			if (id != null) Centre(slot, PixelIcons.Relic(id), SlotSize, SlotSize);
		}

		var ids = D.Relics.Ids;
		int cols = Ui.Fits(BodyW, TileW, Gap), rows = Math.Max(1, (BodyH - GridY - DetailH - 4 + Gap) / (TileH + Gap));
		int per = cols * rows, pages = Math.Max(1, (ids.Count + per - 1) / per);
		_page = Math.Min(_page, pages - 1);
		int x0 = (BodyW - (cols * (TileW + Gap) - Gap)) / 2;
		for (int j = 0; j < per && _page * per + j < ids.Count; j++)
		{
			string id = ids[_page * per + j];
			bool got = R.Meta.Relics.GetValueOrDefault(id) > 0, on = R.Meta.Equipped.Contains(id);
			var tile = Ui.Tile(Body, x0 + j % cols * (TileW + Gap), GridY + j / cols * (TileH + Gap), TileW, TileH, () => Toggle(id),
				colour: on ? Pal.Gold : got ? Pal.Dim : Pal.Deep);
			tile.Fill = on ? Pal.Chosen : Pal.Panel;
			if (got)
			{
				Centre(tile, PixelIcons.Relic(id), TileW, 30);
				Ui.Text(tile, D.Relics[id].Name, 2, 30, on ? Pal.Gold : Pal.Soft, width: TileW - 4, align: HorizontalAlignment.Center);
			}
			else Ui.Text(tile, "?", 0, (TileH - Ui.BigSize) / 2, Pal.Line, big: true, width: TileW, align: HorizontalAlignment.Center);
		}
		int pagerW = Math.Min(200, BodyW - 190);
		Ui.Pager(Body, BodyW - pagerW, 4, pagerW, _page, pages, p => { _page = p; Rebuild(); });

		int detailY = BodyH - DetailH;
		if (_sel != null && R.Meta.Relics.GetValueOrDefault(_sel) > 0)
		{
			Ui.Text(Body, D.Relics[_sel].Says, 0, detailY, Pal.Bright, width: BodyW, align: HorizontalAlignment.Center);
			Ui.Text(Body, D.Relics[_sel].Lore, 0, detailY + Ui.SmallSize + 2, Pal.Mid, width: BodyW, align: HorizontalAlignment.Center);
		}
		else Ui.Text(Body, "Tap a relic you have found to equip it or take it off.", 0, detailY + 8, Pal.Mid, width: BodyW, align: HorizontalAlignment.Center);
	}

	private void Toggle(string id)
	{
		if (R.Meta.Relics.GetValueOrDefault(id) <= 0) { _sel = null; Say("NOT FOUND YET", Pal.Mid); Rebuild(); return; }
		_sel = id;
		R.ToggleRelic(id);
		Say("");
		Commit();
	}

	private static void Centre(Control on, Texture2D icon, int w, int h) =>
		Ui.Icon(on, icon, (w - icon.GetWidth()) / 2, (h - icon.GetHeight()) / 2);
}
