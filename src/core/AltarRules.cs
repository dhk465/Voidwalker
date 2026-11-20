using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidwalker.Core;

/// <summary>A stack of identical cards: its natures (sorted), level and how many copies.</summary>
public sealed record Stack(string Sk, int N, string[] Natures, int Level, string Key);

/// <summary>One card drawn at the Void Rite.</summary>
public sealed record Reveal(string[] Natures, int Level, string Rarity, bool IsNew);

/// <summary>
/// The hub's rules, ported line for line from the browser build (index.html: the stations' update functions,
/// riteDraw, rollRifts, bonuses, unlocked, questVal). Plain C#: it reads <see cref="GameData"/> and changes a
/// <see cref="Meta"/>, and never saves; the caller saves after a change. Messages are the browser's own words.
/// <example><code>var rules = new AltarRules(data, Meta.Demo());
/// var drawn = rules.RiteDraw(1, out string message); // 30 void spent, one card added
/// string pool = $"{rules.DeckCount()} / {rules.DeckMax()}";</code></example>
/// </summary>
public sealed class AltarRules
{
	private readonly GameData _d;

	/// <summary>The tables the rules read.</summary>
	public GameData Data => _d;

	/// <summary>The save these rules change.</summary>
	public Meta Meta { get; set; }

	/// <summary>Rules over <paramref name="meta"/> with the tables in <paramref name="data"/>.</summary>
	public AltarRules(GameData data, Meta meta) { _d = data; Meta = meta; }

	// ---- cards --------------------------------------------------------------------------------

	/// <summary>Natures sorted and joined with '+' (JavaScript's default sort: ordinal).</summary>
	public static string KeyOf(IEnumerable<string> natures) => string.Join('+', natures.OrderBy(n => n, StringComparer.Ordinal));
	/// <summary>A stack key: "natures|level".</summary>
	public static string StackKey(IEnumerable<string> natures, int level) => KeyOf(natures) + "|" + level;

	/// <summary>Reads a stack key.</summary>
	public static Stack ParseStack(string sk, int n = 0)
	{
		int bar = sk.IndexOf('|');
		string key = sk[..bar];
		return new Stack(sk, n, key.Length == 0 ? Array.Empty<string>() : key.Split('+'), int.Parse(sk[(bar + 1)..]), key);
	}

	/// <summary>The stacks of a card map, in the browser's order: fewer natures first, then by key, then by level.</summary>
	public static List<Stack> Stacks(Dictionary<string, int> map) =>
		map.Where(kv => kv.Value > 0).Select(kv => ParseStack(kv.Key, kv.Value))
			.OrderBy(s => s.Natures.Length).ThenBy(s => s.Key, StringComparer.Ordinal).ThenBy(s => s.Level).ToList();

	/// <summary>The recipe for a set of natures, or null for an unstable set.</summary>
	public RecipeDef? RecipeOf(IEnumerable<string> natures) => _d.Recipes.Has(KeyOf(natures)) ? _d.Recipes[KeyOf(natures)] : null;
	/// <summary>A card's colour (#rrggbb).</summary>
	public string CardColor(IEnumerable<string> natures) => RecipeOf(natures)?.Color ?? "#ffffff";
	/// <summary>A card's name with its level in roman numerals ("Wrath II").</summary>
	public string CardName(IEnumerable<string> natures, int level) => (RecipeOf(natures)?.Name ?? "Unstable") + Roman(level);
	/// <summary>" II" for 2 and so on; nothing for level 1.</summary>
	public static string Roman(int n) => n <= 1 ? "" : n switch { 2 => " II", 3 => " III", 4 => " IV", 5 => " V", 6 => " VI", _ => " " + n };

	// ---- what the save adds up to ---------------------------------------------------------------

	/// <summary>Sigils bought and relics equipped, summed by key.</summary>
	public Dictionary<string, double> Bonuses()
	{
		var b = new Dictionary<string, double>();
		void Add(Dictionary<string, double> g) { foreach (var (k, v) in g) b[k] = b.GetValueOrDefault(k) + v; }
		foreach (var (id, lv) in Meta.Upg) if (_d.Sigils.Has(id)) for (int i = 0; i < lv; i++) Add(_d.Sigils[id].Give);
		foreach (var id in Meta.Equipped) if (_d.Relics.Has(id)) Add(_d.Relics[id].Give);
		return b;
	}

	/// <summary>Whether an unlock has opened.</summary>
	public bool Unlocked(string id)
	{
		var u = _d.Unlocks[id];
		double v = u.Stat switch { "relics" => Meta.Relics.Count, "wins" => Meta.Wins, "runs" => Meta.Runs, _ => Meta.Stats.GetValueOrDefault(u.Stat) };
		return v >= u.N;
	}

