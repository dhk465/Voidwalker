using System;
using System.Collections.Generic;
using System.Numerics;

namespace Voidwalker.Core;

/// <summary>Which side of the lobby's objects a void ball travels on.</summary>
public enum FlowLane
{
	/// <summary>Passes behind the islands and what stands on them.</summary>
	Back,
	/// <summary>Passes in front of them.</summary>
	Front,
}

/// <summary>
/// How void balls drift through the lobby and end in the Gate's rift, in hub pixels and seconds. Built from the hub's
/// config (src/data/hub_config.tres); plain numbers so the paths can be tested without the engine.
/// </summary>
/// <param name="Source">Where balls well out (the Rite's crack).</param>
/// <param name="Sink">The rift's heart, where they vanish.</param>
/// <param name="BoundsMin">Top-left of the area a ball drifts in.</param>
/// <param name="BoundsMax">Bottom-right of that area.</param>
/// <param name="StopsMin">Fewest waypoints a ball's curve passes through before it bends toward the rift.</param>
/// <param name="StopsMax">Most waypoints.</param>
/// <param name="SpeedMin">Slowest drift, pixels per second (each ball has its own pace).</param>
/// <param name="SpeedMax">Fastest drift.</param>
/// <param name="MinHop">Least distance from one waypoint to the next, pixels.</param>
/// <param name="MaxHop">Most distance from one waypoint to the next, pixels (the way to the rift may be longer).</param>
/// <param name="Sway">How far a ball's speed slowly swells and ebbs round its pace (0.25 = 25%).</param>
/// <param name="SpiralRadius">How far out of the rift's heart the spiral in begins, pixels (the rift is a tall oval).</param>
/// <param name="SpiralTurns">Turns a ball makes as it is sucked in.</param>
/// <param name="DartDistance">How far a tapped ball darts away, pixels.</param>
/// <param name="DartTime">Seconds the dart takes.</param>
public sealed record FlowSettings(Vector2 Source, Vector2 Sink, Vector2 BoundsMin, Vector2 BoundsMax, int StopsMin, int StopsMax,
	float SpeedMin, float SpeedMax, float MinHop, float MaxHop, float Sway, float SpiralRadius, float SpiralTurns,
	float DartDistance, float DartTime);

/// <summary>
/// One void ball's trip, drifting, not darting: one smooth curve from where it starts through a few waypoints round
/// the lobby, bending gently (never doubling back), at the ball's own pace that slowly swells and ebbs. It eases out
/// of the crack, meets the rift's oval moving the way the rift turns and spirals in at the speed it arrived with,
/// counter-clockwise on screen. The whole trip keeps one lane, behind the lobby's objects or in front, so it never
/// pops from one side to the other. Only a tap moves it suddenly: <see cref="Dodge"/> darts it away first, keeping
/// its lane. A pure function of time: rolled once, then only read.
/// <example><code>var rng = new Rng(7);
/// var path = FlowPath.Roll(rng, settings, settings.Source);
/// Vector2 where = path.At(3.2f);          // seconds since it set off
/// FlowLane lane = path.Lane;             // draw it behind or in front, the whole way
/// if (path.Done(t)) { /* gone into the rift */ }</code></example>
/// </summary>
public sealed class FlowPath
{
	private const float OvalX = 0.45f;       // the rift is about 0.45 as wide as it is tall
	private const float MaxTurn = 1.2f;      // radians (about 70 degrees): how far the next waypoint may bend from the heading
	private const float ApproachLead = 60f;  // pixels before the rift's oval where the curve lines up with the spiral
	private const float LineUp = 70f;        // pixels further back: where the run-in starts
	private const float RiftClear = 100f;    // pixels: waypoints keep this far from the rift, so the way in curves gently
	private const float EaseOut = 1.2f;      // seconds to come up to pace (out of the crack, or after a dart)
	private const float SwayMin = 4f, SwayMax = 8f; // seconds per swell and ebb of the speed
	private const float SpiralEase = 1.2f;
	private const int SamplesPerSpan = 48;
	private const float SamplesPerPixel = 2f;
	private const float Smooth = 22f;        // pixels: half-width of the moving average that rounds the bends
	private const int SmoothPasses = 2;
	private const float Step = 1 / 120f;     // seconds per entry in the distance table
	private const int HopTries = 10;
	private const int RollTries = 8;         // fresh sets of waypoints tried before skipping any
	private const int EntryTries = 360;      // angles round the rift's oval tried for where the spiral begins
	private const int SpiralSamples = 400;
	private const float SuckIn = 1.6f;       // how much faster a ball moves at the rift's heart than when it enters the spiral

