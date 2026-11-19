using System;
using System.Collections.Generic;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Ui;

/// <summary>
/// The Pool (the Deck station): the collection on the left, a tap puts a copy into the pool taken into the rift; the
/// pool on the right, a tap on a row takes a copy out. Below, the form the pool's leading nature gives the blob.
/// </summary>
public partial class PoolScreen : StationScreen
{
	private const int Cols = 5, Rows = 3, TileW = 64, TileH = 64, GapX = 6, GapY = 16, RightX = 356, RightW = BodyW - RightX, RowW = 128;
	private int _page, _poolPage;
	private const int PoolPageSize = 16;

	/// <inheritdoc/>
	protected override string Title => "POOL";

	/// <inheritdoc/>
	protected override void Build()
	{
		var list = AltarRules.Stacks(R.Meta.Cards);
		int per = Cols * Rows, pages = Math.Max(1, (list.Count + per - 1) / per);
		_page = Math.Min(_page, pages - 1);
		for (int i = 0; i < per && _page * per + i < list.Count; i++)
		{
			var st = list[_page * per + i];
			int x = i % Cols * (TileW + GapX), y = i / Cols * (TileH + GapY);
			int inPool = R.Meta.Loadout.GetValueOrDefault(st.Sk);
			CardFace.Make(Body, R, st.Natures, st.Level, x, y, TileW, TileH, () => Add(st.Sk), chosen: inPool > 0, count: st.N);
			if (inPool > 0) Ui.Text(Body, $"{inPool} IN", x, y + TileH + 1, Pal.Mint, width: TileW, align: HorizontalAlignment.Center);
		}
		if (pages > 1)
		{
			Ui.Tile(Body, 0, 238, 40, 22, () => { _page--; Rebuild(); }, "<", Pal.Void, _page > 0);
			Ui.Text(Body, $"{_page + 1}/{pages}", 0, 243, Pal.Mid, width: 344, align: HorizontalAlignment.Center);
			Ui.Tile(Body, 304, 238, 40, 22, () => { _page++; Rebuild(); }, ">", Pal.Void, _page < pages - 1);
		}

		// the pool
		int count = R.DeckCount();
		Ui.Text(Body, "IN THE POOL", RightX, 2, Pal.Mid);
		Ui.Text(Body, $"{count} / {R.DeckMax()}", RightX + 72, 0, count < R.DeckMin() ? Pal.Warn : Pal.Bright, big: true);
		var pool = AltarRules.Stacks(R.Meta.Loadout);
		int poolPages = Math.Max(1, (pool.Count + PoolPageSize - 1) / PoolPageSize);
		_poolPage = Math.Min(_poolPage, poolPages - 1);
		int start = _poolPage * PoolPageSize;
		for (int i = 0; i < PoolPageSize && start + i < pool.Count; i++)
		{
			var st = pool[start + i];
			var colour = new Color(R.CardColor(st.Natures));
			var row = Ui.Tile(Body, RightX + i % 2 * (RowW + 4), 20 + i / 2 * 17, RowW, 15, () => { R.RemoveFromPool(st.Sk); Commit(); }, colour: colour);
			string nature = st.Natures.Length > 0 ? st.Natures[0] : "";
			row.Paint = t => t.DrawTexture(PixelIcons.Sigil(nature, colour), new Vector2(3, 3));
			Ui.Text(row, Ui.Fit(R.CardName(st.Natures, st.Level) + (st.N > 1 ? $" x{st.N}" : ""), (RowW - 18) / 6), 15, 1, colour);
		}

		// the form the pool gives the blob
		string? lead = R.DominantNature();
		var formColour = lead != null ? new Color(D.Natures[lead].Color) : new Color("#b9a6ff");
		if (poolPages > 1)
		{
			Ui.Tile(Body, RightX, 158, 32, 18, () => { _poolPage--; Rebuild(); }, "<", Pal.Void, _poolPage > 0);
			Ui.Text(Body, $"{_poolPage + 1}/{poolPages}", RightX + 32, 161, Pal.Mid, width: RightW - 64, align: HorizontalAlignment.Center);
			Ui.Tile(Body, BodyW - 32, 158, 32, 18, () => { _poolPage++; Rebuild(); }, ">", Pal.Void, _poolPage + 1 < poolPages);
		}
		int formY = poolPages > 1 ? 182 : Math.Max(146, 24 + (pool.Count + 1) / 2 * 17);
		var form = Ui.Tile(Body, RightX, formY, RightW, 92, colour: formColour);
		form.Fill = Pal.Panel;
		Ui.Icon(form, GD.Load<Texture2D>("res://assets/sprites/lobby/blob.png"), 10, 30);
		Ui.Text(form, lead != null ? D.Natures[lead].FormName : "Shapeless", 60, 8, formColour, big: true);
		Ui.Text(form, lead != null ? D.Natures[lead].TraitText : "no trait", 60, 30, Pal.Soft, width: RightW - 68);
		Ui.Text(Body, "Tap a card to put it in the pool. Tap a row to take it out.", RightX, formY + 100, Pal.Mid, width: RightW);
	}

	private void Add(string sk)
	{
		string message = R.AddToPool(sk);
		if (message.Length > 0) { Say(message); return; }
		Say("");
		Commit();
	}
}
