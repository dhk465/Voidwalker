using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Journal: quests to claim, logs to read, the codex of fusions, the bestiary (names until the creatures are
/// drawn), and the natures with how each is discovered. Every list pages to fit the body. Restoring the demo takes
/// two taps within three seconds.
/// </summary>
public partial class JournalScreen : StationScreen
{
	private static readonly string[] Tabs = { "quests", "logs", "codex", "bestiary", "natures" };
	private const double ForgetWindow = 3;
	private const int Top = 28, Line = Ui.SmallSize, PagerRow = Ui.PagerH + 4;
	private string _tab = StartTab();
	private string? _log, _fusion, _creature;
	private ulong _forgetArmedMs;
	private readonly Dictionary<string, int> _pages = new();

	/// <inheritdoc/>
	protected override string Title => "JOURNAL";

	/// <summary>The tab to open on: quests, or for checks and screenshots the one named by <c>-- --tab=codex</c>.</summary>
	private static string StartTab()
	{
		foreach (var arg in OS.GetCmdlineUserArgs())
			if (arg.StartsWith("--tab=") && Array.IndexOf(Tabs, arg["--tab=".Length..]) >= 0) return arg["--tab=".Length..];
		return "quests";
	}

	/// <inheritdoc/>
	protected override void Build()
	{
		int tabW = Math.Min(110, (BodyW - 4 * 4) / Tabs.Length);
		for (int i = 0; i < Tabs.Length; i++)
		{
			string tab = Tabs[i]; bool on = _tab == tab;
			var t = Ui.Tile(Body, i * (tabW + 4), 0, tabW, 22, () => { _tab = tab; Say(""); Rebuild(); }, tab.ToUpperInvariant(), on ? Pal.Void : Pal.Mid);
			t.Fill = on ? Pal.Deep : Pal.Panel;
		}
		switch (_tab)
		{
			case "quests": Quests(); break;
			case "logs": Logs(); break;
			case "codex": Codex(); break;
			case "bestiary": Bestiary(); break;
			default: Natures(); break;
		}
	}

	/// <summary>
	/// Works out a page of a grid with <paramref name="cols"/> columns: how many rows fit <paramref name="room"/>
	/// pixels (leaving a pager row only when everything does not fit), and which page of the tab is showing.
	/// </summary>
	private (int per, int page, int pages) Page(int count, int room, int cols, int cellH, int gap, bool pagerReserved = false)
	{
		int rows = Math.Max(1, (room + gap) / (cellH + gap));
		if (!pagerReserved && cols * rows < count) rows = Math.Max(1, (room - PagerRow + gap) / (cellH + gap));
		int per = cols * rows, pages = Math.Max(1, (count + per - 1) / per);
		int page = Math.Min(_pages.GetValueOrDefault(_tab), pages - 1);
		_pages[_tab] = page;
		return (per, page, pages);
	}

	private void Pager(float x, float y, float w, int page, int pages) =>
		Ui.Pager(Body, x, y, w, page, pages, p => { _pages[_tab] = p; Rebuild(); });

	private void Quests()
	{
		const int h = 48, gap = 4;
		var ids = D.Quests.Ids;
		int cols = BodyW >= 500 ? 2 : 1, tileW = (BodyW - (cols - 1) * 8) / cols;
		var (per, page, pages) = Page(ids.Count, BodyH - Top, cols, h, gap);
		for (int j = 0; j < per && page * per + j < ids.Count; j++)
		{
			string id = ids[page * per + j]; var q = D.Quests[id];
			double v = Math.Min(q.N, R.QuestVal(q));
			bool done = R.Meta.Quests.ContainsKey(id), ready = R.QuestReady(id);
			var tile = Ui.Tile(Body, j % cols * (tileW + 8), Top + j / cols * (h + gap), tileW, h, colour: done ? Pal.Line : ready ? Pal.Gold : Pal.Deep);
			tile.Fill = Pal.Panel;
			Ui.Text(tile, Ui.FitPx(q.Name, tileW - 80), 6, 3, done ? Pal.Dim : Pal.Bright);
			Ui.Text(tile, Ui.FitPx(q.Text, tileW - 12), 6, 22, Pal.Mid);
			Ui.Bar(tile, 6, 41, tileW - 12, 3, v / q.N, ready || done ? Pal.Gold : Pal.Violet);
			if (done) Ui.Text(tile, "DONE", tileW - 68, 3, Pal.Dim, width: 62, align: HorizontalAlignment.Center);
			else
			{
				string reward = q.Reward.Void is { } vd ? $"{vd}" : "RELIC";
				Ui.Tile(tile, tileW - 68, 2, 62, 20, () => Claim(id), ready ? "CLAIM" : reward, Pal.Gold, ready);
			}
		}
		Pager(0, BodyH - Ui.PagerH, BodyW, page, pages);
	}

	private void Claim(string id)
	{
		var q = D.Quests[id];
		var before = R.Meta.Relics.Keys.ToHashSet();
		if (!R.ClaimQuest(id)) return;
		string gotRelic = R.Meta.Relics.Keys.FirstOrDefault(k => !before.Contains(k)) ?? "";
		Say(q.Reward.Void is { } v ? $"+{v} VOID" : gotRelic.Length > 0 ? D.Relics[gotRelic].Name.ToUpperInvariant() : "NO RELIC LEFT", Pal.GoldHi);
		Commit();
	}

	private void Logs()
	{
		const int rowH = 18, step = 20;
		int listW = Math.Clamp(BodyW * 38 / 100, 200, 250); // the full title shows on the paper either way
		var ids = D.Logs.Ids;
		var (per, page, pages) = Page(ids.Count, BodyH - Top, 1, rowH, step - rowH);
		for (int j = 0; j < per && page * per + j < ids.Count; j++)
		{
			string id = ids[page * per + j]; bool got = R.Meta.Logs.GetValueOrDefault(id) > 0, on = _log == id;
			var row = Ui.Tile(Body, 0, Top + j * step, listW, rowH, got ? () => { _log = id; Rebuild(); } : null, colour: got ? Pal.Teal : Pal.Deep);
			row.Fill = on ? Pal.Deep : Pal.Panel;
			Ui.Text(row, got ? Ui.FitPx(D.Logs[id].Title, listW - 10) : "- - -", 5, 1, got ? Pal.Soft : Pal.Line);
		}
		Pager(0, BodyH - Ui.PagerH, listW, page, pages);

		int paperX = listW + 8, paperW = BodyW - paperX, paperH = BodyH - Top;
		var paper = Ui.Tile(Body, paperX, Top, paperW, paperH, colour: Pal.PaperDim);
		paper.Fill = Pal.Paper;
		if (_log == null) { Ui.Text(paper, "Tap a log you have found to read it.", 10, paperH / 2 - Line, Pal.PaperDim, width: paperW - 20, align: HorizontalAlignment.Center); return; }
		var lg = D.Logs[_log];
		Ui.Text(paper, lg.Title, 10, 8, Pal.PaperInk, width: paperW - 20);
		int textY = 8 + (lg.Title.Length * Ui.SmallW > paperW - 20 ? 2 : 1) * Line + 8;
		Ui.Text(paper, lg.Text, 10, textY, Pal.PaperInk, width: paperW - 20);
		Ui.Text(paper, lg.Src == "wreck" ? "FOUND IN A WRECK" : "FOUND IN A RUIN", 10, paperH - Line - 6, Pal.PaperDim);
	}

	private void Codex()
	{
		const int cellW = 40, cellH = 36, gap = 4, detailH = 74;
		var keys = D.FusionKeys;
		int cols = Ui.Fits(BodyW, cellW, gap);
		var (per, page, pages) = Page(keys.Count, BodyH - Top - detailH - 4, cols, cellH, gap);
		int x0 = (BodyW - (cols * (cellW + gap) - gap)) / 2;
		for (int j = 0; j < per && page * per + j < keys.Count; j++)
		{
			string k = keys[page * per + j]; var ns = k.Split('+');
			bool known = R.Meta.Fusions.Contains(k), both = ns.All(R.Meta.Discovered.Contains);
			var colour = new Color(D.Recipes[k].Color);
			var cell = Ui.Tile(Body, x0 + j % cols * (cellW + gap), Top + j / cols * (cellH + gap), cellW, cellH, () => { _fusion = k; Rebuild(); }, colour: known ? colour : Pal.Deep);
			cell.Fill = _fusion == k ? Pal.Deep : known ? Pal.Panel : Pal.Abyss;
			if (known)
				cell.Paint = t =>
				{
					for (int i = 0; i < 2; i++) t.DrawTexture(PixelIcons.Sigil(ns[i], new Color(D.Natures[ns[i]].Color)), new Vector2(10 + i * 11, 13));
				};
			else Ui.Text(cell, "?", 0, (cellH - Line) / 2, both ? Pal.Dim : Pal.Line, width: cellW, align: HorizontalAlignment.Center);
		}
		int detailY = BodyH - detailH;
		Pager(0, detailY - Ui.PagerH - 4, BodyW, page, pages);
		Ui.Text(Body, $"{R.Meta.Fusions.Count} / {keys.Count}", BodyW - 140, detailY, Pal.Bright, big: true, width: 140, align: HorizontalAlignment.Right);
		if (_fusion == null) { Ui.Text(Body, "Tap a fusion to see it.", 0, detailY + 4, Pal.Mid); return; }
		var rec = D.Recipes[_fusion]; var pair = _fusion.Split('+');
		bool seen = R.Meta.Fusions.Contains(_fusion);
		Ui.Text(Body, seen ? rec.Name : "?", 0, detailY, seen ? new Color(rec.Color) : Pal.Mid, big: true);
		Ui.Text(Body, $"{D.Natures[pair[0]].Name} + {D.Natures[pair[1]].Name}", 0, detailY + 26, Pal.Soft);
		if (seen && rec.Says != null) Ui.Text(Body, rec.Says, 0, detailY + 26 + Line, Pal.Mid, width: BodyW);
	}

	private void Bestiary()
	{
		const int cellW = 166, rowH = 18, detailH = 52;
		var all = D.Ybasogs.Ids.Select(k => (k, d: D.Ybasogs[k], matter: false)).Concat(D.Matter.Ids.Select(k => (k, d: D.Matter[k], matter: true))).ToList();
		int cols = Ui.Fits(BodyW, cellW, 0);
		var (per, page, pages) = Page(all.Count, BodyH - Top - detailH - 4, cols, rowH, 0);
		int colW = BodyW / cols;
		for (int j = 0; j < per && page * per + j < all.Count; j++)
		{
			var (k, d, matter) = all[page * per + j];
			bool met = R.Meta.Bestiary.GetValueOrDefault(k) > 0;
			var colour = !met ? Pal.Line : matter ? new Color("#a89a8a") : d.Role == "minion" ? new Color("#b5c43a") : new Color("#e0a040");
			var row = Ui.Tile(Body, j % cols * colW, Top + j / cols * rowH, colW - 4, rowH, met ? () => { _creature = k; Rebuild(); } : null);
			row.Fill = _creature == k ? Pal.Deep : new Color(0, 0, 0, 0);
			row.Border = new Color(0, 0, 0, 0);
			Ui.Text(row, met ? Ui.FitPx(d.Name, colW - 10) : "- - -", 3, 1, colour);
		}
		int detailY = BodyH - detailH;
		Pager(0, detailY - Ui.PagerH - 4, BodyW, page, pages);
		int metCount = all.Count(e => R.Meta.Bestiary.GetValueOrDefault(e.k) > 0);
		Ui.Text(Body, $"{metCount} / {all.Count}", BodyW - 140, detailY - 4, Pal.Bright, big: true, width: 140, align: HorizontalAlignment.Right);
		if (_creature == null) { Ui.Text(Body, "Tap a kind you have met.", 0, detailY, Pal.Mid); return; }
		var c = D.Ybasogs.Has(_creature) ? D.Ybasogs[_creature] : D.Matter[_creature];
		Ui.Text(Body, c.Name, 0, detailY, Pal.Bright);
		Ui.Text(Body, c.Says, 0, detailY + Line + 2, Pal.Mid, width: BodyW);
	}

	private void Natures()
	{
		const int h = 54, gap = 4;
		var ids = D.Natures.Ids;
		int colsWanted = BodyW >= 500 ? 2 : 1, tileW = (BodyW - (colsWanted - 1) * 8) / colsWanted;
		var (per, page, pages) = Page(ids.Count, BodyH - Top - PagerRow, colsWanted, h, gap, pagerReserved: true);
		for (int j = 0; j < per && page * per + j < ids.Count; j++)
		{
			string n = ids[page * per + j]; var b = D.Natures[n];
			bool got = R.Meta.Discovered.Contains(n);
			var colour = new Color(b.Color);
			var tile = Ui.Tile(Body, j % colsWanted * (tileW + 8), Top + j / colsWanted * (h + gap), tileW, h, colour: got ? colour : Pal.Deep);
			tile.Fill = Pal.Panel;
			var sigilColour = got ? colour : Pal.Line;
			tile.Paint = t => t.DrawTexture(PixelIcons.Sigil(n, sigilColour), new Vector2(6, 7));
			Ui.Text(tile, b.Name, 20, 3, got ? colour : Pal.Dim);
			if (n == R.Meta.Innate) Ui.Text(tile, "INNATE", tileW - 110, 3, Pal.Soft, width: 104, align: HorizontalAlignment.Right);
			else if (got) Ui.Text(tile, b.FormName, tileW - 110, 3, Pal.Soft, width: 104, align: HorizontalAlignment.Right);
			if (!got)
			{
				var d = D.Discover[n];
				Ui.Text(tile, d.Text, 20, 20, Pal.Mid, width: tileW - 26);
				Ui.Bar(tile, tileW - 106, 9, 100, 3, R.Meta.Stats.GetValueOrDefault(d.Stat) / d.N, Pal.Violet);
			}
		}
		int resetW = 140;
		Pager(0, BodyH - Ui.PagerH, BodyW - resetW - 8, page, pages);
		Ui.Tile(Body, BodyW - resetW, BodyH - Ui.PagerH, resetW, Ui.PagerH, Forget, "RESET TO DEMO", Pal.Warn);
	}

	private void Forget()
	{
		ulong now = Time.GetTicksMsec();
		if (_forgetArmedMs > 0 && now - _forgetArmedMs < ForgetWindow * 1000)
		{
			_forgetArmedMs = 0;
			if (!S.ForgetAll()) { Say(S.SaveNotice, Pal.Warn); return; }
			_log = _fusion = _creature = null;
			Say("DEMO RESTORED", Pal.Mint);
			Rebuild();
			return;
		}
		_forgetArmedMs = now;
		Say("TAP AGAIN TO REPLACE SAVE WITH DEMO", Pal.Warn);
	}
}
