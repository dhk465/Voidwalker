using System.IO;
using System.Linq;
using System.Text.Json;
using Voidwalker.Core;
using Voidwalker.Hub;
using Xunit;

namespace Voidwalker.Tests.Rules;

/// <summary>
/// The hub's rules against the browser build. Golden values in oracle.json come from the browser's own code
/// (oracle.mjs runs index.html's lines in Node), so a mismatch means the port drifted.
/// </summary>
public class AltarRulesTests
{
	private static readonly GameData Data = GameData.Parse(Resource("game_data.json"));
	private static readonly JsonElement Oracle = JsonDocument.Parse(Resource("oracle.json")).RootElement;

	private static string Resource(string name)
	{
		using var s = typeof(AltarRulesTests).Assembly.GetManifestResourceStream(name)!;
		return new StreamReader(s).ReadToEnd();
	}

	private static AltarRules Demo() => new(Data, Meta.Demo());

	[Fact]
	public void Rng_BrowserSeed_GivesTheBrowsersFirstThreeNumbers()
	{
		var rng = new Rng(1000003);
		var expected = Oracle.GetProperty("firstRng").EnumerateArray().Select(e => e.GetDouble()).ToArray();
		Assert.Equal(expected, new[] { rng.Next(), rng.Next(), rng.Next() });
	}

	[Fact]
	public void RiteDraw_FocusedTenOverTheBank_SpendsNothing()
	{
		var rules = Demo();
		var drawn = rules.RiteDraw(10, out string message);
		Assert.Empty(drawn);
		Assert.Equal("NEEDS 370", message);
		Assert.Equal(Oracle.GetProperty("focusedBankAfter").GetInt32(), rules.Meta.Bank);
	}

	[Fact]
	public void RiteDraw_TenUnfocusedFromTheDemo_MatchesTheBrowserCardForCard()
	{
		var rules = Demo();
		rules.Meta.Focus = null;
		var drawn = rules.RiteDraw(10, out _);
		var expected = Oracle.GetProperty("draw10").EnumerateArray()
			.Select(e => $"{e[0].GetString()}|{e[1].GetInt32()}|{e[2].GetString()}|{e[3].GetBoolean()}").ToArray();
		Assert.Equal(expected, drawn.Select(r => $"{string.Join('+', r.Natures)}|{r.Level}|{r.Rarity}|{r.IsNew}").ToArray());
		Assert.Equal(Oracle.GetProperty("bank").GetInt32(), rules.Meta.Bank);
		Assert.Equal(Oracle.GetProperty("pity").GetInt32(), rules.Meta.Pity);
		Assert.Equal(Oracle.GetProperty("draws").GetInt32(), rules.Meta.Draws);
	}

	[Fact]
	public void RollRifts_TheDemo_MatchesTheBrowsersThreeRifts()
	{
		var rifts = Demo().RollRifts();
		var expected = Oracle.GetProperty("rifts").EnumerateArray()
			.Select(e => new Rift(e.GetProperty("name").GetString()!, e.GetProperty("omen").GetString()!, e.GetProperty("prevail").GetString()!, e.GetProperty("boon").GetString()!)).ToList();
		Assert.Equal(expected, rifts);
	}

	[Fact]
	public void AddToPool_AStackWithCopiesLeft_AddsOne()
	{
		var rules = Demo();
		Assert.Equal("", rules.AddToPool("charity|1"));
		Assert.Equal(1, rules.Meta.Loadout["charity|1"]);
		Assert.Equal(10, rules.DeckCount());
	}

	[Fact]
	public void AddToPool_EveryCopyAlreadyIn_SaysAllInThePool()
	{
		Assert.Equal("ALL IN THE POOL", Demo().AddToPool("wrath|2"));
	}

	[Fact]
	public void AddToPool_AtTheLimit_SaysThePoolIsFull()
	{
		var rules = Demo();
		foreach (var sk in new[] { "charity|1", "charity|1", "temperance|1" }) rules.AddToPool(sk);
		Assert.Equal(12, rules.DeckCount());
		Assert.Equal("THE POOL IS FULL", rules.AddToPool("temperance|1"));
	}

	[Fact]
	public void RemoveFromPool_TheLastCopy_RemovesTheStack()
	{
		var rules = Demo();
		rules.RemoveFromPool("wrath|2");
		Assert.False(rules.Meta.Loadout.ContainsKey("wrath|2"));
	}

	[Fact]
	public void Stacks_MixedKeys_SortLikeTheBrowser()
	{
		var order = AltarRules.Stacks(Meta.Demo().Cards).Select(s => s.Sk).ToArray();
		Assert.Equal(new[] { "|1", "charity|1", "diligence|1", "envy|1", "greed|1", "sloth|1", "temperance|1", "wrath|1", "wrath|2", "charity+diligence|1", "envy+wrath|1", "greed+wrath|1" }, order);
	}

	[Fact]
	public void BuySigil_EnoughVoid_SpendsTheNextLevelsCost()
	{
		var rules = Demo();
		var (message, bought) = rules.BuySigil("vessel");
		Assert.True(bought);
		Assert.Equal("VESSEL", message);
		Assert.Equal(340 - 140, rules.Meta.Bank);
		Assert.Equal(2, rules.Meta.Upg["vessel"]);
	}

	[Fact]
	public void BuySigil_TooLittleVoid_SaysWhatItNeeds()
	{
		var rules = Demo();
		Assert.Equal(("NEEDS 400", false), rules.BuySigil("shrine"));
		Assert.Equal(340, rules.Meta.Bank);
	}

