// Voidwalker's tapped void ball bursting. Written by Claude (claude-opus-5-5) for the Voidwalker hub, 2026-11-19.
// The ring of soft, top-lit puffs thrown out and braked by drag follows pixelfx's spawn/poof, whose puffs follow
// Sprite Fusion's open-source explosion (https://www.spritefusion.com/pixel-art-vfx/explosion), MIT License,
// Copyright (c) 2026 Hugo Duprez. Source of truth: tools/art/fx/arcane/void-pop.js (tools/art/bake_void_fx.py copies it in).
import { fit, TAU, burst, fly } from "../../core/index.js";

export const meta = {
  name: "Void pop",
  desc: "A void ball bursts: a white flash, a ring of tiny violet puffs flung out and braked, lit on top as they swell and thin, pale motes spinning off.",
  tags: ["arcane", "void", "pop", "burst", "puff", "one-shot"],
  size: [44, 44],
  origin: [22, 22],
  duration: 0.75,
  loop: false,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#07040d", "#1a1229", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff"],
    seed: 5,
  },
};

const D = meta.duration, DRAG = 7;

export function draw(p, t, o) {
  const C = fit(o.palette, 8), S = o.seed;
  // the pop: two white frames, then a ring of light that opens and fades
  if (t < 0.067) { p.circle(0, 0, 3.5, C[7]); p.glow(0, 0, 10, C[6], 0.6); }
  else if (t < 0.2) { const k = (t - 0.067) / 0.133; p.ring(0, 0, 3 + k * 6, 1, C[6], 1 - k); }
  // puffs: a ring of soft balls thrown out fast and braked hard, swelling as they slow, thinning out
  burst(7, S, (r, i) => {
    const a = (i / 7) * TAU + r.range(-0.3, 0.3), sp = r.range(60, 90), life = r.range(0.45, 0.7), d = r.range(0, 0.03);
    const age = t - d; if (age < 0 || age > life) return;
    const k = age / life, [x, y] = fly(0, 0, Math.cos(a) * sp, Math.sin(a) * sp * 0.8, age, -14, DRAG);
    const rad = 1 + 1.6 * Math.min(1, age * 6), alpha = (1 - k) * (1 - k);
    p.disc(x, y, rad, C[5], 0.8 * alpha);
    p.disc(x - 0.3, y - rad * 0.4, rad * 0.55, C[7], 0.65 * alpha);
  });
  // motes spun off, twinkling out
  burst(6, S + 9, (r) => {
    const a = r.range(0, TAU), sp = r.range(40, 75), life = r.range(0.3, 0.55);
    if (t > life) return;
    const k = t / life, [x, y] = fly(0, 0, Math.cos(a) * sp, Math.sin(a) * sp, t, 20, 3);
    if (k < 0.85) p.px(x, y, k < 0.4 ? C[7] : C[5]);
  });
}
