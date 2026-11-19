using System;
using System.Collections.Generic;
using Godot;
using Voidwalker.Core;

namespace Voidwalker.Hub;

/// <summary>
/// The void balls of the lobby: the Rite's crack keeps a few in the air on its own (<see cref="HubConfig.FlowAmbient"/>,
/// one every few seconds), each wandering the lobby at its own pace before the Gate's rift takes it; a tap on a ball
/// makes it dart away or burst into a puff (<see cref="Tap"/>); cards drawn at the Rite release more
/// (<see cref="Burst"/>), one by one once no screen is open. Never more than <see cref="HubConfig.FlowMax"/> at once:
/// the balls are a fixed pool, made up front, so nothing is allocated while they fly.
/// <example><code>var field = new VoidField(); field.Setup(art, cfg, crack, rift); stage.AddChild(field);
/// field.Burst(10);       // ten cards drawn at the Rite
/// field.Hold = screenOpen; // releases wait while a screen covers the lobby
/// if (field.Tap(worldPoint)) return; // a ball took the tap</code></example>
/// </summary>
public partial class VoidField : Node2D
{
	private readonly List<VoidFlow> _pool = new();
	private readonly List<SheetPlayer> _pops = new();
	private const int PopPool = 3; // bursts that can play at once
	private HubConfig _cfg = new();
	private FlowSettings _settings = null!;
	private Rng _rng = new(1);
	private float _ambientWait, _burstWait;
	private int _pending;

	/// <summary>While true, balls from <see cref="Burst"/> wait (a screen covers the lobby); the crack's own still come.</summary>
	public bool Hold { get; set; }

	/// <summary>Balls waiting to be released by a Rite draw.</summary>
	public int Pending => _pending;

	/// <summary>Builds the pool and the path settings; <paramref name="crack"/> and <paramref name="rift"/> are in hub pixels.</summary>
	public void Setup(LobbyArt art, HubConfig cfg, Vector2 crack, Vector2 rift, long seed = 1)
	{
		_cfg = cfg; _rng = new Rng(seed);
		_settings = new FlowSettings(V(crack), V(rift), V(cfg.FlowBoundsMin), V(cfg.FlowBoundsMax), cfg.FlowStopsMin, cfg.FlowStopsMax,
			cfg.FlowSpeedMin, cfg.FlowSpeedMax, cfg.FlowMinHop, cfg.FlowMaxHop, cfg.FlowSway, cfg.FlowSpiralRadius, cfg.FlowSpiralTurns,
			cfg.FlowDartDistance, cfg.FlowDartTime);
		for (int i = 0; i < cfg.FlowMax; i++)
		{
			var ball = new VoidFlow { Name = $"Ball{i}" }; ball.Setup(art, cfg); AddChild(ball); _pool.Add(ball);
		}
		for (int i = 0; i < PopPool; i++)
		{
			var pop = SheetPlayer.Load("res://assets/fx/", "void-pop", Vector2.Zero); AddChild(pop); _pops.Add(pop);
		}
		// the lobby opens with the crack's balls already on their way, at different points
		for (int i = 0; i < Math.Min(cfg.FlowAmbient, _pool.Count); i++)
		{
			var path = FlowPath.Roll(_rng, _settings, _settings.Source);
			_pool[i].Launch(path, (float)_rng.Next() * path.Travel);
		}
		_ambientWait = Gap();
	}

	/// <summary>
	/// A tap at <paramref name="world"/> (hub pixels): the nearest flying ball within <see cref="HubConfig.FlowTapRadius"/>
	/// either bursts into a puff (<see cref="HubConfig.FlowPopChance"/>) or darts away from it and wanders on, keeping
	/// its lane. Returns whether a ball took the tap.
	/// </summary>
	public bool Tap(Vector2 world)
	{
		VoidFlow? best = null; float bestD = _cfg.FlowTapRadius;
		foreach (var b in _pool)
		{
			if (!b.Flying) continue;
			var w = b.Where; float d = new Vector2(w.X, w.Y).DistanceTo(world);
			if (d <= bestD) { bestD = d; best = b; }
		}
		if (best == null) return false;
		if (_rng.Next() < _cfg.FlowPopChance && Pop(best)) return true;
		best.Launch(FlowPath.Dodge(_rng, _settings, best.Where, V(world), best.Lane), 0f);
		return true;
	}

	private bool Pop(VoidFlow ball)
	{
		foreach (var pop in _pops)
		{
			if (!pop.Finished) continue;
			var w = ball.Where;
			pop.ZIndex = ball.Lane == FlowLane.Back ? VoidFlow.BehindZ : VoidFlow.FrontZ; // it bursts on the side it was on
			pop.Replay(new Vector2(w.X, w.Y));
			ball.Vanish();
			return true;
		}
		return false; // every burst is busy: it dodges instead
	}

	/// <summary>For checks and recordings: taps the first flying ball from just beside it. Returns whether there was one.</summary>
	public bool TapAny()
	{
		foreach (var b in _pool)
			if (b.Flying)
			{
				var w = b.Where; bool hit = Tap(new Vector2(w.X - 3, w.Y));
				GD.Print($"auto-tap at ({w.X:0},{w.Y:0}) {(b.Flying ? "dodged" : "burst")} lane {b.Lane}");
				return hit;
			}
		return false;
	}

	/// <summary>Queues <paramref name="cards"/> more balls (one per card drawn at the Rite), never more than the pool holds.</summary>
	public void Burst(int cards) => _pending = Math.Min(_cfg.FlowMax, _pending + Math.Max(0, cards));

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		float dt = (float)delta;
		_ambientWait -= dt; _burstWait -= dt;
		int flying = 0;
		foreach (var b in _pool) if (b.Flying) flying++;
		if (_pending > 0 && !Hold && _burstWait <= 0f && Launch()) { _pending--; _burstWait = _cfg.FlowBurstGap; return; }
		if (flying < _cfg.FlowAmbient && _ambientWait <= 0f && Launch()) _ambientWait = Gap();
	}

	private static System.Numerics.Vector2 V(Vector2 v) => new(v.X, v.Y);

	private float Gap() => _cfg.FlowGapMin + (float)_rng.Next() * (_cfg.FlowGapMax - _cfg.FlowGapMin);

	private bool Launch()
	{
		foreach (var b in _pool)
			if (b.Idle) { b.Launch(FlowPath.Roll(_rng, _settings, _settings.Source), 0f); return true; }
		return false; // all at the cap
	}
}
