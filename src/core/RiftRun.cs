using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidwalker.Core;

/// <summary>Where an adventure stands.</summary>
public enum RiftPhase { Start, Wave, Clear, Pick, Over }

/// <summary>Why the pick is open: a cleared wave, or an adventure level gained.</summary>
public enum PickReason { WaveClear, LevelUp }

/// <summary>One offer in the pick: a card from the pool to take, or a card in the deck to temper (+1 level).</summary>
public sealed class PickOffer
{
	/// <summary>A card from the pool (its natures and level), or null for a temper.</summary>
	public RiftCard? Card;
	/// <summary>The deck card a temper raises, or -1.</summary>
	public int TemperId = -1;
}

/// <summary>The pick, open after a cleared wave or a level-up: take one offer or skip, and spend scrap in the shop.</summary>
public sealed class RiftPick
{
	/// <summary>Why it opened.</summary>
	public PickReason Reason;
	/// <summary>Up to three offers.</summary>
	public List<PickOffer> Offers { get; } = new();
}

/// <summary>A run-only boost the scrap shop sells: its id, base price and what one buys.</summary>
public sealed record ScrapBoost(string Id, string Name, int BasePrice, string Says);

/// <summary>How an adventure ended.</summary>
public enum RiftOutcome
{
	/// <summary>Still going.</summary>
	None,
	/// <summary>Energy reached 0 (or the player left): part of the void energy is kept.</summary>
	Fallen,
	/// <summary>Every ported stage was cleared, but the way on is not ported yet: all the void energy is kept, no win.</summary>
	StageCleared,
	/// <summary>The Mother fell: a win.</summary>
	Won,
}

/// <summary>What the presentation should show or play for something that happened this step.</summary>
public enum RiftEventKind { Text, Sparks, Flash, Shot, Hit, Kill, Hurt, Mote, Wave, Clear, Discover, Reshuffle, Hatch, Splash }

/// <summary>Something that happened in a step, for the screen and the sound to show (browser units). For Hit and Splash,
/// <c>Size</c> is the direction of the blow in radians (atan2 of the incoming velocity).</summary>
public sealed record RiftEvent(RiftEventKind Kind, double X, double Y, string Text = "", string Color = "", double Size = 0);

/// <summary>One step's input, already read from the keyboard, mouse or touch.</summary>
/// <param name="MoveX">-1 to 1; a vector longer than 1 is normalised, a shorter one moves more slowly (a stick).</param>
/// <param name="MoveY">-1 to 1, down positive.</param>
/// <param name="Aim">The aim in radians, or null to keep the last one.</param>
/// <param name="Fire">Whether the deck fires this step (when it is ready).</param>
public readonly record struct RiftInput(double MoveX, double MoveY, double? Aim, bool Fire);

/// <summary>Which kinds the port can show so far (the rest of the browser's bestiary is not migrated yet), and how many stages.</summary>
public sealed class RiftSettings
{
	/// <summary>Ybasog kinds the waves may use.</summary>
	public string[] Ybasogs { get; init; } = { "pod", "grazer", "feeder" };
	/// <summary>Kinds of matter the waves may use.</summary>
	public string[] Matter { get; init; } = { "asteroid", "slab" };
	/// <summary>How many stages are ported; clearing the last one ends the adventure.</summary>
	public int Stages { get; init; } = 1;
	/// <summary>Seconds a grazer holds still at the dive line, turning to face the blob, before it charges.</summary>
	public double GrazerTurnTime { get; init; } = 0.35;
	/// <summary>How fast a grazer turns (radians per second).</summary>
	public double GrazerTurnRate { get; init; } = 9;

	// ---- scrap: the adventure-only currency (guessed values; tuned in src/data/rift_config.tres) ----
	/// <summary>Share of asteroids that carry void energy only; <see cref="AsteroidBoth"/> carry both; the rest scrap only.</summary>
	public double AsteroidVoidOnly { get; init; } = 0.5;
	/// <summary>Share of asteroids that carry both void energy and scrap.</summary>
	public double AsteroidBoth { get; init; } = 0.2;
	/// <summary>Scrap from an asteroid that carries it.</summary>
	public int ScrapPerAsteroid { get; init; } = 2;
	/// <summary>Scrap from a piece of wreckage (debris); wreckage carries no void energy.</summary>
	public int ScrapPerWreck { get; init; } = 3;
	/// <summary>Scrap from a ruin (ruins carry void energy too).</summary>
	public int ScrapPerRuin { get; init; } = 2;
	/// <summary>Each boost's price rises by this share of its base for every one already bought this run.</summary>
	public double PriceStep { get; init; } = 0.5;
	/// <summary>Scrap to buy back the first card from the pool; each later one costs <see cref="RetrieveStep"/> more.</summary>
	public int RetrieveBase { get; init; } = 8;
	/// <summary>See <see cref="RetrieveBase"/>.</summary>
	public int RetrieveStep { get; init; } = 4;
	/// <summary>Fire rate one "fire" boost adds (a share).</summary>
	public double FireRateBoost { get; init; } = 0.10;
	/// <summary>Bolt damage one "damage" boost adds (a share).</summary>
	public double DamageBoost { get; init; } = 0.10;
	/// <summary>Pull reach one "magnet" boost adds (browser px).</summary>
	public double MagnetBoost { get; init; } = 40;
	/// <summary>Speed one "speed" boost adds (a share).</summary>
	public double SpeedBoost { get; init; } = 0.06;
	/// <summary>Energy one "heal" restores.</summary>
	public double HealBoost { get; init; } = 5;
	/// <summary>The scrap shop's boosts (run-only), in display order.</summary>
	public ScrapBoost[] Boosts { get; init; } =
	{
		new("fire", "Fire+10%", 6, "+10% fire rate"), new("damage", "Dmg+10%", 6, "+10% bolt damage"),
		new("bounce", "Bounce+1", 10, "+1 ricochet"), new("magnet", "Reach+40", 4, "+40 px pull"),
		new("speed", "Speed+6%", 5, "+6% speed"), new("heal", "Heal+5", 4, "+5 energy"), new("ward", "Ward+1", 8, "+1 ward"),
	};

	// ---- adventure levels (this run only) ----
	/// <summary>Experience to leave level 1; each level after needs <see cref="XpStep"/> more.</summary>
	public double XpBase { get; init; } = 12;
	/// <summary>See <see cref="XpBase"/>.</summary>
	public double XpStep { get; init; } = 6;
	/// <summary>Experience a piece of matter gives when broken (a Ybasog gives its void drop).</summary>
	public double XpPerMatter { get; init; } = 1;

	/// <summary>Experience needed, at <paramref name="level"/>, to reach the next level.</summary>
	public double XpToNext(int level) => XpBase + XpStep * (level - 1);
}

/// <summary>A card in the adventure's deck.</summary>
public sealed class RiftCard
{
	/// <summary>Its id in the draw and discard piles.</summary>
	public int Id;
	/// <summary>Its natures (empty: a Void Bolt).</summary>
	public string[] Natures = Array.Empty<string>();
	/// <summary>Its level, from 1.</summary>
	public int Level = 1;
}

/// <summary>The blob.</summary>
public sealed class RiftBlob
{
	/// <summary>Position (browser units).</summary>
	public double X, Y;
	/// <summary>Aim in radians (straight up to begin).</summary>
	public double Aim = -Math.PI / 2;
	/// <summary>Seconds of safety left after a hit.</summary>
	public double Iframe;
	/// <summary>Whether it stood still this step.</summary>
	public bool Still = true;
}

/// <summary>A Ybasog in the rift.</summary>
public sealed class RiftFoe
{
	/// <summary>Kind id and family.</summary>
	public string K = "", Fam = "";
	/// <summary>Position, velocity, radius, life, drop, and the timers the browser keeps (t1, age, flash).</summary>
	public double X, Y, Vx, Vy, R, Hp, MaxHp, Drop, Flash, Ph, Age, T1, T2, T3, StopY;
	/// <summary>A grazer that has started its dive.</summary>
	public bool Dive;
	/// <summary>Which way it faces, in radians (atan2 of its heading: pi/2 is down the rift).</summary>
	public double Heading = Math.PI / 2;
	/// <summary>A diving grazer still turning to face the blob before it charges.</summary>
	public bool Turning;
	/// <summary>Dead, and gone off the bottom without being killed.</summary>
	public bool Dead, Gone;
}

