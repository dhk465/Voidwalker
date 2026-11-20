// The burst into tumbling pieces follows pixelfx's egg hatch, itself after Sprite Fusion's open-source ice shatter
// (https://www.spritefusion.com/pixel-art-vfx/ice), MIT License, Copyright (c) 2026 Hugo Duprez.
import { Particles, Bits, Lights, P, B, Rng, fit, sparkle, TAU, WHITE, hex } from "../../core/index.js";

// Voidwalker's pod bursting in the rift, when it hatches or is shot open: centred on the pod, nothing to land on.
export const meta = {
  name: "Pod burst",
  desc: "A Ybasog egg sac bursts: a sour flash, torn flaps of violet membrane tumble away, bile goo sprays and drips, and a sickly vapour swells and thins.",
  tags: ["spawn", "death", "egg", "pod", "burst", "gore", "organic", "one-shot", "voidwalker"],
  size: [88, 104],
  origin: [44, 40],
  duration: 1.0,
  loop: false,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#1a0a20", "#3a1746", "#5d2e6e", "#9b55b0", "#d8c8ff"], // the membrane
    bile: ["#6f7a1e", "#b5c43a", "#d6e25a"],
    seed: 7,
  },
};

export function create(o) {
  const C = fit(o.palette, 5), G = fit(o.bile, 3), R = new Rng(o.seed), nowhere = () => 999; // in the void: nothing to land on
  const ps = new Particles({ rng: R, ground: nowhere, sparks: [G[2], G[1], G[1], G[0]], haze: G[0] }); // goo, not fire: no white
  const bits = new Bits({ ground: nowhere }), lights = new Lights();
  // the flash and the light that pours out
  lights.add(0, 0, 22, G[1], 0.8, 0.35);
  lights.add(0, 0, 8, G[2], 0.8, 0.08);
  // torn membrane: curved flaps of skin that tumble away (their width swings as they turn), slowing in the void and
  // sinking a little, lit on the face toward the light with the bile rim still on their outer edge
  const flaps = [];
  for (let i = 0; i < 5; i++) {
    const a = (i / 5) * TAU + R.range(-0.35, 0.35), sp = R.range(30, 70);
    flaps.push({ x: Math.cos(a) * 4, y: Math.sin(a) * 5, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp, a: a + Math.PI / 2, va: R.range(-7, 7),
      tw: R.range(0, TAU), vt: R.range(8, 14), w: R.range(3.2, 4.4), h: R.range(2, 2.8), life: R.range(0.5, 0.75) });
  }
  // the grazer's yolk-sac shreds: small dark chunks
  for (let i = 0; i < 4; i++) {
    const a = R.range(0, TAU), sp = R.range(20, 50);
    bits.add({ kind: B.CHUNK, x: 0, y: 1, vx: Math.cos(a) * sp, vy: Math.sin(a) * sp, g: 0.3, drag: 2.5, bounce: 0, life: R.range(0.4, 0.7), a: 0, va: R.range(-6, 6), s: 1, c: C[0] });
  }
  // bile goo: a spray of bright drops, a few slow drips that sink
  for (let i = 0; i < 12; i++) {
    const a = R.range(0, TAU), sp = R.range(40, 110);
    ps.spawn(P.SPARK, Math.cos(a) * 2, Math.sin(a) * 2, Math.cos(a) * sp, Math.sin(a) * sp, R.range(0.25, 0.5));
  }
  for (let i = 0; i < 4; i++)
    bits.add({ kind: B.DRIP, x: R.range(-4, 4), y: R.range(-2, 4), vx: R.range(-15, 15), vy: R.range(-10, 20), g: 0.35, drag: 1.2, bounce: 0, life: R.range(0.5, 0.8), a: 0, va: 0, s: 1, c: G[1] });
  // the sour vapour that was inside, swelling and thinning
  for (let i = 0; i < 7; i++) { // one cloud: several thin puffs, spread round the burst
    const a = (i / 7) * TAU + R.range(-0.3, 0.3), sp = R.range(10, 24);
    ps.spawn(P.VAPOUR, Math.cos(a) * 3, Math.sin(a) * 3, Math.cos(a) * sp, Math.sin(a) * sp - 3, R.range(0.45, 0.75), C[2], R.range(1.2, 2.0), R.range(0, 0.05));
  }
  let t = 0;
  return {
    update(dt) {
      t += dt; ps.update(dt); bits.update(dt); lights.update(dt);
      for (const f of flaps) {
        const k = Math.exp(-2.4 * dt);
        f.vx *= k; f.vy = f.vy * k + 60 * dt; f.x += f.vx * dt; f.y += f.vy * dt; f.a += f.va * dt; f.tw += f.vt * dt;
      }
    },
    draw(p) {
      if (t < 0.07) sparkle(p, 0, 0, 5 * (1 - t / 0.07), G[2], G[1]); // the hero frame, sour rather than white
      ps.draw(p, "back");
      bits.draw(p);
      for (const f of flaps) { // a flap: a curved patch of skin, narrower as it turns edge-on, fading as it goes
        const life = 1 - t / f.life;
        if (life <= 0) continue;
        const ca = Math.cos(f.a), sa = Math.sin(f.a), w = f.w * Math.max(0.25, Math.abs(Math.cos(f.tw))) * Math.min(1, life * 2.5), h = f.h;
        const P2 = (u, v) => [f.x + u * ca - v * sa, f.y + u * sa + v * ca];
        const pts = [P2(-w, 0), P2(-w * 0.5, -h), P2(w * 0.5, -h), P2(w, 0), P2(0, h * 0.45)];
        const lit = Math.cos(f.tw) > 0;
        p.poly(pts, lit ? C[3] : C[2], life > 0.3 ? 1 : life / 0.3);
        p.line(pts[1][0], pts[1][1], pts[2][0], pts[2][1], lit ? G[1] : G[0], life > 0.3 ? 1 : life / 0.3); // the rim, on the torn flap's outer edge
      }
      ps.draw(p, "front");
      lights.draw(p);
    },
  };
}
