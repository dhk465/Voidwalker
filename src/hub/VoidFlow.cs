using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// One void flame riding the tide round the altar: a void ball on its own path, shedding a trail of puffs that stay
/// in the world where it was, rising and cooling from lavender to deep violet. Nothing turns: the trail follows the
/// real path because it is left behind by it. Puffs step through exact palette colours (a constant colour ramp)
/// and shrink frame by frame (a 4-frame strip), so every pixel stays whole.
/// </summary>
public partial class VoidFlow : Node2D
{
	private static readonly Color[] Ramp =
	{
		new("#ffffff"), new("#efe6ff"), new("#d8c8ff"), new("#b9a6ff"), new("#7b5bdf"), new("#4a3a7a"), new("#2a1636"),
	};

	private HubConfig _cfg = new();
	private int _index, _count;
	private double _t;
	private CpuParticles2D _trail = null!;

	/// <summary>Builds flame <paramref name="index"/> of <paramref name="count"/> with the lobby's art.</summary>
	public void Setup(LobbyArt art, HubConfig cfg, int index, int count)
	{
		_cfg = cfg; _index = index; _count = count; _t = index * 7.3;
		// a constant (stepped) ramp: each puff holds one exact colour at a time as it cools
		var offs = new float[Ramp.Length];
		for (int i = 0; i < Ramp.Length; i++) offs[i] = (float)i / Ramp.Length;
		var gradient = new Gradient { InterpolationMode = Gradient.InterpolationModeEnum.Constant, Offsets = offs, Colors = Ramp };
		_trail = new CpuParticles2D
		{
			Texture = art.Texture("puff-strip"),
			Amount = cfg.TrailAmount,
			Lifetime = cfg.TrailLifetime,
			LocalCoords = false, // the trail stays in the world where the flame shed it
			Direction = Vector2.Up, Spread = 60f,
			InitialVelocityMin = 2f, InitialVelocityMax = 8f,
			Gravity = new Vector2(0, -cfg.TrailLift), DampingMin = 1f, DampingMax = 3f,
			ColorRamp = gradient,
			EmissionShape = CpuParticles2D.EmissionShapeEnum.Sphere, EmissionSphereRadius = 2f,
			Material = new CanvasItemMaterial { ParticlesAnimation = true, ParticlesAnimHFrames = 4, ParticlesAnimVFrames = 1, ParticlesAnimLoop = false },
			AnimSpeedMin = 1f, AnimSpeedMax = 1f, // one pass through the strip over a puff's life: it shrinks to a pixel
		};
		AddChild(_trail);
		AddChild(art.Make("void-ball", Vector2.Zero));
	}

	/// <summary>Where flame i sits on the tide at time t (the tide quickens and slows; the flame bobs).</summary>
	public static Vector2 TideAt(HubConfig c, int i, int n, double t)
	{
		double k = i / (double)n * Mathf.Tau, a = t * c.FlowSpeed + k + Mathf.Sin(t * 0.21 + k) * c.FlowSurge;
		return new Vector2(c.FlowCentre.X + Mathf.Cos((float)a) * c.FlowRadius.X,
			c.FlowCentre.Y + Mathf.Sin((float)a) * c.FlowRadius.Y + Mathf.Sin((float)(t * 0.9 + i * 2)) * c.FlowBob);
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_t += delta;
		Position = TideAt(_cfg, _index, _count, _t).Round();
	}
}