/// <summary>A piece of falling matter.</summary>
public sealed class RiftMatter
{
	/// <summary>Kind id and family.</summary>
	public string K = "", Fam = "";
	/// <summary>Position, velocity, radius, life, drop and hit flash.</summary>
	public double X, Y, Vx, Vy, R, Hp, MaxHp, Drop, Flash, Age;
	/// <summary>A box (debris, ruins) rather than a disc; its size and angle.</summary>
	public bool Rect;
	/// <summary>Box size and angle (kept level, so the pixel sprite and the collision agree).</summary>
	public double W, H, A;
	/// <summary>Which of the kind's sprites it wears.</summary>
	public int Variant;
	/// <summary>What breaking it gives: void energy, scrap, or both (an asteroid shows which).</summary>
	public bool CarriesVoid = true, CarriesScrap;
	/// <summary>Its turn in radians, and how fast it spins (radians per second).</summary>
	public double Rot, Spin;
	/// <summary>Dead, and gone off the bottom without being broken.</summary>
	public bool Dead, Gone;
}

/// <summary>A bolt fired from a card.</summary>
public sealed class RiftBolt
{
	/// <summary>Position, velocity, radius and damage.</summary>
	public double X, Y, Vx, Vy, R, Dmg, Age;
	/// <summary>Ricochets left, and ricochets made.</summary>
	public int Bounces, Bounced;
	/// <summary>The card's colour (#rrggbb), natures and level.</summary>
	public string Color = "#ffffff";
	/// <summary>The card's natures.</summary>
	public string[] Natures = Array.Empty<string>();
	/// <summary>Targets touched since the last ricochet (a bolt hits a target once per pass).</summary>
	public readonly HashSet<object> HitSet = new();
	/// <summary>Spent.</summary>
	public bool Dead;
}

/// <summary>A glob spat by a feeder.</summary>
public sealed class RiftGlob
{
	/// <summary>Position and velocity.</summary>
	public double X, Y, Vx, Vy;
	/// <summary>Spent.</summary>
	public bool Dead;
}

/// <summary>A mote of void energy flying to the blob.</summary>
public sealed class RiftMote
{
	/// <summary>Position, velocity, age and twinkle phase.</summary>
	public double X, Y, Vx, Vy, T, Ph;
	/// <summary>Collected.</summary>
	public bool Dead;
}

/// <summary>A piece of scrap flying to the blob.</summary>
public sealed class RiftScrapBit
{
	/// <summary>Position, velocity, age and turn.</summary>
	public double X, Y, Vx, Vy, T, Ph;
	/// <summary>Collected.</summary>
	public bool Dead;
}

/// <summary>One spawn in a wave's timetable.</summary>
public readonly record struct RiftSpawn(double T, string K, double X);

/// <summary>
/// The adventure, ported from the browser build (index.html: reset, startingHand, makeWave, spawn, damage, kill, fire,
/// launch, updatePlay, endRun). It runs in the browser's own units (a 1280x720 screen, the rift between x 100 and
/// 1180) with the browser's tuning values from <see cref="GameData"/>, so behaviour matches line for line; the Godot
/// scene only scales it to the screen. Plain C#, seeded, no engine types: stepped with the same inputs it repeats.
/// This migration covers the core loop: the deck as ammunition, ricocheting bolts, energy and wards, void motes, pods,
/// grazers, feeders, asteroids and debris slabs, and the stage's waves. Nature effects, card drops and picks, natured
/// Ybasogs, the other kinds, bosses and the Rift Rest come in later migrations (see design/voidwalker-reference.md, 18).
/// <example><code>var run = new RiftRun(data, rules, new RiftSettings());
/// run.Step(1 / 60.0, new RiftInput(0, 0, run.AimAtNearest(), Fire: true));
/// if (run.Outcome != RiftOutcome.None) session.Save(); // the run has banked its void energy into the save</code></example>
/// </summary>
public sealed class RiftRun
{
	/// <summary>The browser's screen, in its own units.</summary>
	public const double W = 1280, H = 720;
	/// <summary>The rift's walls and centre line.</summary>
	public const double RL = 100, RR = 1180, RCX = (RL + RR) / 2;
	/// <summary>The blob's radius, and the highest it may roam.</summary>
	public const double PR = 18, PTOP = 70;
	private const double WF = (RR - RL) / 600; // how much wider the rift is than the first build's

	private readonly GameData _d;
	private readonly AltarRules _rules;
	private readonly RiftSettings _s;
	private readonly Rng _rng;
	private readonly Dictionary<string, double> _tr, _bo, _omen, _boon;
	private readonly HashSet<string> _met = new();
	private readonly List<RiftCard> _pool = new();
	private List<int> _draw = new(), _discard = new();
	private List<RiftSpawn> _wave = new();
	private int _waveIdx, _nextId = 1;
	private double _waveT, _startT = 1.2, _clearT;
	private bool _waveHurt, _waveLeak, _stageHurt;
	private readonly List<double> _recentKills = new();

	/// <summary>The blob.</summary>
	public RiftBlob Player { get; } = new() { X = RCX, Y = 670 };
	/// <summary>Ybasogs in the rift.</summary>
	public List<RiftFoe> Foes { get; } = new();
	/// <summary>Falling matter in the rift.</summary>
	public List<RiftMatter> Matter { get; } = new();
	/// <summary>Bolts in flight.</summary>
	public List<RiftBolt> Bolts { get; } = new();
	/// <summary>Globs in flight.</summary>
	public List<RiftGlob> Globs { get; } = new();
	/// <summary>Void energy motes.</summary>
	public List<RiftMote> Motes { get; } = new();
	/// <summary>Scrap flying to the blob.</summary>
	public List<RiftScrapBit> ScrapBits { get; } = new();
	/// <summary>Scrap gathered this adventure: spent in the pick's shop, lost when the adventure ends.</summary>
	public int Scrap { get; set; }
	/// <summary>The adventure's own level (this run only), from 1.</summary>
	public int Level { get; private set; } = 1;
	/// <summary>Experience toward the next level.</summary>
	public double Xp { get; private set; }
	/// <summary>The pick, while it is open.</summary>
	public RiftPick? Pick { get; private set; }
	/// <summary>The pool's cards not yet found this adventure (the shop can buy one back).</summary>
	public IReadOnlyList<RiftCard> PoolLeft => _pool;
	/// <summary>The settings this run uses.</summary>
	public RiftSettings Settings => _s;
	private readonly Dictionary<string, int> _bought = new();
	private int _retrieved, _levelsOwed;
	/// <summary>What happened this step; the caller reads it after <see cref="Step"/> and clears it.</summary>
	public List<RiftEvent> Events { get; } = new();
	/// <summary>The adventure's deck: one void card to begin, more from sigils.</summary>
	public List<RiftCard> Deck { get; } = new();

