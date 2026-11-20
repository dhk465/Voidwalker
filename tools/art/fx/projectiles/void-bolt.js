import { fit, hex, TAU } from "../../core/index.js";

// Voidwalker's bolt, fired from a card: a diamond head pointing where it flies, a white-hot heart, a short tapering
// trail that shimmers. Baked in three grey steps (rim, body, heart) so the game paints it in each card's colour:
// step 1 a dark of the colour, step 2 the colour, step 3 white. `rings` adds a ring per nature on a fused card.
// dir 0 flies down the screen, turning clockwise like the grazer: dir k flies (-sin a, cos a), a = k / dirs * TAU.
export const meta = {
  name: "Void bolt",
  desc: "A card's bolt in flight: a diamond head with a white-hot heart, a short tapering trail that shimmers; drawn in grey steps for the game to colour per card.",
  tags: ["projectile", "bolt", "magic", "directional", "loop", "voidwalker"],
  size: [40, 40],
  origin: [20, 20],
  duration: 0.4,
  loop: true,
  fps: 10,
  bg: "dark",
  options: {
    palette: ["#555555", "#aaaaaa", "#ffffff"], // the three steps the game repaints: rim, body, heart
    dir: 0,
    dirs: 16,
    rings: 0,
    seed: 1,
  },
};

export function draw(p, time, o) {
  const S = fit(o.palette, 3), a = (o.dir / o.dirs) * TAU, fx = -Math.sin(a), fy = Math.cos(a), sx = -fy, sy = fx;
  const ph = (time / meta.duration) * TAU;
  const at = (f, s) => [f * fx + s * sx, f * fy + s * sy];
  // the trail: a tapering ribbon behind the head, its edges shimmering
  for (let k = 0; k <= 12; k += 0.5) {
    const w = 2.2 * (1 - k / 13) + 0.3 * Math.sin(ph + k * 1.1);
    for (let s = -w; s <= w + 0.01; s += 0.5) {
      const [x, y] = at(-3 - k, s);
      p.px(x, y, Math.abs(s) < w * 0.5 && k < 6 ? S[1] : S[0]);
    }
  }
  // the head: a diamond, rim step round body step, a heart of white
  p.shade(-7, -7, 7, 7, (x, y) => {
    const f = x * fx + y * fy, s = x * sx + y * sy;
    const inHead = (f >= 0 ? f / 6.4 : -f / 4.0) + Math.abs(s) / 3.2 <= 1;
    if (!inHead) return 0;
    const inner = (f >= 0 ? f / 4.6 : -f / 2.6) + Math.abs(s) / 2.0 <= 1;
    const heart = (f >= -0.5 ? (f + 0.5) / 3.0 : -(f + 0.5) / 1.2) + Math.abs(s) / 1.0 <= 1;
    return heart ? S[2] : inner ? S[1] : S[0];
  });
  // a fused card wears a ring per nature
  for (let i = 0; i < o.rings; i++) p.ring(0, 0, 5 + i * 2, 1.0, S[1]); // solid, one pixel wide, just outside the head
}
