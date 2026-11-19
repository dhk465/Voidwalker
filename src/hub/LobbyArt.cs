using System.Collections.Generic;
using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// The lobby's pixel sprites (code-drawn, from tools/art): loads each PNG with its anchor from
/// <c>res://assets/sprites/lobby/lobby-sprites.json</c> and makes Sprite2D nodes placed by that anchor.
/// </summary>
public sealed class LobbyArt
{
	private const string Dir = "res://assets/sprites/lobby/";
	private readonly Dictionary<string, Vector2I> _anchors = new();

	/// <summary>Reads the sprite manifest.</summary>
	public LobbyArt()
	{
		using var file = FileAccess.Open(Dir + "lobby-sprites.json", FileAccess.ModeFlags.Read);
		var root = Json.ParseString(file.GetAsText()).AsGodotDictionary();
		foreach (var (key, value) in root["sprites"].AsGodotDictionary())
		{
			var entry = value.AsGodotDictionary();
			if (!entry.ContainsKey("anchor")) continue;
			var a = entry["anchor"].AsGodotArray();
			_anchors[key.AsString()] = new Vector2I(a[0].AsInt32(), a[1].AsInt32());
		}
	}

	/// <summary>The texture of one sprite.</summary>
	public Texture2D Texture(string name) => GD.Load<Texture2D>(Dir + name + ".png");

	/// <summary>A sprite whose anchor sits at <paramref name="at"/> (whole pixels, never scaled).</summary>
	public Sprite2D Make(string name, Vector2 at, bool flipH = false)
	{
		var anchor = _anchors.TryGetValue(name, out var a) ? a : Vector2I.Zero;
		var tex = Texture(name);
		var sprite = new Sprite2D
		{
			Texture = tex,
			Centered = false,
			FlipH = flipH,
			Offset = flipH ? new Vector2(-(tex.GetWidth() - anchor.X), -anchor.Y) : -(Vector2)anchor,
			Position = at.Round(),
		};
		return sprite;
	}
}