	private readonly Vector2[] _line;        // the curve, finely sampled
	private readonly float[] _len;           // distance along the curve at each sample
	private readonly float[] _dist;          // distance travelled past the dart at t = DartTime + i * Step
	private readonly float _dartTime, _dartLen, _lastStep;
	private readonly float[] _stopLen;
	private readonly Vector2 _sink;
	private readonly Vector2[] _spiral;      // the spiral in, finely sampled
	private readonly float[] _spiralLen;     // distance along it at each sample
	private readonly float _spiralTime;

	private FlowPath(Vector2[] line, float[] len, float[] dist, float lastStep, float dartTime, float dartLen, float[] stopLen, List<Vector2> stops,
		FlowLane lane, float pace, Vector2 sink, Vector2[] spiral, float[] spiralLen, float spiralTime)
	{
		_line = line; _len = len; _dist = dist; _lastStep = lastStep; _dartTime = dartTime; _dartLen = dartLen; _stopLen = stopLen;
		Stops = stops; Lane = lane; Pace = pace; _sink = sink; _spiral = spiral; _spiralLen = spiralLen; _spiralTime = spiralTime;
		Travel = dartTime + (dist.Length - 2) * Step + lastStep; // the last step is only as long as it needs to be
	}

	/// <summary>Seconds from setting off to reaching the rift's oval.</summary>
	public float Travel { get; }
	/// <summary>Seconds from setting off to vanishing in the rift.</summary>
	public float Duration => Travel + _spiralTime;
	/// <summary>The waypoints its curve passes near (it rounds them; a dodge's dart point is not one).</summary>
	public IReadOnlyList<Vector2> Stops { get; }
	/// <summary>Behind the lobby's objects or in front of them, for the whole trip.</summary>
	public FlowLane Lane { get; }
	/// <summary>The ball's own drift speed, pixels per second (it swells and ebbs slowly round it).</summary>
	public float Pace { get; }

	/// <summary>Whether the ball has gone into the rift by <paramref name="t"/>.</summary>
	public bool Done(float t) => t >= Duration;

	/// <summary>The lane at <paramref name="t"/>: the trip's lane, all the way into the rift.</summary>
	public FlowLane LaneAt(float t) => Lane;

	/// <summary>When it passes nearest waypoint <paramref name="i"/> (as far along as the waypoint was before rounding).</summary>
	public float ArriveAt(int i)
	{
		int k = Array.BinarySearch(_dist, _stopLen[i] - _dartLen);
		if (k < 0) k = ~k;
		return _dartTime + Math.Min(k, _dist.Length - 1) * Step;
	}

	/// <summary>A trip from <paramref name="start"/>: a smooth drift through a few waypoints, then the rift, in a lane of its own.</summary>
	public static FlowPath Roll(Rng rng, FlowSettings s, Vector2 start) => Build(rng, s, start, null, null);

	/// <summary>A tapped ball's trip: a quick dart away from <paramref name="tap"/>, then a smooth drift to the rift, keeping <paramref name="lane"/>.</summary>
	public static FlowPath Dodge(Rng rng, FlowSettings s, Vector2 at, Vector2 tap, FlowLane lane)
	{
		var away = at - tap;
		away = away.LengthSquared() < 1e-4f ? new Vector2(0, -1) : Vector2.Normalize(away);
		return Build(rng, s, at, Vector2.Clamp(at + away * s.DartDistance, s.BoundsMin, s.BoundsMax), lane);
	}

