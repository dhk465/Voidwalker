import { fit, hex, TAU } from "../../core/index.js";

// Voidwalker's glob, spat by a feeder: a wobbling drop of bile lit from the top-left, a dark outline, a short trail of
// smaller drops behind it. dir k flies (-sin a, cos a) with a = k / dirs * TAU (dir 0 down the screen).
export const meta = {
  name: "Bile glob",
  desc: "A glob of bile in flight: a wobbling drop with a lit cap and a sour glow, shedding smaller drops behind it.",
  tags: ["projectile", "glob", "acid", "poison", "spit", "directional", "loop", "voidwalker"],
  size: [28, 28],
  origin: [14, 14],
  duration: 0.4,
  loop: true,
  fps: 10,
  bg: "dark",
  options: {
    palette: ["#07040d", "#6f7a1e", "#b5c43a", "#d6e25a"],
    dir: 0,
    dirs: 8,
    seed: 1,
  },
};

export function draw(p, time, o) {
  const C = fit(o.palette, 4), a = (o.dir / o.dirs) * TAU, fx = -Math.sin(a), fy = Math.cos(a), ph = (time / meta.duration) * TAU;
  // the trail: two smaller drops falling behind
  for (const [d, r] of [[-7, 1.0], [-11, 0.6]]) {
    const x = fx * d + Math.sin(ph + d) * 0.4, y = fy * d;
    p.circle(x, y, r + 0.5, C[0]); p.circle(x, y, r, C[1]);
  }
  // the drop: stretched a little along its flight, wobbling
  const st = 1.15 + 0.12 * Math.sin(ph * 2), sq = 1 / st;
  p.shade(-6, -6, 6, 6, (x, y) => {
    const f = x * fx + y * fy, s = x * -fy + y * fx, d = Math.hypot(f / (3.4 * st), s / (3.4 * sq));
    if (d > 1.12) return 0;
    if (d > 1) return C[0]; // the outline
    const lit = x * -0.6 + y * -0.75;
    if (Math.hypot(x + 1.2, y + 1.3) < 0.8) return C[3]; // a small wet glint
    return lit > 0.6 ? C[2] : C[1]; // mostly shadow
  });
  p.glow(0, 0, 6, C[2], 0.25);
}