	/// <summary>Where the adventure stands.</summary>
	public RiftPhase Phase { get; private set; } = RiftPhase.Start;
	/// <summary>How it ended (None while it runs).</summary>
	public RiftOutcome Outcome { get; private set; }
	/// <summary>Stage (1 to 3) and wave (1 to 3).</summary>
	public int Stage { get; private set; } = 1;
	/// <summary>The wave within the stage (1 to 3).</summary>
	public int WaveN { get; private set; } = 1;
	/// <summary>Seconds played.</summary>
	public double T { get; private set; }
	/// <summary>Energy is life.</summary>
	public double Energy { get; set; }
	/// <summary>The most energy the blob holds.</summary>
	public double MaxEnergy { get; set; }
	/// <summary>Void energy gathered this adventure (banked at the end).</summary>
	public int VoidE { get; set; }
	/// <summary>Ybasogs killed.</summary>
	public int Kills { get; private set; }
	/// <summary>Wards: each one stops a hit.</summary>
	public int Ward { get; private set; }
	/// <summary>Seconds the game freezes on a heavy blow (the step does nothing while it runs down).</summary>
	public double Hitstop { get; private set; }
	/// <summary>Screen shake, in browser pixels; it dies away.</summary>
	public double Shake { get; private set; }
	/// <summary>Seconds until the deck can fire again, and until a reshuffle is done.</summary>
	public double FireCd { get; private set; }
	/// <summary>Seconds until a reshuffle of the discard pile is done.</summary>
	public double ReshuffleT { get; private set; }
	/// <summary>The blob's form (its main nature), or null when shapeless.</summary>
	public string? Form { get; }
	/// <summary>Void energy multiplier from the omen, boon, sigils and depth.</summary>
	public double VoidMul { get; }
	/// <summary>Void energy banked when it ended.</summary>
	public int Banked { get; private set; }
	/// <summary>The cards next to fire, in order (the HUD shows a few).</summary>
	public IEnumerable<RiftCard> NextCards => _draw.Select(CardById).OfType<RiftCard>();
	/// <summary>The current wave's timetable.</summary>
	public IReadOnlyList<RiftSpawn> WaveList => _wave;

	/// <summary>A new adventure from the save's pool, form, sigils, relics, rift and depth.</summary>
	public RiftRun(GameData data, AltarRules rules, RiftSettings settings)
	{
		_d = data; _rules = rules; _s = settings;
		var meta = rules.Meta;
		_rng = new Rng((long)data.Seed + (long)meta.Runs * 7919);
		_omen = data.OmenFx.GetValueOrDefault(meta.Omen) ?? new();
		_boon = data.BoonFx.GetValueOrDefault(meta.Boon) ?? new();
		_bo = rules.Bonuses();
		foreach (var s in AltarRules.Stacks(meta.Loadout))
			for (int i = 0; i < s.N; i++) _pool.Add(new RiftCard { Natures = s.Natures, Level = s.Level });
		Form = rules.DominantNature();
		_tr = new Dictionary<string, double> { ["damage"] = 1, ["max_energy"] = 0, ["magnet"] = 0, ["mote_bonus"] = 0, ["fire_rate"] = 1, ["bite"] = 0,
			["bounces"] = 0, ["status"] = 1, ["contact_charm"] = 0, ["speed"] = 1, ["aura"] = 0, ["armor"] = 0, ["wave_heal"] = 0, ["iframes"] = 0,
			["quick_shuffle"] = 0, ["regen"] = 0, ["glob_dodge"] = 0 };
		if (Form != null) foreach (var (k, v) in data.Traits[Form]) _tr[k] = v;
		_tr["max_energy"] += B("max_energy"); _tr["magnet"] += B("magnet"); _tr["bounces"] += B("bounces");
		_tr["speed"] *= 1 + B("speed"); _tr["damage"] *= 1 + B("damage"); _tr["fire_rate"] *= 1 + B("fire_rate"); _tr["status"] *= 1 + B("status");
		_tr["glob_dodge"] = 1 - (1 - _tr["glob_dodge"]) * (1 - B("glob_dodge"));
		VoidMul = O("void", 1) * Bn("void", 1) * (1 + B("void_mul")) * (1 + Tune("depth_void") * meta.Depth);
		_hpMul = O("hp", 1) * (1 + Tune("depth_hp") * meta.Depth);
		MaxEnergy = Energy = Tune("max_energy") + _tr["max_energy"];
		Ward = (int)B("ward_start");
		StartingHand();
		if (meta.Omen != "calm" && _d.Omens.Has(meta.Omen)) Say(_d.Omens[meta.Omen].Name.ToUpperInvariant(), RCX, 280, "#ffd27a", 26);
	}

	private readonly double _hpMul;
	private double Tune(string k) => _d.Tune(k);
	private double B(string k) => _bo.GetValueOrDefault(k);
	private double O(string k, double def = 0) => _omen.TryGetValue(k, out var v) ? v : def;
	private double Bn(string k, double def = 0) => _boon.TryGetValue(k, out var v) ? v : def;
	private double Tr(string k) => _tr[k];
	private RiftCard? CardById(int id) => Deck.FirstOrDefault(c => c.Id == id);

	// ---- cards ------------------------------------------------------------------------------------

	private void StartingHand()
	{
		var meta = _rules.Meta;
		Deck.Add(new RiftCard { Id = _nextId++ });
		if (B("inborn") > 0 && Form != null) Deck.Add(new RiftCard { Id = _nextId++, Natures = new[] { Form } }); // the Inborn sigil
		if (B("chosen") > 0 && meta.Chosen != null) // the Chosen sigil: the card picked at the Gate leaves the pool for the hand
		{
			int i = _pool.FindIndex(c => AltarRules.StackKey(c.Natures, c.Level) == meta.Chosen);
			if (i >= 0) { var c = _pool[i]; _pool.RemoveAt(i); Deck.Add(new RiftCard { Id = _nextId++, Natures = c.Natures, Level = c.Level }); }
		}
	}

	private List<int> Shuffle(List<int> a)
	{
		for (int i = a.Count - 1; i > 0; i--) { int j = (int)(_rng.Next() * (i + 1)); (a[i], a[j]) = (a[j], a[i]); }
		return a;
	}

	private void RebuildPiles() { _draw = Shuffle(Deck.Select(c => c.Id).ToList()); _discard = new(); }

	private double CardDamage(RiftCard card) =>
		Tune("bolt_damage") * Tr("damage") * (1 + Tune("level_bonus") * (card.Level - 1)) * (card.Natures.Length >= 2 ? 1 + Tune("fused_bonus") : 1);

	private void FireDeck()
	{
		if (_draw.Count == 0)
		{
			if (_discard.Count == 0) return;
			_draw = Shuffle(_discard); _discard = new(); ReshuffleT = Tr("quick_shuffle") > 0 ? 0 : Tune("reshuffle_time");
			Events.Add(new RiftEvent(RiftEventKind.Reshuffle, Player.X, Player.Y));
			return;
		}
		int id = _draw[0]; _draw.RemoveAt(0); _discard.Add(id);
		if (CardById(id) is { } card) Launch(card);
	}

	private void Launch(RiftCard card)
	{
		var p = Player; double a = p.Aim, sp = Tune("bolt_speed");
		string color = _rules.CardColor(card.Natures);
		var b = new RiftBolt { X = p.X + Math.Cos(a) * 24, Y = p.Y + Math.Sin(a) * 24, Vx = Math.Cos(a) * sp, Vy = Math.Sin(a) * sp, R = 6, Color = color,
			Natures = card.Natures, Dmg = CardDamage(card), Bounces = (int)(Tune("bolt_bounces") + Tr("bounces") + O("bounces")) };
		Bolts.Add(b);
		Events.Add(new RiftEvent(RiftEventKind.Shot, b.X, b.Y, Color: color));
	}

	// ---- waves --------------------------------------------------------------------------------------

	private int Reach() => Stage + (_rules.Meta.Depth >= 2 ? 1 : 0);
	private bool CanMeet(string k) => _d.YbasogStats.Has(k) ? _d.YbasogStats[k].From <= Reach() : _d.MatterStats[k].From <= Reach();
	private IEnumerable<string> Minions => _s.Ybasogs.Where(k => _d.YbasogStats.Has(k) && _d.YbasogStats[k].Role == "minion");
	private IEnumerable<string> Matters => _s.Matter.Where(k => _d.MatterStats.Has(k) && !_d.MatterStats[k].Only);

	private string PickW(IReadOnlyList<string> list, Func<string, double> w)
	{
		double tot = list.Sum(w), r = _rng.Next() * tot;
		foreach (var k in list) { r -= w(k); if (r <= 0) return k; }
		return list[^1];
	}

	private string? ChooseDebut(IEnumerable<string> list)
	{
		var f = list.Where(k => CanMeet(k) && !_met.Contains(k)).ToList(); if (f.Count == 0) return null;
		var never = f.Where(k => _rules.Meta.Bestiary.GetValueOrDefault(k) <= 0).ToList(); var pool = never.Count > 0 ? never : f;
		int From(string k) => _d.YbasogStats.Has(k) ? _d.YbasogStats[k].From : _d.MatterStats[k].From;
		int lo = pool.Min(From); var low = pool.Where(k => From(k) == lo).ToList();
		return low[_rng.Index(low.Count)];
	}

