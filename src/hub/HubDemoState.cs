using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// What the hub shows until the save system is ported: a collection part-way through, like the browser build's
/// <c>?demo=1</c>. Values live in <c>res://src/data/hub_demo_state.tres</c>.
/// </summary>
[GlobalClass]
public partial class HubDemoState : Resource
{
	/// <summary>Void energy banked.</summary>
	[Export] public int Bank { get; set; } = 340;
	/// <summary>Natures discovered (of 14).</summary>
	[Export] public int Natures { get; set; } = 7;
	/// <summary>Fusions recorded (of 84).</summary>
	[Export] public int Fusions { get; set; } = 4;
	/// <summary>Ybasog kinds met (of 58).</summary>
	[Export] public int Ybasogs { get; set; } = 9;
	/// <summary>Relics found (of 16).</summary>
	[Export] public int Relics { get; set; } = 4;
	/// <summary>Cards in the pool (the Deck station fans up to five of them).</summary>
	[Export] public int PoolCards { get; set; } = 5;
	/// <summary>Sigils bought: that many constellation stars are lit.</summary>
	[Export] public int SigilsBought { get; set; } = 2;
	/// <summary>Relic slots open (of 3 pedestals).</summary>
	[Export] public int RelicSlots { get; set; } = 2;
	/// <summary>The chosen rift's prevailing nature colour, tinting the Gate's swirl.</summary>
	[Export] public Color RiftTint { get; set; } = new("#b9a6ff");
}