	[Fact]
	public void BuySigil_EveryLevelBought_SaysComplete()
	{
		var rules = Demo();
		rules.Meta.Upg["inborn"] = 1;
		Assert.Equal(("COMPLETE", false), rules.BuySigil("inborn"));
	}

	[Fact]
	public void ToggleRelic_SlotsFull_DropsTheOldest()
	{
		var rules = Demo(); // four relics found open a second slot
		Assert.Equal(2, rules.RelicSlots());
		rules.ToggleRelic("bell");
		rules.ToggleRelic("lens");
		Assert.Equal(new[] { "bell", "lens" }, rules.Meta.Equipped);
	}

	[Fact]
	public void ToggleRelic_NotOwned_ChangesNothing()
	{
		var rules = Demo();
		rules.ToggleRelic("pearl");
		Assert.Equal(new[] { "crown" }, rules.Meta.Equipped);
	}

	[Fact]
	public void ClaimQuest_AVoidRewardReached_PaysItOnce()
	{
		var rules = Demo();
		Assert.True(rules.ClaimQuest("natures"));
		Assert.Equal(540, rules.Meta.Bank);
		Assert.False(rules.ClaimQuest("natures"));
	}

	[Fact]
	public void ClaimQuest_ARelicReward_GivesTheBrowsersPick()
	{
		var rules = Demo(); // relics left in order, index (runs 4 * 7 + quest 6) % 12 = 10: the Sealed Psalm
		Assert.True(rules.ClaimQuest("relics"));
		Assert.Equal(1, rules.Meta.Relics["psalm"]);
	}

	[Fact]
	public void ClaimQuest_NotReached_ClaimsNothing()
	{
		var rules = Demo();
		Assert.False(rules.ClaimQuest("mother"));
		Assert.Equal(340, rules.Meta.Bank);
	}

	[Fact]
	public void StepDepth_PastTheDeepest_StaysAtTheDeepest()
	{
		var rules = Demo();
		rules.StepDepth(1); rules.StepDepth(1);
		Assert.Equal(1, rules.Meta.Depth);
		Assert.Equal((25, 20), rules.DepthBonus());
	}

	[Fact]
	public void BeginAdventure_PoolTooSmall_SaysHowManyItNeeds()
	{
		var rules = Demo();
		rules.Meta.Loadout.Clear();
		Assert.Equal("THE POOL NEEDS 6", rules.BeginAdventure());
	}

	[Fact]
	public void DominantNature_TheDemoPool_IsWrath()
	{
		Assert.Equal("wrath", Demo().DominantNature());
	}

	[Fact]
	public void Meta_RoundTrip_KeepsFieldsThisPortDoesNotRead()
	{
		var meta = Meta.FromJson("{\"v\":1,\"bank\":5,\"tempered\":{\"a\":1}}");
		var back = Meta.FromJson(meta.ToJson());
		Assert.Equal(5, back.Bank);
		Assert.Contains("\"tempered\"", back.ToJson());
	}

	[Theory]
	[InlineData("{\"v\":2,\"bank\":999,\"futureInventory\":[1]}")]
	[InlineData("not json")]
	[InlineData("{}")]
	[InlineData("{\"v\":1,\"loadout\":null}")]
	public void UnreadableSaveSurvivesLaunchAndStationChangesUntilExplicitReset(string original)
	{
		var dir = Directory.CreateTempSubdirectory();
		try
		{
			string path = Path.Combine(dir.FullName, "save.json");
			File.WriteAllText(path, original);
			var bytes = File.ReadAllBytes(path);
			var session = Session.Load(Data, path);
			Assert.Equal(bytes, File.ReadAllBytes(path));
			session.Rules.RiteDraw(1, out _);
			Assert.False(session.Save());
			Assert.NotEmpty(session.SaveNotice);
			Assert.Equal(bytes, File.ReadAllBytes(path));
			Assert.True(session.ForgetAll());
			Assert.Equal(Meta.Demo().Bank, Session.Load(Data, path).Rules.Meta.Bank);
			Assert.True(session.Save());
		}
		finally { dir.Delete(true); }
	}

	[Fact]
	public void ResetRestoresTheFirstLaunchCollectionAndItRemainsPlayableAfterRestart()
	{
		var dir = Directory.CreateTempSubdirectory();
		try
		{
			string path = Path.Combine(dir.FullName, "save.json");
			var session = Session.Load(Data, path);
			string starter = File.ReadAllText(path);
			session.Rules.RiteDraw(1, out _);
			Assert.True(session.Save());
			Assert.NotEqual(starter, File.ReadAllText(path));
			Assert.True(session.ForgetAll());
			Assert.Equal(starter, File.ReadAllText(path));
			var rules = Session.Load(Data, path).Rules;
			Assert.True(rules.DeckCount() >= rules.DeckMin());
			Assert.Single(rules.RiteDraw(1, out _));
		}
		finally { dir.Delete(true); }
	}

	[Fact]
	public void FailedResetKeepsTheActiveCollectionAndReportsTheWriteFailure()
	{
		var dir = Directory.CreateTempSubdirectory();
		try
		{
			string path = Path.Combine(dir.FullName, "save.json");
			var session = Session.Load(Data, path);
			session.Rules.RiteDraw(1, out _);
			string before = session.Rules.Meta.ToJson();
			// Occupy the target with a directory so the atomic replacement fails on all platforms.
			File.Move(path, path + ".original");
			Directory.CreateDirectory(path);
			Assert.False(session.ForgetAll());
			Assert.Equal(before, session.Rules.Meta.ToJson());
			Assert.NotEmpty(session.SaveNotice);
			Assert.Empty(Directory.GetFiles(dir.FullName, "*.tmp"));
		}
		finally { dir.Delete(true); }
	}
}
