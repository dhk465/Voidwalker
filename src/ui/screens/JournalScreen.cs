using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// The Journal: quests to claim, logs to read, the codex of fusions, the bestiary (names until the creatures are
/// drawn), and the natures with how each is discovered. Restoring the demo takes two taps within three seconds.
/// </summary>
public partial class JournalScreen : StationScreen
{
	private static readonly string[] Tabs = { "quests", "logs", "codex", "bestiary", "natures" };
	private const double ForgetWindow = 3;
	private string _tab = "quests";
	private string? _log, _fusion, _creature;
	private ulong _forgetArmedMs;

	/// <inheritdoc/>
	protected override string Title => "JOURNAL";

	/// <inheritdoc/>
	protected override void Build()
	{
		for (int i = 0; i < Tabs.Length; i++)
		{
			string tab = Tabs[i]; bool on = _tab == tab;
			var t = Ui.Tile(Body, i * 86, 0, 82, 18, () => { _tab = tab; Say(""); Rebuild(); }, tab.ToUpperInvariant(), on ? Pal.Void : Pal.Mid);
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

	private void Quests()
	{
		var ids = D.Quests.Ids;
		for (int i = 0; i < ids.Count; i++)
		{
			string id = ids[i]; var q = D.Quests[id];
			double v = Math.Min(q.N, R.QuestVal(q));
			bool done = R.Meta.Quests.ContainsKey(id), ready = R.QuestReady(id);
			var tile = Ui.Tile(Body, i % 2 * 312, 24 + i / 2 * 48, 304, 44, colour: done ? Pal.Line : ready ? Pal.Gold : Pal.Deep);
			tile.Fill = Pal.Panel;
			Ui.Text(tile, q.Name, 6, 3, done ? Pal.Dim : Pal.Bright);
			Ui.Text(tile, q.Text, 6, 16, Pal.Mid);
			Ui.Bar(tile, 6, 34, 200, 3, v / q.N, ready || done ? Pal.Gold : Pal.Violet);
			if (done) Ui.Text(tile, "DONE", 226, 16, Pal.Dim, width: 70, align: HorizontalAlignment.Center);
			else
			{
				string reward = q.Reward.Void is { } vd ? $"{vd}" : "RELIC";
				Ui.Tile(tile, 226, 10, 70, 24, () => Claim(id), ready ? "CLAIM" : reward, Pal.Gold, ready);
			}
		}
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
		var ids = D.Logs.Ids;
		for (int i = 0; i < ids.Count; i++)
		{
			string id = ids[i]; bool got = R.Meta.Logs.GetValueOrDefault(id) > 0, on = _log == id;
			var row = Ui.Tile(Body, 0, 24 + i * 17, 200, 16, got ? () => { _log = id; Rebuild(); } : null, colour: got ? Pal.Teal : Pal.Deep);
			row.Fill = on ? Pal.Deep : Pal.Panel;
			Ui.Text(row, got ? Ui.Fit(D.Logs[id].Title, 31) : "- - -", 5, 2, got ? Pal.Soft : Pal.Line);
		}
		var paper = Ui.Tile(Body, 212, 24, 404, 200, colour: Pal.PaperDim);
		paper.Fill = Pal.Paper;
		if (_log == null) { Ui.Text(paper, "Tap a log you have found to read it.", 0, 92, Pal.PaperDim, width: 404, align: HorizontalAlignment.Center); return; }
		var lg = D.Logs[_log];
		Ui.Text(paper, lg.Title, 10, 8, Pal.PaperInk, big: true);
		Ui.Text(paper, lg.Text, 10, 32, Pal.PaperInk, width: 384);
		Ui.Text(paper, lg.Src == "wreck" ? "FOUND IN A WRECK" : "FOUND IN A RUIN", 10, 182, Pal.PaperDim);
	}

	private void Codex()
	{
		var keys = D.FusionKeys;
		for (int i = 0; i < keys.Count; i++)
		{
			string k = keys[i]; var ns = k.Split('+');
			bool known = R.Meta.Fusions.Contains(k), both = ns.All(R.Meta.Discovered.Contains);
			var colour = new Color(D.Recipes[k].Color);
			var cell = Ui.Tile(Body, i % 14 * 44, 24 + i / 14 * 40, 40, 36, () => { _fusion = k; Rebuild(); }, colour: known ? colour : Pal.Deep);
			cell.Fill = _fusion == k ? Pal.Deep : known ? Pal.Panel : Pal.Abyss;
			if (known)
				cell.Paint = t =>
				{
					for (int j = 0; j < 2; j++) t.DrawTexture(PixelIcons.Sigil(ns[j], new Color(D.Natures[ns[j]].Color)), new Vector2(10 + j * 11, 13));
				};
			else Ui.Text(cell, "?", 0, 12, both ? Pal.Dim : Pal.Line, width: 40, align: HorizontalAlignment.Center);
		}
		Ui.Text(Body, $"{R.Meta.Fusions.Count} / {keys.Count}", 416, 270, Pal.Bright, big: true, width: 200, align: HorizontalAlignment.Right);
		if (_fusion == null) { Ui.Text(Body, "Tap a fusion to see it.", 0, 272, Pal.Mid); return; }
		var rec = D.Recipes[_fusion]; var pair = _fusion.Split('+');
		bool seen = R.Meta.Fusions.Contains(_fusion);
		Ui.Text(Body, seen ? rec.Name : "?", 0, 270, seen ? new Color(rec.Color) : Pal.Mid, big: true);
		Ui.Text(Body, $"{D.Natures[pair[0]].Name} + {D.Natures[pair[1]].Name}", 0, 290, Pal.Soft);
		if (seen && rec.Says != null) Ui.Text(Body, rec.Says, 0, 304, Pal.Mid, width: BodyW);
	}

	private void Bestiary()
	{
		var all = D.Ybasogs.Ids.Select(k => (k, d: D.Ybasogs[k], matter: false)).Concat(D.Matter.Ids.Select(k => (k, d: D.Matter[k], matter: true))).ToList();
		for (int i = 0; i < all.Count; i++)
		{
			var (k, d, matter) = all[i];
			bool met = R.Meta.Bestiary.GetValueOrDefault(k) > 0;
			var colour = !met ? Pal.Line : matter ? new Color("#a89a8a") : d.Role == "minion" ? new Color("#b5c43a") : new Color("#e0a040");
			var row = Ui.Tile(Body, i % 4 * 154, 24 + i / 4 * 13, 152, 13, met ? () => { _creature = k; Rebuild(); } : null);
			row.Fill = _creature == k ? Pal.Deep : new Color(0, 0, 0, 0);
			row.Border = new Color(0, 0, 0, 0);
			Ui.Text(row, met ? Ui.Fit(d.Name, 24) : "- - -", 2, 0, colour);
		}
		int metCount = all.Count(e => R.Meta.Bestiary.GetValueOrDefault(e.k) > 0);
		Ui.Text(Body, $"{metCount} / {all.Count}", 416, 296, Pal.Bright, big: true, width: 200, align: HorizontalAlignment.Right);
		if (_creature == null) { Ui.Text(Body, "Tap a kind you have met.", 0, 298, Pal.Mid); return; }
		var c = D.Ybasogs.Has(_creature) ? D.Ybasogs[_creature] : D.Matter[_creature];
		Ui.Text(Body, c.Name, 0, 290, Pal.Bright);
		Ui.Text(Body, c.Says, 0, 304, Pal.Mid, width: 410);
	}

	private void Natures()
	{
		Ui.Tile(Body, 516, 0, 100, 18, Forget, "RESET TO DEMO", Pal.Warn);
		var ids = D.Natures.Ids;
		for (int i = 0; i < ids.Count; i++)
		{
			string n = ids[i]; var b = D.Natures[n];
			bool got = R.Meta.Discovered.Contains(n);
			var colour = new Color(b.Color);
			var tile = Ui.Tile(Body, i % 2 * 312, 24 + i / 2 * 40, 304, 36, colour: got ? colour : Pal.Deep);
			tile.Fill = Pal.Panel;
			var sigilColour = got ? colour : Pal.Line;
			tile.Paint = t => t.DrawTexture(PixelIcons.Sigil(n, sigilColour), new Vector2(6, 13));
			Ui.Text(tile, b.Name, 20, 3, got ? colour : Pal.Dim);
			if (n == R.Meta.Innate) Ui.Text(tile, "INNATE", 200, 3, Pal.Soft, width: 98, align: HorizontalAlignment.Right);
			else if (got) Ui.Text(tile, b.FormName, 200, 3, Pal.Soft, width: 98, align: HorizontalAlignment.Right);
			else
			{
				var d = D.Discover[n];
				Ui.Text(tile, d.Text, 20, 18, Pal.Mid, width: 280);
				Ui.Bar(tile, 200, 8, 96, 3, R.Meta.Stats.GetValueOrDefault(d.Stat) / d.N, Pal.Violet);
			}
		}
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
