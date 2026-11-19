using System;
using System.Collections.Generic;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Ui;

/// <summary>
/// The Pool (the Deck station): the collection on the left, a tap puts a copy into the pool taken into the rift; the
/// pool on the right, a tap on a row takes a copy out; at the bottom right, the form the pool's leading nature gives
/// the blob. Both lists page to fit the body.
/// </summary>
public partial class PoolScreen : StationScreen
{
	private const int TileW = 84, TileH = 64, GapX = 6, UnderH = 18, RowH = 18, RowStep = 20, FormH = 80, Gap = 10;
	private int _page, _poolPage;
	private bool _hinted;

	/// <inheritdoc/>
	protected override string Title => "POOL";

	/// <inheritdoc/>
	protected override void Build()
	{
		if (!_hinted) { _hinted = true; Say("Tap a card to add it, a row to take it out", Pal.Mid); }
		int rightW = BodyW >= 600 ? 260 : 230, rightX = BodyW - rightW, leftW = rightX - Gap;

		// the collection
		var list = AltarRules.Stacks(R.Meta.Cards);
		int cols = Ui.Fits(leftW, TileW, GapX), rows = Math.Max(1, (BodyH - Ui.PagerH - 4) / (TileH + UnderH));
		int per = cols * rows, pages = Math.Max(1, (list.Count + per - 1) / per);
		_page = Math.Min(_page, pages - 1);
		for (int i = 0; i < per && _page * per + i < list.Count; i++)
		{
			var st = list[_page * per + i];
			int x = i % cols * (TileW + GapX), y = i / cols * (TileH + UnderH);
			int inPool = R.Meta.Loadout.GetValueOrDefault(st.Sk);
			CardFace.Make(Body, R, st.Natures, st.Level, x, y, TileW, TileH, () => Add(st.Sk), chosen: inPool > 0, count: st.N);
			if (inPool > 0) Ui.Text(Body, $"{inPool} IN", x, y + TileH + 1, Pal.Mint, width: TileW, align: HorizontalAlignment.Center);
		}
		Ui.Pager(Body, 0, BodyH - Ui.PagerH, cols * (TileW + GapX) - GapX, _page, pages, p => { _page = p; Rebuild(); });

		// the pool
		int count = R.DeckCount();
		Ui.Text(Body, "IN THE POOL", rightX, 5, Pal.Mid);
		Ui.Text(Body, $"{count} / {R.DeckMax()}", rightX, 0, count < R.DeckMin() ? Pal.Warn : Pal.Bright, big: true, width: rightW, align: HorizontalAlignment.Right);
		var pool = AltarRules.Stacks(R.Meta.Loadout);
		int rowsTop = 30, room = BodyH - FormH - 6 - rowsTop;
		int poolPer = Math.Max(1, room / RowStep);
		if (pool.Count > poolPer) poolPer = Math.Max(1, (room - Ui.PagerH - 4) / RowStep);
		int poolPages = Math.Max(1, (pool.Count + poolPer - 1) / poolPer);
		_poolPage = Math.Min(_poolPage, poolPages - 1);
		int start = _poolPage * poolPer;
		for (int i = 0; i < poolPer && start + i < pool.Count; i++)
		{
			var st = pool[start + i];
			var colour = new Color(R.CardColor(st.Natures));
			var row = Ui.Tile(Body, rightX, rowsTop + i * RowStep, rightW, RowH, () => { R.RemoveFromPool(st.Sk); Commit(); }, colour: colour);
			string nature = st.Natures.Length > 0 ? st.Natures[0] : "";
			row.Paint = t => t.DrawTexture(PixelIcons.Sigil(nature, colour), new Vector2(4, 4));
			Ui.Text(row, Ui.FitPx(R.CardName(st.Natures, st.Level) + (st.N > 1 ? $" x{st.N}" : ""), rightW - 22), 17, 1, colour);
		}
		Ui.Pager(Body, rightX, BodyH - FormH - 6 - Ui.PagerH, rightW, _poolPage, poolPages, p => { _poolPage = p; Rebuild(); });

		// the form the pool gives the blob
		string? lead = R.DominantNature();
		var formColour = lead != null ? new Color(D.Natures[lead].Color) : new Color("#b9a6ff");
		var form = Ui.Tile(Body, rightX, BodyH - FormH, rightW, FormH, colour: formColour);
		form.Fill = Pal.Panel;
		Ui.Icon(form, GD.Load<Texture2D>($"res://assets/sprites/lobby/form-{R.DominantForm()}.png"), 4, (FormH - 46) / 2); // 46x46
		Ui.Text(form, lead != null ? D.Natures[lead].FormName : "Shapeless", 54, 4, formColour, big: true);
		Ui.Text(form, lead != null ? D.Natures[lead].TraitText : "no trait", 54, 30, Pal.Soft, width: rightW - 60);
	}

	private void Add(string sk)
	{
		string message = R.AddToPool(sk);
		if (message.Length > 0) { Say(message); return; }
		Say("");
		Commit();
	}
}