	/// <summary>Cards in the pool.</summary>
	public int DeckCount() => Meta.Loadout.Values.Sum();
	/// <summary>The pool's size limit.</summary>
	public int DeckMax() => (int)(_d.Tune("deck_max") + Bonuses().GetValueOrDefault("deck_max"));
	/// <summary>The smallest pool an adventure takes.</summary>
	public int DeckMin() => (int)_d.Tune("deck_min");
	/// <summary>Relic slots open.</summary>
	public int RelicSlots() => 1 + (Unlocked("slot2") ? 1 : 0) + (int)Bonuses().GetValueOrDefault("relic_slots");
	/// <summary>Ybasog kinds met (the top bar's count).</summary>
	public int YbasogsMet() => _d.Ybasogs.Ids.Count(k => Meta.Bestiary.GetValueOrDefault(k) > 0);

	/// <summary>The nature the pool leans to (summing card levels), which shapes the blob; ties go to the innate nature.</summary>
	public string? DominantNature()
	{
		var count = _d.Natures.Ids.ToDictionary(n => n, _ => 0);
		foreach (var s in Stacks(Meta.Loadout)) foreach (var n in s.Natures) count[n] += s.Level * s.N;
		string? best = null; int bestN = 0;
		foreach (var n in _d.Natures.Ids)
			if (count[n] > bestN || (count[n] == bestN && bestN > 0 && n == Meta.Innate)) { best = n; bestN = count[n]; }
		return best;
	}

	/// <summary>The blob's form for this pool: the main nature's form id (e.g. "dragon"), or "blob" when it holds no nature.</summary>
	public string DominantForm() => DominantNature() is { } n ? _d.Natures[n].Form : "blob";

	// ---- the Pool -----------------------------------------------------------------------------

	/// <summary>Puts one more copy of a stack into the pool. Returns a message when it cannot ("" when it did).</summary>
	public string AddToPool(string sk)
	{
		int inDeck = Meta.Loadout.GetValueOrDefault(sk), owned = Meta.Cards.GetValueOrDefault(sk);
		if (inDeck >= owned) return "ALL IN THE POOL";
		if (DeckCount() >= DeckMax()) return "THE POOL IS FULL";
		Meta.Loadout[sk] = inDeck + 1;
		return "";
	}

	/// <summary>Takes one copy of a stack out of the pool.</summary>
	public void RemoveFromPool(string sk)
	{
		if (!Meta.Loadout.TryGetValue(sk, out int n)) return;
		if (n <= 1) Meta.Loadout.Remove(sk); else Meta.Loadout[sk] = n - 1;
	}

	// ---- the Void Rite ------------------------------------------------------------------------

	/// <summary>Focuses the Rite on a nature, or clears the focus when it is already that nature.</summary>
	public void ToggleFocus(string nature) => Meta.Focus = Meta.Focus == nature ? null : nature;

	/// <summary>The void energy for <paramref name="n"/> draws (1 or 10), with the focus surcharge.</summary>
	public int RiteCost(int n)
	{
		double focus = Meta.Focus != null ? _d.Tune("focus_cost") : 0;
		return (int)(n == 10 ? _d.Tune("grand_rite_cost") + focus * 10 : _d.Tune("rite_cost") + focus);
	}

	/// <summary>Draws from the Rite (1 or 10 cards); empty with a message when the bank is short.</summary>
	public List<Reveal> RiteDraw(int n, out string message)
	{
		var reveal = new List<Reveal>();
		int cost = RiteCost(n);
		if (Meta.Bank < cost) { message = $"NEEDS {cost}"; return reveal; }
		Meta.Bank -= cost; message = "";
		var pairs = _d.FusionKeys.Where(k => k.Split('+').All(Meta.Discovered.Contains)).ToList();
		double myth = _d.Tune("mythic_chance") + Bonuses().GetValueOrDefault("mythic");
		for (int i = 0; i < n; i++)
		{
			var rng = new Rng((long)_d.Seed * 1000003 + (long)Meta.Draws * 104729);
			Meta.Draws++; Meta.Pity++;
			string[] natures; int level = 1; string rar = "common";
			if (pairs.Count > 0 && (Meta.Pity >= _d.Tune("mythic_pity") || rng.Next() < myth))
			{
				natures = pairs[rng.Index(pairs.Count)].Split('+'); rar = "mythic"; Meta.Pity = 0;
				string key = KeyOf(natures);
				if (!Meta.Fusions.Contains(key)) Meta.Fusions.Add(key);
			}
			else
			{
				natures = new[] { Meta.Focus != null && rng.Next() < _d.Tune("focus_share") ? Meta.Focus : Meta.Discovered[rng.Index(Meta.Discovered.Count)] };
				if (rng.Next() < _d.Tune("rare_chance")) { level = 2; rar = "rare"; }
			}
			string sk = StackKey(natures, level);
			bool isNew = !Meta.Seen.GetValueOrDefault(sk);
			Meta.Seen[sk] = true;
			Meta.Cards[sk] = Meta.Cards.GetValueOrDefault(sk) + 1;
			reveal.Add(new Reveal(natures, level, rar, isNew));
		}
		return reveal;
	}

