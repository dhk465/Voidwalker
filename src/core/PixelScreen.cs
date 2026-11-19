using Godot;

namespace Voidwalker.Core;

/// <summary>
/// Pixel-perfect full screen on any phone. Godot's "expand" aspect sizes the game area by the screen's shape before
/// integer scaling rounds the zoom down, which leaves bars on every side of a 2.17:1 phone. This picks the largest
/// whole-number zoom at which the base area (640x360) fits, then makes the game area the window divided by that
/// zoom, so the screen fills to within a pixel or two and every game pixel stays the same size.
/// Registered as an autoload in project.godot; it re-fits whenever the window changes size.
/// <example>
/// On a 2772x1280 phone the zoom is 3 and the game area 924x426:
/// <code>var (zoom, area) = PixelScreen.FitFor(new Vector2I(2772, 1280)); // (3, (924, 426))</code>
/// Read the live zoom from the autoload: <c>GetNode&lt;PixelScreen&gt;("/root/PixelScreen").Zoom</c>.
/// </example>
/// </summary>
public partial class PixelScreen : Node
{
	/// <summary>The smallest game area the layout is designed for, in game pixels.</summary>
	public static readonly Vector2I BaseSize = new(640, 360);

	/// <summary>The whole-number zoom in use (game pixels to screen pixels).</summary>
	public int Zoom { get; private set; } = 1;

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetWindow().SizeChanged += Fit;
		Fit();
	}

	/// <summary>The game area for a window of <paramref name="window"/> pixels at the largest zoom that fits the base size.</summary>
	public static (int zoom, Vector2I area) FitFor(Vector2I window)
	{
		int zoom = Mathf.Max(1, Mathf.Min(window.X / BaseSize.X, window.Y / BaseSize.Y));
		return (zoom, new Vector2I(Mathf.Max(BaseSize.X, window.X / zoom), Mathf.Max(BaseSize.Y, window.Y / zoom)));
	}

	private void Fit()
	{
		var win = GetWindow();
		var (zoom, area) = FitFor(win.Size);
		Zoom = zoom;
		win.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;
		win.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep; // the area already has the window's shape
		win.ContentScaleStretch = Window.ContentScaleStretchEnum.Integer;
		win.ContentScaleSize = area;
	}
}
