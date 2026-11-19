using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// Tuning for the Rift Altar hub: the void-energy tide, the flame trails, the sky and the rift. Values live in
/// <c>res://src/data/hub_config.tres</c>, so they are tuned in the inspector, not in code.
/// </summary>
[GlobalClass]
public partial class HubConfig : Resource
{
	/// <summary>How many void flames ride the tide round the altar.</summary>
	[Export] public int FlowCount { get; set; } = 5;
	/// <summary>Radians per second the tide carries a flame round the altar.</summary>
	[Export] public float FlowSpeed { get; set; } = 0.12f;
	/// <summary>How far the tide quickens and slows (radians of lead and lag).</summary>
	[Export] public float FlowSurge { get; set; } = 0.35f;
	/// <summary>The tide's ellipse round the altar, in pixels (640x360 space).</summary>
	[Export] public Vector2 FlowRadius { get; set; } = new(235f, 95f);
	/// <summary>Where the tide is centred, in pixels.</summary>
	[Export] public Vector2 FlowCentre { get; set; } = new(320f, 165f);
	/// <summary>How far a flame bobs up and down as it rides, in pixels.</summary>
	[Export] public float FlowBob { get; set; } = 13f;
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