	private List<string> MetOf(IEnumerable<string> list, Func<string, bool>? fn = null) => list.Where(k => _met.Contains(k) && (fn == null || fn(k))).ToList();
	private double X(double m = 50) => RL + m + _rng.Next() * (RR - RL - 2 * m);
	private string PickY(Func<string, bool>? fn = null)
	{
		var l = MetOf(Minions, fn);
		return l.Count > 0 ? PickW(l, k => _d.YbasogStats[k].W * (_d.YbasogStats[k].Fam == "pod" ? O("pods", 1) : 1)) : "pod";
	}
	private string? PickM(string fam)
	{
		var l = MetOf(Matters, k => _d.MatterStats[k].Fam == fam);
		return l.Count > 0 ? PickW(l, k => _d.MatterStats[k].Wt * (fam == "rock" ? O("rocks", 1) : 1)) : null;
	}

	// The beats a wave is built from, as in the browser; each returns the time after it.
	private double Beat(string b, List<RiftSpawn> L, double t)
	{
		int s = Stage;
		switch (b)
		{
			case "trickle": { int n = (int)Math.Round((3 + s) * 1.4); for (int i = 0; i < n; i++) L.Add(new(t + i * 1.1, PickY(), X())); return t + n * 1.1 + 0.6; }
			case "line": { string k = PickY(k => _d.YbasogStats[k].Fam is "pod" or "grazer" or "mite"); int n = 6 + 2 * (s > 1 ? 1 : 0); for (int i = 0; i < n; i++) L.Add(new(t, k, RL + 70 + i * (RR - RL - 140) / (n - 1))); return t + 3.2; }
			case "vee": { string k = PickY(k => _d.YbasogStats[k].Fam is "pod" or "grazer"); double c = X(200); for (int i = -3; i <= 3; i++) L.Add(new(t + Math.Abs(i) * 0.3, k, c + i * 55)); return t + 3.6; }
			case "column": { string k = PickY(); double x = X(); for (int i = 0; i < 3; i++) L.Add(new(t + i * 0.7, k, x)); return t + 2.8; }
			case "flank": { string k = PickY(k => _d.YbasogStats[k].Fam != "crawler"); for (int i = 0; i < 3; i++) { L.Add(new(t + i * 0.7, k, RL + 45)); L.Add(new(t + i * 0.7 + 0.35, k, RR - 45)); } return t + 3.6; }
			case "rockfall": { int n = (int)Math.Round((3 + s) * 1.4); for (int i = 0; i < n; i++) { var k = PickM("rock"); if (k != null) L.Add(new(t + i * 0.7, k, X())); } return t + 4; }
			case "wreckfield": { int n = (int)Math.Round((2 + s) * 1.4); for (int i = 0; i < n; i++) { var k = PickM("wreck"); if (k != null) L.Add(new(t + i * 0.9, k, X(80))); } return t + 4; }
			case "elite": { string k = PickY(k => _d.YbasogStats[k].Hp >= 8); L.Add(new(t, k, X(100))); for (int i = 0; i < 4; i++) L.Add(new(t + 0.5 + i * 0.4, "pod", X())); if (s > 1) L.Add(new(t + 1.8, PickY(k => _d.YbasogStats[k].Hp >= 8), X(100))); return t + 5; }
			default: return t + 3.5; // lull
		}
	}

	private Dictionary<string, double> BeatWeights()
	{
		int s = Stage;
		bool Has(string fam) => MetOf(Matters, k => _d.MatterStats[k].Fam == fam).Count > 0;
		// the swarm and ruin-wall beats wait for mites and ruins, which are not migrated yet
		return new() { ["trickle"] = 3, ["line"] = 1.4, ["vee"] = 1.1, ["column"] = 1, ["flank"] = s > 1 ? 1.2 : 0.4,
			["rockfall"] = 1.2 * O("rocks", 1), ["wreckfield"] = Has("wreck") ? 1 : 0, ["elite"] = 0.4 + 0.4 * s, ["lull"] = 0.5 };
	}

	/// <summary>Builds wave <paramref name="w"/> of stage <paramref name="s"/>: the first minute by hand, then debuts and beats.</summary>
	private List<RiftSpawn> MakeWave(int s, int w)
	{
		var L = new List<RiftSpawn>();
		_rng.Next(); // the browser rolls the wave's nature tide here; natured Ybasogs are not migrated yet, so it is drawn and unused
		if (s == 1 && w == 1)
		{
			foreach (var k in new[] { "pod", "grazer", "asteroid", "slab" }) _met.Add(k);
			var hand = new List<RiftSpawn> { new(0.6, "asteroid", 560), new(1.6, "pod", 520), new(2.6, "pod", 760), new(3.6, "pod", 640),
				new(9, "pod", 450), new(11, "pod", 830), new(13, "asteroid", 720), new(18, "pod", 500), new(21, "pod", 780),
				new(27, "slab", 640), new(28.4, "grazer", 600), new(29, "grazer", 680), new(29.6, "grazer", 640), new(34, "pod", 560), new(35, "pod", 720) };
			if (O("rocks") > 0) { hand.Add(new(6, "asteroid", 450)); hand.Add(new(16, "asteroid", 820)); }
			if (O("pods") > 0) { hand.Add(new(5, "pod", 600)); hand.Add(new(14, "pod", 700)); hand.Add(new(24, "pod", 460)); }
			L.AddRange(hand.Select(e => e with { X = RCX + (e.X - 640) * WF })); // spread to the wide rift
			return L.OrderBy(e => e.T).ToList();
		}
		double t = 0.8;
		for (int i = 0; i < Tune("debuts_per_wave") + Bn("debut"); i++) // debut: the new kind, alone, so it can be read
		{
			var k = ChooseDebut(Minions); if (k == null) break;
			_met.Add(k); int n = _d.YbasogStats[k].Group > 0 ? 1 : 2; for (int j = 0; j < n; j++) L.Add(new(t + j * 1.4, k, X(120))); t += 4.5;
		}
		var km = ChooseDebut(Matters); if (km != null) { _met.Add(km); L.Add(new(t, km, X(110))); t += 3; }
		int beats = (int)(Tune("beats_base") + Tune("beats_step") * (w - 1 + s - 1));
		var bw = BeatWeights(); var keys = bw.Keys.ToList(); string last = "";
		for (int i = 0; i < beats; i++)
		{
			string b; do { b = PickW(keys, k => bw[k]); } while (b == last && b == "lull");
			last = b; t = Beat(b, L, t) + 0.4;
		}
		// a mini-boss closes wave 3 of stage 1 and wave 2 of later stages once mini-bosses are migrated
		return L.OrderBy(e => e.T).ToList();
	}

	private void StartWave()
	{
		if (WaveN == 1 && Stage > 1 && B("ward_stage") > 0) Ward = (int)Math.Min(Tune("ward_max"), Ward + B("ward_stage"));
		_wave = MakeWave(Stage, WaveN); _waveIdx = 0; _waveT = 0;
		Phase = RiftPhase.Wave; _waveHurt = false; _waveLeak = false; RebuildPiles();
		Say(WaveN == 1 ? $"STAGE {Stage}" : $"WAVE {WaveN}", RCX, 330, "#e9dcff", 30);
		Events.Add(new RiftEvent(RiftEventKind.Wave, RCX, 330, $"{Stage}.{WaveN}"));
	}

	private double HpScale() => (1 + Tune("stage_hp_scale") * (Stage - 1)) * _hpMul;
	private double SpScale() => 1 + Tune("stage_speed_scale") * (Stage - 1);

	private void Meet(string k) => _rules.Meta.Bestiary[k] = 1;