	private static FlowPath Build(Rng rng, FlowSettings s, Vector2 start, Vector2? dart, FlowLane? keep)
	{
		float R(float a, float b) => a + (float)rng.Next() * (b - a);
		var lane = keep ?? (rng.Next() < 0.5 ? FlowLane.Back : FlowLane.Front);
		float pace = R(s.SpeedMin, s.SpeedMax);

		// waypoints: each hop bends no more than MaxTurn from the heading, so the curve never doubles back
		var basePts = new List<Vector2> { start };
		if (dart is { } d) basePts.Add(d);
		float heading0 = dart is { } dd ? MathF.Atan2(dd.Y - start.Y, dd.X - start.X) : R(0, MathF.Tau);
		int firstStop = basePts.Count;
		var middle = (s.BoundsMin + s.BoundsMax) / 2;
		float turns = s.SpiralTurns * R(0.8f, 1.2f);

		List<Vector2> Waypoints()
		{
			var pts = new List<Vector2>(basePts);
			float heading = heading0;
			int stops = s.StopsMin + rng.Index(s.StopsMax - s.StopsMin + 1);
			for (int i = 0; i < stops; i++)
			{
				var at = pts[^1];
				Vector2 p = at;
				for (int k = 0; k < HopTries; k++)
				{
					// a bend within MaxTurn; after a few misses, lean it toward the middle of the lobby (still within MaxTurn)
					float toMiddle = Wrap(MathF.Atan2(middle.Y - at.Y, middle.X - at.X) - heading);
					float bend = k < HopTries / 2 ? R(-MaxTurn, MaxTurn) : Math.Clamp(toMiddle + R(-0.3f, 0.3f), -MaxTurn, MaxTurn);
					float a = heading + bend, hop = R(s.MinHop, s.MaxHop);
					var q = at + new Vector2(MathF.Cos(a), MathF.Sin(a)) * hop;
					p = Vector2.Clamp(q, s.BoundsMin, s.BoundsMax);
					if (p == q && Vector2.Distance(p, s.Sink) >= RiftClear) break; // inside the lobby and clear of the rift: take it
				}
				if (Vector2.Distance(p, at) < s.MinHop * 0.5f) continue; // hemmed in by the edge: drop this waypoint rather than kink
				heading = MathF.Atan2(p.Y - at.Y, p.X - at.X);
				pts.Add(p);
			}
			return pts;
		}

		// it meets the rift's oval where neither the bend at its last waypoint nor the bend into the straight run-in is
		// sharper than MaxTurn (the gentlest such entry wins)
		bool Entry(List<Vector2> pts, out float entry)
		{
			entry = 0;
			var last = pts[^1];
			var lastDir = pts.Count > 1 ? Vector2.Normalize(last - pts[^2] + new Vector2(1e-6f, 0)) : Vector2.Zero;
			float best = float.MaxValue;
			for (int k = 0; k < EntryTries; k++)
			{
				float a = k * MathF.Tau / EntryTries;
				var e = s.Sink + new Vector2(MathF.Cos(a) * s.SpiralRadius * OvalX, MathF.Sin(a) * s.SpiralRadius);
				var tan = SpiralDir(s, a, turns);
				var come = e - tan * (ApproachLead + LineUp) - last; // from the last waypoint to where the run-in starts
				if (come.Length() < LineUp) continue;                // too close to swing round gently
				var cd = Vector2.Normalize(come);
				float bendIn = MathF.Acos(Math.Clamp(Vector2.Dot(cd, tan), -1f, 1f));
				float bendAt = lastDir == Vector2.Zero ? 0 : MathF.Acos(Math.Clamp(Vector2.Dot(lastDir, cd), -1f, 1f));
				if (bendIn > MaxTurn || bendAt > MaxTurn) continue;
				float cost = bendIn + bendAt + 0.002f * come.Length() / ApproachLead;
				if (cost < best) { best = cost; entry = a; }
			}
			return best < float.MaxValue;
		}

		// a few fresh sets of waypoints until one has a gentle way into the rift; failing that, skip waypoints from the end
		var pts = Waypoints(); float entry;
		bool fits = pts.Count > firstStop && Entry(pts, out entry);
		for (int attempt = 1; !fits && attempt < RollTries; attempt++) { pts = Waypoints(); fits = pts.Count > firstStop && Entry(pts, out entry); }
		while (!Entry(pts, out entry) && pts.Count > firstStop) pts.RemoveAt(pts.Count - 1);
		int lastStop = pts.Count - 1;
		var entryPt = s.Sink + new Vector2(MathF.Cos(entry) * s.SpiralRadius * OvalX, MathF.Sin(entry) * s.SpiralRadius);
		var tangent = SpiralDir(s, entry, turns); // the way the spiral sets off: round the oval counter-clockwise, and a little in
		pts.Add(entryPt - tangent * (ApproachLead + LineUp)); // a long straight run-in, so the swing into it is wide
		pts.Add(entryPt - tangent * ApproachLead);
		pts.Add(entryPt);

		// the curve: centripetal Catmull-Rom through every point, sampled finely, with distances along it
		var line = new List<Vector2>(); var len = new List<float>(); var spanStart = new List<int>();
		for (int i = 0; i < pts.Count - 1; i++)
		{
			var p0 = i == 0 ? pts[0] - (pts[1] - pts[0]) : pts[i - 1];
			var p3 = i + 2 < pts.Count ? pts[i + 2] : pts[^1] + tangent * ApproachLead;
			spanStart.Add(line.Count);
			int n = Math.Max(SamplesPerSpan, (int)(Vector2.Distance(pts[i], pts[i + 1]) * SamplesPerPixel)); // fine enough that the heading turns smoothly
			for (int k = 0; k < n; k++)
			{
				var q = Centripetal(p0, pts[i], pts[i + 1], p3, k / (float)n);
				len.Add(line.Count == 0 ? 0 : len[^1] + Vector2.Distance(line[^1], q));
				line.Add(q);
			}
		}
		line.Add(pts[^1]);
		spanStart.Add(line.Count - 1);

		// smoothed: a moving average over about Smooth pixels rounds every bend into a wide arc (the ball passes near its
		// waypoints rather than through them); the dart and the straight run-in into the rift are left exactly as they are
		int head = dart != null ? spanStart[1] : 0, tail = spanStart[pts.Count - 3];
		var sm = line.ToArray();
		int win = (int)(Smooth * SamplesPerPixel);
		for (int pass = 0; pass < SmoothPasses; pass++)
		{
			var src = (Vector2[])sm.Clone();
			for (int i = head + 1; i < tail; i++)
			{
				int w = Math.Min(win, Math.Min(i - head, tail - i)); // narrower near the fixed ends, so they join smoothly
				var sum = Vector2.Zero;
				for (int j = i - w; j <= i + w; j++) sum += src[j];
				sm[i] = sum / (2 * w + 1);
			}
		}
		line = new List<Vector2>(sm); len.Clear();
		for (int i = 0; i < line.Count; i++) len.Add(i == 0 ? 0 : len[^1] + Vector2.Distance(line[i - 1], line[i]));
		float total = len[^1];
		float dartLen = dart != null ? len[spanStart[1]] : 0f, dartTime = dart != null ? s.DartTime : 0f;
		var stopLen = new float[lastStop - firstStop + 1]; var stopList = new List<Vector2>();
		for (int i = firstStop; i <= lastStop; i++) { stopLen[i - firstStop] = len[spanStart[i]]; stopList.Add(pts[i]); }

		// the drift: up to pace over EaseOut seconds, then a slow swell and ebb round it; integrated into a table
		float swayPeriod = R(SwayMin, SwayMax), swayPhase = R(0, MathF.Tau);
		float Speed(float tau)
		{
			float up = Math.Min(1f, tau / EaseOut); up = up * up * (3 - 2 * up);
			return pace * Math.Max(0.15f, up) * (1 + s.Sway * MathF.Sin(MathF.Tau * tau / swayPeriod + swayPhase));
		}
		var dist = new List<float> { 0f };
		float travelled = 0f, tau0 = 0f, rest = total - dartLen, lastStep = Step;
		while (travelled < rest)
		{
			float stepLen = Speed(tau0 + Step * 0.5f) * Step;
			if (travelled + stepLen >= rest) { lastStep = Step * (rest - travelled) / stepLen; tau0 += lastStep; dist.Add(rest); break; }
			travelled += stepLen; tau0 += Step; dist.Add(travelled);
		}

		// the spiral starts at the speed the ball arrived with: its time follows from the spiral's opening speed
		// the spiral, sampled by distance along it: the ball enters at the speed it arrived with and quickens as it is
		// sucked in (SuckIn times faster at the heart), so it neither lurches nor surges round the rift's narrow oval
		float vEnd = Math.Max(1f, Speed(tau0));
		var spiral = new Vector2[SpiralSamples + 1]; var spiralLen = new float[SpiralSamples + 1];
		for (int k = 0; k <= SpiralSamples; k++)
		{
			spiral[k] = SpiralAt(s.Sink, entry, s.SpiralRadius, turns, k / (float)SpiralSamples);
			spiralLen[k] = k == 0 ? 0 : spiralLen[k - 1] + Vector2.Distance(spiral[k - 1], spiral[k]);
		}
		float spiralTime = spiralLen[^1] / (vEnd * (1 + (SuckIn - 1) / 2)); // distance over the mean speed of a steady quickening

		return new FlowPath(line.ToArray(), len.ToArray(), dist.ToArray(), lastStep, dartTime, dartLen, stopLen, stopList, lane, pace,
			s.Sink, spiral, spiralLen, spiralTime);
	}

