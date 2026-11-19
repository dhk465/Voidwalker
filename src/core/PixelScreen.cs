using Godot;

namespace Voidwalker.Core;

/// <summary>
/// Pixel-perfect full screen on any phone, with pixels big enough to read. It picks the largest whole-number zoom
/// that still leaves at least <see cref="MinSize"/> game pixels, then makes the game area the window divided by that
/// zoom, so the screen fills to within a pixel or two and every game pixel stays the same size. The hub's camera and
/// the station screens lay themselves out in whatever area results (never smaller than <see cref="MinSize"/>).
/// Registered as an autoload in project.godot; it re-fits whenever the window changes size.
/// <example>
/// On a 2772x1280 phone the zoom is 4 and the game area 693x320; a 1280x720 window gets 2 and 640x360:
/// <code>var (zoom, area) = PixelScreen.FitFor(new Vector2I(2772, 1280)); // (4, (693, 320))</code>
/// Read the live zoom from the autoload: <c>GetNode&lt;PixelScreen&gt;("/root/PixelScreen").Zoom</c>.
/// </example>
/// </summary>
public partial class PixelScreen : Node
{
	/// <summary>
	/// The smallest game area every screen is laid out for, in game pixels. Smaller means bigger pixels: 300 rows is
	/// what lets a 1280-pixel-tall phone reach 4x (body text about 3.5 mm tall) while 720p and 1080p desktops keep a
	/// 640x360 area.
	/// </summary>
	public static readonly Vector2I MinSize = new(533, 300);

	/// <summary>The whole-number zoom in use (game pixels to screen pixels).</summary>
	public int Zoom { get; private set; } = 1;

	/// <inheritdoc/>
	public override void _Ready()
	{
		GetWindow().SizeChanged += Fit;
		Fit();
	}

	/// <summary>The game area for a window of <paramref name="window"/> pixels at the largest zoom that keeps <see cref="MinSize"/>.</summary>
	public static (int zoom, Vector2I area) FitFor(Vector2I window)
	{
		int zoom = Mathf.Max(1, Mathf.Min(window.X / MinSize.X, window.Y / MinSize.Y));
		return (zoom, new Vector2I(Mathf.Max(MinSize.X, window.X / zoom), Mathf.Max(MinSize.Y, window.Y / zoom)));
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