	/// <summary>Brings a Ybasog or a piece of matter into the rift (y -50: just above the top).</summary>
	public void Spawn(string k, double x, double y = -50)
	{
		if (_d.YbasogStats.Has(k)) SpawnY(k, x, y);
		else if (_d.MatterStats.Has(k)) SpawnM(k, x, y);
	}

	private RiftFoe SpawnY(string k, double x, double y)
	{
		var d = _d.YbasogStats[k];
		double hp = d.Hp * HpScale();
		var e = new RiftFoe { K = k, Fam = d.Fam, X = x, Y = y, Vy = d.Sp * SpScale(), R = d.R, Hp = hp, MaxHp = hp, Drop = d.Drop,
			Ph = _rng.Next() * 6.28, T1 = 1 + _rng.Next(), T2 = 2 + _rng.Next() * 2, T3 = 3 + _rng.Next() * 2, StopY = 140 + _rng.Next() * 120 };
		if (e.Fam == "pod") e.Vx = (_rng.Next() - 0.5) * 20;
		Foes.Add(e); Meet(k);
		return e;
	}

	private RiftMatter SpawnM(string k, double x, double y)
	{
		var d = _d.MatterStats[k];
		double hp = d.Hp * HpScale();
		double drop = d.Drop * (d.Fam == "rock" && O("rockVoid") > 0 ? O("rockVoid") : d.Fam == "ruin" && O("ruinVoid") > 0 ? O("ruinVoid") : 1);
		var m = new RiftMatter { K = k, Fam = d.Fam, X = x, Y = y, Vx = (_rng.Next() - 0.5) * d.Drift, Vy = d.Sp * (0.85 + _rng.Next() * 0.3) * SpScale(),
			Hp = hp, MaxHp = hp, Drop = drop };
		if (d.Shape == "rect")
		{
			m.Rect = true; m.W = d.W; m.H = d.H; m.R = Math.Sqrt(d.W * d.W + d.H * d.H) / 2;
			_rng.Next(); if (d.Tilt) _rng.Next(); // the browser tilts and spins debris; the port keeps it level for the pixel sprite
		}
		else
		{
			m.R = d.R;
			int n = 9 + (int)(_rng.Next() * 4); for (int i = 0; i < n; i++) _rng.Next(); // the browser's outline: here one of three drawn shapes
			m.Variant = (n - 9) % 3;
			m.Rot = _rng.Next() * 6; m.Spin = (_rng.Next() - 0.5) * 0.8; // the sprite sheet shows the turn
		}
		if (d.Fam == "rock") // an asteroid carries void energy, scrap, or both
		{
			double r = _rng.Next();
			m.CarriesVoid = r < _s.AsteroidVoidOnly + _s.AsteroidBoth;
			m.CarriesScrap = r >= _s.AsteroidVoidOnly;
		}
		else if (d.Fam == "wreck") { m.CarriesVoid = false; m.CarriesScrap = true; } // wreckage is scrap
		else if (d.Fam == "ruin") { m.CarriesVoid = true; m.CarriesScrap = true; }
		Matter.Add(m); Meet(k);
		return m;
	}

	// ---- damage, kills and the blob's health ----------------------------------------------------------

	private void Say(string s, double x, double y, string c, double size = 18) => Events.Add(new RiftEvent(RiftEventKind.Text, x, y, s, c, size));
	private void Sparks(double x, double y, string c, int n) => Events.Add(new RiftEvent(RiftEventKind.Sparks, x, y, Color: c, Size: n));

	private void Track(string stat, double n = 1)
	{
		foreach (var nat in _rules.Track(stat, n))
			Say($"{_d.Natures[nat].Name.ToUpperInvariant()} FOUND", RCX, 220, _d.Natures[nat].Color, 34);
	}

	private void Hurt(double n)
	{
		if (n <= 0) return;
		if (Ward > 0) { Ward--; Say("Ward!", Player.X, Player.Y - 40, "#fff0b0"); return; }
		if (Tr("armor") > 0) n = Math.Max(1, n - Tr("armor"));
		Energy -= n; _waveHurt = true; _stageHurt = true; Hitstop = Math.Max(Hitstop, Tune("hitstop")); Shake = Math.Min(12, Shake + 4 + n * 2);
		Say($"-{n}", Player.X, Player.Y - 40, "#ff8a8a", 20);
		Events.Add(new RiftEvent(RiftEventKind.Hurt, Player.X, Player.Y));
	}

	private void Heal(double n)
	{
		if (n <= 0 || Energy >= MaxEnergy) return;
		Energy = Math.Min(MaxEnergy, Energy + n);
		Say($"+{Math.Round(n * 10) / 10}", Player.X + 20, Player.Y - 30, "#c8ffe8", 15);
	}

	private void SpawnScrap(double x, double y, int n)
	{
		for (int i = 0; i < n; i++)
		{
			double a = _rng.Next() * 6.283, v = 50 + _rng.Next() * 100;
			ScrapBits.Add(new RiftScrapBit { X = x, Y = y, Vx = Math.Cos(a) * v, Vy = Math.Sin(a) * v, Ph = _rng.Next() * 6 });
		}
	}

	// ---- adventure levels and the pick ------------------------------------------------------------------------

	private void GainXp(double xp)
	{
		Xp += xp;
		while (Xp >= _s.XpToNext(Level)) { Xp -= _s.XpToNext(Level); Level++; _levelsOwed++; Say($"LEVEL {Level}", Player.X, Player.Y - 60, "#ffd27a", 24); }
		if (_levelsOwed > 0 && Phase == RiftPhase.Wave && Pick == null) { _levelsOwed--; OpenPick(PickReason.LevelUp); }
	}

	/// <summary>
	/// Opens the pick (the rift holds still): up to three cards from the pool, one of them sometimes a temper (+1 level to
	/// a deck card), as the browser's wave pick; plus the scrap shop, open while the pick is.
	/// </summary>
	public void OpenPick(PickReason reason)
	{
		var pick = new RiftPick { Reason = reason };
		for (int i = 0; i < 3 && _pool.Count > 0; i++) { int j = _rng.Index(_pool.Count); pick.Offers.Add(new PickOffer { Card = _pool[j] }); _pool.RemoveAt(j); }
		var temperable = Deck.Where(c => c.Level < Tune("max_level")).ToList();
		if (temperable.Count > 0 && (pick.Offers.Count < 3 || _rng.Next() < Tune("pick_temper")))
		{
			if (pick.Offers.Count >= 3) { _pool.Add(pick.Offers[^1].Card!); pick.Offers.RemoveAt(pick.Offers.Count - 1); }
			pick.Offers.Add(new PickOffer { TemperId = temperable[_rng.Index(temperable.Count)].Id });
		}
		Pick = pick; Phase = RiftPhase.Pick;
	}

	/// <summary>Takes offer <paramref name="i"/> (or skips with -1); the others go back to the pool, and the rift goes on.</summary>
	public void TakeOffer(int i)
	{
		if (Pick is not { } pick) return;
		for (int j = 0; j < pick.Offers.Count; j++) if (j != i && pick.Offers[j].Card is { } back) _pool.Add(back);
		if (i >= 0 && i < pick.Offers.Count)
		{
			var o = pick.Offers[i];
			if (o.Card is { } c) GainCard(c);
			else if (CardById(o.TemperId) is { } t) { t.Level++; Say(_rules.CardName(t.Natures, t.Level).ToUpperInvariant(), Player.X, Player.Y - 50, _rules.CardColor(t.Natures), 18); }
		}
		Pick = null;
		if (pick.Reason == PickReason.WaveClear) { WaveN++; StartWave(); }
		else Phase = RiftPhase.Wave;
		if (_levelsOwed > 0 && Phase == RiftPhase.Wave) { _levelsOwed--; OpenPick(PickReason.LevelUp); }
	}

	/// <summary>Skips the pick's offers (the shop's purchases stand).</summary>
	public void SkipPick() => TakeOffer(-1);

	private void GainCard(RiftCard c)
	{
		var card = new RiftCard { Id = _nextId++, Natures = c.Natures, Level = c.Level };
		Deck.Add(card); _draw.Insert((int)(_rng.Next() * (_draw.Count + 1)), card.Id);
		Say(_rules.CardName(card.Natures, card.Level).ToUpperInvariant(), Player.X, Player.Y - 50, _rules.CardColor(card.Natures), 18);
	}

