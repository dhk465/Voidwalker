namespace Voidwalker.Core;

/// <summary>
/// mulberry32, bit for bit the browser build's <c>rng()</c>, so a seed gives the same draws in both builds.
/// <example><code>var rng = new Rng(1L * 1000003 + 0 * 104729);
/// double first = rng.Next(); // in [0, 1)</code></example>
/// </summary>
public sealed class Rng
{
	private int _seed;

	/// <summary>Starts from a seed; like JavaScript's <c>| 0</c>, only its low 32 bits matter.</summary>
	public Rng(long seed) { _seed = unchecked((int)seed); }

	/// <summary>The next number in [0, 1).</summary>
	public double Next()
	{
		unchecked
		{
			_seed += 0x6D2B79F5;
			int t = (int)((uint)_seed ^ ((uint)_seed >> 15)) * (_seed | 1);
			t ^= t + (int)((uint)t ^ ((uint)t >> 7)) * (t | 61);
			return ((uint)t ^ ((uint)t >> 14)) / 4294967296.0;
		}
	}

	/// <summary>A uniformly picked index below <paramref name="count"/> (the browser's <c>Math.floor(rng() * n)</c>).</summary>
	public int Index(int count) => (int)(Next() * count);
}
