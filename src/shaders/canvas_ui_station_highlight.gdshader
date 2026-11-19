// Voidwalker, Godot 4.8 .NET; written by Claude (claude-opus-5-5) for the hub port, 2026-11-19.
// Target: Compatibility renderer (GLES3), Android first; full-viewport cost budget: about 1 ms on a mid phone.
// Lights a hub station's own pixels when it is selected (tapped) or hovered: a 1 px outline hugging the sprite's
// silhouette, a dithered outer ring that breathes, and a band of light sweeping across it. The sprite needs a 3 px
// transparent margin (tools/art/pack_lobby.py adds it) for the outline and ring to land in. Exact colours only.
shader_type canvas_item;

uniform bool selected = false;
group_uniforms colours;
uniform vec3 rim : source_color = vec3(1.0, 0.941, 0.690);   // #fff0b0
uniform vec3 ring : source_color = vec3(1.0, 0.824, 0.478);  // #ffd27a
uniform vec3 sheen : source_color = vec3(0.937, 0.902, 1.0); // #efe6ff

float bayer2(vec2 a) { a = floor(a); return fract(a.x / 2.0 + a.y * a.y * 0.75); }
float bayer4(vec2 a) { return bayer2(0.5 * a) * 0.25 + bayer2(a); }
#define INSIDE_AT(coord) (texture(TEXTURE, (coord)).a > 0.5 ? 1.0 : 0.0)

void fragment() {
	vec4 c = texture(TEXTURE, UV);
	COLOR = c;
	if (selected) {
		vec2 px = TEXTURE_PIXEL_SIZE, q = floor(FRAGCOORD.xy);
		float d = bayer4(q);
		if (c.a > 0.5) { // on the object: a band of light sweeping across it, dithered at its edges
			float b = fract((q.x + q.y) / 110.0 - TIME * 0.45), w = smoothstep(0.0, 0.05, b) * (1.0 - smoothstep(0.05, 0.11, b));
			if (w > d) COLOR = vec4(sheen, 1.0);
		} else {
			float n1 = INSIDE_AT(UV + vec2(px.x, 0)) + INSIDE_AT(UV - vec2(px.x, 0)) + INSIDE_AT(UV + vec2(0, px.y)) + INSIDE_AT(UV - vec2(0, px.y));
			float n2 = INSIDE_AT(UV + vec2(2.0 * px.x, 0)) + INSIDE_AT(UV - vec2(2.0 * px.x, 0)) + INSIDE_AT(UV + vec2(0, 2.0 * px.y)) + INSIDE_AT(UV - vec2(0, 2.0 * px.y))
				+ INSIDE_AT(UV + px) + INSIDE_AT(UV - px) + INSIDE_AT(UV + vec2(px.x, -px.y)) + INSIDE_AT(UV - vec2(px.x, -px.y));
			if (n1 > 0.0) COLOR = vec4(rim, 1.0); // the outline, one pixel out from the silhouette
			else if (n2 > 0.0 && d < 0.35 + 0.25 * sin(TIME * 4.0)) COLOR = vec4(ring, 1.0); // a breathing dithered ring
		}
	}
}
