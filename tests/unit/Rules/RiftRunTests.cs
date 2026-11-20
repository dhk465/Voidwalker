using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Voidwalker.Core;
using Xunit;

namespace Voidwalker.Tests.Rules;

/// <summary>
/// The adventure's core loop (RiftRun) against the browser build's rules: the hand-made first minute, the deck as
/// ammunition, ricochets, kills and void motes, grazers and feeders, a fall, a cleared stage, and the banking after.
/// </summary>
public class RiftRunTests
{
	private static readonly GameData Data = GameData.Parse(Resource("game_data.json"));
	private const double Dt = 1 / 60.0;

	private static string Resource(string name)
	{
		using var s = typeof(RiftRunTests).Assembly.GetManifestResourceStream(name)!;
		return new StreamReader(s).ReadToEnd();
	}

	// The demo collection in a calm rift (or the omen given), and a run started on it.
	private static (AltarRules rules, RiftRun run) NewRun(string omen = "calm")
	{
		var meta = Meta.Demo(); meta.Omen = omen; meta.Boon = "calm";
		var rules = new AltarRules(Data, meta);
		return (rules, new RiftRun(Data, rules, new RiftSettings()));
	}

	private static void ToFirstWave(RiftRun run) { run.Step(1.3, default); Assert.Equal(RiftPhase.Wave, run.Phase); }
	private static double Gap(RiftRun run, double x, double y) => Math.Sqrt((run.Player.X - x) * (run.Player.X - x) + (run.Player.Y - y) * (run.Player.Y - y));

