// Voidwalker, Godot 4.8 .NET; written by Claude (claude-opus-5-5) for the hub port, 2026-11-19.
// Target: Compatibility renderer (GLES3), Android first; full-viewport cost budget: about 1 ms on a mid phone.
// The Rift Gate's swirl: six elliptical rings of light turning at different speeds round a black heart, alternating
// the chosen rift's colour with deep violet. Hard pixels only. Put it on a ColorRect centred on the gate.
shader_type canvas_item;

group_uniforms colours;
uniform vec3 tint : source_color = vec3(0.725, 0.651, 1.0);  // the chosen rift's prevailing nature
uniform vec3 deep : source_color = vec3(0.290, 0.227, 0.478); // #4a3a7a
group_uniforms shape;
uniform vec2 rect_size = vec2(56.0, 104.0);                  // the ColorRect's size in pixels (set from code; a ColorRect has no texture to read it from)
uniform vec2 radius = vec2(24.0, 48.0);                       // the outer ring, in pixels
uniform float rings = 6.0;
uniform float ring_width = 1.6;                               // px

void fragment() {
	vec2 p = floor(UV * rect_size) + 0.5 - rect_size * 0.5;
	vec4 col = vec4(0.0);
	for (float k = 0.0; k < 6.0; k += 1.0) {
		if (k >= rings) break;
		vec2 r = radius * (1.0 - k * 0.15);
		float e = length(p / r), along = abs(e - 1.0) * min(r.x, r.y);
		if (along < ring_width * 0.5) {
			float a = atan(p.y / r.y, p.x / r.x) / 6.2832 + 0.5;
			if (fract(a - TIME * (0.16 + k * 0.05)) < 0.67) col = vec4(mod(k, 2.0) < 1.0 ? deep : tint, 1.0);
		}
	}
	if (length(p / vec2(5.0, 11.0)) < 1.0) col = vec4(0.0, 0.0, 0.0, 1.0); // the black heart
	COLOR = col;
}
