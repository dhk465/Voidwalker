using System.Collections.Generic;
using Godot;

namespace Voidwalker.Adventure;

/// <summary>
/// The adventure's sprite sheets (code-drawn by tools/art/gen_rift.py): each is a grid of frames, rows for headings or
/// variants and columns for animation, read from <c>res://assets/sprites/rift/rift-sheets.json</c>. A figure is drawn
/// by picking a frame, so turning and animating never rotates or blends pixels.
/// <example><code>var sheets = new RiftSheets();
/// sheets.Draw(canvas, "grazer", row: heading, col: walkFrame, at: screenPos, Colors.White);</code></example>
/// </summary>
public sealed class RiftSheets
{
	private const string Dir = "res://assets/sprites/rift/";

	/// <summary>One sheet: its texture, frame size, grid and the frame's pivot.</summary>
	public sealed record Sheet(Texture2D Texture, Vector2I Frame, int Cols, int Rows, Vector2I Anchor, int Frames, float Fps, bool Loop);

	private readonly Dictionary<string, Sheet> _sheets = new();

	/// <summary>Reads the manifest and loads every sheet.</summary>
	public RiftSheets()
	{
		using var file = FileAccess.Open(Dir + "rift-sheets.json", FileAccess.ModeFlags.Read);
		var root = Json.ParseString(file.GetAsText()).AsGodotDictionary();
		foreach (var (key, value) in root["sheets"].AsGodotDictionary())
		{
			var e = value.AsGodotDictionary();
			var fr = e["frame"].AsGodotArray(); var an = e["anchor"].AsGodotArray();
			_sheets[key.AsString()] = new Sheet(GD.Load<Texture2D>(Dir + e["file"].AsString()), new Vector2I(fr[0].AsInt32(), fr[1].AsInt32()),
				e["cols"].AsInt32(), e["rows"].AsInt32(), new Vector2I(an[0].AsInt32(), an[1].AsInt32()),
				e.ContainsKey("frames") ? e["frames"].AsInt32() : e["cols"].AsInt32() * e["rows"].AsInt32(),
				e.ContainsKey("fps") ? (float)e["fps"].AsDouble() : 12f, !e.ContainsKey("loop") || e["loop"].AsBool());
		}
	}

	/// <summary>A sheet by name (pod, grazer, feeder, rock-0..2, slab).</summary>
	public Sheet this[string name] => _sheets[name];

	/// <summary>Whether a sheet exists.</summary>
	public bool Has(string name) => _sheets.ContainsKey(name);

	/// <summary>
	/// Draws a baked effect (an animation in reading order) at <paramref name="seconds"/> into it: a loop wraps, a
	/// one-shot holds nothing once it has played. Returns false when a one-shot is over.
	/// </summary>
	public bool DrawAt(CanvasItem canvas, string name, double seconds, Vector2 at, Color modulate)
	{
		var s = _sheets[name];
		int i = (int)(seconds * s.Fps);
		if (s.Loop) i = Mathf.PosMod(i, s.Frames); else if (i >= s.Frames) return false;
		Draw(canvas, name, i / s.Cols, i % s.Cols, at, modulate);
		return true;
	}

	/// <summary>
	/// Draws variant <paramref name="row"/> of a sheet whose rows are variants (headings, poses) and whose columns are
	/// frames, at <paramref name="seconds"/> into its animation: a loop wraps, a one-shot draws nothing once over.
	/// </summary>
	public bool DrawRow(CanvasItem canvas, string name, int row, double seconds, Vector2 at, Color modulate)
	{
		var s = _sheets[name];
		int i = (int)(seconds * s.Fps);
		if (s.Loop) i = Mathf.PosMod(i, s.Frames); else if (i >= s.Frames) return false;
		Draw(canvas, name, row, i, at, modulate);
		return true;
	}

	/// <summary>Seconds a baked effect plays for.</summary>
	public double Length(string name) => _sheets[name].Frames / (double)_sheets[name].Fps;

	/// <summary>Draws frame (<paramref name="row"/>, <paramref name="col"/>) of a sheet with its pivot on <paramref name="at"/> (whole pixels).</summary>
	public void Draw(CanvasItem canvas, string name, int row, int col, Vector2 at, Color modulate)
	{
		var s = _sheets[name];
		row = Mathf.PosMod(row, s.Rows); col = Mathf.PosMod(col, s.Cols);
		var src = new Rect2(col * s.Frame.X, row * s.Frame.Y, s.Frame.X, s.Frame.Y);
		canvas.DrawTextureRectRegion(s.Texture, new Rect2(at - (Vector2)s.Anchor, s.Frame), src, modulate);
	}
}