	private static float DartEase(float s) => 1 - MathF.Pow(1 - s, 3); // fast out, slowing

	private static Vector2 SpiralDir(FlowSettings s, float entry, float turns) =>
		Vector2.Normalize(SpiralAt(s.Sink, entry, s.SpiralRadius, turns, 1e-4f) - SpiralAt(s.Sink, entry, s.SpiralRadius, turns, 0f));

	private static float Wrap(float a) => a - MathF.Tau * MathF.Floor((a + MathF.PI) / MathF.Tau);

	// q from 0 (on the oval) to 1 (the heart); the angle falls as it goes: counter-clockwise on screen
	private static Vector2 SpiralAt(Vector2 sink, float entry, float radius, float turns, float q)
	{
		float r = radius * MathF.Pow(1 - q, SpiralEase), a = entry - turns * MathF.Tau * q;
		return sink + new Vector2(MathF.Cos(a) * r * OvalX, MathF.Sin(a) * r);
	}

	/// <summary>Where the ball is <paramref name="t"/> seconds after setting off.</summary>
	public Vector2 At(float t)
	{
		if (t <= 0) return _line[0];
		if (t >= Duration) return _sink;
		if (t >= Travel)
		{
			// a steady quickening from 1 to SuckIn times the entry speed: distance = v0 (tau + (SuckIn - 1) tau^2 / (2 T))
			float tau = t - Travel, total = _spiralLen[^1], v0 = total / (_spiralTime * (1 + (SuckIn - 1) / 2));
			return Along(_spiral, _spiralLen, Math.Min(total, v0 * (tau + (SuckIn - 1) * tau * tau / (2 * _spiralTime))));
		}
		float along;
		if (t < _dartTime) along = _dartLen * DartEase(t / _dartTime);
		else
		{
			float f = (t - _dartTime) / Step; int i = Math.Clamp((int)f, 0, _dist.Length - 2);
			float u = i == _dist.Length - 2 ? (t - _dartTime - i * Step) / _lastStep : f - i;
			along = _dartLen + _dist[i] + (_dist[i + 1] - _dist[i]) * Math.Clamp(u, 0f, 1f);
		}
		return PointAt(along);
	}