	/// <summary>Draws until a mythic is certain.</summary>
	public int MythicIn() => Math.Max(1, (int)_d.Tune("mythic_pity") - Meta.Pity);

	// ---- the Sigils ---------------------------------------------------------------------------

	/// <summary>Whether a sigil is still locked behind an unlock.</summary>
	public bool SigilLocked(string id) => _d.Sigils[id].Need is { } need && !Unlocked(need);

	/// <summary>The next level's cost, or null when the sigil is complete or locked.</summary>
	public int? SigilCost(string id)
	{
		if (SigilLocked(id)) return null;
		var costs = _d.Sigils[id].Costs; int lv = Meta.Upg.GetValueOrDefault(id);
		return lv < costs.Length ? costs[lv] : null;
	}

	/// <summary>Buys a sigil's next level. Returns the message to show and whether it was bought.</summary>
	public (string message, bool bought) BuySigil(string id)
	{
		if (SigilLocked(id)) return ("LOCKED", false);
		if (SigilCost(id) is not { } cost) return ("COMPLETE", false);
		if (Meta.Bank < cost) return ($"NEEDS {cost}", false);
		Meta.Bank -= cost; Meta.Upg[id] = Meta.Upg.GetValueOrDefault(id) + 1;
		return (_d.Sigils[id].Name.ToUpperInvariant(), true);
	}

	// ---- the Relics ---------------------------------------------------------------------------

	/// <summary>Equips or unequips an owned relic; equipping into full slots drops the oldest.</summary>
	public void ToggleRelic(string id)
	{
		if (Meta.Relics.GetValueOrDefault(id) <= 0) return;
		int k = Meta.Equipped.IndexOf(id);
		if (k >= 0) { Meta.Equipped.RemoveAt(k); return; }
		if (Meta.Equipped.Count >= RelicSlots()) Meta.Equipped.RemoveAt(0);
		Meta.Equipped.Add(id);
	}

	// ---- the Journal --------------------------------------------------------------------------

	/// <summary>A quest's progress value.</summary>
	public double QuestVal(QuestDef q) => q.Stat switch
	{
		"natures" => Meta.Discovered.Count,
		"logs" => Meta.Logs.Count,
		"relics" => Meta.Relics.Count,
		"bestiary" => YbasogsMet(),
		"codex" => Meta.Fusions.Count,
		_ => Meta.Stats.GetValueOrDefault(q.Stat),
	};

	/// <summary>Whether a quest is reached and not yet claimed.</summary>
	public bool QuestReady(string id) => !Meta.Quests.ContainsKey(id) && QuestVal(_d.Quests[id]) >= _d.Quests[id].N;

	/// <summary>Claims a ready quest: its void, or a relic not yet owned (picked by the run count and the quest's place).</summary>
	public bool ClaimQuest(string id)
	{
		if (!QuestReady(id)) return false;
		var q = _d.Quests[id]; int i = _d.Quests.Ids.ToList().IndexOf(id);
		Meta.Quests[id] = "claimed";
		if (q.Reward.Void is { } v) Meta.Bank += v;
		if (q.Reward.Relic is not null)
		{
			var left = _d.Relics.Ids.Where(r => Meta.Relics.GetValueOrDefault(r) <= 0).ToList();
			if (left.Count > 0) Meta.Relics[left[(Meta.Runs * 7 + i) % left.Count]] = 1;
		}
		return true;
	}

	// ---- the Rift Gate ------------------------------------------------------------------------

	/// <summary>Rolls the rifts offered at the Gate: two, or three once the first win opens a third; the first is always calm.</summary>
	public List<Rift> RollRifts()
	{
		var rng = new Rng((long)_d.Seed * 7 + (long)Meta.Runs * 131 + (long)Meta.Draws * 3);
		T Pick<T>(IReadOnlyList<T> l) => l[rng.Index(l.Count)];
		int n = Unlocked("rift3") ? 3 : 2;
		bool dark = Unlocked("omens");
		var omens = _d.Omens.Ids.Where(k => dark || k is "calm" or "swarm" or "rockfall" or "ruinwake").ToList();
		var boons = _d.Boons.Ids.Where(k => k != "calm").ToList();
		var list = new List<Rift>();
		for (int i = 0; i < n; i++)
		{
			string name = $"The {Pick(_d.RiftAdjectives)} {Pick(_d.RiftNouns)}";
			string omen = i == 0 ? "calm" : Pick(omens);
			string prevail = Pick(_d.Natures.Ids);
			string boon = i == 0 ? "calm" : Pick(boons);
			list.Add(new Rift(name, omen, prevail, boon));
		}
		return list;
	}

