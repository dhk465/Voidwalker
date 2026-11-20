using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Voidwalker.Core;

/// <summary>One of the fourteen innate natures (seven sins, seven virtues). Opposites annul.</summary>
public sealed record NatureDef(string Name, string Kind, string Element, string Color, string Opposite, string Form, string FormName, string TraitText);
/// <summary>A card recipe: a base nature, a fusion of two, or the plain Void Bolt (key "").</summary>
public sealed record RecipeDef(string Name, string Color, string? Says);
/// <summary>How a nature not yet known is discovered: reach <c>N</c> of a lifetime stat.</summary>
public sealed record DiscoverDef(string Stat, int N, string Text);
/// <summary>A rift's omen: the danger it brings.</summary>
public sealed record OmenDef(string Name, string Says);
/// <summary>A permanent upgrade bought at the Sigils station; each level costs the next entry of <c>Costs</c>.</summary>
public sealed record SigilDef(string Name, string Says, int[] Costs, Dictionary<string, double> Give, string? Need);
/// <summary>A relic: found on adventures, equipped at the Relics station.</summary>
public sealed record RelicDef(string Name, string Says, string Lore, Dictionary<string, double> Give);
/// <summary>A log found in a wreck or a ruin, read in the Journal.</summary>
public sealed record LogDef(string Src, string Title, string Text);
/// <summary>What a quest gives when claimed: void energy, or a relic not yet owned.</summary>
public sealed record QuestReward(int? Void, int? Relic);
/// <summary>A long goal claimed in the Journal once <c>Stat</c> reaches <c>N</c>.</summary>
public sealed record QuestDef(string Name, string Text, string Stat, int N, QuestReward Reward);
/// <summary>Something that opens over time, once <c>Stat</c> reaches <c>N</c>.</summary>
public sealed record UnlockDef(string Name, string Stat, int N);
/// <summary>A rift's boon.</summary>
public sealed record BoonDef(string Name, string Says);
/// <summary>A Ybasog kind or a piece of matter, for the bestiary.</summary>
public sealed record CreatureDef(string Name, string Says, string? Role);

/// <summary>A Ybasog kind's numbers in the adventure (browser units: a 1280x720 screen, px and px/s).</summary>
/// <param name="W">How often waves pick it (a weight).</param><param name="Dive">A grazer's dive speed.</param>
/// <param name="Spit">A spitter's seconds between globs.</param><param name="Group">How many come together (0: one).</param>
public sealed record YbasogStats(string Name, string Fam, string Role, double Hp, double Sp, double R, double Drop, int From, double W, double Dive, double Spit, int Group, string? Nature);
/// <summary>A kind of falling matter's numbers in the adventure (browser units). <c>Shape</c> is rock, circle, rect or arch.</summary>
/// <param name="Wt">How often waves pick it.</param><param name="Only">Only made by something else breaking.</param>
public sealed record MatterStats(string Name, string Fam, string Shape, double R, double W, double H, double Hp, double Sp, double Drop, int From, double Wt, double Drift, string? OnBreak, bool Only, bool Tilt, double Spin, bool Brood, bool Absorb);

/// <summary>A table keyed by id that keeps the source's order (the browser build lists things in a meaningful order).</summary>
public sealed class Table<T>
{
	private readonly Dictionary<string, T> _byId = new();
	private readonly List<string> _ids = new();

	/// <summary>The ids in source order.</summary>
	public IReadOnlyList<string> Ids => _ids;
	/// <summary>How many entries.</summary>
	public int Count => _ids.Count;
	/// <summary>The entry with this id; throws when absent.</summary>
	public T this[string id] => _byId[id];
	/// <summary>Whether the id exists.</summary>
	public bool Has(string id) => _byId.ContainsKey(id);

	internal void Add(string id, T value) { _byId[id] = value; _ids.Add(id); }
}