	/// <summary>What the next one of a boost costs.</summary>
	public int PriceOf(string id)
	{
		var b = Array.Find(_s.Boosts, x => x.Id == id) ?? throw new ArgumentException($"no boost {id}");
		return (int)Math.Round(b.BasePrice * (1 + _s.PriceStep * _bought.GetValueOrDefault(id)), MidpointRounding.AwayFromZero);
	}

	/// <summary>Buys a run-only boost with scrap while the pick is open. Returns whether it was bought.</summary>
	public bool Buy(string id)
	{
		if (Pick == null) return false;
		int price = PriceOf(id);
		if (Scrap < price) return false;
		Scrap -= price; _bought[id] = _bought.GetValueOrDefault(id) + 1;
		switch (id)
		{
			case "fire": _tr["fire_rate"] *= 1 + _s.FireRateBoost; break;
			case "damage": _tr["damage"] *= 1 + _s.DamageBoost; break;
			case "bounce": _tr["bounces"] += 1; break;
			case "magnet": _tr["magnet"] += _s.MagnetBoost; break;
			case "speed": _tr["speed"] *= 1 + _s.SpeedBoost; break;
			case "heal": Energy = Math.Min(MaxEnergy, Energy + _s.HealBoost); break;
			case "ward": Ward = (int)Math.Min(Tune("ward_max"), Ward + 1); break;
		}
		return true;
	}

	/// <summary>How many of a boost were bought this run.</summary>
	public int BoughtOf(string id) => _bought.GetValueOrDefault(id);

	/// <summary>What buying back the next card from the pool costs.</summary>
	public int RetrievePrice() => _s.RetrieveBase + _s.RetrieveStep * _retrieved;

	/// <summary>Buys back card <paramref name="poolIndex"/> of the pool into the deck with scrap, while the pick is open.</summary>
	public bool Retrieve(int poolIndex)
	{
		if (Pick == null || poolIndex < 0 || poolIndex >= _pool.Count) return false;
		int price = RetrievePrice();
		if (Scrap < price) return false;
		Scrap -= price; _retrieved++;
		var c = _pool[poolIndex]; _pool.RemoveAt(poolIndex); GainCard(c);
		return true;
	}

	/// <summary>Checks: fires the deck once at <paramref name="aim"/>, whatever the phase.</summary>
	public void FireForTest(double aim) { Player.Aim = aim; if (_draw.Count == 0 && _discard.Count == 0) RebuildPiles(); FireDeck(); }

	/// <summary>Checks: ends the current wave's timetable and empties the rift, so the wave clears.</summary>
	public void ClearWaveForTest() { _waveIdx = _wave.Count; Foes.Clear(); Matter.Clear(); }

	private void SpawnMotes(double x, double y, int n)
	{
		for (int i = 0; i < n; i++)
		{
			double a = _rng.Next() * 6.283, v = 60 + _rng.Next() * 120;
			Motes.Add(new RiftMote { X = x, Y = y, Vx = Math.Cos(a) * v, Vy = Math.Sin(a) * v, Ph = _rng.Next() * 6 });
		}
	}

	/// <summary>Deals damage to a Ybasog or a piece of matter; at 0 life it is killed.</summary>
	public void Damage(object t, double d, RiftBolt? src)
	{
		if (d <= 0) return;
		if (t is RiftFoe e) { if (e.Dead) return; e.Hp -= d; e.Flash = 0.08; if (e.Hp <= 0) Kill(e, src); }
		else if (t is RiftMatter m) { if (m.Dead) return; m.Hp -= d; m.Flash = 0.08; if (m.Hp <= 0) Break(m); }
	}

	private void Kill(RiftFoe e, RiftBolt? src)
	{
		if (e.Dead) return;
		e.Dead = true; Sparks(e.X, e.Y, "#d9b8ff", 12);
		SpawnMotes(e.X, e.Y, (int)Math.Round(e.Drop + O("killVoid")));
		Events.Add(new RiftEvent(RiftEventKind.Flash, e.X, e.Y, Size: e.R + 6));
		Events.Add(new RiftEvent(RiftEventKind.Kill, e.X, e.Y, e.K));
		Kills++; Track("kills");
		if (e.Fam == "pod") Track("pods");
		if (e.Fam == "feeder") Track("feeders");
		var p = Player;
		if (Math.Sqrt((e.X - p.X) * (e.X - p.X) + (e.Y - p.Y) * (e.Y - p.Y)) < 140) Track("closekills");
		if (p.Still) Track("stillkills");
		if (src != null && src.Bounced >= 2) Track("ricochets");
		_recentKills.Add(T); _recentKills.RemoveAll(x => T - x > 0.5);
		if (_recentKills.Count >= 3) { Track("multikill"); _recentKills.Clear(); }
		GainXp(e.Drop);
	}

	private void Break(RiftMatter m)
	{
		if (m.Dead) return;
		m.Dead = true; Sparks(m.X, m.Y, "#d9b8ff", 12);
		if (m.CarriesVoid) SpawnMotes(m.X, m.Y, (int)Math.Round(m.Drop));
		if (m.CarriesScrap) SpawnScrap(m.X, m.Y, m.Fam == "wreck" ? _s.ScrapPerWreck : m.Fam == "ruin" ? _s.ScrapPerRuin : _s.ScrapPerAsteroid);
		GainXp(_s.XpPerMatter);
		Events.Add(new RiftEvent(RiftEventKind.Flash, m.X, m.Y, Size: (m.Rect ? 20 : m.R) + 6));
		Events.Add(new RiftEvent(RiftEventKind.Kill, m.X, m.Y, m.K));
		if (m.Fam == "rock") Track("asteroids");
		if (m.Fam == "wreck") Track("debris");
		if (m.Fam == "ruin") Track("ruins");
	}

	// ---- collisions: a circle, and a box (debris) ---------------------------------------------------------

	private static (double nx, double ny, double push)? CircleNormal(double bx, double by, double br, double tx, double ty, double tr)
	{
		double dx = bx - tx, dy = by - ty, d = Math.Sqrt(dx * dx + dy * dy);
		if (d >= br + tr || d == 0) return null;
		return (dx / d, dy / d, br + tr - d);
	}

	private static (double nx, double ny, double push)? BoxNormal(double bx, double by, double r, RiftMatter t)
	{
		double c = Math.Cos(-t.A), s = Math.Sin(-t.A), dx = bx - t.X, dy = by - t.Y;
		double lx = dx * c - dy * s, ly = dx * s + dy * c, hw = t.W / 2, hh = t.H / 2;
		double cx = Math.Clamp(lx, -hw, hw), cy = Math.Clamp(ly, -hh, hh);
		double nx = lx - cx, ny = ly - cy, d = Math.Sqrt(nx * nx + ny * ny), push;
		if (d > r) return null;
		if (d == 0)
		{
			double px = hw - Math.Abs(lx), py = hh - Math.Abs(ly);
			if (px < py) { nx = Math.Sign(lx) == 0 ? 1 : Math.Sign(lx); ny = 0; push = px + r; } else { nx = 0; ny = Math.Sign(ly) == 0 ? 1 : Math.Sign(ly); push = py + r; }
		}
		else { nx /= d; ny /= d; push = r - d; }
		double C = Math.Cos(t.A), S = Math.Sin(t.A);
		return (nx * C - ny * S, nx * S + ny * C, push);
	}

	private static (double nx, double ny, double push)? HitNormal(double x, double y, double r, object t) => t switch
	{
		RiftMatter { Rect: true } m => BoxNormal(x, y, r, m),
		RiftMatter m => CircleNormal(x, y, r, m.X, m.Y, m.R),
		RiftFoe e => CircleNormal(x, y, r, e.X, e.Y, e.R),
		_ => null,
	};

	// ---- the step -------------------------------------------------------------------------------------------

