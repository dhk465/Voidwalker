using Godot;
using Voidwalker.Core;

namespace Voidwalker.Adventure;

/// <summary>
/// Tuning for the adventure's screen and controls, and which kinds the port shows so far. Values live in
/// <c>res://src/data/rift_config.tres</c>; the game's own numbers (speeds, life, damage) stay in game_data.json.
/// <example><code>var settings = (Config ?? new RiftConfig()).Settings(); // what RiftRun may spawn</code></example>
/// </summary>
[GlobalClass]
public partial class RiftConfig : Resource
{
	/// <summary>Ybasog kinds migrated so far (the waves use only these).</summary>
	[Export] public string[] Ybasogs { get; set; } = { "pod", "grazer", "feeder" };
	/// <summary>Kinds of matter migrated so far.</summary>
	[Export] public string[] Matter { get; set; } = { "asteroid", "slab" };
	/// <summary>Stages migrated so far; clearing the last ends the adventure.</summary>
	[Export] public int Stages { get; set; } = 1;
	/// <summary>Seconds a grazer stops at the dive line to turn and face the blob before it charges.</summary>
	[Export] public float GrazerTurnTime { get; set; } = 0.35f;
	/// <summary>How fast a grazer turns, radians per second.</summary>
	[Export] public float GrazerTurnRate { get; set; } = 9f;
	/// <summary>Share of asteroids carrying void energy only (guessed; the rest carry scrap, or both).</summary>
	[Export] public float AsteroidVoidOnly { get; set; } = 0.5f;
	/// <summary>Share of asteroids carrying both void energy and scrap.</summary>
	[Export] public float AsteroidBoth { get; set; } = 0.2f;
	/// <summary>Scrap from an asteroid that carries it.</summary>
	[Export] public int ScrapPerAsteroid { get; set; } = 2;
	/// <summary>Scrap from a piece of wreckage (which carries no void energy).</summary>
	[Export] public int ScrapPerWreck { get; set; } = 3;
	/// <summary>Scrap from a ruin.</summary>
	[Export] public int ScrapPerRuin { get; set; } = 2;
	/// <summary>A boost's price rises by this share of its base for each one already bought this run.</summary>
	[Export] public float PriceStep { get; set; } = 0.5f;
	/// <summary>Scrap to buy back the first card from the pool; each later one costs RetrieveStep more.</summary>
	[Export] public int RetrieveBase { get; set; } = 8;
	/// <summary>See RetrieveBase.</summary>
	[Export] public int RetrieveStep { get; set; } = 4;
	/// <summary>Experience to leave adventure level 1; each level after needs XpStep more.</summary>
	[Export] public float XpBase { get; set; } = 12f;
	/// <summary>See XpBase.</summary>
	[Export] public float XpStep { get; set; } = 6f;
	// ---- feel: the tells and motion the screen shows (the rules' own numbers stay in game_data.json) ----
	/// <summary>Seconds before a spit that a feeder's maw gapes wide: the tell that a glob is coming.</summary>
	[Export] public float FeederGapeTell { get; set; } = 0.35f;
	/// <summary>Share of a pod's hatch time after which it shudders and cracks: the tell that a grazer is coming.</summary>
	[Export] public float PodCrackShare { get; set; } = 0.6f;
	/// <summary>How fast a grazer's legs run while it turns at the dive line, against its walk (1).</summary>
	[Export] public float GrazerPaceTurning { get; set; } = 0.5f;
	/// <summary>How fast a grazer's legs run on the charge, against its walk (1).</summary>
	[Export] public float GrazerPaceDive { get; set; } = 1.7f;
	/// <summary>Blinks per second while the blob is safe after a hit.</summary>
	[Export] public float IframeBlinkRate { get; set; } = 14f;
	/// <summary>How fast the blob bobs a pixel while it stands still (radians per second of the bob's wave).</summary>
	[Export] public float BobStill { get; set; } = 3f;
	/// <summary>How fast the blob bobs while it moves.</summary>
	[Export] public float BobMoving { get; set; } = 12f;
	/// <summary>How bright a struck Ybasog or rock flashes (its colour times this, clamped to white).</summary>
	[Export] public float HitFlash { get; set; } = 2.5f;
	/// <summary>Seconds a small floating word (damage, a card's name) stays up.</summary>
	[Export] public float WordTime { get; set; } = 0.9f;
	/// <summary>Seconds a big floating word (STAGE 1, a level, a discovery) stays up.</summary>
	[Export] public float BigWordTime { get; set; } = 1.8f;
	/// <summary>The longest single step of the rules, seconds (a slow frame takes several, so nothing jumps through a target).</summary>
	[Export] public float MaxStep { get; set; } = 1f / 30f;
	/// <summary>The most time one frame catches up on, seconds (a stall is not played out in full).</summary>
	[Export] public float MaxCatchUp { get; set; } = 0.5f;
	/// <summary>Game pixels of drag for full speed on a touch screen (the floating stick's reach).</summary>
	[Export] public float StickRadius { get; set; } = 22f;
	/// <summary>Game pixels of drag ignored, so a resting thumb does not creep.</summary>
	[Export] public float StickDeadzone { get; set; } = 3f;
	/// <summary>Seconds a hit flash shows white on a Ybasog or matter.</summary>
	[Export] public float FlashTime { get; set; } = 0.08f;
	/// <summary>How many of the next bolts the HUD shows.</summary>
	[Export] public int NextShown { get; set; } = 5;

	/// <summary>The rules' view of this config.</summary>
	public RiftSettings Settings() => new()
	{
		Ybasogs = Ybasogs, Matter = Matter, Stages = Stages, GrazerTurnTime = GrazerTurnTime, GrazerTurnRate = GrazerTurnRate,
		AsteroidVoidOnly = AsteroidVoidOnly, AsteroidBoth = AsteroidBoth, ScrapPerAsteroid = ScrapPerAsteroid, ScrapPerWreck = ScrapPerWreck,
		ScrapPerRuin = ScrapPerRuin, PriceStep = PriceStep, RetrieveBase = RetrieveBase, RetrieveStep = RetrieveStep, XpBase = XpBase, XpStep = XpStep,
	};
}
