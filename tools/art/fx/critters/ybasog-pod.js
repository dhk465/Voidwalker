import { fit, hex, mix, clamp, lerp, ease, TAU } from "../../core/index.js";
import { Grid, shift } from "./_lib.js";

// Voidwalker's pod: a Ybasog egg sac drifting down the rift (the game moves it; the effect stays on its centre).
// Drawn at the game's own size, about 16 x 19 px, in the browser build's colours: violet flesh, a bile rim.
export const meta = {
  name: "Ybasog pod",
  desc: "A Ybasog egg sac breathes on a double heartbeat while the grazer curled inside kicks and its eye glints. With crack, it shudders and splits along seams of bile light.",
  tags: ["critter", "egg", "pod", "monster", "organic", "horror", "loop", "voidwalker"],
  size: [40, 48],
  origin: [20, 22],
  duration: 2.4,
  loop: true,
  fps: 30,
  bg: "dark",
  options: {
    palette: ["#1a0a20", "#3a1746", "#5d2e6e", "#9b55b0", "#d8c8ff"], // the membrane, dark to bright
    bile: ["#6f7a1e", "#b5c43a", "#d6e25a"],
    crack: 0, // 0 a calm pod; 1 a pod about to hatch
    seed: 1,
  },
};

const D = meta.duration, G = new Grid(-12, -13, 24, 26);
const RX = 7.4, RY = 9.0, INK = hex("#07040d");
const LX = -0.55, LY = -0.65, LZ = 0.53; // the light, from the top-left

// a double heartbeat twice a loop: thump-thump, rest
const beat = (t) => {
  const u = (t % (D / 2)) / (D / 2);
  return Math.exp(-(((u - 0.04) / 0.035) ** 2)) + 0.6 * Math.exp(-(((u - 0.16) / 0.04) ** 2));
};
// the grazer inside kicks once a loop, a sharp jab that eases back
const kick = (t) => {
  const u = t - 1.5;
  return u < 0 || u > 0.5 ? 0 : u < 0.06 ? ease.outQuad(u / 0.06) : 1 - ease.inOutQuad((u - 0.06) / 0.44);
};

