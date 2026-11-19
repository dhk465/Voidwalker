// The flame tongue, puff and ember recipes come from ./_lib.js, adapted there from Sprite Fusion's open-source fire
// (https://www.spritefusion.com/pixel-art-vfx/fire), MIT License, Copyright (c) 2026 Hugo Duprez.
import { fit, clamp, rng, TAU } from "../../core/index.js";
import { flameTongue, firePuff, ember, noiseFor } from "./_lib.js";

// A void flame on its own path. Nothing is turned: the tongues lean because the flame drags them, and the tail is
// what it sheds, left in the air where it was, so the tail follows the real path, bends with every turn and rises
// as it cools. Give `path(t)` -> [x, y] to fly it anywhere; with `follow`, the drawing is centred on the flame's
// head (for a game that moves the effect itself), otherwise the path is drawn round the origin.
export const meta = {
  name: "Void trail",
  desc: "A void flame flies its own path: a dark heart, tongues dragged back by its speed, and a tail of flame shed where it has been that cools and rises as embers break off.",
  tags: ["fire", "void", "spirit", "flame", "trail", "flying", "magic", "loop"],
  size: [200, 136],
  origin: [100, 92],
  duration: 4,
  loop: true,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#07040d", "#1a0a20", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#ffffff"],
    path: null, // (t) => [x, y]; null: a figure eight round the origin, one lap per `period`
    follow: false, // true: draw relative to the head (the caller places the effect at the head)
    period: 4, // s per lap of a looping path (keeps the shed puffs seamless); 0 for a path that never repeats
    trail: 1.1, // s a shed puff lives: the tail's length in time
    rate: 60, // puffs shed per second
    size: 1, // scales the head and its tongues
    tongues: true, // false: the ball and its trail only, no flame tongues on its crown
    seed: 3,
  },
};

const lap = (t, o) => {
  const a = (t / o.period) * TAU;
  return [64 * Math.sin(a), -12 - 14 * Math.sin(2 * a)];
};
const pos = (t, o) => (o.path ? o.path(t) : lap(t, o));
const vel = (t, o) => { const a = pos(t - 0.02, o), b = pos(t + 0.02, o); return [(b[0] - a[0]) / 0.04, (b[1] - a[1]) / 0.04]; };

export function draw(p, t, o) {
  const C = fit(o.palette, 8), N0 = noiseFor(o.path ? 0 : o.duration || meta.duration);
  const [hx, hy] = pos(t, o), [vx, vy] = vel(t, o), [bx, by] = o.follow ? [hx, hy] : [0, 0];
  const X = hx - bx, Y = hy - by, s = o.size;
  // the hot-first ramp of the shed puffs: pale and bright when shed, cooling to deep violet
  const heat = [C[7], C[6], C[6], C[5], C[5], C[4], C[4], C[3], C[2]];
  const emberCols = [C[7], C[6], C[4], C[2]];

  // the tail: puffs shed along the real path, each left in the air where the head was, rising and slowing
  const step = 1 / o.rate, kNow = Math.floor(t / step), n = o.period && !o.path ? Math.round(o.period * o.rate) : 0;
  for (let k = kNow - Math.ceil(o.trail * 2.2 / step); k <= kNow; k++) {
    const born = k * step, age = t - born;
    if (age < 0) continue;
    const r = rng(o.seed * 131 + 7, n ? ((k % n) + n) % n : k), L = o.trail * r.range(0.55, 1);
    const ember_ = r.chance(0.12), life = ember_ ? L * 2 : L;
    if (age >= life) continue;
    const [hx0, hy0] = pos(born, o), [ivx, ivy] = vel(born, o);
    // a third tear off the tongue tips, swept back as the tongues were when they left
    const tip = r.chance(0.35) ? r.range(0.45, 1) : 0, px0 = hx0 + tip * clamp(-ivx * 0.11, -15, 15) * s, py0 = hy0 - tip * 22 * s;
    // it keeps a little of the flame's speed, then buoyancy lifts it and the air slows it
    const sx = ivx * 0.12 + r.range(-10, 10), sy = ivy * 0.12 - r.range(4, 14), drag = ember_ ? 1.4 : 2.6, lift = ember_ ? 34 : 58;
    const e = Math.exp(-drag * age), x = px0 + r.range(-2.5, 2.5) * s + (sx * (1 - e)) / drag - bx;
    const y = py0 + r.range(-2, 2) * s + (sy * (1 - e)) / drag - (lift * (age - (1 - e) / drag)) / drag - by;
    const k1 = 1 - age / life;
    if (ember_) { ember(p, x + Math.sin(age * 3 + r.next() * 9) * 2, y, k1, r.chance(0.3) ? 2 : 1, emberCols, age * 23 + k); continue; }
    // its little tongue points the way the air pushes it: up, and back against the flame's travel
    const tl = Math.hypot(-ivx * 0.01, -1) || 1;
    firePuff(p, x, y, k1, (3.2 + r.range(0, 2)) * s * (0.45 + 0.55 * k1), r.next(), heat, 0.12, (-ivx * 0.01) / tl, -1 / tl);
  }

  // the head: tongues licking up off its crown, dragged back by its speed and stretched as it climbs
  const N = (rate, sd, off = 0) => N0(t, rate, sd, off);
  const H = [C[1], C[2], C[3], C[4], C[5], C[6], C[6]]; // inside-out: a dark heart, a pale violet skin
  const LAY = [[1, 1, 6], [0.8, 0.9, 5], [0.6, 0.74, 3], [0.36, 0.5, 1]];
  const sweep = clamp(-vx * 0.11, -15, 15) * s, stretch = clamp(1 - vy * 0.004, 0.65, 1.45);
  // five tongues, each breathing at its own pace; the tallest in the middle, the drag bending the tall ones most
  if (o.tongues) for (const [dx, h, w, sd] of [[-5, 13, 3.6, 3], [5, 14, 3.6, 5], [-2.5, 19, 4.4, 7], [2.5, 20, 4.4, 9], [0, 27, 5.6, 11]])
    flameTongue(p, X + dx * s, Y - 3 * s, h * stretch * s * (0.72 + 0.28 * (N(3 + sd % 4, sd) * 0.5 + 0.5) + 0.12 * N(11, sd + 40)), w * s, sweep * (h / 20), N, sd, H, LAY, 0.6, 1.6);
  // the heart: a dark core in a ring of cold fire, its rim crackling
  const R = 6.5 * s;
  p.shade(X - R - 2, Y - R - 2, X + R + 2, Y + R + 2, (x, y) => {
    const dx = x - X, dy = y - Y, ang = Math.atan2(dy, dx), rr = R + 0.8 * N(14, 21, ang * 1.7 + 4);
    const d = Math.hypot(dx, dy) / rr;
    if (d > 1) return 0;
    return d < 0.45 ? C[1] : d < 0.66 ? C[3] : d < 0.86 ? C[5] : C[7];
  });
  // its light: a cold pool round it, leaning back along the tail
  p.glow(X - vx * 0.06, Y - 6 - vy * 0.04, 34 * s, C[4], 0.32);
  p.glow(X, Y, 10 * s, C[6], 0.28);
}
