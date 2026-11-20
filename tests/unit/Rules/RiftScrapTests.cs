using System;
using System.IO;
using System.Linq;
using Voidwalker.Core;
using Xunit;

namespace Voidwalker.Tests.Rules;

/// <summary>
/// Scrap (the adventure-only currency from asteroids, debris and ruins) and the adventure's own levels: a level-up, like a
/// cleared wave, opens the pick of a card and the scrap shop (run-only boosts, or a card bought back from the pool).
/// </summary>
public class RiftScrapTests
{
	private static readonly GameData Data = GameData.Parse(Resource("game_data.json"));
	private const double Dt = 1 / 60.0;

	private static string Resource(string name)
	{
		using var s = typeof(RiftScrapTests).Assembly.GetManifestResourceStream(name)!;
		return new StreamReader(s).ReadToEnd();
	}

	private static (AltarRules rules, RiftRun run) NewRun(RiftSettings? settings = null)
	{
		var meta = Meta.Demo(); meta.Omen = "calm"; meta.Boon = "calm";
		var rules = new AltarRules(Data, meta);
		var run = new RiftRun(Data, rules, settings ?? new RiftSettings());
		run.Step(1.3, default); // into wave 1
		return (rules, run);
	}

	private static void Collect(RiftRun run, double seconds = 7)
	{
		for (int i = 0; i < seconds * 60 && run.Phase == RiftPhase.Wave; i++) run.Step(Dt, default);
	}

	[Fact]
	public void Asteroids_CarryVoidScrapOrBoth_InTheConfiguredShares()
	{
		var (_, run) = NewRun();
		int v = 0, s = 0, both = 0, n = 600;
		for (int i = 0; i < n; i++)
		{
			run.Spawn("asteroid", 640, -400);
			var m = run.Matter[^1];
			Assert.True(m.CarriesVoid || m.CarriesScrap); // never empty-handed
			if (m.CarriesVoid && m.CarriesScrap) both++; else if (m.CarriesVoid) v++; else s++;
		}
		var cfg = new RiftSettings();
		Assert.InRange(v / (double)n, cfg.AsteroidVoidOnly - 0.06, cfg.AsteroidVoidOnly + 0.06);
		Assert.InRange(both / (double)n, cfg.AsteroidBoth - 0.06, cfg.AsteroidBoth + 0.06);
	}

	[Fact]
	public void ScrapAsteroid_Broken_DropsScrapThatTheBlobCollects()
	{
		var (rules, run) = NewRun(new RiftSettings { AsteroidVoidOnly = 0, AsteroidBoth = 0 }); // every asteroid: scrap only
		int bankBefore = rules.Meta.Bank;
		run.Spawn("asteroid", run.Player.X, run.Player.Y - 200);
		run.Damage(run.Matter[^1], 100, null);
		Assert.Empty(run.Motes); // no void energy in it
		Assert.Equal(new RiftSettings().ScrapPerAsteroid, run.ScrapBits.Count);
		Collect(run);
		Assert.Equal(new RiftSettings().ScrapPerAsteroid, run.Scrap);
		Assert.Equal(bankBefore, rules.Meta.Bank); // scrap never reaches the save
	}

	[Fact]
	public void DebrisSlab_Broken_DropsScrapButNoVoid()
	{
		var (_, run) = NewRun();
		run.Spawn("slab", run.Player.X, run.Player.Y - 200);
		run.Damage(run.Matter[^1], 100, null);
		Assert.Empty(run.Motes);
		Assert.Equal(new RiftSettings().ScrapPerWreck, run.ScrapBits.Count);
	}

	[Fact]
	public void Kills_EarnExperience_AndALevelUpOpensThePick()
	{
		var (_, run) = NewRun();
		var cfg = new RiftSettings();
		Assert.Equal(1, run.Level);
		while (run.Phase == RiftPhase.Wave)
		{
			run.Spawn("feeder", 640, 200);
			run.Damage(run.Foes[^1], 100, null);
			run.Step(Dt, default);
		}
		Assert.Equal(RiftPhase.Pick, run.Phase);
		Assert.Equal(2, run.Level);
		Assert.Equal(PickReason.LevelUp, run.Pick!.Reason);
		Assert.True(run.Xp < cfg.XpToNext(2));
	}

