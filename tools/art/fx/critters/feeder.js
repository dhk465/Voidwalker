import { fit, hex, clamp, TAU } from "../../core/index.js";
import { Grid } from "./_lib.js";

// Voidwalker's feeder, a Ybasog that hovers and spits: a violet bulb hanging under five swaying tendrils, bile sacs either
// side, a toothed maw that breathes. With gape it stretches its maw wide, bile light welling inside, before a spit.
// Drawn at the game's size (about 22 px across); the origin is the bulb's centre.
export const meta = {
  name: "Feeder",
  desc: "A Ybasog feeder hovers on five swaying tendrils: a violet bulb with bile sacs, a toothed maw that breathes. With gape it stretches wide, bile welling inside.",
  tags: ["critter", "monster", "float", "maw", "horror", "loop", "voidwalker"],
  size: [44, 52],
  origin: [22, 32],
  duration: 1.6,
  loop: true,
  fps: 24,
  bg: "dark",
  options: {
    palette: ["#1a0a20", "#3a1746", "#5d2e6e", "#9b55b0", "#d8c8ff"],
    bile: ["#6f7a1e", "#b5c43a", "#d6e25a"],
    teeth: "#e8e6c0",
    gape: 0,
    seed: 1,
  },
};

const D = meta.duration, G = new Grid(-21, -31, 42, 50), INK = hex("#07040d");
const LX = -0.55, LY = -0.65, LZ = 0.53, R = 10.5;

export function draw(p, time, o) {
  const C = fit(o.palette, 5), B = fit(o.bile, 3), T = hex(o.teeth), gape = clamp(o.gape), ph = (time / D) * TAU;
  const g = G.clear();
  // tendrils: five tubes rising from the crown, each swaying on its own phase, thinning to a tip
  for (let i = -2; i <= 2; i++) {
    const pts = [];
    for (let k = 0; k <= 10; k++) { // the tips swing most, and curl
      const q = k / 10, sw = Math.sin(ph + i * 1.3 + q * 2.6) * 4.2 * q ** 1.4, curl = q > 0.8 ? (q - 0.8) * 10 * Math.sign(i || 1) : 0;
      pts.push([i * 3.2 * (1 - 0.2 * q) + sw + curl, -7 - q * 16 + (q > 0.8 ? (q - 0.8) * 4 : 0)]);
    }
    for (let k = 1; k < pts.length; k++) {
      const c = k > 7 ? C[3] : k > 3 ? C[2] : C[1]; // darker at the root, catching light toward the tip
      g.line(pts[k - 1][0], pts[k - 1][1], pts[k][0], pts[k][1], c);
      if (k < 3) g.line(pts[k - 1][0] + 1, pts[k - 1][1], pts[k][0] + 1, pts[k][1], C[0]); // a thicker, shaded root
    }
  }
  // bile sacs, swelling a little out of step with the body
  const sw = 1 + 0.08 * Math.sin(ph * 2 + 1);
  for (const side of [-1, 1]) {
    const cx = side * (R + 0.5), cy = 1.2;
    g.shade(cx - 4, cy - 6, cx + 4, cy + 6, (x, y) => {
      const nx = (x - cx) / (2.7 * sw), ny = (y - cy) / (4.0 * sw);
      if (nx * nx + ny * ny > 1) return 0;
      const nz = Math.sqrt(clamp(1 - nx * nx - ny * ny, 0.05, 1)), lam = (nx * LX + ny * LY + nz * LZ) / Math.hypot(nx, ny, nz);
      return lam > 0.9 ? B[2] : lam > 0.66 ? B[1] : B[0]; // mostly shadow, a small lit cap
    });
  }
  // the bulb: a wobbling dome lit from the top-left, a wet glint
  const inBody = (x, y) => {
    const a = Math.atan2(y, x), r = R * (1 + 0.045 * Math.sin(a * 4 + ph) + 0.03 * Math.sin(a * 3 - ph * 2));
    return Math.hypot(x, y) <= r;
  };
  g.shade(-R - 2, -R - 2, R + 2, R + 2, (x, y) => {
    if (!inBody(x, y)) return 0;
    const nx = x / R, ny = y / R, nz = Math.sqrt(clamp(1 - nx * nx - ny * ny, 0.05, 1));
    const lam = (nx * LX + ny * LY + nz * LZ) / Math.hypot(nx, ny, nz);
    if (Math.hypot(nx + 0.42, ny + 0.52) < 0.15) return C[4];
    return lam > 0.72 ? C[3] : lam > 0.35 ? C[2] : lam > 0.1 ? C[1] : C[0];
  });
  // the bile rim on the lit side
  g.shade(-R - 2, -R - 2, R + 2, R + 2, (x, y) => {
    if (!inBody(x, y)) return 0;
    let edge = false;
    for (const [dx, dy] of [[1, 0], [-1, 0], [0, 1], [0, -1]]) if (!inBody(x + dx, y + dy)) edge = true;
    if (!edge) return 0;
    const s = x * LX + y * LY;
    return s > 2 ? B[2] : s > -3 ? B[1] : 0;
  });
  // the maw: breathing open and shut, or stretched wide before a spit; a ring of teeth round the dark
  const mw = gape ? 6.4 : 4.6, mh = gape ? 5.6 : 3.2 + 0.9 * Math.sin(ph * 2), my = 3.4;
  g.shade(-mw - 1, my - mh - 1, mw + 1, my + mh + 1, (x, y) => {
    const nx = x / mw, ny = (y - my) / mh, d = nx * nx + ny * ny;
    if (d > 1) return 0;
    if (gape && d < 0.12) return B[2]; // bile welling up its throat: a small hot core
    if (gape && d < 0.3) return B[0];
    const a = Math.atan2(ny, nx);
    return d > 0.5 && Math.cos(a * 7) > 0.3 ? T : C[0];
  });
  g.outline(INK);
  g.draw(p);
  // light: the throat glows when it gapes; a faint sour pool round the sacs
  if (gape) p.glow(0, my, 4, B[1], 0.35);
  for (const side of [-1, 1]) p.glow(side * (R + 0.5), 1.2, 4, B[1], 0.12);
}
