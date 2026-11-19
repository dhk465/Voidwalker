using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// Tuning for the Rift Altar hub: the void balls from the Rite's crack to the Gate's rift, their trails, the sky. Values live in
/// <c>res://src/data/hub_config.tres</c>, so they are tuned in the inspector, not in code.
/// </summary>
[GlobalClass]
public partial class HubConfig : Resource
{
	/// <summary>Void balls the Rite's crack keeps in the air on its own.</summary>
	[Export] public int FlowAmbient { get; set; } = 4;
	/// <summary>The most void balls at once, Rite draws included (the pool's size).</summary>
	[Export] public int FlowMax { get; set; } = 9;
	/// <summary>Seconds, least: between the crack's own balls.</summary>
	[Export] public float FlowGapMin { get; set; } = 1.2f;
	/// <summary>Seconds, most: between the crack's own balls.</summary>
	[Export] public float FlowGapMax { get; set; } = 3.2f;
	/// <summary>Seconds between balls released by a Rite draw.</summary>
	[Export] public float FlowBurstGap { get; set; } = 0.35f;
	/// <summary>Top-left of the area a void ball wanders in, in hub pixels.</summary>
	[Export] public Vector2 FlowBoundsMin { get; set; } = new(70f, 80f);
	/// <summary>Bottom-right of that area.</summary>
	[Export] public Vector2 FlowBoundsMax { get; set; } = new(570f, 320f);
	/// <summary>Fewest waypoints a ball's curve passes near before it bends toward the rift.</summary>
	[Export] public int FlowStopsMin { get; set; } = 1;
	/// <summary>Most waypoints.</summary>
	[Export] public int FlowStopsMax { get; set; } = 3;
	/// <summary>Pixels per second: the slowest a ball drifts (each has its own pace).</summary>
	[Export] public float FlowSpeedMin { get; set; } = 15f;
	/// <summary>Pixels per second: the fastest.</summary>
	[Export] public float FlowSpeedMax { get; set; } = 50f;
	/// <summary>Pixels: least distance from one stop to the next.</summary>
	[Export] public float FlowMinHop { get; set; } = 80f;
	/// <summary>Pixels: most distance from one stop to the next (the way to the rift may be longer).</summary>
	[Export] public float FlowMaxHop { get; set; } = 200f;
	/// <summary>How far a ball's speed slowly swells and ebbs round its pace (0.25 = 25%).</summary>
	[Export] public float FlowSway { get; set; } = 0.25f;
	/// <summary>Pixels: how far out of the rift's heart the spiral in begins.</summary>
	[Export] public float FlowSpiralRadius { get; set; } = 26f;
	/// <summary>Turns a ball makes round the rift as it is sucked in.</summary>
	[Export] public float FlowSpiralTurns { get; set; } = 1.5f;
	/// <summary>Pixels: how far a tapped ball darts away from the finger.</summary>
	[Export] public float FlowDartDistance { get; set; } = 40f;
	/// <summary>Seconds the dart takes.</summary>
	[Export] public float FlowDartTime { get; set; } = 0.45f;
	/// <summary>Pixels: how close to a ball a tap must land to touch it.</summary>
	[Export] public float FlowTapRadius { get; set; } = 10f;
	/// <summary>Chance a tapped ball bursts into a puff rather than darting away.</summary>
	[Export] public float FlowPopChance { get; set; } = 0.4f;
	/// <summary>Seconds a shed puff lives: the trail's length in time.</summary>
	[Export] public float TrailLifetime { get; set; } = 2.0f;
	/// <summary>Puffs alive at once per flame.</summary>
	[Export] public int TrailAmount { get; set; } = 60;
	/// <summary>Pixels per second a shed puff rises as it cools.</summary>
	[Export] public float TrailLift { get; set; } = 14f;
	/// <summary>Share of the sky's 7x7 cells that hold a star.</summary>
	[Export] public float StarDensity { get; set; } = 0.07f;
	/// <summary>Seconds: the quickest a star brightens and fades.</summary>
	[Export] public float StarBlinkMin { get; set; } = 6f;
	/// <summary>Seconds: the slowest a star brightens and fades.</summary>
	[Export] public float StarBlinkMax { get; set; } = 18f;
	/// <summary>Pixels per second the far asteroids drift.</summary>
	[Export] public float AsteroidDrift { get; set; } = 3f;
}
