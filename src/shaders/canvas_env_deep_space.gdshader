// Voidwalker, Godot 4.8 .NET; written by Claude (claude-opus-5-5) for the hub port, 2026-11-19.
// Target: Compatibility renderer (GLES3), Android first; full-viewport cost budget: about 1 ms on a mid phone.
// Deep space behind the hub: a slow, domain-warped nebula (violet, a cold teal vein, dark dust lanes) and fixed stars
// that brighten and fade on their own long cycles. Exact palette colours only, stepped by a 4x4 ordered (Bayer)
// dither: no blending, one colour per pixel. Put it on a ColorRect that covers the viewport.
shader_type canvas_item;
render_mode unshaded, blend_disabled;

group_uniforms palette;
uniform vec3 violet[6] : source_color;   // dark to bright
uniform vec3 teal[3] : source_color;
uniform vec3 star_ramp[7] : source_color; // a star's light, dim to bright
group_uniforms motion;
uniform float drift_speed = 1.0;
uniform float star_density = 0.07;        // share of 7x7 cells that hold a star
uniform float blink_min = 6.0;            // s: the quickest a star brightens and fades
uniform float blink_max = 18.0;           // s: the slowest
group_uniforms;
uniform vec2 offset = vec2(0.0);          // keeps the sky still when the stage is recentred

float h(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float n(vec2 p) { vec2 i = floor(p), f = fract(p); f = f * f * (3.0 - 2.0 * f);
	return mix(mix(h(i), h(i + vec2(1, 0)), f.x), mix(h(i + vec2(0, 1)), h(i + vec2(1, 1)), f.x), f.y); }
float fbm(vec2 p) { float v = 0.0, a = 0.5; for (int i = 0; i < 5; i++) { v += a * n(p); p = p * 2.03 + vec2(17.1, 9.2); a *= 0.5; } return v; }
float bayer2(vec2 a) { a = floor(a); return fract(a.x / 2.0 + a.y * a.y * 0.75); }
float bayer4(vec2 a) { return bayer2(0.5 * a) * 0.25 + bayer2(a); }

void fragment() {
	vec2 px = floor(FRAGCOORD.xy) + offset, p = px / 360.0 * 2.2;
	float t = TIME * drift_speed;
	vec2 drift = vec2(t * 0.006, t * 0.002);
	vec2 w = vec2(fbm(p * 0.9 + drift), fbm(p * 0.9 + vec2(5.2, 1.3) - drift)); // the warp: gas folding on itself
	float gas = fbm(p * 1.1 + w * 1.6 + drift * 2.0);
	float dust = smoothstep(0.52, 0.72, fbm(p * 2.4 + w * 2.2 + vec2(3.0, 8.0)));          // dark lanes cutting through
	float vein = smoothstep(0.62, 0.86, fbm(p * 1.6 - w * 1.3 + vec2(11.0, 2.0) + drift)); // a cold teal vein
	float v = clamp(pow(clamp(gas * 1.25 - 0.28, 0.0, 1.0), 1.6) * (1.0 - 0.75 * dust), 0.0, 1.0);
	float d = bayer4(px);
	float L = v * 5.0, i = floor(L); if (fract(L) > d) i += 1.0;
	vec3 col = violet[int(min(i, 5.0))];
	float T = vein * (1.0 - dust) * 2.0, j = floor(T); if (fract(T) > d) j += 1.0;
	if (j >= 1.0 && j >= i * 0.5) col = teal[int(min(j, 2.0))];
	// the stars: one at most per 7x7 cell, fixed in place, each on its own slow blink
	vec2 cell = floor(px / 7.0);
	if (h(cell) < star_density) {
		vec2 at = cell * 7.0 + floor(vec2(h(cell + 3.1), h(cell + 7.7)) * 7.0);
		if (px == at) {
			float period = mix(blink_min, blink_max, h(cell + 1.7)), wave = 0.5 + 0.5 * sin(TIME * 6.2832 / period + h(cell + 5.3) * 6.2832);
			float lvl = floor(wave * wave * (0.35 + 0.65 * h(cell + 9.9)) * 7.0);
			if (lvl >= 1.0) col = star_ramp[int(min(lvl, 6.0))];
		}
	}
	COLOR = vec4(col, 1.0);
}
