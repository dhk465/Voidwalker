using Godot;
using Voidwalker.Core;

namespace Voidwalker.Hub;

/// <summary>
/// One void ball: it wells out of the Rite's crack, wanders the lobby on its <see cref="FlowPath"/> and is sucked
/// into the Gate's rift, shedding a trail of puffs that stay in the world where it was, rising and cooling from
/// lavender to deep violet. Puffs step through exact palette colours (a constant colour ramp) and shrink frame by
/// frame (a 4-frame strip), so every pixel stays whole. A back-lane trip draws behind the islands and what stands on
/// them, a front-lane trip in front, the whole way (the rift itself sits behind both lanes, so either is seen going
/// in). Pooled: <see cref="Launch"/> reuses it; a tap sends it off again from where it is, or bursts it (<see cref="Vanish"/>).
/// <example><code>var ball = new VoidFlow(); ball.Setup(art, cfg); stage.AddChild(ball);
/// ball.Launch(FlowPath.Roll(rng, settings, settings.Source), 0f);
/// if (ball.Idle) { /* free to launch again */ }</code></example>
/// </summary>
public partial class VoidFlow : Node2D
{
	private static readonly Color[] Ramp =
	{
		new("#ffffff"), new("#efe6ff"), new("#d8c8ff"), new("#b9a6ff"), new("#7b5bdf"), new("#4a3a7a"), new("#2a1636"),
	};

	/// <summary>Draw depth behind the lobby's objects (which sit at 0).</summary>
	public const int BehindZ = -1;
	/// <summary>Draw depth in front of them.</summary>
	public const int FrontZ = 1;

	private CpuParticles2D _trail = null!;
	private Sprite2D _ball = null!;
	private FlowPath _path = null!;
	private float _t, _fade, _trailLife;
	private bool _flying;

	/// <summary>True when the ball is free: not flying and its trail has faded.</summary>
	public bool Idle => !_flying && _fade <= 0f;
	/// <summary>True while the ball is on its way (not yet gone into the rift).</summary>
	public bool Flying => _flying;
	/// <summary>Where the ball is now, in hub pixels (exact, before rounding to a pixel).</summary>
	public System.Numerics.Vector2 Where => _path.At(_t);
	/// <summary>The lane of its trip: behind the lobby's objects or in front of them.</summary>
	public FlowLane Lane => _path.Lane;

	/// <summary>Ends the trip where the ball is (it burst): the ball goes, its trail fades in place.</summary>
	public void Vanish()
	{
		if (!_flying) return;
		_flying = false; _ball.Visible = false; _trail.Emitting = false; _fade = _trailLife;
	}

	/// <summary>Builds the ball and its trail with the lobby's art; it starts idle and hidden.</summary>
	public void Setup(LobbyArt art, HubConfig cfg)
	{
		// a constant (stepped) ramp: each puff holds one exact colour at a time as it cools
		var offs = new float[Ramp.Length];
		for (int i = 0; i < Ramp.Length; i++) offs[i] = (float)i / Ramp.Length;
		var gradient = new Gradient { InterpolationMode = Gradient.InterpolationModeEnum.Constant, Offsets = offs, Colors = Ramp };
		_trailLife = cfg.TrailLifetime;
		_trail = new CpuParticles2D
		{
			Texture = art.Texture("puff-strip"),
			Amount = cfg.TrailAmount,
			Lifetime = cfg.TrailLifetime,
			LocalCoords = false, // the trail stays in the world where the ball shed it
			Emitting = false,
			Direction = Vector2.Up, Spread = 60f,
			InitialVelocityMin = 2f, InitialVelocityMax = 8f,
			Gravity = new Vector2(0, -cfg.TrailLift), DampingMin = 1f, DampingMax = 3f,
			ColorRamp = gradient,
			EmissionShape = CpuParticles2D.EmissionShapeEnum.Sphere, EmissionSphereRadius = 2f,
			Material = new CanvasItemMaterial { ParticlesAnimation = true, ParticlesAnimHFrames = 4, ParticlesAnimVFrames = 1, ParticlesAnimLoop = false },
			AnimSpeedMin = 1f, AnimSpeedMax = 1f, // one pass through the strip over a puff's life: it shrinks to a pixel
		};
		AddChild(_trail);
		_ball = art.Make("void-ball", Vector2.Zero);
		_ball.Visible = false;
		AddChild(_ball);
	}

	/// <summary>Sends the ball off on <paramref name="path"/>, already <paramref name="startAt"/> seconds along it.</summary>
	public void Launch(FlowPath path, float startAt)
	{
		_path = path; _t = startAt; _flying = true; _fade = 0f;
		Place();
		_ball.Visible = true;
		_trail.Emitting = true;
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		if (_flying)
		{
			_t += (float)delta;
			if (_path.Done(_t)) // gone into the rift: the ball vanishes, its trail fades where it was
			{
				_flying = false; _ball.Visible = false; _trail.Emitting = false; _fade = _trailLife;
				return;
			}
			Place();
		}
		else if (_fade > 0f) _fade -= (float)delta;
	}

	private void Place()
	{
		var p = _path.At(_t);
		Position = new Vector2(p.X, p.Y).Round();
		ZIndex = _path.Lane == FlowLane.Back ? BehindZ : FrontZ;
	}
}
