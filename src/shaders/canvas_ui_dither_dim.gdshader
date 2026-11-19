// canvas_ui_dither_dim: dims what is behind a station screen (Voidwalker; Claude Code for the project).
// Target: Compatibility renderer (GLES3, Android first); budget: one compare per pixel, no texture reads.
// A one-in-two checker of the deepest palette colour: every pixel is either the scene's own colour or the shade,
// never a blend, so the screen stays palette-exact. FRAGCOORD counts game pixels here (the viewport is drawn at
// game resolution, then scaled up in whole steps). The discard is the effect itself, not a branch to optimise away.
shader_type canvas_item;

group_uniforms dim;
/** The colour of the shading pixels (the palette's deepest, #07040d). */
uniform vec4 shade : source_color = vec4(0.027, 0.016, 0.051, 1.0);
group_uniforms;

// A checker cell is one game pixel: (x + y) is even on half the pixels.
const float CHECKER_PERIOD = 2.0;

void fragment() {
	mediump vec2 p = floor(FRAGCOORD.xy);
	if (mod(p.x + p.y, CHECKER_PERIOD) < 1.0) {
		discard;
	}
	COLOR = shade;
}