export function draw(p, time, o) {
  const t = shift(time, D, o.seed), C = fit(o.palette, 5), B = fit(o.bile, 3), crack = clamp(o.crack);
  const swell = 1 + 0.045 * beat(t), k = kick(t);
  const sx = crack ? Math.round(Math.sin(t * TAU * 7) * 0.9 * crack) : 0; // a pod about to hatch shudders
  const ka = -0.9, kx = Math.cos(ka) * RX, ky = Math.sin(ka) * RY; // where the kick pushes the membrane out (upper right)
  // the outline of the sac: an egg, narrower at the top, a little lumpy, bulging where the grazer kicks
  const inside = (x, y) => {
    const ry = RY * swell, rx = RX * swell * (1 + 0.1 * (y / ry));
    const a = Math.atan2(y, x), lump = 1 + 0.05 * Math.sin(a * 3 + 0.7) + 0.035 * Math.sin(a * 5 - 1.1);
    let d = (x / (rx * lump)) ** 2 + (y / (ry * lump)) ** 2;
    d -= 0.55 * k * Math.exp(-((x - kx * 0.8) ** 2 + (y - ky * 0.8) ** 2) / 10);
    return d;
  };
  const g = G.clear();
  // the embryo: a grazer curled tight, head tucked at the top, its segmented tail wrapped round under it like a 6;
  // it turns a little with the loop and jerks when it kicks
  const rot = 0.25 * Math.sin((t / D) * TAU) + 0.45 * k, ex = 0.2, ey = 1.6;
  const hx = ex + Math.cos(rot + 2.6) * 2.0, hy = ey + Math.sin(rot + 2.6) * 2.0; // the head, low on the left, seen side-on
  const spine = [];
  for (let i = 0; i <= 10; i++) { // the tail spirals in from behind the head, thinning
    const s = i / 10, a = rot + 2.6 + 0.6 + s * 4.0, r = 3.6 - 1.8 * s; // round under, up the far side and over
    spine.push([ex + Math.cos(a) * r, ey + Math.sin(a) * r, 1.45 - 0.8 * s, s]);
  }
  const body = (x, y) => {
    if (Math.hypot((x - hx) / 1.15, y - hy) < 2.1) return 2; // head, a little longer than tall
    for (const [sx2, sy2, r, q] of spine) if (Math.hypot(x - sx2, y - sy2) < r) return Math.sin(q * 26) > 0.6 ? 1 : 3; // plates and seams
    return 0;
  };
  g.shade(-RX - 3, -RY - 3, RX + 3, RY + 3, (x, y) => {
    const d = inside(x, y);
    if (d > 1) return 0;
    // a translucent sac: mostly shadow, lit in a crescent toward the light, one small wet glint
    const nx = x / (RX * swell), ny = y / (RY * swell), nz = Math.sqrt(clamp(1 - nx * nx - ny * ny, 0.04, 1));
    const lam = (nx * LX + ny * LY + nz * LZ) / Math.hypot(nx, ny, nz);
    const rim = d > 0.55; // the membrane is thicker, and catches more light, toward its edge
    let c = lam > 0.8 && rim ? C[3] : lam > 0.55 ? C[2] : lam > 0.15 ? C[1] : C[0];
    // the fluid in its heart glows with the yolk's light, so whatever is curled inside shows against it
    const fl = Math.hypot((x - ex) / 5.4, (y - ey) / 5.8);
    if (fl < 1) c = c === C[0] || c === C[1] ? C[2] : c;
    if (Math.hypot(nx + 0.45, ny + 0.55) < 0.17) c = C[4]; // the wet glint
    const e = body(x, y);
    if (e) c = e === 1 ? C[1] : C[0]; // the curled grazer, a dark shape against the lit fluid (seams a shade lighter)
    return c;
  });
  // its eyes catch the yolk's light on each heartbeat
  const eyeC = beat(t) > 0.35 ? B[2] : B[1];
  g.set(hx - 0.9, hy - 0.4, eyeC); // one eye, the head seen side-on
  // veins: two dark threads under the lit side of the skin
  const veins = [[[-6.8, -3.5], [-5.6, -1.6], [-5.8, 1.2]], [[-2.6, -8.4], [-3.4, -6.6], [-5.2, -5.4]]];
  for (const pts of veins)
    for (let i = 1; i < pts.length; i++)
      g.line(pts[i - 1][0], pts[i - 1][1], pts[i][0], pts[i][1], C[1]); // dark threads under the lit skin
  // the cracks of a pod about to hatch: jagged seams of bile light from the crown down
  const seams = [];
  if (crack) {
    const len = 0.45 + 0.55 * crack;
    const paths = [[[0.2, -6.6], [-1.2, -5.0], [-0.4, -3.4], [-2.4, -2.0], [-2.8, 0.2], [-4.4, 1.8]], [[0.2, -6.6], [1.8, -5.2], [1.2, -3.6], [3.0, -2.6], [4.2, -0.4]]];
    for (const pts of paths) {
      const n = Math.max(2, Math.round(pts.length * len));
      for (let i = 1; i < n; i++) {
        g.line(pts[i - 1][0] + 0.9, pts[i - 1][1] + 0.9, pts[i][0] + 0.9, pts[i][1] + 0.9, C[0]); // the lip below the seam, in shadow
        g.line(pts[i - 1][0], pts[i - 1][1], pts[i][0], pts[i][1], B[2]); // the seam, light inside showing through
      }
      seams.push(pts.slice(0, n));
    }
  }
  // the bile rim, lit top-left, sour in shadow, then the dark outline
  g.shade(-RX - 3, -RY - 3, RX + 3, RY + 3, (x, y) => {
    if (inside(x, y) > 1) return 0;
    let edge = false;
    for (const [dx, dy] of [[1, 0], [-1, 0], [0, 1], [0, -1]]) if (inside(x + dx, y + dy) > 1) edge = true;
    if (!edge) return 0;
    const s = x * LX + y * LY; // + toward the light
    return s > 3.0 ? B[2] : s > -0.5 ? B[1] : s > -4.5 ? B[0] : C[1]; // bile where the light falls, gone in the deepest shadow
  });
  // a thread of mucus from the base, a drop swelling on its end that falls once a loop
  const u = t / D, tip = RY * swell + 2 + 2.5 * Math.sin(u * TAU);
  const sway = Math.sin(u * TAU * 2) * 0.8;
  g.line(0.5, RY * swell, 0.5 + sway, tip, (q) => (q < 0.5 ? B[1] : B[0]));
  const grow = clamp(u / 0.7), fall = clamp((u - 0.7) / 0.25);
  if (u < 0.7) { if (grow > 0.35) g.set(0.5 + sway, tip + 1, B[1]); if (grow > 0.75) g.set(0.5 + sway, tip + 2, B[2]); }
  else if (u < 0.95) g.set(0.5 + sway, tip + 1 + 10 * fall * fall, B[1]);
  g.outline(INK);
  g.draw(p, 1, sx, 0);
  // light: the yolk throbs on the heartbeat, the seams leak (the kick shows in the bulge alone)
  p.glow(sx + ex, ey, 4, B[1], 0.1 + 0.3 * beat(t)); // a tight sour light in the yolk, round the curled body
  p.glow(sx, 0, 14, C[2], 0.07);
  for (const pts of seams) for (const [x, y] of pts) p.glow(sx + x, y, 2.5, B[2], 0.12 * crack);
}
