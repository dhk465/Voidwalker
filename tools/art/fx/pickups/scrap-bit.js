import { fit, TAU } from "../../core/index.js";

// Voidwalker's scrap: a twisted shard of hull plating tumbling through the void, a rivet in it, its edge catching the
// light as it turns. Drawn at the game's size (about 7 px); the game flies it to the blob like a mote.
export const meta = {
  name: "Scrap bit",
  desc: "A shard of hull scrap tumbles end over end: dark plating, a rivet, its bright edge flashing each time it turns to the light.",
  tags: ["pickup", "scrap", "metal", "debris", "currency", "loop", "voidwalker"],
  size: [20, 20],
  origin: [10, 10],
  duration: 0.8,
  loop: true,
  fps: 12,
  bg: "dark",
  options: {
    palette: ["#07040d", "#241a3a", "#353a44", "#8b95a3", "#cfc6e6", "#d8f4ff"],
    seed: 1,
  },
};

const SHAPE = [[-3.4, -1.2], [-0.6, -2.6], [3.2, -1.6], [2.4, 1.8], [-1.0, 2.4], [-3.0, 0.8]];

export function draw(p, time, o) {
  const C = fit(o.palette, 6), a = (time / meta.duration) * TAU, ca = Math.cos(a), sa = Math.sin(a);
  const sq = Math.max(0.3, Math.abs(Math.cos(a * 0.5))); // it tumbles end over end: its depth squashes as it turns
  const pts = SHAPE.map(([u, v]) => [u * ca - v * sa * sq, u * sa + v * ca * sq]);
  const lit = Math.cos(a * 0.5) > 0;
  p.poly(pts.map(([x, y]) => [x * 1.15, y * 1.15]), C[0]); // the outline
  p.poly(pts, lit ? C[3] : C[2]);
  // the edge facing the light flashes
  for (let i = 0; i < pts.length; i++) {
    const [x0, y0] = pts[i], [x1, y1] = pts[(i + 1) % pts.length];
    if ((x0 + x1) * -0.6 + (y0 + y1) * -0.75 > 0.8) p.line(x0, y0, x1, y1, lit ? C[5] : C[4]);
  }
  p.px(0.3 * ca, 0.3 * sa, C[1]); // the rivet
  if (lit && Math.abs(Math.sin(a * 0.5)) < 0.25) p.glow(0, 0, 4, C[5], 0.35); // the glint as it squares to the light
}
