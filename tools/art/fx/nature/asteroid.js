import { Rng, fit, hex, TAU } from "../../core/index.js";
import { stoneShape, drawRock } from "./_lib.js";

// Voidwalker's falling asteroid: pixelfx's faceted stone (nature/_lib drawRock, as in the boulder) turning once a loop,
// its pits and a crater turning with it. Each seed is a different rock. The game picks the frame from the rock's angle,
// so the loop is one whole turn in `frames` steps. Drawn at the game's size (radius about 13 px).
export const meta = {
  name: "Asteroid",
  desc: "A jagged asteroid tumbles through the void, a crater turning with it; with void it carries glowing violet crystal, with scrap embedded hull metal that glints.",
  tags: ["nature", "rock", "asteroid", "space", "spin", "loop", "voidwalker"],
  size: [40, 40],
  origin: [20, 20],
  duration: 1.2,
  loop: true,
  fps: 10,
  bg: "dark",
  options: {
    palette: ["#07040d", "#1a1229", "#2c2520", "#45404d", "#5a4a36", "#6e6156", "#8a8296", "#a89a8a"],
    size: 12.5,
    void: 0, // 1: veins of void crystal (it drops void energy)
    scrap: 0, // 1: shards of hull metal in it (it drops scrap)
    seed: 1,
  },
};

export function draw(p, time, o) {
  const S = fit(o.palette, 8), R = new Rng(o.seed * 7919 + 11), shape = stoneShape(R, 12 + (o.seed % 3)); // many corners: jagged, not a gem
  const a = (time / meta.duration) * TAU, r = o.size;
  drawRock(p, 0, 0, r, a, shape, S);
  // one crater on the rock's own surface, so it turns with it: a dark bowl, its rim lit on the side away from the light
  const ca = Math.cos(a), sa = Math.sin(a), u = 0.22, v = 0.18;
  const x = Math.round((u * ca - v * sa) * r), y = Math.round((u * sa + v * ca) * r);
  p.px(x, y, S[1]); p.px(x + 1, y, S[2]); p.px(x, y + 1, S[2]); p.px(x + 1, y + 1, S[2]);
  p.px(x + 2, y + 1, S[6]); p.px(x + 1, y + 2, S[6]);
  const at = (u2, v2) => [Math.round((u2 * ca - v2 * sa) * r), Math.round((u2 * sa + v2 * ca) * r)];
  if (o.void) { // two small crystals of void growing out of the rock, lit violet, glowing
    for (const [u2, v2, k] of [[-0.42, -0.2, 0], [0.12, -0.52, 1]]) {
      const [cx, cy] = at(u2, v2);
      p.px(cx, cy, VOID[1]); p.px(cx, cy - 1, VOID[2]); p.px(cx + 1, cy, VOID[0]); if (k) p.px(cx, cy - 2, VOID[3]);
      p.glow(cx, cy - 1, 4, VOID[2], 0.35);
    }
  }
  if (o.scrap) { // shards of hull plating sunk in the rock, one edge catching a glint as it turns
    const glint = Math.cos(a * 2) > 0.6;
    for (const [u2, v2] of [[0.46, -0.18], [-0.1, 0.5]]) {
      const [cx, cy] = at(u2, v2);
      for (let i = -1; i <= 2; i++) { p.px(cx + i, cy, METAL[1]); p.px(cx + i, cy + 1, METAL[0]); } // a little plate, its underside dark
      p.px(cx - 1, cy - 1, METAL[2]); p.px(cx, cy - 1, glint ? METAL[3] : METAL[2]); p.px(cx + 1, cy - 1, METAL[2]); // its lit top edge
      if (glint) p.glow(cx, cy - 1, 3, METAL[3], 0.3);
    }
  }
}
const VOID = ["#4a3a7a", "#7b5bdf", "#b9a6ff", "#efe6ff"].map(hex), METAL = ["#353a44", "#8b95a3", "#cfc6e6", "#d8f4ff"].map(hex);