	/// <summary>The direction to the nearest Ybasog (or, with none, the nearest matter), for auto-aim; null when the rift is empty.</summary>
	public double? AimAtNearest()
	{
		var p = Player; double best = double.MaxValue; (double x, double y)? at = null;
		foreach (var e in Foes) { if (e.Dead || e.Y < -e.R) continue; double d = (e.X - p.X) * (e.X - p.X) + (e.Y - p.Y) * (e.Y - p.Y); if (d < best) { best = d; at = (e.X, e.Y); } }
		if (at == null) foreach (var m in Matter) { if (m.Dead || m.Y < -m.R) continue; double d = (m.X - p.X) * (m.X - p.X) + (m.Y - p.Y) * (m.Y - p.Y); if (d < best) { best = d; at = (m.X, m.Y); } }
		return at is { } a ? Math.Atan2(a.y - p.Y, a.x - p.X) : null;
	}

	/// <summary>Advances the adventure by <paramref name="dt"/> seconds with this step's input.</summary>
	public void Step(double dt, RiftInput input)
	{
		if (Outcome != RiftOutcome.None) return;
		if (Phase == RiftPhase.Pick) return; // the pick holds the rift still
		if (Hitstop > 0) { Hitstop -= dt; return; }
		var p = Player;
		T += dt;
		double mx = input.MoveX, my = input.MoveY, ml = Math.Sqrt(mx * mx + my * my);
		if (ml > 1) { mx /= ml; my /= ml; }
		p.Still = mx == 0 && my == 0;
		double sp = Tune("player_speed") * Tr("speed");
		p.X += mx * sp * dt; p.Y += my * sp * dt;
		ClampPlayer();
		if (input.Aim is { } aim) p.Aim = aim;
		p.Iframe = Math.Max(0, p.Iframe - dt);
		if (Tr("regen") > 0) Energy = Math.Min(MaxEnergy, Energy + Tr("regen") * dt);
		FireCd -= dt; ReshuffleT -= dt;
		bool live = Phase is RiftPhase.Wave or RiftPhase.Clear;
		if (input.Fire && live && FireCd <= 0 && ReshuffleT <= 0) { FireDeck(); FireCd = Tune("fire_interval") / Tr("fire_rate"); }

		if (Phase == RiftPhase.Start && (_startT -= dt) <= 0) StartWave();
		if (Phase == RiftPhase.Wave)
		{
			_waveT += dt;
			while (_waveIdx < _wave.Count && _wave[_waveIdx].T <= _waveT) { var s = _wave[_waveIdx++]; Spawn(s.K, s.X); }
			bool final = Stage == 3 && WaveN == 3;
			if (_waveIdx >= _wave.Count && Foes.Count == 0 && Matter.Count == 0)
			{
				Phase = RiftPhase.Clear; _clearT = final ? 2.4 : 2;
				if (!_waveHurt) Track("cleanwave");
				if (!_waveLeak) Track("sealedwave");
				if (Energy <= 5) Track("lowwave");
				if (Tr("wave_heal") > 0) Heal(Tr("wave_heal"));
				bool stageEnd = WaveN == 3 && !final;
				if (stageEnd) Heal(Tune("stage_heal") + B("stage_heal"));
				Say(final ? "SILENCE" : stageEnd ? "STAGE CLEAR" : "CLEAR", RCX, 300, "#e9dcff", 30);
				Events.Add(new RiftEvent(RiftEventKind.Clear, RCX, 300));
			}
		}
		if (Phase == RiftPhase.Clear && (_clearT -= dt) <= 0 && ((Motes.Count == 0 && ScrapBits.Count == 0) || _clearT < -2))
		{
			Motes.Clear();
			Scrap += ScrapBits.Count; ScrapBits.Clear(); // anything still flying arrives
			if (Stage == 3 && WaveN == 3) { End(RiftOutcome.Won); return; }
			if (WaveN < 3) OpenPick(PickReason.WaveClear); // the pick after waves 1 and 2 (the next wave starts when it closes)
			else
			{
				if (!_stageHurt) Track("cleanstage");
				if (Stage >= _s.Stages) { End(RiftOutcome.StageCleared); return; }
				Stage++; WaveN = 1; _stageHurt = false; StartWave(); // the Rift Rest between stages is not migrated yet
			}
			return;
		}

		UpdateFoes(dt);
		UpdateMatter(dt);
		UpdateBolts(dt);
		UpdateGlobs(dt);
		UpdateMotes(dt);
		UpdateScrap(dt);
		Foes.RemoveAll(e => e.Dead); Matter.RemoveAll(m => m.Dead); Bolts.RemoveAll(b => b.Dead);
		if (Bolts.Count > 120) Bolts.RemoveRange(0, Bolts.Count - 120);
		Globs.RemoveAll(g => g.Dead); Motes.RemoveAll(m => m.Dead); ScrapBits.RemoveAll(b => b.Dead);
		Shake = Math.Max(0, Shake - dt * 30);
		if (Energy <= 0) End(RiftOutcome.Fallen);
	}

	private void ClampPlayer()
	{
		Player.X = Math.Clamp(Player.X, RL + PR, RR - PR);
		Player.Y = Math.Clamp(Player.Y, PTOP, H - PR - 6);
	}

	private double Dist(double ax, double ay, double bx, double by) => Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
	private double AimAt(RiftFoe e) => Math.Atan2(Player.Y - e.Y, Player.X - e.X);

	private void UpdateFoes(double dt)
	{
		var p = Player;
		double diveY = H * Tune("grazer_dive_line");
		for (int i = 0; i < Foes.Count; i++)
		{
			var e = Foes[i];
			e.Age += dt; e.Flash = Math.Max(0, e.Flash - dt); if (e.Dead) continue;
			double f = Tr("aura") > 0 && Dist(e.X, e.Y, p.X, p.Y) < 160 ? 1 - Tr("aura") : 1, tk = dt * f;
			e.T1 -= tk; e.T2 -= tk; e.T3 -= tk;
			var d = _d.YbasogStats[e.K];
			switch (e.Fam)
			{
				case "pod":
					e.X += e.Vx * tk; e.Y += e.Vy * tk;
					if (e.Age > Tune("pod_hatch")) { e.Dead = true; Events.Add(new RiftEvent(RiftEventKind.Hatch, e.X, e.Y, e.K)); SpawnY("grazer", e.X, e.Y); }
					break;
				case "grazer":
					if (!e.Dive) // falling, facing the way it falls
					{
						e.Y += e.Vy * tk; e.Heading = Math.Atan2(e.Vy, e.Vx);
						if (e.Y > diveY) { e.Dive = true; e.Turning = true; e.T3 = _s.GrazerTurnTime; e.Vx = 0; e.Vy = 0; }
					}
					else if (e.Turning) // at the line: it stops and turns to face the blob
					{
						double want = AimAt(e);
						e.Heading = TurnToward(e.Heading, want, _s.GrazerTurnRate * tk);
						if (e.T3 <= 0)
						{
							e.Turning = false; e.Heading = want; // then charges straight ahead
							e.Vx = Math.Cos(want) * d.Dive * SpScale(); e.Vy = Math.Sin(want) * d.Dive * SpScale();
						}
					}
					else { e.X += e.Vx * tk; e.Y += e.Vy * tk; }
					break;
				case "feeder":
					if (e.Y < e.StopY || e.Age > 16) e.Y += e.Vy * tk; else e.X += Math.Sin(e.Age * 0.8 + e.Ph) * 50 * tk;
					if (e.T1 <= 0 && e.Y > 20) { e.T1 = d.Spit * O("spit", 1); ShootGlob(e.X, e.Y + 18, AimAt(e)); }
					break;
				default: // a kind not migrated yet drifts down like a pod
					e.Y += e.Vy * tk;
					break;
			}
			if (e.X < RL + e.R) { e.X = RL + e.R; e.Vx = Math.Abs(e.Vx); }
			if (e.X > RR - e.R) { e.X = RR - e.R; e.Vx = -Math.Abs(e.Vx); }
			if (!e.Dead && Dist(e.X, e.Y, p.X, p.Y) < e.R + PR)
			{
				e.Dead = true; Sparks(e.X, e.Y, "#b5c43a", 10); // it bites, and is spent
				if (p.Iframe <= 0) { Hurt(Tune("hit_damage") + Tr("bite")); p.Iframe = Tune("iframes") + Tr("iframes"); }
			}
			if (!e.Dead && e.Y > H + e.R + 30) { e.Dead = true; e.Gone = true; _waveLeak = true; } // gone into the dark: no harm, but it escaped
		}
	}

