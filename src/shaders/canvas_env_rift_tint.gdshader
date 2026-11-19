// Voidwalker, Godot 4.8 .NET; written by Claude (claude-opus-5-5) for the Rift Gate, 2026-11-19.
// Target: Compatibility renderer (GLES3), Android first. Cost: one texture read per sprite pixel, no loops or
// branches (the hub's rift is 69x104, a card's 37x52).
// Paints a stepped effect sheet (tools/art/bake_gate_rift.py: grey = step * 36, steps 0..6) in a ramp built from
// one tint: the void's black, three steps from black toward the tint, the tint, then two toward white. Each step is
// one flat colour, so the sprite stays palette-exact for whatever nature the chosen rift has.
shader_type canvas_item;

group_uniforms colours;
uniform vec3 tint : source_color = vec3(0.725, 0.651, 1.0);    // the chosen rift's prevailing nature (#b9a6ff when none)
uniform vec3 abyss : source_color = vec3(0.027, 0.016, 0.051); // #07040d, the dark well
group_uniforms;

const float STEP_GREY = 36.0;       // the bake stores step k as grey k * 36 (out of 255)
const float FIRST_LIGHT_STEP = 4.5; // steps 5 and 6 lighten the tint; 0 to 4 darken it
// How far each step moves from the abyss toward the tint, and from the tint toward white.
const float TOWARD_TINT[7] = float[](0.0, 0.22, 0.45, 0.72, 1.0, 1.0, 1.0);
const float TOWARD_WHITE[7] = float[](0.0, 0.0, 0.0, 0.0, 0.0, 0.5, 0.85);

void fragment() {
	vec4 s = texture(TEXTURE, UV);
	int k = clamp(int(round(s.r * 255.0 / STEP_GREY)), 0, 6);
	float kf = float(k);
	vec3 dark = mix(abyss, tint, TOWARD_TINT[k]);
	vec3 light = mix(tint, vec3(1.0), TOWARD_WHITE[k]);
	COLOR = vec4(mix(dark, light, step(FIRST_LIGHT_STEP, kf)), step(0.5, s.a));
}
