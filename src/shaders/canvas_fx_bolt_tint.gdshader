// canvas_fx_bolt_tint: Voidwalker's bolt colouring, by the rift scene (src/adventure/RiftScene.cs, BoltLayer).
// Paints a bolt sheet's three grey steps (tools/art/bake_rift_fx.py "bolt", grey 0x55 / 0xaa / 0xff) in its card's
// colour, passed as the draw's modulate: the dark step becomes the colour sunk toward ink, the middle step the colour
// itself, the light step white. Pixels stay hard: each pixel takes exactly one of the three colours.
// Target: Compatibility renderer (desktop and Android, mobile GPUs); one texture read, no branches, mediump is enough.
shader_type canvas_item;

group_uniforms tint;
/** What the dark step sinks the card's colour toward (#1a1229, the rift's ink). */
uniform vec4 ink : source_color = vec4(0.102, 0.071, 0.161, 1.0);
/** How far the dark step sinks toward ink (0 = the card's colour, 1 = ink). */
uniform float rim_sink : hint_range(0.0, 1.0) = 0.55;
group_uniforms;

const float MID_STEP = 0.5;   // grey above this is the middle step (0xaa)
const float LIGHT_STEP = 0.9; // grey above this is the light step (0xff)
const float SOLID = 0.5;      // alpha above this is drawn

varying mediump vec4 card;

void vertex() {
	card = COLOR; // the draw's modulate: the card's colour
}

void fragment() {
	mediump vec4 t = texture(TEXTURE, UV);
	mediump float g = t.r;
	mediump vec3 rim = mix(card.rgb, ink.rgb, rim_sink);
	mediump vec3 c = mix(rim, card.rgb, step(MID_STEP, g)); // dark step -> rim, else the card's colour
	c = mix(c, vec3(1.0), step(LIGHT_STEP, g));              // light step -> white
	COLOR = vec4(c, step(SOLID, t.a));
}