	/// <summary>Rolls the rifts when none are offered yet (on opening the Altar). Returns whether it changed the save.</summary>
	public bool EnsureRifts()
	{
		if (Meta.Rifts != null) return false;
		Meta.Rifts = RollRifts(); Meta.RiftPick = 0; ApplyRift();
		return true;
	}

	/// <summary>Chooses a rift.</summary>
	public void PickRift(int i) { if (Meta.Rifts == null || i < 0 || i >= Meta.Rifts.Count) return; Meta.RiftPick = i; ApplyRift(); }

	private void ApplyRift()
	{
		var r = Meta.RiftPick < Meta.Rifts!.Count ? Meta.Rifts[Meta.RiftPick] : Meta.Rifts[0];
		Meta.Omen = r.Omen; Meta.Prevail = r.Prevail; Meta.Boon = r.Boon;
	}

	/// <summary>Moves the depth by <paramref name="delta"/> within 0 and the deepest open.</summary>
	public void StepDepth(int delta) => Meta.Depth = Math.Clamp(Meta.Depth + delta, 0, Meta.MaxDepth);

	/// <summary>Extra life and void per depth, as whole percents.</summary>
	public (int life, int @void) DepthBonus() =>
		((int)Math.Round(_d.Tune("depth_hp") * Meta.Depth * 100), (int)Math.Round(_d.Tune("depth_void") * Meta.Depth * 100));

	/// <summary>Whether the Chosen sigil lets the Gate pick a first card.</summary>
	public bool HasChosen() => Bonuses().GetValueOrDefault("chosen") > 0;

	/// <summary>Steps the chosen first card through the pool's stacks.</summary>
	public void StepChosen(int d)
	{
		var l = Stacks(Meta.Loadout); if (l.Count == 0) return;
		int i = l.FindIndex(s => s.Sk == Meta.Chosen);
		Meta.Chosen = l[((i < 0 ? 0 : i) + d + l.Count) % l.Count].Sk;
	}

	/// <summary>Whether the rift can be entered: "" when it can, else why not (the browser's words).</summary>
	public string BeginAdventure() => DeckCount() < DeckMin() ? $"THE POOL NEEDS {DeckMin()}" : "";

	// ---- after an adventure ---------------------------------------------------------------------

	/// <summary>
	/// Adds to a lifetime stat (kills, motes, pods, ...) and discovers every nature whose goal it now reaches: the
	/// nature joins the known ones, its cards join the collection and the discovery bonus is banked (browser:
	/// track and discover). Returns the natures discovered. Nothing is discovered before the innate nature is known.
	/// </summary>
	public List<string> Track(string stat, double n = 1)
	{
		Meta.Stats[stat] = Meta.Stats.GetValueOrDefault(stat) + n;
		var found = new List<string>();
		if (Meta.Innate == null) return found;
		foreach (var nat in _d.Discover.Ids)
		{
			var d = _d.Discover[nat];
			if (d.Stat != stat || Meta.Discovered.Contains(nat) || Meta.Stats[stat] < d.N) continue;
			Meta.Discovered.Add(nat);
			string sk = StackKey(new[] { nat }, 1);
			Meta.Cards[sk] = Meta.Cards.GetValueOrDefault(sk) + (int)_d.Tune("discovery_copies");
			Meta.Bank += (int)_d.Tune("discovery_bonus");
			found.Add(nat);
		}
		return found;
	}

	/// <summary>
	/// Banks an adventure's void energy (browser: endRun): <paramref name="gathered"/> is the void energy already
	/// multiplied by the rift and the share kept; a win adds the win bonus, counts the win and opens the next depth.
	/// The adventure counts as one more run, and the Gate offers new rifts. Returns what was banked.
	/// </summary>
	public int EndAdventure(double gathered, bool won)
	{
		int banked = (int)Math.Round(gathered, MidpointRounding.AwayFromZero) + (won ? (int)_d.Tune("win_bonus") : 0);
		Meta.Bank += banked; Meta.Runs++;
		if (won)
		{
			Meta.Wins++;
			if (Meta.Depth >= 1) Track("deepwin");
			if (Meta.Depth >= Meta.MaxDepth) Meta.MaxDepth = Meta.Depth + 1;
		}
		Meta.Rifts = RollRifts(); Meta.RiftPick = 0; ApplyRift();
		return banked;
	}
}
