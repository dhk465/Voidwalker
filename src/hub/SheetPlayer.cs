using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// Plays a baked pixelfx effect sheet (frames in a grid, described by a JSON file beside it) as a Sprite2D at the
/// effect's own frame rate: round and round for a loop, or once and then hidden for a one-shot ("loop": false), ready
/// to <see cref="Replay"/> somewhere else. The sheet is already cut to whole pixels.
/// </summary>
public partial class SheetPlayer : Sprite2D
{
	private float _fps = 30f;
	private int _count = 1;
	private double _t;
	private bool _loop = true;

	/// <summary>True when a one-shot has played out (it is hidden until replayed); always false for a loop.</summary>
	public bool Finished { get; private set; }

	/// <summary>Loads <c>name.png</c> and <c>name.json</c> from a folder and places the effect's pivot at <paramref name="at"/>.</summary>
	public static SheetPlayer Load(string dir, string name, Vector2 at)
	{
		using var file = FileAccess.Open($"{dir}{name}.json", FileAccess.ModeFlags.Read);
		var meta = Json.ParseString(file.GetAsText()).AsGodotDictionary();
		var size = meta["frame_size"].AsGodotArray(); var pivot = meta["pivot"].AsGodotArray();
		var tex = GD.Load<Texture2D>($"{dir}{name}.png");
		int cols = meta["columns"].AsInt32(), count = meta["frames"].AsInt32();
		var p = new SheetPlayer
		{
			Texture = tex, Hframes = cols, Vframes = (count + cols - 1) / cols, Centered = false,
			Offset = -new Vector2(pivot[0].AsInt32(), pivot[1].AsInt32()), Position = at.Round(),
			_fps = meta["fps"].AsSingle(), _count = count,
			_loop = !meta.ContainsKey("loop") || meta["loop"].AsBool(),
		};
		if (!p._loop) { p.Visible = false; p.Finished = true; } // a one-shot waits to be played
		_ = size;
		return p;
	}

	/// <summary>Plays the effect from its first frame with its pivot at <paramref name="at"/> (in its parent's space).</summary>
	public void Replay(Vector2 at)
	{
		Position = at.Round(); _t = 0; Frame = 0; Finished = false; Visible = true;
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		if (Finished) return;
		_t += delta;
		int f = (int)(_t * _fps);
		if (!_loop && f >= _count) { Finished = true; Visible = false; return; }
		Frame = f % _count;
	}
}
