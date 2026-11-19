using System.Collections.Generic;
using Godot;

namespace Voidwalker.Ui;

/// <summary>
/// Small pixel icons drawn from hand-placed grids, one character per pixel, shown at 1x and never scaled (every
/// pixel on screen stays the same size). The nature sigils are one colour, given when asked; the relics carry their
/// own palette colours. Textures are built once per (icon, colour) and cached.
/// <example><code>Texture2D wrath = PixelIcons.Sigil("wrath", new Color("#ff4b3a")); // 9x9
/// Texture2D bell = PixelIcons.Relic("bell");</code></example>
/// </summary>
public static class PixelIcons
{
	/// <summary>Width and height of a nature sigil.</summary>
	public const int SigilSize = 9;

	// '#' is the sigil's colour, '.' is empty. Shapes follow the browser build's sigils.
	private static readonly Dictionary<string, string[]> Sigils = new()
	{
		["wrath"] = new[] { "....#....", "...##....", "...###...", "..####.#.", "..######.", ".#######.", ".#######.", ".#######.", "..#####.." },
		["patience"] = new[] { "....#....", ".#..#..#.", "..#.#.#..", "...###...", "#########", "...###...", "..#.#.#..", ".#..#..#.", "....#...." },
		["greed"] = new[] { "#########", "........#", ".######.#", ".#....#.#", ".#.##.#.#", ".#.#..#.#", ".#.####.#", ".#......#", ".########" },
		["charity"] = new[] { ".#.....#.", "##.....##", "##.....##", "###...###", ".###.###.", "..#####..", "...###...", "....#....", "........." },
		["pride"] = new[] { "....#....", ".#..#..#.", "..#...#..", "...###...", "##.###.##", "...###...", "..#...#..", ".#..#..#.", "....#...." },
		["humility"] = new[] { ".........", "..#......", ".###..#..", ".####.##.", "#########", "#########", ".........", "#########", "........." },
		["envy"] = new[] { ".........", "...###...", ".##...##.", "#...#...#", "#...#...#", "#...#...#", ".##...##.", "...###...", "........." },
		["kindness"] = new[] { "....#....", "...###...", "..#####..", ".###.###.", ".###.###.", ".###.###.", "..##.##..", "...#.#...", "....#...." },
		["lust"] = new[] { ".........", ".##...##.", "####.####", "#########", "#########", ".#######.", "..#####..", "...###...", "....#...." },
		["chastity"] = new[] { "....#....", "...#.#...", "...#.#...", "..#...#..", ".#..#..#.", ".#.###.#.", ".#..#..#.", "..#...#..", "...###..." },
		["gluttony"] = new[] { "..####...", ".######..", "#####....", "####.....", "####.....", "#####....", ".######..", "..####...", "........." },
		["sloth"] = new[] { "...####..", ".###.....", ".##......", "##.......", "##.......", "##.......", ".##......", ".###.....", "...####.." },
		["temperance"] = new[] { ".##...##.", "#..###..#", ".........", ".##...##.", "#..###..#", ".........", ".##...##.", "#..###..#", "........." },
		["diligence"] = new[] { "....###..", "...###...", "..###....", ".#######.", "....###..", "...###...", "..##.....", ".##......", ".#......." },
		[""] = new[] { "....#....", "...#.#...", "..#...#..", ".#.....#.", "#.......#", ".#.....#.", "..#...#..", "...#.#...", "....#...." },
	};

	// Relic grids: each letter is a palette colour (see RelicInk); '.' is empty.
	private static readonly Dictionary<char, string> RelicInk = new()
	{
		['G'] = "#ffd27a", ['D'] = "#e0a040", ['B'] = "#9ad7ff", ['W'] = "#d8f4ff", ['K'] = "#1a1229", ['Y'] = "#d6e25a",
		['C'] = "#45404d", ['L'] = "#b5c43a", ['R'] = "#ff4b3a", ['M'] = "#5a4a36", ['P'] = "#fff0b0", ['T'] = "#e8e6c0",
		['S'] = "#8a8296", ['V'] = "#cfc6e6", ['X'] = "#07040d", ['H'] = "#ffffff", ['N'] = "#8b95a3", ['O'] = "#353a44",
		['E'] = "#5ac8b9", ['Q'] = "#8a1030", ['U'] = "#6e6156", ['A'] = "#b9a6ff", ['Z'] = "#d8c8ff", ['F'] = "#1f3a72",
	};

