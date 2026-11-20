using System;
using System.Linq;
using Godot;
using Voidwalker.Core;
using Voidwalker.Ui;
using UiKit = Voidwalker.Ui.Ui;

namespace Voidwalker.Adventure;

/// <summary>
/// The pick, over the rift while it holds still: after a cleared wave or a level-up. Take one of up to three cards (or a
/// temper, +1 to a deck card), or skip; and spend scrap, the adventure's own currency, on run-only boosts or on buying a
/// card back from the pool prepared at the Altar. It rebuilds after every purchase; taking or skipping closes it.
/// <example><code>var panel = new RiftPickPanel(run, rules, () => panelLayer.QueueFree()); layer.AddChild(panel);</code></example>
/// </summary>
public partial class RiftPickPanel : Control
{
	private readonly RiftRun _run;
	private readonly AltarRules _rules;
	private readonly Action _closed;
	private int _poolPage;

	/// <summary>A pick panel for <paramref name="run"/>; <paramref name="closed"/> runs once an offer is taken or skipped.</summary>
	public RiftPickPanel(RiftRun run, AltarRules rules, Action closed)
	{
		_run = run; _rules = rules; _closed = closed;
		MouseFilter = MouseFilterEnum.Stop; // the rift below takes no taps while the pick is open
	}

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetViewport().SizeChanged += Build;
		Build();
	}

	/// <inheritdoc/>
	public override void _ExitTree() => GetViewport().SizeChanged -= Build;

	private void Build()
	{
		foreach (var c in GetChildren()) c.QueueFree();
		var area = GetViewportRect().Size;
		Position = Vector2.Zero; Size = area;
		AddChild(new ColorRect { Size = area, Color = Pal.Abyss with { A = 0.9f }, MouseFilter = MouseFilterEnum.Ignore });
		var pick = _run.Pick;
		if (pick == null) return;
		float w = Mathf.Min(area.X - 16, 620), x0 = Mathf.Floor((area.X - w) / 2), y = 6;

		// the title, and the scrap to spend
		string title = pick.Reason == PickReason.LevelUp ? $"LEVEL {_run.Level}" : $"WAVE {_run.WaveN} CLEARED";
		UiKit.Text(this, title, x0, y, Pal.Gold, big: true);
		UiKit.Text(this, $"SCRAP {_run.Scrap}", x0, y + 6, Pal.Soft, width: w, align: HorizontalAlignment.Right);
		y += 30;

		// the offers: take one, or skip
		UiKit.Text(this, "TAKE ONE", x0, y, Pal.Mid); y += 16;
		float cw = 92, ch = 56, gap = 10, total = pick.Offers.Count * cw + (pick.Offers.Count - 1) * gap, ox = x0 + Mathf.Floor((w - total) / 2);
		for (int i = 0; i < pick.Offers.Count; i++)
		{
			int k = i; var o = pick.Offers[i];
			if (o.Card is { } c) CardFace.Make(this, _rules, c.Natures, c.Level, ox + i * (cw + gap), y, cw, ch, () => Take(k));
			else
			{
				var t = _run.Deck.First(d => d.Id == o.TemperId);
				CardFace.Make(this, _rules, t.Natures, t.Level + 1, ox + i * (cw + gap), y, cw, ch, () => Take(k));
				UiKit.Text(this, "+1", ox + i * (cw + gap) + 4, y + 2, Pal.Rare);
			}
		}
		if (pick.Offers.Count == 0) UiKit.Text(this, "THE POOL IS EMPTY", x0, y + 24, Pal.Dim, width: w, align: HorizontalAlignment.Center);
		y += ch + 4;
		UiKit.Tile(this, x0 + Mathf.Floor((w - 110) / 2), y, 110, 20, () => Take(-1), "SKIP", Pal.Mid);
		y += 28;

		// the shop: run-only boosts, each dearer the more you buy
		UiKit.Text(this, "SCRAP SHOP - THIS ADVENTURE ONLY", x0, y, Pal.Mid); y += 16;
		var boosts = _run.Settings.Boosts;
		int per = Math.Max(1, Math.Min(boosts.Length, (int)((w + 4) / 66))); // all on one row where they fit
		float bw = Mathf.Floor((w - (per - 1) * 4) / per), bh = 34;
		for (int i = 0; i < boosts.Length; i++)
		{
			var b = boosts[i]; int price = _run.PriceOf(b.Id), row = i / per, col = i % per;
			bool can = _run.Scrap >= price;
			var tile = UiKit.Tile(this, x0 + col * (bw + 4), y + row * (bh + 4), bw, bh, () => { if (_run.Buy(b.Id)) Build(); }, colour: can ? Pal.Teal : Pal.Line, enabled: can);
			UiKit.Text(tile, UiKit.FitPx(b.Name, bw - 4), 0, 1, can ? Pal.Bright : Pal.Dim, width: bw, align: HorizontalAlignment.Center);
			int n = _run.BoughtOf(b.Id);
			UiKit.Text(tile, $"{price}" + (n > 0 ? $"  x{n}" : ""), 0, 17, can ? Pal.Gold : Pal.Dim, width: bw, align: HorizontalAlignment.Center);
		}
		y += ((boosts.Length + per - 1) / per) * (bh + 4) + 6;

		// buy a card back from the pool prepared at the Altar
		int rp = _run.RetrievePrice();
		UiKit.Text(this, $"FROM YOUR POOL - {rp} SCRAP EACH", x0, y, Pal.Mid); y += 16;
		var pool = _run.PoolLeft;
		float pw = 56, ph = 40;
		int fit = Math.Max(1, (int)((w - 60 + 6) / (pw + 6))), pages = Math.Max(1, (pool.Count + fit - 1) / fit);
		_poolPage = Math.Clamp(_poolPage, 0, pages - 1);
		if (pool.Count == 0) UiKit.Text(this, "EVERY CARD IS FOUND", x0, y + 12, Pal.Dim);
		for (int i = _poolPage * fit, j = 0; i < pool.Count && j < fit; i++, j++)
		{
			int k = i; var c = pool[i];
			var tile = CardFace.Make(this, _rules, c.Natures, c.Level, x0 + j * (pw + 6), y, pw, ph, () => { if (_run.Retrieve(k)) Build(); });
			if (_run.Scrap < rp) tile.Enabled = false;
		}
		if (pages > 1)
		{
			UiKit.Tile(this, x0 + w - 52, y, 24, ph, () => { _poolPage--; Build(); }, "<", Pal.Mid, _poolPage > 0);
			UiKit.Tile(this, x0 + w - 24, y, 24, ph, () => { _poolPage++; Build(); }, ">", Pal.Mid, _poolPage < pages - 1);
		}
	}

	private void Take(int i)
	{
		_run.TakeOffer(i);
		_closed();
	}
}
