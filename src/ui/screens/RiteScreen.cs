using System;
using System.Collections.Generic;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Ui;

/// <summary>
/// The Void Rite: focus a nature, then draw one card or ten. The cards rise one by one (a tap shows them all at
/// once); a rare is framed in pale blue, a mythic in gold with a two-frame flash.
/// </summary>
public partial class RiteScreen : StationScreen
{
	private const double Step = 0.3, FlashTime = 0.1;
	private const int CardW = 56, CardH = 72, CardGap = 8, RevealY = 92;
	private List<Reveal> _reveal = new();
	private double _t, _flash;
	private int _shown;
	private ColorRect? _flashRect;

	/// <inheritdoc/>
	protected override string Title => "VOID RITE";

	/// <inheritdoc/>
	protected override void Build()
	{
		Ui.Text(Body, "FOCUS", 0, 0, Pal.Mid);
		for (int i = 0; i < R.Meta.Discovered.Count; i++)
		{
			string n = R.Meta.Discovered[i];
			bool on = R.Meta.Focus == n;
			var colour = new Color(D.Natures[n].Color);
			var tile = Ui.Tile(Body, i * 34, 14, 30, 26, () => { R.ToggleFocus(n); Commit(); }, colour: on ? colour : Pal.Line);
			tile.Fill = on ? Pal.Chosen : Pal.Ink;
			tile.Paint = t => t.DrawTexture(PixelIcons.Sigil(n, colour), new Vector2(10, 8));
		}
		int one = R.RiteCost(1), ten = R.RiteCost(10);
		Ui.Tile(Body, 0, 50, 128, 26, () => DrawCards(1), $"RITE  {one}", Pal.Void, R.Meta.Bank >= one, big: true);
		Ui.Tile(Body, 136, 50, 128, 26, () => DrawCards(10), $"X10  {ten}", Pal.Gold, R.Meta.Bank >= ten, big: true);
		Ui.Text(Body, $"VOID {R.Meta.Bank}", 300, 50, Pal.Void, width: 316, align: HorizontalAlignment.Right);
		Ui.Text(Body, $"MYTHIC IN {R.MythicIn()}", 300, 64, Pal.Mid, width: 316, align: HorizontalAlignment.Right);

		// a tap anywhere on the reveal shows every card at once
		var skip = new Control { Position = new Vector2(0, RevealY - 6), Size = new Vector2(BodyW, BodyH - RevealY + 6), MouseFilter = MouseFilterEnum.Stop };
		skip.GuiInput += e => { if (e is InputEventMouseButton { Pressed: true } && _shown < _reveal.Count) { _t = 1e6; } };
		Body.AddChild(skip);

		int cols = Math.Min(5, Math.Max(1, _reveal.Count));
		int x0 = (BodyW - (cols * (CardW + CardGap) - CardGap)) / 2;
		for (int i = 0; i < _shown; i++)
		{
			var r = _reveal[i];
			int x = x0 + i % 5 * (CardW + CardGap), y = RevealY + i / 5 * (CardH + 30);
			var card = CardFace.Make(Body, R, r.Natures, r.Level, x, y, CardW, CardH);
			card.MouseFilter = MouseFilterEnum.Ignore;
			if (r.Rarity == "mythic") card.Inner = Pal.Gold;
			else if (r.Rarity == "rare") card.Inner = Pal.Rare;
			string label = r.Rarity == "common" ? "" : r.Rarity.ToUpperInvariant();
			if (label.Length > 0) Ui.Text(Body, label, x - 8, y + CardH + 2, r.Rarity == "mythic" ? Pal.Gold : Pal.Rare, width: CardW + 16, align: HorizontalAlignment.Center);
			if (r.IsNew)
			{
				var badge = Ui.Tile(Body, x + CardW - 24, y - 6, 28, 14);
				badge.Fill = Pal.Gold; badge.Border = Pal.Gold; badge.MouseFilter = MouseFilterEnum.Ignore;
				Ui.Text(badge, "NEW", 0, 1, Pal.Abyss, width: 28, align: HorizontalAlignment.Center);
			}
		}
	}

	private void DrawCards(int n)
	{
		var drawn = R.RiteDraw(n, out string message);
		if (message.Length > 0) { Say(message, Pal.Warn); return; }
		Say("");
		_reveal = drawn; _t = 0; _shown = 0;
		Commit();
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		if (_flash > 0) { _flash -= delta; if (_flash <= 0 && _flashRect != null) _flashRect.Visible = false; }
		if (_shown >= _reveal.Count) return;
		_t += delta;
		int due = Math.Min(_reveal.Count, (int)Math.Floor(_t / Step));
		if (due <= _shown) return;
		bool mythic = false;
		for (int i = _shown; i < due; i++) mythic |= _reveal[i].Rarity == "mythic";
		_shown = due;
		Rebuild();
		if (mythic && _t < 1e5) Flash();
	}

	private void Flash()
	{
		if (_flashRect == null)
		{
			_flashRect = new ColorRect { Color = Pal.GoldHi, MouseFilter = MouseFilterEnum.Ignore };
			_flashRect.SetAnchorsPreset(LayoutPreset.FullRect);
			AddChild(_flashRect);
		}
		_flashRect.Visible = true; _flash = FlashTime;
	}
}
