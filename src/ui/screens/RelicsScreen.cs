using System.Collections.Generic;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Relics: the slots at the top, every relic below. A tap on a relic found equips it (a full set drops the
/// oldest) or takes it off, and shows what it does and where it came from.
/// </summary>
public partial class RelicsScreen : StationScreen
{
	private const int Cols = 8, TileW = 73, TileH = 70, Gap = 4, GridY = 40, SlotSize = 30;
	private string? _sel;

	/// <inheritdoc/>
	protected override string Title => "RELICS";

	/// <inheritdoc/>
	protected override void Build()
	{
		int slots = R.RelicSlots(), x0 = (BodyW - (3 * SlotSize + 2 * 6)) / 2;
		for (int i = 0; i < 3; i++)
		{
			bool open = i < slots;
			string? id = i < R.Meta.Equipped.Count ? R.Meta.Equipped[i] : null;
			var slot = Ui.Tile(Body, x0 + i * (SlotSize + 6), 0, SlotSize, SlotSize, id != null ? () => Toggle(id) : null,
				colour: open ? (id != null ? Pal.Gold : Pal.Frame) : Pal.Deep);
			slot.Fill = open ? Pal.Ink : Pal.Abyss;
			if (id != null) Centre(slot, PixelIcons.Relic(id), SlotSize, SlotSize);
		}
		Ui.Text(Body, $"SLOTS {slots}", 0, 9, Pal.Mid);

		var ids = D.Relics.Ids;
		for (int i = 0; i < ids.Count; i++)
		{
			string id = ids[i];
			bool got = R.Meta.Relics.GetValueOrDefault(id) > 0, on = R.Meta.Equipped.Contains(id);
			var tile = Ui.Tile(Body, 2 + i % Cols * (TileW + Gap), GridY + i / Cols * (TileH + Gap), TileW, TileH, () => Toggle(id),
				colour: on ? Pal.Gold : got ? Pal.Dim : Pal.Deep);
			tile.Fill = on ? Pal.Chosen : Pal.Panel;
			if (got)
			{
				Centre(tile, PixelIcons.Relic(id), TileW, 32);
				Ui.Text(tile, D.Relics[id].Name, 2, 34, on ? Pal.Gold : Pal.Soft, width: TileW - 4, align: HorizontalAlignment.Center);
			}
			else Ui.Text(tile, "?", 0, 26, Pal.Line, big: true, width: TileW, align: HorizontalAlignment.Center);
		}

		if (_sel != null && R.Meta.Relics.GetValueOrDefault(_sel) > 0)
		{
			Ui.Text(Body, D.Relics[_sel].Says, 0, 196, Pal.Bright, width: BodyW, align: HorizontalAlignment.Center);
			Ui.Text(Body, D.Relics[_sel].Lore, 40, 214, Pal.Mid, width: BodyW - 80, align: HorizontalAlignment.Center);
		}
		else Ui.Text(Body, "Tap a relic you have found to equip it or take it off.", 0, 204, Pal.Mid, width: BodyW, align: HorizontalAlignment.Center);
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