	private static readonly Dictionary<string, string[]> Relics = new()
	{
		["bell"] = new[] { "......GGG......", ".....GDDDG.....", "....GDDDDDG....", "....GDDDDDG....", "...GDDDDDDDG...", "...GDDDDDDDG...", "..GDDDDDDDDDG..", "..GDDDDDDDDDG..", ".GGGGGGGGGGGGG.", ".......G.......", "......GGG......" },
		["shard"] = new[] { ".....W.....", "....WBW....", "...WBBBW...", "..WBBBBBW..", "..WBBBBBW..", "...WBBBBW..", "...WBBBW...", "....WBBW...", "....WBW....", ".....W....." },
		["lens"] = new[] { "...GGGGG...", "..GKKKKKG..", ".GKKKKKKKG.", "GKKKKKKKKKG", "GKKKYYYKKKG", "GKKKYYYKKKG", "GKKKYYYKKKG", "GKKKKKKKKKG", ".GKKKKKKKG.", "..GKKKKKG..", "...GGGGG..." },
		["charm"] = new[] { ".....L.....", "....LCL....", "...LCCCL...", "..LCCCCCL..", ".LCCCCCCCL.", ".LCCCCCCCL.", "..LCCCCCL..", "..LCCCCCL..", "..LLLLLLL.." },
		["ember"] = new[] { ".....G.....", "....GRG....", "....GRRG...", "...GRRRG...", "..GRRRRRG..", "..GRRGRRG..", ".GRRGGGRRG.", ".GRRGGGRRG.", "..GRRRRRG..", "...GGGGG..." },
		["tide"] = new[] { "WWWW...PPPP", "WBBF...PRRQ", "WBBF...PRRQ", "WBBF...PRRQ", "WBBF...PRRQ", "WBBF...PRRQ", "FFFF...QQQQ" },
		["lamp"] = new[] { "....GGG....", "...G...G...", "..GGGGGGG..", "..GMMMMMG..", "..GMPPPMG..", "..GMPPPMG..", ".GMMPPPMMG.", ".GMMMMMMMG.", ".GGGGGGGGG." },
		["crown"] = new[] { "G....G....G", "GG..GDG..GG", "GDGGDDDGGDG", "GDDDDDDDDDG", "GDDDDDDDDDG", "GDDDDDDDDDG", "GGGGGGGGGGG" },
		["tooth"] = new[] { "SSSSSSSSS", "STTTTTTTS", "STTTTTTTS", ".STTTTTS.", ".STTTTTS.", "..STTTS..", "..STTTS..", "...STS...", "...STS...", "....S...." },
		["salt"] = new[] { "...VVVVV...", "...V...V...", "....VVV....", "...V...V...", "..VKKKKKV..", "..VKKKKKV..", "..VXXXXXV..", "..VXXXXXV..", "..VXXXXXV..", "...VVVVV..." },
		["scale"] = new[] { "....H....", "...HVH...", "..HVVVH..", ".HVVHVVH.", "HVVHHHVVH", ".HVVHVVH.", "..HVVVH..", "...HVH...", "....H...." },
		["nail"] = new[] { "NNNNNNN", "NNNNNNN", "..NON..", "..NON..", "..NON..", "..NON..", "..NON..", "..NON..", "..NON..", "...N..." },
		["orrery"] = new[] { "...GGGGG...", "..G.....G..", ".G..GGG..G.", "G..G...G.DD", "G..G.D.G.DD", "G..G...G..G", ".G..GGG..G.", "..G.....G..", "...GGGGG..." },
		["idoleye"] = new[] { ".....S.....", "...SSCSS...", ".SSCCECCSS.", "SCCCEEECCCS", ".SSCCECCSS.", "...SSCSS...", ".....S....." },
		["psalm"] = new[] { "UUUUUUUUU", "UTTTTTTTU", "UTUUUUUTU", "UTTTTTTTU", "UTTTQQTTU", "UTTQQQQTU", "UTTTQQTTU", "UTUUUUUTU", "UTTTTTTTU", "UUUUUUUUU" },
		["pearl"] = new[] { "..AAAAA..", ".AXXXXXA.", "AXZZXXXXA", "AXZXXXXXA", "AXXXXXXXA", "AXXXXXXXA", "AXXXXXXXA", ".AXXXXXA.", "..AAAAA.." },
	};

	// A level pip: a small diamond.
	private static readonly string[] PipGrid = { ".#.", "###", ".#." };

	private static readonly Dictionary<string, Texture2D> Cache = new();

	/// <summary>A nature's sigil in one colour ("" gives the void bolt's diamond).</summary>
	public static Texture2D Sigil(string nature, Color colour) =>
		Cached($"s:{nature}:{colour.ToHtml()}", () => Build(Sigils.TryGetValue(nature, out var g) ? g : Sigils[""], c => c == '#' ? colour : null));

	/// <summary>A relic's icon in its own colours.</summary>
	public static Texture2D Relic(string id) =>
		Cached($"r:{id}", () => Build(Relics[id], c => RelicInk.TryGetValue(c, out var hex) ? new Color(hex) : null));

	/// <summary>A level pip (3x3 diamond) in one colour.</summary>
	public static Texture2D Pip(Color colour) => Cached($"p:{colour.ToHtml()}", () => Build(PipGrid, c => c == '#' ? colour : null));

	private static Texture2D Cached(string key, System.Func<Texture2D> make)
	{
		if (!Cache.TryGetValue(key, out var t)) Cache[key] = t = make();
		return t;
	}

	private static Texture2D Build(string[] rows, System.Func<char, Color?> ink)
	{
		int h = rows.Length, w = 0;
		foreach (var r in rows) w = Mathf.Max(w, r.Length);
		var img = Image.CreateEmpty(w, h, false, Image.Format.Rgba8);
		for (int y = 0; y < h; y++)
			for (int x = 0; x < rows[y].Length; x++)
				if (ink(rows[y][x]) is { } c) img.SetPixel(x, y, c);
		return ImageTexture.CreateFromImage(img);
	}
}
