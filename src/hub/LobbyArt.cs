using System.Collections.Generic;
using Godot;

namespace Voidwalker.Hub;

/// <summary>
/// The code-drawn pixel sprites (from tools/art): loads each PNG with its anchor from a set's
/// <c>lobby-sprites.json</c> (the lobby's by default, or the rift's) and makes Sprite2D nodes placed by that anchor.
/// <example><code>var rift = new LobbyArt(LobbyArt.RiftDir);
/// var pod = rift.Texture("pod"); Vector2I anchor = rift.Anchor("pod");</code></example>
/// </summary>
public sealed class LobbyArt
{
	/// <summary>The lobby's sprites.</summary>
	public const string LobbyDir = "res://assets/sprites/lobby/";
	/// <summary>The rift's sprites: Ybasogs, matter and the blob's forms at the adventure's size.</summary>
	public const string RiftDir = "res://assets/sprites/rift/";
	private readonly string Dir;
	private readonly Dictionary<string, Vector2I> _anchors = new();
	private readonly Dictionary<string, Texture2D> _textures = new();

	/// <summary>Reads a set's sprite manifest (the lobby's unless <paramref name="dir"/> names another).</summary>
	public LobbyArt(string dir = LobbyDir)
	{
		Dir = dir;
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

	/// <summary>The texture of one sprite (loaded once, then kept).</summary>
	public Texture2D Texture(string name) => _textures.TryGetValue(name, out var t) ? t : _textures[name] = GD.Load<Texture2D>(Dir + name + ".png");

	/// <summary>Whether the set has this sprite.</summary>
	public bool Has(string name) => _anchors.ContainsKey(name);

	/// <summary>The pixel in a sprite that sits on its position (its centre or the point it stands on).</summary>
	public Vector2I Anchor(string name) => _anchors.TryGetValue(name, out var a) ? a : Vector2I.Zero;

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