	[Fact]
	public void Pick_IsOpen_TheRiftHoldsStill()
	{
		var (_, run) = NewRun();
		run.OpenPick(PickReason.LevelUp);
		run.Spawn("pod", 640, 100);
		double y = run.Foes[^1].Y;
		run.Step(0.5, default);
		Assert.Equal(y, run.Foes[^1].Y);
	}

	[Fact]
	public void WaveOneCleared_OpensThePick_WithCardsFromThePool()
	{
		var (rules, run) = NewRun();
		run.ClearWaveForTest();
		for (int i = 0; i < 6 * 60 && run.Phase != RiftPhase.Pick; i++) run.Step(Dt, default);
		Assert.Equal(RiftPhase.Pick, run.Phase);
		Assert.Equal(PickReason.WaveClear, run.Pick!.Reason);
		Assert.InRange(run.Pick.Offers.Count, 1, 3);
		int poolBefore = run.PoolLeft.Count, deckBefore = run.Deck.Count;
		var cardOffer = run.Pick.Offers.FindIndex(o => o.Card != null);
		run.TakeOffer(cardOffer);
		Assert.Equal(deckBefore + 1, run.Deck.Count);
		Assert.Equal(RiftPhase.Wave, run.Phase);
		Assert.Equal(2, run.WaveN);
		Assert.True(run.PoolLeft.Count < poolBefore + 3); // the untaken offers went back, the taken one did not
	}

	[Fact]
	public void Shop_BuyBoost_SpendsScrapAppliesItAndRaisesThePrice()
	{
		var (_, run) = NewRun();
		run.OpenPick(PickReason.LevelUp);
		run.Scrap = 100;
		int price = run.PriceOf("bounce");
		Assert.True(run.Buy("bounce"));
		Assert.Equal(100 - price, run.Scrap);
		Assert.True(run.PriceOf("bounce") > price);
		run.FireForTest(-Math.PI / 2);
		Assert.Equal((int)(Data.Tune("bolt_bounces") + 1), run.Bolts[^1].Bounces); // +1 ricochet this run
	}

	[Fact]
	public void Shop_NotEnoughScrap_BuysNothing()
	{
		var (_, run) = NewRun();
		run.OpenPick(PickReason.LevelUp);
		run.Scrap = 0;
		Assert.False(run.Buy("damage"));
		Assert.Equal(0, run.Scrap);
	}

	[Fact]
	public void Shop_RetrieveACardFromThePool_ForScrap()
	{
		var (_, run) = NewRun();
		run.OpenPick(PickReason.LevelUp);
		run.Scrap = 100;
		int price = run.RetrievePrice(), poolBefore = run.PoolLeft.Count, deckBefore = run.Deck.Count;
		Assert.True(run.Retrieve(0));
		Assert.Equal(100 - price, run.Scrap);
		Assert.Equal(poolBefore - 1, run.PoolLeft.Count);
		Assert.Equal(deckBefore + 1, run.Deck.Count);
		Assert.True(run.RetrievePrice() > price);
	}

	[Fact]
	public void Shop_HealBoost_RestoresEnergyUpToTheMost()
	{
		var (_, run) = NewRun();
		run.OpenPick(PickReason.LevelUp);
		run.Scrap = 100; run.Energy = run.MaxEnergy - 1;
		Assert.True(run.Buy("heal"));
		Assert.Equal(run.MaxEnergy, run.Energy);
	}

	[Fact]
	public void Run_Ending_LeftoverScrapIsLost()
	{
		var (rules, run) = NewRun();
		run.Scrap = 40; run.VoidE = 0;
		int bank = rules.Meta.Bank;
		run.Abandon();
		Assert.Equal(bank, rules.Meta.Bank);
		Assert.False(rules.Meta.Extra?.ContainsKey("scrap") ?? false);
	}
}