/// <summary>
/// The game's data tables, exported from the browser build by <c>tools/data/export_data.mjs</c> into
/// <c>src/data/game_data.json</c>. Plain C# with no engine types, so the rules can be unit-tested.
/// <example><code>var data = GameData.Parse(File.ReadAllText("src/data/game_data.json"));
/// double cost = data.Tune("rite_cost"); // 30
/// string wrath = data.Natures["wrath"].Color; // "#ff4b3a"</code></example>
/// </summary>
public sealed class GameData
{
	private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
	private readonly Dictionary<string, double> _tune = new();

	/// <summary>The base seed every gameplay random draw starts from.</summary>
	public int Seed { get; private init; }
	/// <summary>The fourteen natures, in the browser's order (sins, then virtues).</summary>
	public Table<NatureDef> Natures { get; } = new();
	/// <summary>Every recipe, keyed by sorted nature ids joined with '+'.</summary>
	public Table<RecipeDef> Recipes { get; } = new();
	/// <summary>The two-nature fusion keys (84), in the browser's order.</summary>
	public List<string> FusionKeys { get; } = new();
	/// <summary>How each nature is discovered.</summary>
	public Table<DiscoverDef> Discover { get; } = new();
	/// <summary>Rift omens.</summary>
	public Table<OmenDef> Omens { get; } = new();
	/// <summary>Sigils, in display order.</summary>
	public Table<SigilDef> Sigils { get; } = new();
	/// <summary>Relics, in display order.</summary>
	public Table<RelicDef> Relics { get; } = new();
	/// <summary>Logs, in display order.</summary>
	public Table<LogDef> Logs { get; } = new();
	/// <summary>Quests, in display order (a relic reward's pick depends on it).</summary>
	public Table<QuestDef> Quests { get; } = new();
	/// <summary>Unlocks.</summary>
	public Table<UnlockDef> Unlocks { get; } = new();
	/// <summary>Rift boons.</summary>
	public Table<BoonDef> Boons { get; } = new();
	/// <summary>Words a rift's name is made of: "The [adjective] [noun]".</summary>
	public string[] RiftAdjectives { get; private set; } = Array.Empty<string>();
	/// <summary>See <see cref="RiftAdjectives"/>.</summary>
	public string[] RiftNouns { get; private set; } = Array.Empty<string>();
	/// <summary>The Ybasog kinds (counted in the hub's top bar).</summary>
	public Table<CreatureDef> Ybasogs { get; } = new();
	/// <summary>Matter shown in the bestiary after the Ybasogs.</summary>
	public Table<CreatureDef> Matter { get; } = new();

	/// <summary>Every Ybasog kind's adventure numbers, in the browser's order.</summary>
	public Table<YbasogStats> YbasogStats { get; } = new();
	/// <summary>Every kind of matter's adventure numbers, pieces made by breaking included.</summary>
	public Table<MatterStats> MatterStats { get; } = new();
	/// <summary>Each omen's effects by key (hp, void, pods, rocks, spit, bounces, killVoid, rockVoid, ...); empty for calm.</summary>
	public Dictionary<string, Dictionary<string, double>> OmenFx { get; } = new();
	/// <summary>Each boon's effects by key (void, relic, debut, natured).</summary>
	public Dictionary<string, Dictionary<string, double>> BoonFx { get; } = new();
	/// <summary>Each nature's form trait by key (fire_rate, bite, iframes, ...), what the blob gains in that form.</summary>
	public Dictionary<string, Dictionary<string, double>> Traits { get; } = new();

	/// <summary>A tuning value by its browser name (rite_cost, deck_max, ...); throws when absent.</summary>
	public double Tune(string key) => _tune.TryGetValue(key, out var v) ? v : throw new KeyNotFoundException($"tune.{key}");

