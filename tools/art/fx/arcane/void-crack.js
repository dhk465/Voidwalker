// Voidwalker's Void Rite: a crack in space-time that leaks void. Written by Claude (claude-opus-5-5) for the Voidwalker
// hub, 2026-11-19. Source of truth: tools/art/fx/arcane/void-crack.js (tools/art/bake_rite_crack.py copies it into pixelfx).
import { fit, mix, clamp, TAU, cycle, fly, rng } from "../../core/index.js";

export const meta = {
  name: "Void crack",
  desc: "A jagged crack hangs in the air, black inside with a hard pale rim; void seeps out of it: wisps ooze from its lips and rise, motes leak away, a bead drips from its tip.",
  tags: ["arcane", "void", "crack", "rift", "leak", "seep", "ambient", "loop"],
  size: [80, 100],
  origin: [40, 94],
  duration: 3,
  loop: true,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#07040d", "#1a1229", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff"],
    height: 46, // the crack's length in px
    width: 4.5, // its widest half-opening in px
    seed: 3,
  },
};

const D = meta.duration;

// The crack's spine: zigzag nodes from the top tip down to the bottom tip, fixed for a seed.
function spine(o) {
  // uneven steps and offsets, so it reads as a fracture, not a saw blade: some long straight runs, some sharp kinks
  const R = rng(o.seed, 1), top = -(o.height + 14), bot = -14, pts = [[0, top]];
  let y = top, x = 0, side = 1;
  while (y < bot - 4) {
    y += R.range(2.5, 7.5);
    side = R.next() < 0.7 ? -side : side;
    x = clamp(x + side * R.range(0.5, 3.2), -4, 4);
    pts.push([x, Math.min(y, bot - 3)]);
  }
  pts.push([x * 0.4, bot]);
  return pts;
}

// x of the spine at height y (linear between nodes)
function spineX(pts, y) {
  for (let i = 1; i < pts.length; i++) {
    const [x0, y0] = pts[i - 1], [x1, y1] = pts[i];
    if (y <= y1) return x0 + (x1 - x0) * ((y - y0) / (y1 - y0 || 1));
  }
  return pts[pts.length - 1][0];
}