	/// <summary>Turns angle <paramref name="from"/> toward <paramref name="to"/> by at most <paramref name="step"/> radians, the short way round.</summary>
	public static double TurnToward(double from, double to, double step)
	{
		double d = Math.IEEERemainder(to - from, Math.Tau);
		return Math.Abs(d) <= step ? to : from + Math.Sign(d) * step;
	}

	private void ShootGlob(double x, double y, double a)
	{
		double s = Tune("glob_speed") * SpScale();
		Globs.Add(new RiftGlob { X = x, Y = y, Vx = Math.Cos(a) * s, Vy = Math.Sin(a) * s });
	}

	private void UpdateMatter(double dt)
	{
		var p = Player;
		foreach (var m in Matter)
		{
			m.Flash = Math.Max(0, m.Flash - dt); if (m.Dead) continue;
			m.X += m.Vx * dt; m.Y += m.Vy * dt; m.Age += dt;
			if (!m.Rect) m.Rot += m.Spin * dt;
			double ext = m.Rect ? 20 : m.R;
			if (m.X < RL + ext) { m.X = RL + ext; m.Vx = Math.Abs(m.Vx); }
			if (m.X > RR - ext) { m.X = RR - ext; m.Vx = -Math.Abs(m.Vx); }
			if (HitNormal(p.X, p.Y, PR, m) is { } n)
			{
				p.X += n.nx * n.push; p.Y += n.ny * n.push + 1; ClampPlayer();
				if (p.Iframe <= 0) { Hurt(Tune("bump_damage")); p.Iframe = Tune("iframes") + Tr("iframes"); }
			}
			if (m.Y > H + m.R + 20) { m.Dead = true; m.Gone = true; if (m.Drop > 0) _waveLeak = true; }
		}
	}

	private void UpdateBolts(double dt)
	{
		foreach (var b in Bolts)
		{
			b.X += b.Vx * dt; b.Y += b.Vy * dt; b.Age += dt;
			void WallHit() { b.Bounces--; b.Bounced++; b.HitSet.Clear(); }
			if (b.X < RL + b.R) { b.X = RL + b.R; b.Vx = Math.Abs(b.Vx); WallHit(); }
			if (b.X > RR - b.R) { b.X = RR - b.R; b.Vx = -Math.Abs(b.Vx); WallHit(); }
			if (b.Y < -b.R - 4 || b.Y > H + b.R + 4) { b.Dead = true; continue; } // out of the view: gone
			for (int i = 0; i < Foes.Count; i++) if (!Foes[i].Dead) BoltTouches(b, Foes[i]); // two plain loops: nothing allocated per frame
			for (int i = 0; i < Matter.Count; i++) if (!Matter[i].Dead) BoltTouches(b, Matter[i]);
			if (b.Bounces < 0) { b.Dead = true; Sparks(b.X, b.Y, b.Color, 3); }
		}
	}

	private void BoltTouches(RiftBolt b, object t)
	{
		if (b.Dead || b.HitSet.Contains(t)) return;
		if (HitNormal(b.X, b.Y, b.R, t) is not { } n) return;
		double blow = Math.Atan2(b.Vy, b.Vx); // the way it was flying when it struck
		b.X += n.nx * n.push; b.Y += n.ny * n.push; // reflect off it
		double dot = b.Vx * n.nx + b.Vy * n.ny;
		if (dot < 0) { b.Vx -= 2 * dot * n.nx; b.Vy -= 2 * dot * n.ny; }
		b.Bounces--; b.Bounced++; b.HitSet.Clear(); b.HitSet.Add(t);
		Damage(t, b.Dmg, b); // natured Ybasogs (weak to the opposite nature) come in a later migration
		Sparks(b.X, b.Y, b.Color, 4);
		Events.Add(new RiftEvent(RiftEventKind.Hit, b.X, b.Y, Color: b.Color, Size: blow));
	}

	private void UpdateGlobs(double dt)
	{
		var p = Player;
		foreach (var g in Globs)
		{
			g.X += g.Vx * dt; g.Y += g.Vy * dt;
			if (g.Y > H + 20 || g.Y < -40 || g.X < RL - 10 || g.X > RR + 10) g.Dead = true;
			if (!g.Dead && Dist(g.X, g.Y, p.X, p.Y) < PR + 8)
			{
				g.Dead = true;
				if (Tr("glob_dodge") > 0 && _rng.Next() < Tr("glob_dodge")) Sparks(p.X, p.Y, "#ffffff", 4);
				else
				{
					Events.Add(new RiftEvent(RiftEventKind.Splash, g.X, g.Y, Size: Math.Atan2(g.Vy, g.Vx)));
					if (p.Iframe <= 0) { Hurt(Tune("glob_damage")); p.Iframe = Tune("iframes") + Tr("iframes"); }
				}
			}
		}
	}

	private void UpdateMotes(double dt)
	{
		var p = Player; double magnet = Tune("magnet_radius") + Tr("magnet");
		foreach (var m in Motes)
		{
			m.T += dt; double dx = p.X - m.X, dy = p.Y - m.Y, d = Math.Sqrt(dx * dx + dy * dy); if (d == 0) d = 1;
			if (d < magnet || Phase == RiftPhase.Clear || m.T > 6)
			{
				double s = Tune("mote_speed") * Math.Min(1, 0.4 + m.T), k = Math.Min(1, dt * 8);
				m.Vx += (dx / d * s - m.Vx) * k; m.Vy += (dy / d * s - m.Vy) * k;
			}
			else { m.Vx *= 1 - dt * 2.5; m.Vy = m.Vy * (1 - dt * 2.5) + 30 * dt; }
			m.X += m.Vx * dt; m.Y += m.Vy * dt;
			if (d < PR + 6)
			{
				m.Dead = true; int n = 1 + (Tr("mote_bonus") > 0 && _rng.Next() < Tr("mote_bonus") ? 1 : 0);
				VoidE += n; Track("motes", n);
				Events.Add(new RiftEvent(RiftEventKind.Mote, p.X, p.Y, Size: VoidE));
			}
		}
	}

	private void UpdateScrap(double dt)
	{
		var p = Player; double magnet = Tune("magnet_radius") + Tr("magnet");
		foreach (var m in ScrapBits)
		{
			m.T += dt; double dx = p.X - m.X, dy = p.Y - m.Y, d = Math.Sqrt(dx * dx + dy * dy); if (d == 0) d = 1;
			if (d < magnet || Phase == RiftPhase.Clear || m.T > 6)
			{
				double s = Tune("mote_speed") * Math.Min(1, 0.4 + m.T), k = Math.Min(1, dt * 8);
				m.Vx += (dx / d * s - m.Vx) * k; m.Vy += (dy / d * s - m.Vy) * k;
			}
			else { m.Vx *= 1 - dt * 2.5; m.Vy = m.Vy * (1 - dt * 2.5) + 30 * dt; }
			m.X += m.Vx * dt; m.Y += m.Vy * dt;
			if (d < PR + 6) { m.Dead = true; Scrap++; Events.Add(new RiftEvent(RiftEventKind.Mote, p.X, p.Y, "scrap", Size: Scrap)); }
		}
	}

	/// <summary>Leaves the rift early: it counts as a fall.</summary>
	public void Abandon() { if (Outcome == RiftOutcome.None) End(RiftOutcome.Fallen); }

	private void End(RiftOutcome outcome)
	{
		Outcome = outcome; Phase = RiftPhase.Over;
		double kept = outcome == RiftOutcome.Fallen ? Math.Min(1, Tune("loss_keep") + B("loss_keep")) : 1;
		Banked = _rules.EndAdventure(VoidE * VoidMul * kept, outcome == RiftOutcome.Won);
	}
}