	private Vector2 PointAt(float along) => Along(_line, _len, along);

	private static Vector2 Along(Vector2[] pts, float[] lens, float along)
	{
		int k = Array.BinarySearch(lens, along);
		if (k >= 0) return pts[k];
		k = ~k;
		if (k <= 0) return pts[0];
		if (k >= lens.Length) return pts[^1];
		float u = (along - lens[k - 1]) / Math.Max(1e-5f, lens[k] - lens[k - 1]);
		return Vector2.Lerp(pts[k - 1], pts[k], u);
	}

	// Catmull-Rom with centripetal knots: no cusps or loops where waypoints bunch up
	private static Vector2 Centripetal(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float u)
	{
		static float Knot(float t, Vector2 a, Vector2 b) => t + MathF.Sqrt(Math.Max(1e-4f, Vector2.Distance(a, b)));
		float t0 = 0, t1 = Knot(t0, p0, p1), t2 = Knot(t1, p1, p2), t3 = Knot(t2, p2, p3);
		float t = t1 + (t2 - t1) * u;
		var a1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
		var a2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
		var a3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;
		var b1 = (t2 - t) / (t2 - t0) * a1 + (t - t0) / (t2 - t0) * a2;
		var b2 = (t3 - t) / (t3 - t1) * a2 + (t - t1) / (t3 - t1) * a3;
		return (t2 - t) / (t2 - t1) * b1 + (t - t1) / (t2 - t1) * b2;
	}
}