	/// <summary>Reads the exported JSON.</summary>
	public static GameData Parse(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		var d = new GameData { Seed = root.GetProperty("seed").GetInt32() };
		foreach (var p in root.GetProperty("tune").EnumerateObject()) d._tune[p.Name] = p.Value.GetDouble();
		Fill(d.Natures, root.GetProperty("natures"));
		Fill(d.Recipes, root.GetProperty("recipes"));
		foreach (var k in d.Recipes.Ids) if (k.Split('+').Length == 2) d.FusionKeys.Add(k);
		Fill(d.Discover, root.GetProperty("discover"));
		Fill(d.Omens, root.GetProperty("omens"));
		Fill(d.Sigils, root.GetProperty("sigils"));
		Fill(d.Relics, root.GetProperty("relics"));
		Fill(d.Logs, root.GetProperty("logs"));
		Fill(d.Quests, root.GetProperty("quests"));
		Fill(d.Unlocks, root.GetProperty("unlocks"));
		var rifts = root.GetProperty("rifts");
		Fill(d.Boons, rifts.GetProperty("boons"));
		d.RiftAdjectives = rifts.GetProperty("adj").Deserialize<string[]>(Json)!;
		d.RiftNouns = rifts.GetProperty("noun").Deserialize<string[]>(Json)!;
		Fill(d.Ybasogs, root.GetProperty("ybasogs"));
		Fill(d.Matter, root.GetProperty("matter"));
		foreach (var p in root.GetProperty("omens").EnumerateObject()) d.OmenFx[p.Name] = Effects(p.Value);
		foreach (var p in rifts.GetProperty("boons").EnumerateObject()) d.BoonFx[p.Name] = Effects(p.Value);
		foreach (var p in root.GetProperty("natures").EnumerateObject())
			d.Traits[p.Name] = p.Value.TryGetProperty("trait", out var t) ? Effects(t) : new();
		if (root.TryGetProperty("adventure", out var adv))
		{
			foreach (var p in adv.GetProperty("ybasogs").EnumerateObject())
			{
				var y = p.Value;
				d.YbasogStats.Add(p.Name, new YbasogStats(Str(y, "name")!, Str(y, "fam")!, Str(y, "role") ?? "minion", Num(y, "hp"), Num(y, "sp"), Num(y, "r"),
					Num(y, "drop"), (int)Num(y, "from", 1), Num(y, "w", 1), Num(y, "dive"), Num(y, "spit"), (int)Num(y, "group"), Str(y, "nature")));
			}
			foreach (var p in adv.GetProperty("matter").EnumerateObject())
			{
				var m = p.Value;
				d.MatterStats.Add(p.Name, new MatterStats(Str(m, "name")!, Str(m, "fam")!, Str(m, "shape") ?? "rock", Num(m, "r"), Num(m, "w"), Num(m, "h"), Num(m, "hp"),
					Num(m, "sp"), Num(m, "drop"), (int)Num(m, "from", 1), Num(m, "wt", 1), Num(m, "drift", 16), Str(m, "onBreak"), Num(m, "only") > 0,
					Num(m, "tilt") > 0, Num(m, "spin", 1), Num(m, "brood") > 0, Num(m, "absorb") > 0));
			}
		}
		return d;
	}

	// a number, or a flag (true = 1), or the default when the key is absent
	private static double Num(JsonElement o, string key, double def = 0) =>
		o.TryGetProperty(key, out var v) ? v.ValueKind switch { JsonValueKind.Number => v.GetDouble(), JsonValueKind.True => 1, JsonValueKind.False => 0, _ => def } : def;
	private static string? Str(JsonElement o, string key) => o.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
	// an omen's, boon's or trait's numeric keys (its name and words left out)
	private static Dictionary<string, double> Effects(JsonElement o)
	{
		var fx = new Dictionary<string, double>();
		foreach (var p in o.EnumerateObject())
			if (p.Value.ValueKind is JsonValueKind.Number or JsonValueKind.True) fx[p.Name] = p.Value.ValueKind == JsonValueKind.True ? 1 : p.Value.GetDouble();
		return fx;
	}

	private static void Fill<T>(Table<T> table, JsonElement obj)
	{
		foreach (var p in obj.EnumerateObject()) table.Add(p.Name, p.Value.Deserialize<T>(Json)!);
	}
}
