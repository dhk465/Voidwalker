using System.Collections.Generic;
using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// One of the hub's six stations. Touch-first: a tap selects it and lights its own pixels (the highlight shader);
/// a tap on the selected station opens it. On PC the pointer hovering it lights it too.
/// </summary>
public partial class Station : Node2D
{
	/// <summary>Raised when the selected station is tapped again: open its screen.</summary>
	[Signal] public delegate void OpenedEventHandler(string id);

	/// <summary>The station's id (deck, rite, gate, sigils, relics, journal).</summary>
	public string Id { get; private set; } = "";
	/// <summary>Tap radius round the station's centre, in pixels.</summary>
	public float Radius { get; private set; }
	/// <summary>Whether it is lit (selected or hovered).</summary>
	public bool Lit { get; private set; }

	private readonly List<ShaderMaterial> _materials = new();

	/// <summary>Sets up the station at its centre with its tap radius.</summary>
	public void Setup(string id, Vector2 centre, float radius)
	{
		Id = id; Name = id; Position = centre.Round(); Radius = radius;
	}

	/// <summary>Adds a sprite to the station; it is lit with the rest when the station is selected.</summary>
	public T AddArt<T>(T node, Shader highlight) where T : CanvasItem
	{
		var mat = new ShaderMaterial { Shader = highlight };
		node.Material = mat; _materials.Add(mat);
		AddChild(node);
		return node;
	}

	/// <summary>Whether a point (in the stage's space) is within the station's tap area.</summary>
	public bool Hits(Vector2 stagePoint) => stagePoint.DistanceTo(Position) <= Radius;

	/// <summary>Lights or dims the station's pixels.</summary>
	public void SetLit(bool lit)
	{
		if (lit == Lit) return;
		Lit = lit;
		foreach (var m in _materials) m.SetShaderParameter("selected", lit);
	}
}
