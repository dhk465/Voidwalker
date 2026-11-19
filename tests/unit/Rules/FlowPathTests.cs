using System;
using System.Linq;
using System.Numerics;
using Voidwalker.Core;
using Xunit;

namespace Voidwalker.Tests.Rules;

/// <summary>The void balls' smooth drifting paths from the Rite's crack to the Gate's rift (src/core/FlowPath.cs).</summary>
public class FlowPathTests
{
	private const float Frame = 1 / 60f;
	private static readonly FlowSettings Settings = new(
		Source: new Vector2(118, 213), Sink: new Vector2(512, 225),
		BoundsMin: new Vector2(70, 80), BoundsMax: new Vector2(570, 320),
		StopsMin: 1, StopsMax: 3, SpeedMin: 15f, SpeedMax: 50f, MinHop: 80f, MaxHop: 200f, Sway: 0.25f,
		SpiralRadius: 26f, SpiralTurns: 1.5f, DartDistance: 40f, DartTime: 0.45f);

	private static FlowPath[] Roll(int n, long seed = 7)
	{
		var rng = new Rng(seed);
		return Enumerable.Range(0, n).Select(_ => FlowPath.Roll(rng, Settings, Settings.Source)).ToArray();
	}

	private static float Speed(FlowPath p, float t) => Vector2.Distance(p.At(t + Frame), p.At(t)) / Frame;

	[Fact]
	public void At_Start_IsTheCrack()
	{
		foreach (var p in Roll(20)) Assert.True(Vector2.Distance(p.At(0f), Settings.Source) < 0.01f);
	}

	[Fact]
	public void At_End_IsTheRiftsHeart_AndStaysThere()
	{
		foreach (var p in Roll(20))
		{
			Assert.True(Vector2.Distance(p.At(p.Duration), Settings.Sink) < 0.05f);
			Assert.True(Vector2.Distance(p.At(p.Duration + 5f), Settings.Sink) < 0.01f);
			Assert.True(p.Done(p.Duration + 0.01f));
			Assert.False(p.Done(p.Duration - 0.01f));
		}
	}

	[Fact]
	public void Roll_WaypointsAreInsideTheLobbyAndOneToThree()
	{
		foreach (var p in Roll(30))
		{
			Assert.InRange(p.Stops.Count, Settings.StopsMin, Settings.StopsMax);
			foreach (var s in p.Stops)
			{
				Assert.InRange(s.X, Settings.BoundsMin.X, Settings.BoundsMax.X);
				Assert.InRange(s.Y, Settings.BoundsMin.Y, Settings.BoundsMax.Y);
			}
		}
	}

	[Fact]
	public void At_EachWaypoint_TheBallPassesNearIt()
	{
		// the curve is smoothed so it rounds its waypoints in wide arcs: near, not through
		foreach (var p in Roll(10))
			for (int i = 0; i < p.Stops.Count; i++)
			{
				float nearest = float.MaxValue;
				for (float t = 0; t < p.Travel; t += Frame) nearest = Math.Min(nearest, Vector2.Distance(p.At(t), p.Stops[i]));
				Assert.True(nearest < 12f, $"waypoint {i}: nearest {nearest:0.0} px");
			}
	}

	[Fact]
	public void Roll_ATripLastsFifteenToThirtySecondsMostly()
	{
		var times = Roll(60).Select(p => p.Duration).OrderBy(d => d).ToArray();
		Assert.InRange(times[times.Length / 2], 15f, 30f);
	}

	[Fact]
	public void Roll_BallsDriftAtVisiblyDifferentPaces()
	{
		var paces = Roll(20).Select(p => p.Pace).ToArray();
		Assert.True(paces.Max() >= 2f * paces.Min(), $"paces {paces.Min()}..{paces.Max()}");
		Assert.All(paces, v => Assert.InRange(v, Settings.SpeedMin, Settings.SpeedMax));
	}

	[Fact]
	public void Roll_HopsStayWithinTheLimits()
	{
		foreach (var p in Roll(30))
			for (int i = 1; i < p.Stops.Count; i++)
				Assert.InRange(Vector2.Distance(p.Stops[i - 1], p.Stops[i]), 0f, Settings.MaxHop + 0.01f);
	}

	[Fact]
	public void At_NeverJumps()
	{
		foreach (var p in Roll(20))
			for (float t = 0f; t <= p.Duration; t += Frame)
				Assert.True(Vector2.Distance(p.At(t + Frame), p.At(t)) < 2f, $"jumped {Vector2.Distance(p.At(t + Frame), p.At(t)):0.0} px at t={t:0.00} of {p.Duration:0.00} (travel {p.Travel:0.00})");
	}

	[Fact]
	public void At_NoSuddenChangesOfSpeed()
	{
		foreach (var p in Roll(20))
		{
			float last = Speed(p, 0f);
			// up to the last tenth of a second: there the ball curls into the rift's heart so tightly that a frame's
			// straight-line distance falls short of the way it went (it is vanishing, not slowing)
			for (float t = Frame; t < p.Duration - 0.1f; t += Frame)
			{
				float v = Speed(p, t);
				Assert.True(MathF.Abs(v - last) < 3f, $"speed jumped {last:0.0} -> {v:0.0} px/s at t={t:0.00} of {p.Duration:0.00}");
				last = v;
			}
		}
	}

	[Fact]
	public void At_TurnsGentlyUntilTheSpiral()
	{
		foreach (var p in Roll(20))
		{
			Vector2 Dir(float t) => Vector2.Normalize(p.At(t + Frame) - p.At(t) + new Vector2(1e-6f, 0));
			var last = Dir(0.5f - Frame);
			for (float t = 0.5f; t < p.Travel; t += Frame)
			{
				var d = Dir(t);
				float deg = MathF.Acos(Math.Clamp(Vector2.Dot(last, d), -1f, 1f)) * 180f / MathF.PI;
				Assert.True(deg < 6f, $"turned {deg:0.0} degrees in a frame at t={t:0.00} of travel {p.Travel:0.00}, near {p.At(t)}");
				last = d;
			}
		}
	}

	[Fact]
	public void Roll_TripsDifferAndUseBothLanes()
	{
		var paths = Roll(20);
		Assert.True(paths.Select(p => MathF.Round(p.Stops[0].X)).Distinct().Count() > 12);
		Assert.Contains(paths, p => p.Lane == FlowLane.Back);
		Assert.Contains(paths, p => p.Lane == FlowLane.Front);
	}

	[Fact]
	public void LaneAt_StaysTheSameForTheWholeTrip()
	{
		foreach (var p in Roll(20))
			for (float t = 0f; t <= p.Duration; t += 0.25f) Assert.Equal(p.Lane, p.LaneAt(t));
	}

	[Fact]
	public void Dodge_DartsAwayFromTheTapThenDriftsToTheRift_InItsLane()
	{
		var rng = new Rng(3);
		var at = new Vector2(300, 200);
		var p = FlowPath.Dodge(rng, Settings, at, tap: new Vector2(290, 200), FlowLane.Back);
		Assert.Equal(FlowLane.Back, p.Lane);
		Assert.True(Vector2.Distance(p.At(0f), at) < 0.01f);
		Assert.True(p.At(Settings.DartTime).X > at.X + Settings.DartDistance * 0.5f, "it should dart away from the tap (to the right)");
		Assert.True(Vector2.Distance(p.At(p.Duration), Settings.Sink) < 0.05f);
	}
}
