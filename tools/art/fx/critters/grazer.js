import { fit, hex, clamp, TAU } from "../../core/index.js";
import { Grid } from "./_lib.js";

// Voidwalker's grazer, a Ybasog bug: three shell plates on a spine, six legs, mandibles, two eyes. Drawn at the game's
// size (about 20 px long) in any of `dirs` headings, each one rasterised afresh so turning never rotates pixels.
// dir 0 faces down the screen, turning clockwise: dir k faces (-sin a, cos a) with a = k / dirs * TAU.
export const meta = {
  name: "Grazer",
  desc: "A Ybasog grazer scuttles on six legs in a tripod gait, shell plates lit from the top-left, mandibles working; when it dives its eyes burn red.",
  tags: ["critter", "bug", "insect", "monster", "walk", "loop", "voidwalker"],
  size: [36, 36],
  origin: [18, 18],
  duration: 0.5,
  loop: true,
  fps: 24,
  bg: "dark",
  options: {
    palette: ["#1a0a20", "#3a1746", "#5d2e6e", "#9b55b0", "#d8c8ff"], // shell, dark to bright
    bile: ["#6f7a1e", "#b5c43a", "#d6e25a"],
    eye: ["#8a1030", "#ff4466"], // the dive's red
    dir: 0,
    dirs: 16,
    dive: 0,
    seed: 1,
  },
};

const D = meta.duration, G = new Grid(-17, -17, 34, 34), INK = hex("#07040d");
const LX = -0.55, LY = -0.65, LZ = 0.53;
const PLATES = [[-4.6, 3.6, 3.4], [0, 4.6, 4.0], [4.8, 3.6, 3.0]]; // v (toward the head), half-width, half-length

export function draw(p, time, o) {
  const C = fit(o.palette, 5), B = fit(o.bile, 3), E = fit(o.eye, 2), dive = !!o.dive;
  const a = (o.dir / o.dirs) * TAU, ca = Math.cos(a), sa = Math.sin(a);
  const toLocal = (x, y) => [x * ca + y * sa, -x * sa + y * ca]; // world px -> (u across, v forward)
  const toWorld = (u, v) => [u * ca - v * sa, u * sa + v * ca];
  const ph = (time / D) * TAU, g = G.clear();
  // legs first, under the shell: a tripod gait, three legs swinging while the other three push
  for (let i = -1; i <= 1; i++)
    for (const side of [-1, 1]) {
      const swing = Math.sin(ph + ((i + (side > 0 ? 1 : 0)) % 2 ? Math.PI : 0)) * 1.6;
      const hip = [side * 2.8, i * 3.2], knee = [side * 7.2, i * 3.2 + 1.6 + swing], foot = [side * 8.6, i * 3.2 + 4.6 + swing * 1.3];
      const P = [hip, knee, foot].map(([u, v]) => toWorld(u, v));
      g.line(P[0][0], P[0][1], P[1][0], P[1][1], B[0]);
      g.line(P[1][0], P[1][1], P[2][0], P[2][1], B[0]);
      g.set(P[1][0], P[1][1], B[1]); // the knee joint catches the light
    }
  // the shell: three lit domes, seams between them, a bile rim
  const plate = (u, v) => {
    for (let k = 0; k < PLATES.length; k++) {
      const [pv, ru, rv] = PLATES[k], nu = u / ru, nv = (v - pv) / rv;
      if (nu * nu + nv * nv <= 1) return [k, nu, nv];
    }
    return null;
  };
  g.shade(-12, -12, 12, 12, (x, y) => {
    const [u, v] = toLocal(x, y), hit = plate(u, v);
    if (!hit) return 0;
    const [k, nu, nv] = hit, [wx, wy] = toWorld(nu, nv), nz = Math.sqrt(clamp(1 - nu * nu - nv * nv, 0.05, 1));
    const lam = (wx * LX + wy * LY + nz * LZ) / Math.hypot(wx, wy, nz);
    if (nv > 0.78 && k < 2) return C[0]; // the seam where one plate tucks under the next
    if (k === 1 && Math.hypot(wx + 0.4, wy + 0.5) < 0.22) return C[4]; // a wet glint on the big plate
    return lam > 0.7 ? C[3] : lam > 0.3 ? C[2] : C[1];
  });
  g.shade(-12, -12, 12, 12, (x, y) => { // the rim
    const [u, v] = toLocal(x, y);
    if (!plate(u, v)) return 0;
    let edge = false;
    for (const [dx, dy] of [[1, 0], [-1, 0], [0, 1], [0, -1]]) { const [u2, v2] = toLocal(x + dx, y + dy); if (!plate(u2, v2)) edge = true; }
    if (!edge) return 0;
    const s = x * LX + y * LY;
    return s > 1.5 ? B[2] : s > -2.5 ? B[1] : B[0];
  });
  // the head: a dark knob, mandibles that open and close as it walks, two eyes
  const [hx, hy] = toWorld(0, 8.6);
  g.shade(hx - 4, hy - 4, hx + 4, hy + 4, (x, y) => { // a lit knob of a head
    const [u, v] = toLocal(x, y), nu = u / 3.0, nv = (v - 8.6) / 2.6;
    if (nu * nu + nv * nv > 1) return 0;
    const [wx, wy] = toWorld(nu, nv), nz = Math.sqrt(clamp(1 - nu * nu - nv * nv, 0.05, 1));
    const lam = (wx * LX + wy * LY + nz * LZ) / Math.hypot(wx, wy, nz);
    return lam > 0.55 ? C[2] : C[1];
  });
  const open = 0.6 + 0.6 * Math.sin(ph * 2); // the mandibles work as it walks
  for (const side of [-1, 1]) {
    const P = [[side * 1.2, 10.8], [side * (2.2 + open), 12.0], [side * 1.0, 13.2]].map(([u, v]) => toWorld(u, v));
    g.line(P[0][0], P[0][1], P[1][0], P[1][1], B[0]);
    g.set(P[2][0], P[2][1], B[1]); // only the tips catch the light
  }
  const eyes = [toWorld(-1.3, 9.4), toWorld(1.3, 9.4)];
  for (const [ex, ey] of eyes) g.set(ex, ey, dive ? E[1] : B[2]);
  g.outline(INK);
  g.draw(p);
  // light: the eyes glow, red and hot when it dives
  for (const [ex, ey] of eyes) p.glow(ex, ey, dive ? 2.5 : 2, dive ? E[1] : B[2], dive ? 0.3 : 0.15); // tight, so the two eyes stay two
}
