// The chunks that tumble and the grit follow pixelfx's boulder, itself after Sprite Fusion's open-source explosion
// (https://www.spritefusion.com/pixel-art-vfx/explosion), MIT License, Copyright (c) 2026 Hugo Duprez.
import { Particles, Bits, Lights, P, B, Rng, fit, sparkle, TAU, WHITE, hex, mix } from "../../core/index.js";
import { stoneShape, drawRock } from "./_lib.js";

// Voidwalker's asteroid (or, with the metal palette, a debris slab) breaking apart in the void: nothing to land on.
export const meta = {
  name: "Rock break",
  desc: "A falling rock cracks apart in the void: a white crack of light, faceted chunks that tumble away and slow, grit, and a thin puff of dust.",
  tags: ["nature", "rock", "asteroid", "break", "debris", "space", "one-shot", "voidwalker"],
  size: [96, 116],
  origin: [48, 44],
  duration: 0.9,
  loop: false,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#07040d", "#1a1229", "#2c2520", "#45404d", "#5a4a36", "#6e6156", "#8a8296", "#a89a8a"],
    sparks: ["#fff0b0", "#ffd27a", "#e0a040", "#8a1030"],
    size: 12,
    seed: 3,
  },
};

export function create(o) {
  const S = fit(o.palette, 8), R = new Rng(o.seed), nowhere = () => 999;
  const ps = new Particles({ rng: R, ground: nowhere, sparks: o.sparks }), bits = new Bits({ ground: nowhere }), lights = new Lights();
  const DUST = mix(S[5], hex("#8a8296"), 0.4);
  lights.add(0, 0, 18, WHITE, 0.6, 0.08);
  // chunks of the rock, thrown out, spinning, slowing in the void
  for (let i = 0; i < 7; i++) {
    const a = (i / 7) * TAU + R.range(-0.3, 0.3), sp = R.range(40, 95);
    bits.add({ kind: B.CHUNK, x: Math.cos(a) * 3, y: Math.sin(a) * 3, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp, g: 0.15, drag: 2.0, bounce: 0,
      life: R.range(0.55, 0.85), a: R.range(0, TAU), va: R.range(-10, 10), s: R.range(2.4, 4.2), pts: stoneShape(R) });
  }
  // grit, and a few hot sparks where it cracked
  for (let i = 0; i < 18; i++) {
    const a = R.range(0, TAU), sp = R.range(50, 150);
    ps.spawn(P.FLAKE, Math.cos(a) * 2, Math.sin(a) * 2, Math.cos(a) * sp * 0.6, Math.sin(a) * sp * 0.6, R.range(0.3, 0.6), R.pick([S[6], S[5], S[4], S[3]]), 1); // grit drifts in the void: no floor, no fall
  }
  for (let i = 0; i < 5; i++) {
    const a = R.range(0, TAU), sp = R.range(80, 160);
    ps.spawn(P.SPARK, 0, 0, Math.cos(a) * sp, Math.sin(a) * sp, R.range(0.15, 0.3));
  }
  for (let i = 0; i < 5; i++) {
    const a = R.range(0, TAU), sp = R.range(8, 20);
    ps.spawn(P.DUST, Math.cos(a) * 3, Math.sin(a) * 3, Math.cos(a) * sp, Math.sin(a) * sp, R.range(0.4, 0.7), DUST, R.range(1.6, 2.6));
  }
  let t = 0;
  return {
    update(dt) { t += dt; ps.update(dt); bits.update(dt); lights.update(dt); },
    draw(p) {
      if (t < 0.06) sparkle(p, 0, 0, 6 * (1 - t / 0.06), WHITE, S[7]);
      ps.draw(p, "back");
      for (const b of bits.list) drawRock(p, b.x, b.y, b.s, b.a, b.pts, S, Math.min(1, b.life / 0.3));
      ps.draw(p, "front");
      lights.draw(p);
    },
  };
}
