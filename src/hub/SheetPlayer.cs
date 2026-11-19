using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// Plays a baked pixelfx effect sheet (frames in a grid, described by a JSON file beside it) as a looping
/// Sprite2D, at the effect's own frame rate. The sheet is already cut to whole pixels.
/// </summary>
public partial class SheetPlayer : Sprite2D
{
	private float _fps = 30f;
	private int _count = 1;
	private double _t;

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
		};
		_ = size;
		return p;
	}

	/// <inheritdoc/>
	public override void _Process(double delta)
	{
		_t += delta;
		Frame = (int)(_t * _fps) % _count;
	}
}
