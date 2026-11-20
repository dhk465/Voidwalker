import { fit, hex, cycle, TAU } from "../../core/index.js";

// Voidwalker's debris slab: a torn length of hull plating falling through the rift, the wreck of something that flew.
// Drawn at the game's size (about 75 x 11 px, the browser's 150 x 22 slab); its collision box is level, so it is too.
export const meta = {
  name: "Debris slab",
  desc: "A torn slab of hull plating drifts in the void: panel seams and rivets, a bent lit edge, dead running lights that stutter, sparks spitting from its torn end.",
  tags: ["scifi", "debris", "wreck", "hull", "metal", "space", "loop", "voidwalker"],
  size: [104, 30],
  origin: [56, 13],
  duration: 2.0,
  loop: true,
  fps: 12,
  bg: "dark",
  options: {
    palette: ["#07040d", "#241a3a", "#353a44", "#45404d", "#8b95a3", "#cfc6e6"], // hull, dark to bright
    lights: ["#3a0a1c", "#8a1030", "#ff4466"],
    sparks: ["#fff0b0", "#ffd27a", "#e0a040"],
    seed: 1,
  },
};

const W = 37, Hh = 5; // half length, half height (px)
// the slab's outline: ragged at the torn left end, a dent on top, a lip bent up at the right
const top = (x) => -Hh + (x < -30 ? Math.abs(Math.sin(x * 1.7)) * 2.4 : 0) + (x > -8 && x < 6 ? 0.9 : 0) - (x > 31 ? (x - 31) * 0.4 : 0);
const bottom = (x) => Hh - (x < -31 ? Math.abs(Math.cos(x * 2.1)) * 2.2 : 0);

export function draw(p, time, o) {
  const C = fit(o.palette, 6), L = fit(o.lights, 3), S = fit(o.sparks, 3), u = time / meta.duration;
  p.shade(-W - 1, -Hh - 4, W + 1, Hh + 1, (x, y) => {
    const t = top(x), b = bottom(x);
    if (x < -W || x > W || y < t || y > b) return 0;
    if (y < t + 1) return C[x > 31 ? 5 : 4]; // the lit top edge, brightest on the bent lip
    if (y > b - 1) return C[1]; // the shadowed underside
    if (Math.abs(((x + W) % 15) - 0) < 0.6) return C[1]; // panel seams
    if ((Math.round(x) + W) % 15 === 3 && Math.round(y) === Math.round(t + 2)) return C[4]; // a rivet below each seam
    const scorch = Math.hypot((x + 33) / 9, (y - 0.5) / 4.5) + 0.25 * Math.sin(x * 0.8 + y * 1.3) < 1; // one burnt patch at the torn end
    if (scorch) return C[1];
    return y < 0 ? C[3] : C[2];
  });
  // two cables hanging from the torn end, swaying
  for (const [cy, len, ph] of [[-1, 6, 0], [2, 4.5, 1.7]]) {
    let px0 = -W - 1, py0 = cy;
    for (let k = 1; k <= len; k++) {
      const x = -W - 1 - k * 0.7 + Math.sin(u * TAU + ph + k * 0.6) * 0.8 * (k / len), y = cy + k * 0.8;
      p.line(px0, py0, x, y, k === Math.round(len) ? C[5] : C[3]); px0 = x; py0 = y; // a frayed bright end
    }
  }
  // the outline: a dark rim round the whole slab
  for (let x = -W; x <= W; x++) { p.px(x, Math.floor(top(x)) - 1, C[0]); p.px(x, Math.ceil(bottom(x)) + 0, C[0]); }
  for (let y = Math.floor(top(-W)); y <= Math.ceil(bottom(-W)); y++) p.px(-W - 1, y, C[0]);
  for (let y = Math.floor(top(W)); y <= Math.ceil(bottom(W)); y++) p.px(W + 1, y, C[0]);
  // dead running lights: three that stutter, each on its own broken rhythm
  [[-22, 0.13], [6, 0.47], [27, 0.81]].forEach(([lx, ph], i) => {
    const on = Math.sin((u + ph) * TAU * (2 + i)) > 0.55 && Math.sin((u + ph) * TAU * 7) > -0.2;
    p.px(lx, 1, on ? L[2] : L[1]);
    p.px(lx + 1, 1, on ? L[1] : L[0]);
    if (on) p.glow(lx, 1, 4, L[2], 0.4);
  });
  // sparks spitting from the torn end, falling away and cooling
  cycle(time, meta.duration, 5, o.seed, 0.45, (age, L, r) => {
    const life = age / L, x = -W - 1 - life * (4 + r.next() * 6), y = -1 + r.next() * 4 + life * life * 6;
    p.px(x, y, life < 0.3 ? S[0] : life < 0.6 ? S[1] : S[2]);
    if (life < 0.3) p.glow(x, y, 3, S[1], 0.3);
  });
}