	[Fact]
	public void FirstWave_Calm_IsTheBrowsersHandMadeMinuteSpreadToTheWideRift()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		var w = run.WaveList;
		Assert.Equal(15, w.Count);
		Assert.Equal(new RiftSpawn(0.6, "asteroid", 640 + (560 - 640) * 1.8), w[0]);
		Assert.Contains(new RiftSpawn(27, "slab", 640), w);
		Assert.Equal(new RiftSpawn(35, "pod", 640 + (720 - 640) * 1.8), w[^1]);
	}

	[Fact]
	public void FirstWave_Rockfall_AddsTwoAsteroids()
	{
		var (_, run) = NewRun("rockfall");
		ToFirstWave(run);
		Assert.Equal(17, run.WaveList.Count);
		Assert.Contains(new RiftSpawn(6, "asteroid", 640 + (450 - 640) * 1.8), run.WaveList);
	}

	[Fact]
	public void Deck_OneVoidCard_FiresOnceThenReshuffles()
	{
		var (_, run) = NewRun();
		Assert.Single(run.Deck); // the demo owns no Inborn or Chosen sigil: one Void Bolt
		ToFirstWave(run);
		run.Step(0.01, new RiftInput(0, 0, -Math.PI / 2, true));
		Assert.Single(run.Bolts);
		run.Step(0.2, new RiftInput(0, 0, -Math.PI / 2, true)); // the draw pile is empty: the discard is shuffled back, no bolt
		Assert.Single(run.Bolts);
		Assert.Equal(Data.Tune("reshuffle_time"), run.ReshuffleT, 6);
	}

	[Fact]
	public void Bolt_HitsTheLeftWall_RicochetsAndLosesABounce()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		var b = new RiftBolt { X = RiftRun.RL + 10, Y = 400, Vx = -640, R = 6, Bounces = 4 };
		run.Bolts.Add(b);
		run.Step(0.02, default);
		Assert.True(b.Vx > 0);
		Assert.Equal(3, b.Bounces);
	}

	[Fact]
	public void Bolt_PastTheTop_IsGone()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		run.Bolts.Add(new RiftBolt { X = 640, Y = 2, Vy = -640, R = 6, Bounces = 4 });
		run.Step(0.05, default);
		Assert.Empty(run.Bolts);
	}

	[Fact]
	public void Pod_Killed_DropsMotesThatTheBlobCollects()
	{
		var (rules, run) = NewRun();
		ToFirstWave(run);
		double motesBefore = rules.Meta.Stats["motes"];
		run.Spawn("pod", run.Player.X, run.Player.Y - 200);
		run.Damage(run.Foes[^1], 100, null);
		Assert.Equal(2, run.Motes.Count); // a pod drops 2
		for (int i = 0; i < 7 * 60; i++) run.Step(Dt, default);
		Assert.Equal(2, run.VoidE);
		Assert.Equal(motesBefore + 2, rules.Meta.Stats["motes"]);
		Assert.Equal(1, run.Kills);
	}

	[Fact]
	public void Grazer_PastTheDiveLine_StopsAndTurnsToFaceTheBlobFirst()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		run.Spawn("grazer", run.Player.X + 200, RiftRun.H * Data.Tune("grazer_dive_line") - 0.1);
		var g = run.Foes[^1];
		Assert.Equal(Math.PI / 2, g.Heading, 6); // falling: it faces down the rift
		run.Step(Dt, default);
		Assert.True(g.Dive);
		Assert.True(g.Turning);
		Assert.Equal(0, Math.Sqrt(g.Vx * g.Vx + g.Vy * g.Vy), 6); // it holds still while it turns
		double toBlob = Math.Atan2(run.Player.Y - g.Y, run.Player.X - g.X);
		Assert.True(Math.Abs(Math.IEEERemainder(g.Heading - toBlob, Math.Tau)) > 0.1); // it has not finished turning yet
	}

	[Fact]
	public void Grazer_AfterTurning_ChargesForwardAlongItsHeadingAtTheBlob()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		run.Spawn("grazer", run.Player.X + 200, RiftRun.H * Data.Tune("grazer_dive_line") - 0.1);
		var g = run.Foes[^1];
		for (int i = 0; i < 40 && g.Turning | !g.Dive; i++) run.Step(Dt, default);
		Assert.False(g.Turning);
		double speed = Math.Sqrt(g.Vx * g.Vx + g.Vy * g.Vy);
		Assert.Equal(Data.YbasogStats["grazer"].Dive, speed, 3);
		Assert.Equal(Math.Cos(g.Heading), g.Vx / speed, 6); // moving the way it faces
		Assert.True(g.Vx * (run.Player.X - g.X) + g.Vy * (run.Player.Y - g.Y) > 0); // at the blob
	}

	[Fact]
	public void Asteroid_Falling_Spins()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		run.Spawn("asteroid", 640, 100);
		var m = run.Matter[^1];
		double before = m.Rot;
		run.Step(0.5, default);
		Assert.NotEqual(0, m.Spin);
		Assert.Equal(before + m.Spin * 0.5, m.Rot, 6);
	}

	[Fact]
	public void Feeder_Hovering_SpitsAGlobAtTheBlob()
	{
		var (_, run) = NewRun();
		ToFirstWave(run);
		run.Spawn("feeder", 640, 300);
		for (int i = 0; i < 3 * 60 && run.Globs.Count == 0; i++) run.Step(Dt, default);
		var glob = Assert.Single(run.Globs);
		Assert.True(glob.Vy > 0); // down toward the blob at the bottom
	}

	[Fact]
	public void Fall_EnergyToZero_BanksTheLossShareAndCountsARun()
	{
		var (rules, run) = NewRun();
		ToFirstWave(run);
		int bank = rules.Meta.Bank, runs = rules.Meta.Runs;
		run.VoidE = 10; run.Energy = 1;
		run.Spawn("grazer", run.Player.X, run.Player.Y); // it bites
		run.Step(Dt, default);
		Assert.Equal(RiftOutcome.Fallen, run.Outcome);
		int expected = (int)Math.Round(10 * run.VoidMul * Data.Tune("loss_keep"), MidpointRounding.AwayFromZero);
		Assert.Equal(expected, run.Banked);
		Assert.Equal(bank + expected, rules.Meta.Bank);
		Assert.Equal(runs + 1, rules.Meta.Runs);
	}

	[Fact]
	public void StageOne_PlayedWithAutoAim_EndsStageClearedAndBanksEverything()
	{
		var (rules, run) = NewRun();
		int wins = rules.Meta.Wins, runs = rules.Meta.Runs;
		run.Energy = run.MaxEnergy = 1e6; // the test plays to the end, not to win
		for (int i = 0; i < 900 * 60 && run.Outcome == RiftOutcome.None; i++)
		{
			if (run.Phase == RiftPhase.Pick) { run.SkipPick(); continue; } // after a wave or a level-up: skip, as a player may
			run.Step(Dt, new RiftInput(0, 0, run.AimAtNearest(), true));
		}
		Assert.Equal(RiftOutcome.StageCleared, run.Outcome);
		Assert.Equal(3, run.WaveN);
		Assert.True(run.Kills > 10);
		Assert.Equal((int)Math.Round(run.VoidE * run.VoidMul, MidpointRounding.AwayFromZero), run.Banked);
		Assert.Equal(wins, rules.Meta.Wins); // the way on is not ported: no win
		Assert.Equal(runs + 1, rules.Meta.Runs);
		Assert.True(rules.Meta.Bestiary.TryGetValue("feeder", out int met) && met > 0);
	}

	[Fact]
	public void SameSaveSameInputs_PlaysTheSameRun()
	{
		var (_, a) = NewRun(); var (_, b) = NewRun();
		for (int i = 0; i < 40 * 60; i++)
		{
			if (a.Phase == RiftPhase.Pick) a.SkipPick();
			if (b.Phase == RiftPhase.Pick) b.SkipPick();
			a.Step(Dt, new RiftInput(Math.Sin(i * 0.01), 0, a.AimAtNearest(), true));
			b.Step(Dt, new RiftInput(Math.Sin(i * 0.01), 0, b.AimAtNearest(), true));
		}
		Assert.Equal((a.Kills, a.VoidE, a.Energy, a.Player.X), (b.Kills, b.VoidE, b.Energy, b.Player.X));
	}

	[Fact]
	public void Track_ReachingADiscoveryGoal_DiscoversTheNature()
	{
		var rules = new AltarRules(Data, Meta.Demo());
		var goal = Data.Discover["pride"];
		int bank = rules.Meta.Bank;
		var found = rules.Track(goal.Stat, goal.N);
		Assert.Contains("pride", found);
		Assert.Contains("pride", rules.Meta.Discovered);
		Assert.Equal((int)Data.Tune("discovery_copies"), rules.Meta.Cards["pride|1"]);
		Assert.Equal(bank + found.Count * (int)Data.Tune("discovery_bonus"), rules.Meta.Bank);
	}

	[Fact]
	public void EndAdventure_Win_AddsTheBonusAndOpensTheNextDepth()
	{
		var rules = new AltarRules(Data, Meta.Demo());
		int bank = rules.Meta.Bank, wins = rules.Meta.Wins;
		int banked = rules.EndAdventure(50, won: true);
		Assert.Equal(50 + (int)Data.Tune("win_bonus"), banked);
		Assert.Equal(bank + banked, rules.Meta.Bank);
		Assert.Equal(wins + 1, rules.Meta.Wins);
		Assert.Equal(Math.Max(rules.Meta.MaxDepth, 1), rules.Meta.MaxDepth);
		Assert.NotNull(rules.Meta.Rifts);
	}

	[Fact]
	public void BeginAdventure_FullPool_Enters()
	{
		var rules = new AltarRules(Data, Meta.Demo());
		Assert.Equal("", rules.BeginAdventure());
	}
}
