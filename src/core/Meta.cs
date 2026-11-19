using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voidwalker.Core;

/// <summary>A rift offered at the Gate.</summary>
public sealed record Rift(string Name, string Omen, string Prevail, string Boon);

/// <summary>
/// The save: the collection kept between adventures. Same shape and field names as the browser build's
/// <c>voidwalker.save.v1</c>, so a browser save can be carried over as JSON. Fields this port does not use yet are
/// kept as they were (<see cref="Extra"/>).
/// <example><code>var meta = Meta.Demo();
/// string json = meta.ToJson();
/// var back = Meta.FromJson(json); // back.Bank == 340</code></example>
/// </summary>
public sealed class Meta
{
	private static readonly JsonSerializerOptions Json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, RespectNullableAnnotations = true };

	/// <summary>Save format version; only 1 exists.</summary>
	[JsonRequired] public int V { get; set; } = 1;
	/// <summary>The nature the blob was born with.</summary>
	public string? Innate { get; set; }
	/// <summary>Natures discovered, in the order found.</summary>
	public List<string> Discovered { get; set; } = new();
	/// <summary>The collection: stack key ("natures|level") to copies owned.</summary>
	public Dictionary<string, int> Cards { get; set; } = new();
	/// <summary>The pool taken into the rift: stack key to copies.</summary>
	public Dictionary<string, int> Loadout { get; set; } = new();
	/// <summary>Fusion keys recorded in the codex.</summary>
	public List<string> Fusions { get; set; } = new();
	/// <summary>Lifetime stats (kills, motes, ...).</summary>
	public Dictionary<string, double> Stats { get; set; } = new();
	/// <summary>Void energy banked.</summary>
	public int Bank { get; set; }
	/// <summary>Adventures begun.</summary>
	public int Runs { get; set; }
	/// <summary>Adventures won.</summary>
	public int Wins { get; set; }
	/// <summary>The rift depth chosen at the Gate.</summary>
	public int Depth { get; set; }
	/// <summary>The deepest depth open.</summary>
	public int MaxDepth { get; set; }
	/// <summary>Draws since the last mythic.</summary>
	public int Pity { get; set; }
	/// <summary>Rite draws ever made (seeds each draw).</summary>
	public int Draws { get; set; }
	/// <summary>The chosen rift's omen.</summary>
	public string Omen { get; set; } = "calm";
	/// <summary>The nature the Rite is focused on, if any.</summary>
	public string? Focus { get; set; }
	/// <summary>Stack keys ever seen (a draw of an unseen one is NEW).</summary>
	public Dictionary<string, bool> Seen { get; set; } = new();
	/// <summary>Kinds met.</summary>
	public Dictionary<string, int> Bestiary { get; set; } = new();
	/// <summary>The chosen rift's prevailing nature.</summary>
	public string? Prevail { get; set; }
	/// <summary>Sigil id to level bought.</summary>
	public Dictionary<string, int> Upg { get; set; } = new();
	/// <summary>Relics owned.</summary>
	public Dictionary<string, int> Relics { get; set; } = new();
	/// <summary>Relics equipped, oldest first.</summary>
	public List<string> Equipped { get; set; } = new();
	/// <summary>Logs found.</summary>
	public Dictionary<string, int> Logs { get; set; } = new();
	/// <summary>Quest id to "claimed".</summary>
	public Dictionary<string, string> Quests { get; set; } = new();
	/// <summary>The rifts offered at the Gate (rolled once, until the next adventure).</summary>
	public List<Rift>? Rifts { get; set; }
	/// <summary>Which rift is chosen.</summary>
	public int RiftPick { get; set; }
	/// <summary>The chosen rift's boon.</summary>
	public string Boon { get; set; } = "calm";
	/// <summary>The stack chosen as the first card (the Chosen sigil).</summary>
	public string? Chosen { get; set; }
	/// <summary>Browser fields this port does not read yet, kept so nothing is lost.</summary>
	[JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }

	/// <summary>The save as JSON.</summary>
	public string ToJson() => JsonSerializer.Serialize(this, Json);

	/// <summary>Reads a version-1 save. An unreadable or unsupported save raises a JsonException.</summary>
	public static Meta FromJson(string json)
	{
		var meta = JsonSerializer.Deserialize<Meta>(json, Json);
		return meta is { V: 1 } ? meta : throw new JsonException("Unsupported save version.");
	}

	/// <summary>A new save: nothing owned.</summary>
	public static Meta Fresh() => new();

	/// <summary>A collection part-way through (the browser's <c>?demo=1</c>), used until the title screen is ported.</summary>
	public static Meta Demo()
	{
		var m = Fresh();
		m.Innate = "wrath";
		m.Discovered = new() { "wrath", "greed", "envy", "charity", "sloth", "temperance", "diligence" };
		m.Cards = new() { ["|1"] = 4, ["wrath|1"] = 3, ["wrath|2"] = 1, ["greed|1"] = 2, ["envy|1"] = 2, ["charity|1"] = 2, ["sloth|1"] = 1, ["temperance|1"] = 2, ["diligence|1"] = 1, ["greed+wrath|1"] = 1, ["envy+wrath|1"] = 1, ["charity+diligence|1"] = 1 };
		m.Loadout = new() { ["|1"] = 2, ["wrath|1"] = 3, ["wrath|2"] = 1, ["greed+wrath|1"] = 1, ["envy|1"] = 2 };
		m.Fusions = new() { "greed+wrath", "envy+wrath", "charity+diligence", "envy+sloth" };
		m.Stats = new() { ["kills"] = 52, ["motes"] = 410, ["asteroids"] = 5, ["ricochets"] = 10, ["pods"] = 14, ["feeders"] = 6, ["debris"] = 8, ["minis"] = 2, ["fell_choir"] = 1 };
		m.Bank = 340; m.Runs = 4; m.Wins = 1; m.MaxDepth = 1; m.Omen = "rockfall"; m.Focus = "wrath"; m.Pity = 6; m.Prevail = "patience";
		m.Upg = new() { ["vessel"] = 1, ["reach"] = 2 };
		m.Relics = new() { ["bell"] = 1, ["lens"] = 1, ["crown"] = 1, ["tooth"] = 1 };
		m.Equipped = new() { "crown" };
		m.Logs = new() { ["l1"] = 1, ["l9"] = 1, ["l14"] = 1 };
		foreach (var k in new[] { "pod", "husk", "grazer", "swift", "feeder", "spitter", "weeper", "mite", "warden", "asteroid", "big", "plate", "slab", "comet", "cargo", "panel" }) m.Bestiary[k] = 1;
		foreach (var k in m.Cards.Keys) m.Seen[k] = true;
		return m;
	}
}