export function draw(p, t, o) {
  const C = fit(o.palette, 8), ph = t / D, S = o.seed, pts = spine(o);
  const top = pts[0][1], bot = pts[pts.length - 1][1], L = bot - top;
  // half-opening at u (0 top tip .. 1 bottom tip): a lens, breathing slowly
  const half = (u) => o.width * Math.pow(Math.sin(Math.PI * u), 0.85) * (1 + 0.14 * Math.sin(ph * TAU * 2 + u * 5));
  const tip = [pts[pts.length - 1][0], bot];

  // light: a cold pool round the crack, a line of light along it, a faint pool on the floor below
  p.glow(0, (top + bot) / 2, 34, C[3], 0.16);
  for (let k = 0; k <= 4; k++) { const y = top + L * (0.1 + 0.2 * k); p.glow(spineX(pts, y), y, 10, C[4], 0.12); }
  p.glow(tip[0], 0, 16, C[3], 0.12);

  // hairline fractures off the lips, fixed, a little light at their roots
  const R = rng(S, 2);
  for (let i = 0; i < 6; i++) {
    const u = R.range(0.15, 0.85), y = top + L * u, side = i % 2 ? 1 : -1, x = spineX(pts, y) + side * (half(u) + 1);
    const len = R.range(3, 8), a = side > 0 ? R.range(-0.9, 0.3) : Math.PI + R.range(-0.3, 0.9);
    const x1 = x + Math.cos(a) * len, y1 = y + Math.sin(a) * len * 0.8;
    p.line(x, y, x1, y1, (s) => (s < 0.5 ? C[5] : C[3]));
  }

  // the crack itself, row by row: black inside, slow violet streaks rising through the dark, a hard rim
  for (let y = Math.ceil(top); y <= Math.floor(bot); y++) {
    const u = (y - top) / L, cx = spineX(pts, y), hw = half(u);
    const x0 = Math.round(cx - hw), x1 = Math.round(cx + hw);
    for (let x = x0; x <= x1; x++) {
      const streak = Math.sin(y * 0.55 + ph * TAU * 3 + x * 1.7) + Math.sin(y * 0.21 - ph * TAU * 2 + x);
      p.px(x, y, streak > 1.35 ? C[2] : streak > 0.9 ? C[1] : C[0]);
    }
    // the rim: brighter on the left (the key light), pulses of white running up both lips and out
    const pulse = (k) => Math.pow(Math.max(0, Math.sin(u * 9 + ph * TAU * 3 + k)), 6);
    const lp = pulse(0), rp = pulse(1.9);
    p.px(x0 - 1, y, lp > 0.6 ? C[7] : lp > 0.2 ? C[6] : C[5]);
    p.px(x1 + 1, y, rp > 0.6 ? C[7] : rp > 0.2 ? C[5] : C[4]);
    if (lp > 0.2) p.light(x0 - 1, y, C[6], 0.35 * lp);
    if (rp > 0.2) p.light(x1 + 1, y, C[6], 0.3 * rp);
  }
  // the tips: one pale pixel each, a hairline beyond
  p.px(pts[0][0], top - 1, C[5]); p.px(pts[0][0], top - 2, C[3]);
  p.px(tip[0], bot + 1, C[5]);

  // void seeping out: wisps ooze from the lips, push out a little, then rise and swell, lit on top, thinning
  cycle(t, D, 28, S + 10, (g) => g.range(2, 3), (age, life, r) => {
    const u = r.range(0.12, 0.88), side = r.next() < 0.5 ? -1 : 1, y0 = top + L * u, x0 = spineX(pts, y0) + side * (half(u) + 1);
    // a short push out of the lip, then buoyancy takes over and the wisp climbs, leaning away from the crack
    const [dx, dy] = fly(0, 0, side * r.range(6, 11), r.range(-3, 1), age, -r.range(14, 24), 1.8);
    const sway = Math.sin(age * r.range(1.6, 2.8) + r.range(0, TAU)) * 1.6 * age;
    const k = age / life, a = Math.min(1, age * 4) * (1 - k) * (1 - k);
    const x = x0 + dx + sway + side * age * 1.5, y = y0 + dy, s = 0.8 + age * r.range(0.7, 1.2);
    p.disc(x, y, s, C[4], 0.65 * a);
    p.disc(x - 0.4, y - s * 0.4, s * 0.55, C[6], 0.55 * a);
    // a thin tail back toward where it came from
    p.line(x, y + s, x - side * 1.5, y + s + 3 + age * 2, C[3], 0.45 * a);
    p.glow(x, y, s + 3, C[4], 0.1 * a);
  });

  // vapour welling over the lips all along the crack and streaming up them, so it leaks everywhere at once
  cycle(t, D, 40, S + 30, (g) => g.range(0.5, 1.1), (age, life, r) => {
    const u = r.range(0.08, 0.95), side = r.next() < 0.5 ? -1 : 1, y0 = top + L * u, x0 = spineX(pts, y0) + side * (half(u) + 1.5);
    const k = age / life, y = y0 - age * r.range(10, 18), x = spineX(pts, y) + side * (half(clamp((y - top) / L, 0, 1)) + 1.5 + age * 3);
    const a = Math.sin(Math.PI * k);
    p.disc(x, y, 1 + k, C[5], 0.35 * a);
  });

  // motes leaking off the lips and drifting away, twinkling
  cycle(t, D, 22, S + 20, (g) => g.range(1, 2), (age, life, r) => {
    const u = r.range(0.1, 0.9), side = r.next() < 0.5 ? -1 : 1, y0 = top + L * u, x0 = spineX(pts, y0) + side * (half(u) + 1);
    const [dx, dy] = fly(0, 0, side * r.range(10, 24), r.range(-6, 4), age, -r.range(6, 14), 1.1);
    const k = 1 - age / life, tw = 0.5 + 0.5 * Math.sin(age * 13 + r.range(0, TAU));
    if (k * (0.4 + 0.6 * tw) < 0.22) return;
    const x = x0 + dx + Math.sin(age * 3 + u * 9) * 1.5, y = y0 + dy;
    p.px(x, y, tw > 0.75 && k > 0.5 ? C[7] : k > 0.5 ? C[6] : C[4]);
    p.glow(x, y, 4, C[4], 0.25 * k);
  });

  // a bead of void gathers at the lower tip, stretches, drops and splashes (once a loop)
  const swell = 1.7, fall = 0.28, g = 2 * 13 / (fall * fall); // it falls the 13 px to the floor in `fall` seconds
  if (t < swell) {
    const k = t / swell, r = 0.6 + 1.1 * k, y = bot + 1 + r + k * 1.5;
    p.line(tip[0], bot + 1, tip[0], y - r, C[3]);
    p.disc(tip[0], y, r, C[1], 1);
    p.px(tip[0] - Math.round(r * 0.5), y - Math.round(r * 0.5), C[6]);
    p.glow(tip[0], y, 4, C[4], 0.2 * k);
  } else if (t < swell + fall) {
    const a = t - swell, y = bot + 3.5 + 0.5 * g * a * a;
    p.disc(tip[0], Math.min(y, -1), 1.4, C[1], 1);
    p.px(tip[0], Math.min(y, -1) - 1, C[5]);
    p.line(tip[0], Math.min(y, -1) - 2, tip[0], Math.min(y, -1) - 4, C[3]);
  } else {
    const a = t - swell - fall, k = clamp(1 - a / (D - swell - fall), 0, 1);
    if (a < 0.6) {
      const sp = a / 0.6;
      for (const s of [-1, 1]) {
        const [sx, sy] = fly(0, 0, s * 26, -30, a, 160);
        if (sy < 0.5) p.px(tip[0] + sx, sy - 0.5, sp < 0.5 ? C[6] : C[4]);
      }
      p.light(tip[0], -1, C[5], 0.4 * (1 - sp));
    }
    // the splash spreads into a thin dark pool on the floor that fades
    const w = 2 + 6 * Math.min(1, a / 0.4);
    p.ellipse(tip[0], 0, w * k + 0.5, 1, C[2], k > 0.5 ? 1 : k);
    if (k > 0.3) p.line(tip[0] - w * k * 0.6, -1, tip[0] - w * k * 0.2, -1, C[4], k);
  }
}
