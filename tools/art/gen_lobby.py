# Writes the lobby's PixelSpecs (codeart2d.pixelspec.v1): code-drawn pixel art, no image model.
# Big props are shaded by a small pixel "shader": a hard mask, a bevel from the distance to its edge, light from the
# top-left, a little value noise for stone, then exact palette steps chosen with a 4x4 ordered dither. Small details
# (runes, clasps, page lines, inlays) are placed pixel by pixel on top.
#
# Shapes are written in layout units (the browser hub's 1280x720 space) and sampled at --scale, so the same design
# renders for the browser build (--scale 1) or for the Godot build's 640x360 screen (--scale 0.5): re-shaded at the
# target size, never shrunk. Hand-placed details have a small version where the full one would not fit.
# Run: python tools/art/gen_lobby.py <out-dir> [--scale 0.5]
# Godot build: python tools/art/gen_lobby.py assets/art-src/lobby --scale 0.5 (the browser prototype keeps its own copy at scale 1)
import json, math, os, sys
import numpy as np

ARGS = [a for a in sys.argv[1:] if not a.startswith("--")]
OUT = ARGS[0] if ARGS else os.path.dirname(os.path.abspath(__file__))
S = float(sys.argv[sys.argv.index("--scale") + 1]) if "--scale" in sys.argv else 1.0
SMALL = S < 0.75
PAL = {  # every colour is in config.js window.PALETTE
    "o": "#0e0a18",  # outline
    "a": "#1a1229", "b": "#241a3a", "c": "#353a44", "d": "#45404d", "e": "#6d6488", "f": "#8a8296", "g": "#8b95a3", "h": "#cfc6e6",  # stone
    "p": "#2c2520", "q": "#5a4a36", "r": "#6e6156", "s": "#a89a8a",  # earth under the altar
    "t": "#16302e", "u": "#2e5a55", "v": "#5ac8b9", "w": "#7fd6c8",  # teal glow
    "H": "#e0a040", "I": "#ffd27a", "J": "#fff0b0",  # gold
    "V": "#4a3a7a", "W": "#7b5bdf", "X": "#b9a6ff", "Y": "#d8c8ff", "Z": "#efe6ff",  # void violet
    "P": "#e8e6c0", "L": "#3a1746", "M": "#5d2e6e", "N": "#9b55b0", "K": "#1a0a20", "E": "#3b3350", "F": "#9d92bb",
}
STONE = ["a", "b", "c", "d", "e", "f", "g", "h"]  # dark to light
COOL = ["b", "E", "d", "e", "f", "F", "h"]  # carved stone with a violet lean
EARTH = ["K", "p", "q", "r", "s"]
LEATHER = ["K", "L", "M", "N"]
BAYER = np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]) / 16.0
SHAPE = (0, 0)  # the current canvas, in pixels

def px(v): return int(round(v * S))

def canvas(w, h):
    """A blank canvas w x h layout units big; every mask below samples it at pixel centres, in layout units."""
    global SHAPE
    SHAPE = (max(3, px(h)), max(3, px(w)))
    return np.full(SHAPE, ".", object)

def grid():
    y, x = np.mgrid[0:SHAPE[0], 0:SHAPE[1]]
    return (x + 0.5) / S, (y + 0.5) / S

def poly_mask(pts):
    """Even-odd fill of a polygon (layout units) at pixel centres."""
    x, y = grid(); m = np.zeros(SHAPE, bool); n = len(pts)
    for i in range(n):
        x0, y0 = pts[i]; x1, y1 = pts[(i + 1) % n]
        if y0 == y1: continue
        cond = ((y0 <= y) & (y < y1)) | ((y1 <= y) & (y < y0))
        xc = x0 + (y - y0) * (x1 - x0) / (y1 - y0)
        m ^= cond & (x < xc)
    return m

def ellipse_mask(cx, cy, rx, ry):
    x, y = grid(); return ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1

def dist_in(m, cap=64):
    """Chamfer distance (pixels) from each inside pixel to the nearest outside pixel."""
    h, w = m.shape; INF = 10 ** 6; d = np.where(m, INF, 0).astype(np.int64)
    pad = np.full((h + 2, w + 2), 0, np.int64); pad[1:-1, 1:-1] = d
    for y in range(1, h + 1):  # forward pass
        row = pad[y]; up = pad[y - 1]
        for x in range(1, w + 1):
            if row[x]: row[x] = min(row[x], up[x] + 3, up[x - 1] + 4, up[x + 1] + 4, row[x - 1] + 3)
    for y in range(h, 0, -1):  # backward pass
        row = pad[y]; dn = pad[y + 1]
        for x in range(w, 0, -1):
            if row[x]: row[x] = min(row[x], dn[x] + 3, dn[x - 1] + 4, dn[x + 1] + 4, row[x + 1] + 3)
    return np.minimum(pad[1:-1, 1:-1] / 3.0, cap)

def dist_u(m): return dist_in(m) / S  # the same distance in layout units

def on(coord, v):
    """The one pixel row (or column) that holds layout value v."""
    return np.floor(coord * S) == np.floor(v * S)

def vnoise(scale, seed):
    """Value noise with features `scale` layout units across (kept at least 1.5 pixels)."""
    scale = max(scale, 1.5 / S)
    W, H = SHAPE[1] / S, SHAPE[0] / S
    rng = np.random.default_rng(seed); g = rng.random((int(H / scale) + 3, int(W / scale) + 3))
    x, y = grid(); fx, fy = x / scale, y / scale; ix, iy = fx.astype(int), fy.astype(int); tx, ty = fx - ix, fy - iy
    tx, ty = tx * tx * (3 - 2 * tx), ty * ty * (3 - 2 * ty)
    a, b, c, d = g[iy, ix], g[iy, ix + 1], g[iy + 1, ix], g[iy + 1, ix + 1]
    return a + (b - a) * tx + (c - a) * ty + (a - b - c + d) * tx * ty

def shade(m, ramp, bevel=4.0, tex=0.0, seed=1, base=0.55, light=(-0.6, -0.75, 0.6), tilt=None):
    """The pixel shader: bevel normals from the edge distance, lambert light, noise, dithered ramp steps."""
    dd = dist_in(m); hgt = np.clip(dd / max(1.0, bevel * S), 0, 1); hgt = hgt * hgt * (3 - 2 * hgt)
    gy, gx = np.gradient(hgt); L = np.array(light) / np.linalg.norm(light)
    nz = np.full_like(gx, 0.35); nl = np.sqrt(gx ** 2 + gy ** 2 + nz ** 2)
    lam = (-gx * L[0] - gy * L[1] + nz * L[2]) / nl  # the surface tips up toward the edge
    v = base + 0.55 * (lam - 0.45)
    if tilt is not None: v = v + tilt
    if tex: v = v + tex * (vnoise(6, seed) - 0.5) + tex * 0.5 * (vnoise(2.5, seed + 7) - 0.5)
    v = np.clip(v, 0, 0.999) * (len(ramp) - 1)
    y, x = np.mgrid[0:SHAPE[0], 0:SHAPE[1]]; i = np.floor(v).astype(int); i = i + ((v - i) > BAYER[y % 4, x % 4])
    i = np.clip(i, 0, len(ramp) - 1)
    out = np.full(SHAPE, ".", object); out[m] = np.array(ramp, object)[i[m]]
    return out

def put(img, rows, ox, oy, center=False):
    """Hand-placed pixels (native size) at a layout position; center=True centres them on it."""
    X, Y = px(ox), px(oy)
    if center: X -= len(rows[0]) // 2; Y -= len(rows) // 2
    for j, r in enumerate(rows):
        for i, ch in enumerate(r):
            if ch != "." and 0 <= Y + j < img.shape[0] and 0 <= X + i < img.shape[1]: img[Y + j, X + i] = ch

def segments(img):
    out = []
    for row in img:
        s, prev, n = [], None, 0
        for ch in list(row) + [None]:
            if ch == prev: n += 1; continue
            if prev is not None: s.append((str(n) if n > 1 else "") + prev)
            prev, n = ch, 1
        out.append("".join(s))
    return out

def clean_corners(img, rounds=6):
    """Pixel-perfect silhouette: fill each outline L-corner (the checker's own test) with its inner neighbour's colour."""
    sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", ".claude", "skills", "codeart2d", "scripts"))
    import codeart_core as cc
    for _ in range(rounds):
        m = img != "."; ring = np.zeros_like(m)
        ring[1:, :] |= m[:-1, :]; ring[:-1, :] |= m[1:, :]; ring[:, 1:] |= m[:, :-1]; ring[:, :-1] |= m[:, 1:]; ring &= ~m
        ys, xs = np.nonzero(cc._l_corner_mask(ring))
        if not len(ys): return img
        for y, x in zip(ys, xs):
            for dy, dx in ((0, 1), (0, -1), (1, 0), (-1, 0)):
                if 0 <= y + dy < img.shape[0] and 0 <= x + dx < img.shape[1] and img[y + dy, x + dx] != ".": img[y, x] = img[y + dy, x + dx]; break
    return img

def spec(name, img, anchor, variants=None, outline=True, pal=None):
    """Writes the PixelSpec; `pal` adds or overrides palette characters for this sprite alone."""
    pal = {**PAL, **(pal or {})}
    if outline: img = clean_corners(img)
    h, w = img.shape
    used = sorted(set(ch for ch in img.flatten() if ch != "."))
    sp = {"schema": "codeart2d.pixelspec.v1", "name": name, "canvas": [w, h], "anchor_px": [px(anchor[0]), px(anchor[1])],
          "palette": {k: pal[k] for k in sorted(set(used) | ({"o"} if outline else set()))},
          "layers": [{"name": "body", "origin": [0, 0], "segments": segments(img)}]}
    if variants: sp["variants"] = {k: {c: v for c, v in o.items() if c in sp["palette"]} for k, o in variants.items()}
    if outline: sp["outline"] = {"mode": "solid", "color": "o"}
    with open(os.path.join(OUT, name + ".pixelspec.json"), "w", encoding="utf-8") as f: json.dump(sp, f, indent=1)
    return sp


# ---- a star of the Sigils constellation: bought (lit) or not yet ---------------------------------------------------
def sigil_star():
    if SMALL:
        img = canvas(18, 18)  # 9 x 9 pixels: the star and a 1 px margin for its outline
        put(img, ["..J..", ".JIJ.", "JIZIJ", ".JIJ.", "..J.."], 9, 9, center=True)
    else:
        img = canvas(11, 11)
        put(img, [".....", "..w..", ".wZw.", "..w..", "....."], 3, 3)
        put(img, ["....J....", "....J....", "...JIJ...", "JJIIZIIJJ", "...JIJ...", "....J....", "....J...."], 1, 2)
    return spec("sigil-star", img, [9, 9] if SMALL else [5, 5], variants={"lit": {}, "dark": {"J": "#3b3350", "I": "#4a3a7a", "Z": "#6d6488", "w": "#4a3a7a"}})

# ---- the void-energy icon in the top bar: a faceted crystal --------------------------------------------------------
def void_icon():
    img = canvas(15, 23); x, y = grid(); cx, cy = 7.5, 11.5
    dx, dy = x - cx, y - cy; m = np.abs(dx) / 6.2 + np.abs(dy) / 10.2 <= 1
    img[m & (dy < 0) & (dx <= 0)] = "Y"; img[m & (dy < 0) & (dx < 0) & (np.abs(dx) + np.abs(dy) < 6)] = "Z"
    img[m & (dy < 0) & (dx > 0)] = "X"; img[m & (dy >= 0) & (dx <= 0)] = "W"; img[m & (dy >= 0) & (dx > 0)] = "V"
    img[m & (on(y, cy))] = "X"
    return spec("void-icon", img, [7.5, 11.5])

# ---- the head of a flowing void flame: a dark heart in a pale crackling ring --------------------------------------------
def void_ball():
    img = canvas(18, 18); x, y = grid(); r = np.hypot(x - 9, y - 9)
    img[r < 7] = "Y"; img[r < 5.6] = "W"; img[r < 3.6] = "K"; img[(r < 7) & (x + y < 14)] = "Z"  # lit top-left rim
    return spec("void-ball", img, [9, 9])

# ---- the shapeless blob of void that stands on the altar (a stand-in until the forms are drawn) ----------------------
def blob():
    img = canvas(40, 36); x, y = grid(); r = np.hypot((x - 20) / 17, (y - 20) / 14)
    body = r < 1 + 0.06 * np.sin(np.arctan2(y - 20, x - 20) * 5)
    img[body] = "K"; img[body & (dist_u(body) < 3)] = "W"; img[body & (dist_u(body) < 3) & (x + y < 36)] = "X"
    eye = ["Z", "Z", "Y"] if SMALL else ["ZZ", "ZZ", "YY", "YY"]
    put(img, eye, 15, 17, center=True); put(img, eye, 25, 17, center=True)
    return spec("blob", img, [20, 34])

# ---- puffs for the flame trail: discs from big to one pixel, white (the particle colour ramp tints them) -------------
def puffs():
    for k, rad in enumerate([5, 3.6, 2.4, 1.2]):
        img = canvas(14, 14); x, y = grid(); img[np.hypot(x - 7, y - 7) < rad] = "Z"
        spec(f"puff-{k}", img, [7, 7], outline=False)

# ---- the blob's 15 forms: shapeless plus one per nature, transcribed from the browser's FORM_PATHS ------------------
# Each form is the prototype's canvas path (facing up, about 68 layout units across) on an 80x80 canvas centred at
# (40, 40). Every subpath is filled (implicitly closed, as canvas fill does) into an inky body; every drawn segment is
# stroked in the nature's colour, so open strokes (antlers, bee stripes) and inner edges (lion mane, tortoise shell)
# read as glowing lines. The rim lights up on its top-left; the eyes are two pale pixels at the path's eye
# height, each in a ring of ink so it stands clear of the strokes.
FORM_CX, FORM_CY = 40, 40

class Path:
    """A tiny canvas-path recorder: subpaths as polylines in layout units, plus whether each was closed."""
    def __init__(self): self.subs = []; self.cur = None
    def _pt(self, x, y): self.cur[0].append((FORM_CX + x, FORM_CY + y))
    def moveTo(self, x, y): self.cur = [[], False]; self.subs.append(self.cur); self._pt(x, y)
    def lineTo(self, x, y):
        if self.cur is None: self.moveTo(x, y)
        else: self._pt(x, y)
    def last(self): x, y = self.cur[0][-1]; return x - FORM_CX, y - FORM_CY
    def quadraticCurveTo(self, cx, cy, x, y, n=10):
        x0, y0 = self.last()
        for i in range(1, n + 1):
            t = i / n; u = 1 - t; self._pt(u * u * x0 + 2 * u * t * cx + t * t * x, u * u * y0 + 2 * u * t * cy + t * t * y)
    def bezierCurveTo(self, c1x, c1y, c2x, c2y, x, y, n=14):
        x0, y0 = self.last()
        for i in range(1, n + 1):
            t = i / n; u = 1 - t
            self._pt(u ** 3 * x0 + 3 * u * u * t * c1x + 3 * u * t * t * c2x + t ** 3 * x, u ** 3 * y0 + 3 * u * u * t * c1y + 3 * u * t * t * c2y + t ** 3 * y)
    def arc(self, cx, cy, r, a0, a1, n=12):
        for i in range(n + 1):
            a = a0 + (a1 - a0) * i / n; self.lineTo(cx + math.cos(a) * r, cy + math.sin(a) * r)
    def closePath(self): self.cur[1] = True

def _blob(p, t, w):
    for i in range(15):
        a = i / 14 * 6.283; r = 18 + math.sin(a * 3 + t * 2.4) * 2.5 + math.sin(a * 5 - t * 1.7) * 1.5; p.lineTo(math.cos(a) * r, math.sin(a) * r)
    p.closePath(); return -4

def _dragon(p, t, w):
    p.moveTo(0, -24); p.lineTo(5, -18); p.lineTo(9, -26); p.lineTo(8, -14); p.quadraticCurveTo(14, -8, 30, -16 + w); p.lineTo(26, -6 + w); p.lineTo(34, -2 + w); p.lineTo(24, 2 + w); p.lineTo(28, 8 + w)
    p.quadraticCurveTo(14, 4, 8, 10); p.quadraticCurveTo(6, 22, 12, 30); p.quadraticCurveTo(0, 22, -8, 10); p.quadraticCurveTo(-14, 4, -28, 8 - w); p.lineTo(-24, 2 - w); p.lineTo(-34, -2 - w); p.lineTo(-26, -6 - w); p.lineTo(-30, -16 - w)
    p.quadraticCurveTo(-14, -8, -8, -14); p.lineTo(-9, -26); p.lineTo(-5, -18); p.closePath(); return -17

def _bird(p, t, w):
    p.moveTo(0, -26); p.lineTo(5, -16); p.quadraticCurveTo(18, -14, 34, -4 + w * 2); p.quadraticCurveTo(20, 0, 9, 4); p.lineTo(10, 16); p.lineTo(4, 12); p.lineTo(0, 22); p.lineTo(-4, 12); p.lineTo(-10, 16); p.lineTo(-9, 4)
    p.quadraticCurveTo(-20, 0, -34, -4 - w * 2); p.quadraticCurveTo(-18, -14, -5, -16); p.closePath(); return -17

def _rat(p, t, w):
    p.moveTo(0, -24); p.quadraticCurveTo(6, -18, 7, -14); p.arc(12, -15, 6, math.pi * 0.9, math.pi * 2.4); p.quadraticCurveTo(16, 0, 10, 12); p.quadraticCurveTo(4, 18, 2, 18)
    p.quadraticCurveTo(14 + w * 3, 26, 4 + w * 3, 34); p.quadraticCurveTo(10 + w * 3, 24, -2, 18); p.quadraticCurveTo(-8, 18, -10, 12); p.quadraticCurveTo(-16, 0, -12, -9); p.arc(-12, -15, 6, math.pi * 0.6, math.pi * 2.1); p.quadraticCurveTo(-6, -18, 0, -24); p.closePath(); return -16

def _humanoid(p, t, w):
    p.moveTo(0, -27); p.quadraticCurveTo(8, -27, 7, -19); p.quadraticCurveTo(6, -15, 4, -14); p.quadraticCurveTo(14, -13, 15, -6); p.lineTo(19, 10 + w); p.lineTo(14, 10 + w); p.lineTo(11, -2); p.lineTo(9, 14)
    p.quadraticCurveTo(6, 22, 3, 28 + w); p.lineTo(0, 20); p.lineTo(-3, 28 - w); p.quadraticCurveTo(-6, 22, -9, 14); p.lineTo(-11, -2); p.lineTo(-14, 10 - w); p.lineTo(-19, 10 - w); p.lineTo(-15, -6)
    p.quadraticCurveTo(-14, -13, -4, -14); p.quadraticCurveTo(-6, -15, -7, -19); p.quadraticCurveTo(-8, -27, 0, -27); p.closePath(); return -21

def _lion(p, t, w):
    p.moveTo(-11, 2); p.quadraticCurveTo(-15, 18, -8, 26); p.lineTo(8, 26); p.quadraticCurveTo(15, 18, 11, 2); p.closePath()
    p.moveTo(6, 24); p.quadraticCurveTo(18 + w * 2, 30, 20 + w * 2, 18); p.lineTo(25 + w * 2, 14); p.lineTo(18 + w * 2, 13); p.quadraticCurveTo(16, 26, 6, 21)
    p.moveTo(19, -12)
    for i in range(1, 17):
        a = i / 16 * 6.283; r = 14 if i % 2 else 20; p.lineTo(math.cos(a) * r, -12 + math.sin(a) * r)
    p.closePath(); return -14

def _serpent(p, t, w):
    pts = [(math.sin(i * 0.65 + t * 3) * 10, -12 + i * 3.6, 6 - i * 0.35) for i in range(13)]
    p.moveTo(0, -30); p.lineTo(9, -20); p.lineTo(pts[0][0] + pts[0][2], pts[0][1])
    for x, y, r in pts: p.lineTo(x + r, y)
    for x, y, r in reversed(pts): p.lineTo(x - r, y)
    p.lineTo(-9, -20); p.closePath(); return -22

def _moth(p, t, w):
    f = 1 + math.sin(t * 9) * 0.08
    p.moveTo(0, -12); p.bezierCurveTo(14 * f, -28, 34 * f, -18, 30 * f, -2); p.quadraticCurveTo(14, 0, 3, 0); p.bezierCurveTo(18 * f, 6, 22 * f, 20, 8, 20); p.lineTo(2, 12); p.lineTo(0, 22)
    p.lineTo(-2, 12); p.lineTo(-8, 20); p.bezierCurveTo(-22 * f, 20, -18 * f, 6, -3, 0); p.quadraticCurveTo(-14, 0, -30 * f, -2); p.bezierCurveTo(-34 * f, -18, -14 * f, -28, 0, -12); p.closePath()
    p.moveTo(-1, -12); p.quadraticCurveTo(-6, -24, -13, -29); p.lineTo(-12, -26); p.quadraticCurveTo(-5, -22, 1, -12)
    p.moveTo(1, -12); p.quadraticCurveTo(6, -24, 13, -29); p.lineTo(12, -26); p.quadraticCurveTo(5, -22, -1, -12); return -9

def _toad(p, t, w):
    p.moveTo(-15, -16); p.quadraticCurveTo(-15, -26, -8, -25); p.quadraticCurveTo(-3, -24, -3, -19); p.lineTo(3, -19); p.quadraticCurveTo(3, -24, 8, -25); p.quadraticCurveTo(15, -26, 15, -16)
    p.quadraticCurveTo(22, -6, 20, 6); p.lineTo(30, 4 + w); p.lineTo(26, 12 + w); p.lineTo(18, 14); p.quadraticCurveTo(10, 22, 0, 22); p.quadraticCurveTo(-10, 22, -18, 14); p.lineTo(-26, 12 - w); p.lineTo(-30, 4 - w); p.lineTo(-20, 6); p.quadraticCurveTo(-22, -6, -15, -16); p.closePath(); return -20

def _snail(p, t, w):
    p.moveTo(-6, -8); p.quadraticCurveTo(-8, -20, -5, -26); p.lineTo(-2, -25); p.quadraticCurveTo(-3, -16, 0, -10); p.quadraticCurveTo(3, -16, 2, -25); p.lineTo(5, -26); p.quadraticCurveTo(8, -20, 6, -8)
    p.quadraticCurveTo(12, 6, 6, 28); p.lineTo(-6, 28); p.quadraticCurveTo(-12, 6, -6, -8); p.closePath()
    p.moveTo(20, 10); p.arc(4, 10, 16, 0, 6.283, n=24); p.closePath()  # the shell, solid at this size (the browser fills the spiral)
    p.moveTo(20, 10)
    for i in range(31):
        a = i * 0.42; r = 16 - i * 0.45; p.lineTo(4 + math.cos(a) * r, 10 + math.sin(a) * r)
    return -24

def _tortoise(p, t, w):
    p.moveTo(0, -30); p.quadraticCurveTo(7, -30, 6, -20); p.lineTo(10, -18); p.lineTo(22, -24 + w); p.lineTo(18, -12); p.quadraticCurveTo(22, 2, 18, 12); p.lineTo(24, 22 - w); p.lineTo(12, 20)
    p.lineTo(4, 26); p.lineTo(0, 30); p.lineTo(-4, 26); p.lineTo(-12, 20); p.lineTo(-24, 22 + w); p.lineTo(-18, 12); p.quadraticCurveTo(-22, 2, -18, -12); p.lineTo(-22, -24 - w); p.lineTo(-10, -18); p.lineTo(-6, -20); p.quadraticCurveTo(-7, -30, 0, -30); p.closePath()
    p.moveTo(-8, -6); p.lineTo(0, -12); p.lineTo(8, -6); p.lineTo(8, 6); p.lineTo(0, 12); p.lineTo(-8, 6); p.closePath(); return -25

def _stag(p, t, w):
    p.moveTo(0, -12); p.quadraticCurveTo(8, -12, 8, -2); p.quadraticCurveTo(14, 4, 12, 18); p.lineTo(6, 28); p.lineTo(-6, 28); p.lineTo(-12, 18); p.quadraticCurveTo(-14, 4, -8, -2); p.quadraticCurveTo(-8, -12, 0, -12); p.closePath()
    for s in (-1, 1):
        p.moveTo(s * 5, -10); p.lineTo(s * 12, -22); p.lineTo(s * 22, -28); p.moveTo(s * 12, -22); p.lineTo(s * 10, -34); p.moveTo(s * 17, -25); p.lineTo(s * 26, -20); p.moveTo(s * 8, -4); p.lineTo(s * 18, -8)
    return -5

def _swan(p, t, w):
    p.moveTo(-2, -30); p.lineTo(5, -32); p.lineTo(3, -27); p.quadraticCurveTo(-8, -20, 2, -8); p.quadraticCurveTo(18, 0, 22, -14 + w * 2); p.quadraticCurveTo(26, 8, 10, 20); p.quadraticCurveTo(0, 28, -10, 20)
    p.quadraticCurveTo(-18, 10, -10, 0); p.quadraticCurveTo(-6, -4, -4, -8); p.quadraticCurveTo(-14, -20, -2, -30); p.closePath(); return -29

def _manta(p, t, w):
    p.moveTo(-6, -20); p.lineTo(-4, -12); p.lineTo(4, -12); p.lineTo(6, -20); p.lineTo(9, -10); p.quadraticCurveTo(24, -8, 36, 4 + w * 2); p.quadraticCurveTo(18, 6, 8, 14); p.lineTo(2, 16); p.lineTo(1, 34)
    p.lineTo(-1, 34); p.lineTo(-2, 16); p.lineTo(-8, 14); p.quadraticCurveTo(-18, 6, -36, 4 - w * 2); p.quadraticCurveTo(-24, -8, -9, -10); p.closePath(); return -7

def _bee(p, t, w):
    f = math.sin(t * 30) * 3
    p.moveTo(0, -22); p.quadraticCurveTo(9, -20, 8, -10); p.quadraticCurveTo(12, 6, 6, 18); p.lineTo(0, 28); p.lineTo(-6, 18); p.quadraticCurveTo(-12, 6, -8, -10); p.quadraticCurveTo(-9, -20, 0, -22); p.closePath()
    p.moveTo(8, -6); p.quadraticCurveTo(28, -22 + f, 26, -4); p.quadraticCurveTo(20, 2, 8, -2); p.closePath()
    p.moveTo(-8, -6); p.quadraticCurveTo(-28, -22 + f, -26, -4); p.quadraticCurveTo(-20, 2, -8, -2); p.closePath()
    p.moveTo(-10, 2); p.lineTo(10, 2); p.moveTo(-10, 10); p.lineTo(10, 10); return -15

FORM_PATHS = {"blob": _blob, "lion": _lion, "rat": _rat, "dragon": _dragon, "serpent": _serpent, "moth": _moth, "toad": _toad, "snail": _snail,
              "tortoise": _tortoise, "bird": _bird, "humanoid": _humanoid, "stag": _stag, "swan": _swan, "manta": _manta, "bee": _bee}

def _mix(a, b, k):
    a = [int(a[i:i + 2], 16) for i in (1, 3, 5)]; b = [int(b[i:i + 2], 16) for i in (1, 3, 5)]
    return "#" + "".join(f"{round(x + (y - x) * k):02x}" for x, y in zip(a, b))

def _seg_mask(x0, y0, x1, y1, half):
    """Pixels whose centre lies within `half` layout units of the segment."""
    x, y = grid(); dx, dy = x1 - x0, y1 - y0; L = dx * dx + dy * dy
    t = np.clip(((x - x0) * dx + (y - y0) * dy) / L, 0, 1) if L else np.zeros_like(x)
    return np.hypot(x - (x0 + t * dx), y - (y0 + t * dy)) <= half

def form_sprite(form, color):
    """One form: inky body (K), a deep inner glow (R), the rim and strokes in the nature's colour (S), lit top-left (T)."""
    img = canvas(80, 80); p = Path(); ey = FORM_PATHS[form](p, 0.0, 0.0)
    body = np.zeros(SHAPE, bool); stroke = np.zeros(SHAPE, bool); half = 0.6 / S
    for pts, closed in p.subs:
        if len(pts) >= 3: body |= poly_mask(pts)
        segs = list(zip(pts, pts[1:])) + ([(pts[-1], pts[0])] if closed else [])
        for (x0, y0), (x1, y1) in segs: stroke |= _seg_mask(x0, y0, x1, y1, half)
    sil = body | stroke
    edge = sil & (dist_in(sil) <= 1.0)
    img[sil] = "K"; img[sil & (dist_in(sil) <= 1.5)] = "R"; img[edge | stroke] = "S"
    up = np.zeros(SHAPE, bool); up[1:, :] = ~sil[:-1, :]; up[0, :] = True
    left = np.zeros(SHAPE, bool); left[:, 1:] = ~sil[:, :-1]; left[:, 0] = True
    img[(edge | stroke) & (up | left)] = "T"
    # the eyes: one pale column each side of the centre line (symmetric in pixels), in a ring of ink that may cover the
    # inner glow but never the rim; nothing is drawn past the silhouette
    # The browser's eye height often sits on the rim at this size, so the eyes slide down to the first rows where both
    # land on interior ink with ink all round them
    # (a head too thin for that takes the first rows where the eyes themselves are on ink, and one too thin for two
    # eyes gets one).
    c = px(FORM_CX); r0 = px(FORM_CY + ey) - (1 if SMALL else 2); hgt = 2 if SMALL else 3; gap = 1 if SMALL else 2
    cols = (c - 1 - gap, c + gap)
    inner = lambda r: all(0 <= yy < SHAPE[0] and 0 <= xx < SHAPE[1] and img[yy, xx] in ("K", "R")
                          for x0 in cols for yy in range(r - 1, r + hgt + 1) for xx in (x0 - 1, x0, x0 + 1))
    bare = lambda r: all(0 <= yy < SHAPE[0] and img[yy, x0] in ("K", "R") for x0 in cols for yy in range(r, r + hgt))
    hit = next((r for r in range(r0, r0 + px(12)) if inner(r)), None)
    if hit is None: hit = next((r for r in range(r0, r0 + px(12)) if bare(r)), None)  # thin heads: no ring
    if hit is None:  # too thin for two eyes: one, on the interior pixel nearest the browser's eye point
        ys, xs = np.nonzero(np.isin(img, ["K", "R"]))
        k = np.argmin((ys - px(FORM_CY + ey)) ** 2 + (xs + 0.5 - c) ** 2); img[ys[k], xs[k]] = "T"
    else:
        for x0 in cols:
            for yy in range(hit - 1, hit + hgt + 1):
                for xx in range(x0 - 1, x0 + 2):
                    if 0 <= yy < SHAPE[0] and 0 <= xx < SHAPE[1] and sil[yy, xx] and img[yy, xx] in ("K", "R"): img[yy, xx] = "K"
            for yy in range(hit, hit + hgt): img[yy, x0] = "T"
    pal = {"R": _mix(color, PAL["K"], 0.6), "S": color, "T": _mix(color, "#ffffff", 0.6)}
    return spec("form-" + form, img, [FORM_CX, FORM_CY], pal=pal)

def forms():
    data = json.load(open(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "src", "data", "game_data.json"), encoding="utf-8"))
    form_sprite("blob", PAL["X"])
    for n in data["natures"].values():
        if n.get("form") in FORM_PATHS: form_sprite(n["form"], n["color"])

# ---- shared bits for the islands and ruins -------------------------------------------------------------------------
ROCK = ["a", "b", "c", "d", "e", "f"]  # raw rock, dark to light (no top highlight: it is under the carved stone)

def jag(points, seed, amp):
    """Roughens a polygon outline: each edge gets midpoints pushed in or out by up to amp layout units."""
    rng = np.random.default_rng(seed); out = []
    for i in range(len(points)):
        (x0, y0), (x1, y1) = points[i], points[(i + 1) % len(points)]
        out.append((x0, y0))
        for k in (1, 2):
            t = k / 3; nx, ny = y1 - y0, -(x1 - x0); n = math.hypot(nx, ny) or 1
            d = rng.uniform(-amp, amp)
            out.append((x0 + (x1 - x0) * t + nx / n * d, y0 + (y1 - y0) * t + ny / n * d))
    return out

def crystal(img, x0, y0, length, width, ang):
    """A teal shard poking out at angle ang (radians, 0 = right, pi/2 = down): lit face, ridge, shaded face."""
    ca, sa = math.cos(ang), math.sin(ang); nx, ny = -sa, ca
    tip = (x0 + ca * length, y0 + sa * length)
    left = poly_mask([(x0 + nx * width, y0 + ny * width), tip, (x0, y0)])
    right = poly_mask([(x0 - nx * width, y0 - ny * width), tip, (x0, y0)])
    lit_first = (nx + ny) < 0  # the face toward the top-left takes the light
    img[left] = "w" if lit_first else "u"; img[right] = "u" if lit_first else "w"
    x, y = grid()
    ridge = left & right | (poly_mask([(x0 + nx * 0.7 / S, y0 + ny * 0.7 / S), tip, (x0 - nx * 0.7 / S, y0 - ny * 0.7 / S)]))
    img[ridge] = "v"

# ---- faceted rock: the way pixel artists build rock, so undersides and asteroids read as stone, not slabs ------------
# The mass is cut into irregular facets (a jittered Voronoi), each a flat face tilted its own way and lit from the
# top-left; facets meet in dark crevices with a lit lip on the edge that faces the light; pockmarks and rare mineral
# flecks sit inside the faces; the bottom edge breaks away in chunks. Seeds make every rock its own.

def facet_rock(img, m, ramp, seed, cell=13.0, base=0.48, depth_dark=0.28, flecks="", fleck_rate=0.0, chip=True, pocks=0.03):
    """Paints mask m as faceted rock in `ramp` (dark to light). Returns the mask actually painted (chips removed)."""
    x, y = grid(); rng = np.random.default_rng(seed)
    if not m.any(): return m
    ys, xs = np.nonzero(m); x0, x1 = xs.min() / S, (xs.max() + 1) / S; y0, y1 = ys.min() / S, (ys.max() + 1) / S
    # facet seeds: a jittered grid over the mask's box, finer where the rock is lit
    gx = np.arange(x0 - cell, x1 + cell, cell); gy = np.arange(y0 - cell, y1 + cell, cell * 0.8)
    sx, sy = np.meshgrid(gx, gy); sx = sx.ravel() + rng.uniform(-0.45, 0.45, sx.size) * cell; sy = sy.ravel() + rng.uniform(-0.45, 0.45, sy.size) * cell
    d = (x[..., None] - sx) ** 2 + ((y[..., None] - sy) * 1.15) ** 2
    order = np.argsort(d, axis=-1)[..., :2]
    near = order[..., 0]; d1 = np.take_along_axis(d, order[..., :1], -1)[..., 0]; d2 = np.take_along_axis(d, order[..., 1:2], -1)[..., 0]
    edge = np.sqrt(d2) - np.sqrt(d1)                       # distance to the facet's border (layout units, roughly)
    # each facet's tilt: mostly facing the viewer, some up toward the light, some down into shadow
    nx = rng.uniform(-0.9, 0.9, sx.size); ny = rng.uniform(-0.9, 0.7, sx.size)
    lam = (-nx * -0.6 - ny * -0.75 + 0.6) / np.sqrt(nx ** 2 + ny ** 2 + 0.6 ** 2 + 1e-6) / 1.15
    lift = rng.uniform(-0.08, 0.08, sx.size)
    h = max(1e-3, y1 - y0)
    v = base + 0.42 * (lam[near] - 0.55) + lift[near] - depth_dark * (y - y0) / h   # darker toward the bottom: it hangs in shadow
    # a gentle gradient across each facet along its tilt, so faces read as planes
    cxs, cys = sx[near], sy[near]
    v = v + 0.10 * (-(x - cxs) * nx[near] - (y - cys) * ny[near]) / cell
    v = v + 0.06 * (vnoise(3, seed + 5) - 0.5)                                     # grain
    paint = m.copy()
    if chip:  # chunks broken off the lower edge: facets that touch the outline low down drop away
        rim = m & (dist_u(m) < cell * 0.45) & ((y - y0) / h > 0.45)
        cand = np.unique(near[rim])
        # only facets centred outside the outline: they are slivers along the edge, so the silhouette breaks without holes
        px_x = np.clip((sx[cand] * S).astype(int), 0, SHAPE[1] - 1); px_y = np.clip((sy[cand] * S).astype(int), 0, SHAPE[0] - 1)
        outside = ~m[px_y, px_x]
        gone = cand[outside & (rng.uniform(0, 1, cand.size) < 0.6)]
        paint &= ~np.isin(near, gone)
    vv = np.clip(v, 0, 0.999) * (len(ramp) - 1)
    yy, xx = np.mgrid[0:SHAPE[0], 0:SHAPE[1]]; i = np.floor(vv).astype(int); i = i + ((vv - i) > BAYER[yy % 4, xx % 4])
    i = np.clip(i, 0, len(ramp) - 1)
    img[paint] = np.array(ramp, object)[i[paint]]
    w = 0.75 / S
    crevice = paint & (edge < w) & (vnoise(cell * 0.6, seed + 11) > 0.33)  # broken: some facets merge, so it reads as rock, not tiles
    img[crevice] = ramp[0]
    # the lit lip: just past a crevice, on facets whose border faces the top-left
    lip = paint & (edge >= w) & (edge < w + 1.0 / S) & (lam[near] > 0.55)
    img[lip] = ramp[min(len(ramp) - 1, 3)] if len(ramp) > 3 else ramp[-1]
    if pocks:  # pockmarks: a dark pixel with a lit one above-left
        pk = paint & (rng.uniform(0, 1, paint.shape) < pocks) & (edge > 2.0 / S)
        img[pk] = ramp[0]
        up = np.zeros_like(pk); up[:-1, :-1] = pk[1:, 1:]
        img[up & paint & ~pk] = ramp[min(len(ramp) - 1, 2)]
    if flecks and fleck_rate:
        fl = paint & (rng.uniform(0, 1, paint.shape) < fleck_rate) & (edge > 1.5 / S)
        img[fl] = np.array(list(flecks), object)[rng.integers(0, len(flecks), paint.shape)][fl]
    return paint

def rock_under(img, top_pts, bottom_pts, seed, ramp=ROCK, base=0.4, cell=19.0, flecks="", fleck_rate=0.0):
    """A rough rock mass between a top edge and a jagged bottom: faceted, chipped, lit from the top-left."""
    m = poly_mask(jag(top_pts + bottom_pts, seed, 6))
    return facet_rock(img, m, ramp, seed, cell=cell, base=base + 0.05, flecks=flecks, fleck_rate=fleck_rate)

def slab_top(img, cx, top, rx, ry, thick, seed, ramp=COOL, base=0.52, seams=12):
    """A carved stone top (an ellipse seen from above) with a face `thick` units deep and block seams round the face."""
    x, y = grid()
    face = ellipse_mask(cx, top + thick, rx, ry) & ~ellipse_mask(cx, top, rx, ry) & (y > top) | (ellipse_mask(cx, top, rx, ry) & (y > top) & ~ellipse_mask(cx, top, rx, ry))
    face = (ellipse_mask(cx, top + thick, rx, ry) | ellipse_mask(cx, top, rx, ry)) & ~ellipse_mask(cx, top, rx, ry) & (y > top)
    img[face] = shade(face, STONE, bevel=2, tex=0.28, seed=seed + 1, base=0.3)[face]
    img[face & ((np.floor(x * S) % max(3, px(seams))) == 0)] = "b"
    disc = ellipse_mask(cx, top, rx, ry)
    tilt = -0.16 * (x - cx) / rx - 0.2 * (y - top) / ry
    img[disc] = shade(disc, ramp, bevel=4, tex=0.2, seed=seed, base=base, tilt=tilt)[disc]
    return disc, face

def crack_line(img, m, x0, y0, x1, y1, seed, ch="b"):
    """A one-pixel crack wandering from (x0, y0) to (x1, y1), only inside mask m."""
    rng = np.random.default_rng(seed); x, y = grid(); n = 6; pts = [(x0, y0)]
    for k in range(1, n):
        t = k / n; pts.append((x0 + (x1 - x0) * t + rng.uniform(-3, 3), y0 + (y1 - y0) * t + rng.uniform(-2, 2)))
    pts.append((x1, y1))
    for (ax, ay), (bx, by) in zip(pts, pts[1:]):
        dx, dy = bx - ax, by - ay; L = dx * dx + dy * dy or 1
        t = np.clip(((x - ax) * dx + (y - ay) * dy) / L, 0, 1)
        img[m & (np.hypot(x - (ax + t * dx), y - (ay + t * dy)) <= 0.55 / S)] = ch

# ---- the Rite's island: a jagged asteroid, its top worn flat, teal crystals growing out of it -------------------------
def rite_isle():
    img = canvas(240, 150); x, y = grid(); cx, top = 120, 34
    rock_under(img, [(14, top + 4), (226, top + 4)],
               [(222, 58), (204, 82), (190, 80), (176, 104), (150, 112), (132, 140), (112, 124), (92, 130), (70, 104), (44, 96), (30, 72), (18, 60)], 81)
    for x0, y0, ln, w, a in [(40, 92, 22, 4, 2.2), (60, 104, 30, 5, 1.95), (150, 108, 26, 4, 1.25), (186, 86, 20, 4, 0.9), (124, 128, 18, 3, 1.6), (210, 66, 16, 3, 0.45)]:
        crystal(img, x0, y0, ln, w, a)
    disc, face = slab_top(img, cx, top, 104, 22, 8, 82, ramp=["b", "E", "d", "e", "f"], base=0.45, seams=40)
    for (ax, ay, bx, by, sd) in [(70, top - 6, 98, top + 10, 83), (150, top - 12, 176, top + 2, 84)]:
        crack_line(img, disc, ax, ay, bx, by, sd)
    moss = disc & (vnoise(5, 85) > 0.62) & (dist_u(disc) < 10)  # lichen round the worn rim
    img[moss] = "u"
    return spec("rite-isle", img, [cx, top])

# ---- the Gate's dais: a round stone platform, a broken step in front, rubble and rock underneath ----------------------
def gate_dais():
    img = canvas(310, 170); x, y = grid(); cx, top = 155, 40
    rock_under(img, [(26, top + 30), (284, top + 30)],
               [(270, 84), (250, 104), (220, 118), (196, 140), (168, 132), (150, 160), (128, 136), (100, 128), (70, 112), (42, 98)], 91, ramp=EARTH, base=0.42)
    for x0, y0, ln, w, a in [(120, 136, 20, 4, 1.7), (178, 134, 16, 3, 1.4)]:
        crystal(img, x0, y0, ln, w, a)
    # the lower step: a wider ring in front, with a bite broken out of its right side
    step, sface = slab_top(img, cx, top + 22, 146, 28, 10, 92, base=0.48, seams=14)
    bite = poly_mask(jag([(210, top + 30), (262, top + 22), (300, top + 40), (290, top + 70), (236, top + 66)], 93, 4))
    img[bite & (step | sface)] = "."
    # the dais itself
    disc, face = slab_top(img, cx, top, 128, 22, 14, 94, base=0.56, seams=12)
    radial = ((x - cx) / 128.0) ** 2 + ((y - top) / 22.0) ** 2
    img[disc & (np.abs(np.sqrt(radial) - 0.72) < 0.03)] = "d"  # a carved ring where the horns stand
    crack_line(img, disc, cx - 40, top + 4, cx + 6, top + 18, 95)
    for rx0, ry0, r in [(54, top + 52, 5), (250, top + 76, 4)]:  # rubble
        m = ellipse_mask(rx0, ry0, r, r * 0.7); img[m] = shade(m, STONE, bevel=2, base=0.5)[m]
    return spec("gate-dais", img, [cx, top])

# ---- the Sigils' observatory: a cracked star disc ringed in gold, a broken piece drifting off its edge ---------------
def sigil_disc():
    img = canvas(230, 100); x, y = grid(); cx, top = 110, 28
    rock_under(img, [(30, top + 10), (190, top + 10)], [(180, 46), (150, 66), (118, 84), (96, 70), (60, 60), (36, 44)], 101, base=0.38)
    disc, face = slab_top(img, cx, top, 92, 18, 8, 102, ramp=["a", "b", "E", "d", "e"], base=0.42, seams=30)
    radial = np.sqrt(((x - cx) / 92.0) ** 2 + ((y - top) / 18.0) ** 2); ang = np.arctan2((y - top) / 18.0, (x - cx) / 92.0)
    img[disc & (np.abs(radial - 0.86) < 0.045)] = "H"  # the gold ring
    img[disc & (np.abs(radial - 0.86) < 0.045) & (y < top)] = "I"
    img[disc & (np.abs(radial - 0.45) < 0.05)] = "V"
    ticks = disc & (radial > 0.5) & (radial < 0.8) & (np.abs(((ang / (2 * math.pi) * 12) % 1) - 0.5) > 0.47)
    img[ticks] = "H"
    # the crack and the broken-off wedge, drifting a little away down and right
    wedge = (img != ".") & poly_mask(jag([(152, top - 30), (230, top - 30), (230, top + 70), (170, top + 70), (164, top + 8)], 104, 3))
    piece = img.copy(); piece[~wedge] = "."
    img[wedge] = "."
    sh = (px(8), px(10)); moved = np.roll(np.roll(piece, sh[1], axis=0), sh[0], axis=1)
    keep = moved != "."; img[keep] = moved[keep]
    crack_line(img, disc, cx - 60, top - 4, cx - 20, top + 10, 103)
    return spec("sigil-disc", img, [cx, top])



# ---- redrawn: a card back, framed in violet and gold, a void eye at its heart ----------------------------------------
def card_back():
    img = canvas(52, 70); x, y = grid()
    m = poly_mask([(2, 4), (4, 2), (48, 2), (50, 4), (50, 66), (48, 68), (4, 68), (2, 66)])
    img[m] = shade(m, ["K", "b", "L", "V"], bevel=4, tex=0.12, seed=71, base=0.38)[m]
    d = dist_u(m)
    img[m & (d > 2.5) & (d < 2.5 + 1.0 / S)] = "W"
    img[m & (d > 6.5) & (d < 6.5 + 1.0 / S)] = "H"
    for cx0, cy0 in [(9, 9), (43, 9), (9, 61), (43, 61)]:
        put(img, [".I.", "IJI", ".I."] if not SMALL else ["I"], cx0, cy0, center=True)
    eye = (np.abs(x - 26) / 14) ** 1.4 + (np.abs(y - 35) / 8) ** 1.4 <= 1
    img[eye] = "X"; img[eye & (y > 35)] = "W"
    img[np.hypot(x - 26, y - 35) < 4.5] = "K"; img[np.hypot(x - 24.5, y - 33.5) < 1.6] = "Z"
    rays = m & (d > 9) & (np.abs(np.sin(np.arctan2(y - 35, x - 26) * 4)) > 0.97) & (np.hypot(x - 26, y - 35) > 12)
    img[rays] = "V"
    return spec("card-back", img, [26, 35])


# ---- far asteroids: craggy, faceted, each its own (one veined, one with a crystal, one pitted) -----------------------
def asteroids():
    out = []
    looks = [  # size, seed, facet size, accent
        (50, 42, 61, 12.0, ("", 0.0)),
        (70, 56, 62, 15.0, ("WV", 0.012)),
        (96, 76, 63, 18.0, ("", 0.0)),
    ]
    for k, (w, h, seed, cell, (fl, rate)) in enumerate(looks):
        img = canvas(w, h); x, y = grid(); cx, cy = w / 2, h / 2
        rng = np.random.default_rng(seed); n = 11 + k * 3
        pts = []
        for a in np.linspace(0, 2 * math.pi, n, endpoint=False):
            r = rng.uniform(0.62, 1.0)
            pts.append((cx + math.cos(a) * (w / 2 - 3) * r, cy + math.sin(a) * (h / 2 - 3) * r))
        m = poly_mask(jag(pts, seed, 2.5))
        paint = facet_rock(img, m, ["a", "b", "E", "d", "e", "f", "F"], seed, cell=cell, base=0.62, depth_dark=0.22, flecks=fl, fleck_rate=rate, chip=False, pocks=0.04)
        for _ in range(1 + k):  # a crater or two: a dark bowl, its lower-right rim lit
            ccx, ccy, rr = rng.uniform(0.35, 0.65) * w, rng.uniform(0.35, 0.65) * h, rng.uniform(3, 4 + k * 1.5)
            dd = np.hypot(x - ccx, (y - ccy) * 1.2); cr = paint & (dd < rr) & (dist_u(paint) > 3)
            img[cr] = "b"; img[cr & ((x - ccx) + (y - ccy) < -rr * 0.3)] = "a"
            img[paint & (np.abs(dd - rr) < 0.8 / S) & ((x - ccx) + (y - ccy) > rr * 0.4)] = "e"
        if k == 2:  # the big one carries a teal crystal
            crystal(img, w * 0.66, h * 0.3, 12, 3, -1.0)
        out.append(spec(f"asteroid-{k}", img, [w / 2, h / 2]))
    return out

# ---- redrawn: the altar, narrowed to 70% so the Rite and the Gate float on their own islands beside it -----------------
ALTAR_K = 0.7
def altar():
    k = ALTAR_K; W, H = int(704 * k) + 4, 262; cx, top = W / 2, 82; RX = 335 * k
    X = lambda v: cx + (v - 352) * k
    img = canvas(W, H); x, y = grid()
    under = poly_mask([(X(26), 92), (X(678), 92), (X(546), 196), (X(420), 236), (X(352), 254), (X(286), 236), (X(160), 198)])
    under &= ~((dist_u(under) < 4) & (np.sin(x / 7.0) * np.sin(y / 5.0 + x / 31.0) > 0.55))
    under = facet_rock(img, under, EARTH, 4, cell=24.0, base=0.5, depth_dark=0.3, flecks="v", fleck_rate=0.004)
    strata = under & (np.abs(((y + 4 * np.sin(x / 29.0)) % 16) - 8) < max(0.9, 0.7 / S)) & (vnoise(14, 9) > 0.35)
    img[strata] = "p"
    for cxr, ln in [(250, 22), (330, 34), (402, 26), (470, 18), (204, 14)]:
        y0 = 200 + (abs(cxr - 352) < 90) * 26; c = X(cxr)
        tip = poly_mask([(c - max(3, ln / 6), y0), (c + max(3, ln / 6), y0), (c, y0 + ln)])
        img[tip & (x < c - 0.5 / S)] = "w"; img[tip & (np.abs(x - c) <= 0.5 / S)] = "v"; img[tip & (x > c + 0.5 / S)] = "u"
    side = ellipse_mask(cx, top + 14, RX, 72) & ~ellipse_mask(cx, top, RX, 72) & (y > top)
    img[side] = shade(side, STONE, bevel=3, tex=0.25, seed=5, base=0.28)[side]
    img[side & ((np.floor(x * S) % max(3, px(9))) == 0)] = "b"
    notches = side & (vnoise(9, 7) > 0.72) & (y < top + 72 + 6)  # chips broken out of the rim
    img[notches] = "a"
    disc = ellipse_mask(cx, top, RX, 72)
    radial = ((x - cx) / RX) ** 2 + ((y - top) / 72.0) ** 2
    tilt = -0.18 * (x - cx) / RX - 0.22 * (y - top) / 72.0
    img[disc] = shade(disc, COOL, bevel=6, tex=0.18, seed=6, base=0.5, tilt=tilt)[disc]
    ang = np.arctan2((y - top) / 72.0, (x - cx) / RX)
    seam_w = 0.03 / S
    seams = disc & (radial > 0.12) & (np.abs(((ang / (2 * math.pi) * 20) % 1) - 0.5) > 0.5 - seam_w)
    img[seams] = "d"
    for rr in (0.36, 0.12):
        img[disc & (np.abs(radial - rr) < 0.008 / S)] = "d"
    ring = disc & (np.abs(np.sqrt(radial) - 0.8) < 0.025)
    img[ring] = np.where(y[ring] < top, "v", "u")
    img[disc & (np.abs(np.sqrt(radial) - 0.8) < 0.007 / S) & (y < top - 30)] = "w"
    for (ax, ay, bx, by, sd) in [(cx - 150, top + 20, cx - 100, top + 48, 8), (cx + 90, top - 50, cx + 140, top - 30, 9)]:
        crack_line(img, disc & ~ring, ax, ay, bx, by, sd)
    dais = ellipse_mask(cx, top - 2, 96, 22)
    img[dais] = shade(dais, COOL, bevel=3, base=0.62, tilt=-0.1 * (y - top) / 22.0)[dais]
    return spec("altar", img, [cx, top])

# ---- depth: objects drawn at the platforms' angle (seen from the front and above, lit from the top-left) -------------
# Round things are cylinders: an elliptical top about a quarter as deep as it is wide, a body shaded across its width
# (light left, dark right). Blocks show their front, their lit top and their darker right side.

def cylinder(img, cx, y_top, y_bot, rx, ry, seed, ramp=COOL, base=0.5, top_ramp=None, flutes=0):
    """A short cylinder: body from y_top to y_bot, an elliptical lit top at y_top. Returns (body, top) masks."""
    x, y = grid()
    body = ((np.abs(x - cx) <= rx) & (y >= y_top) & (y <= y_bot)) | ellipse_mask(cx, y_bot, rx, ry)
    across = (x - cx) / rx  # -1 at the lit left, 1 at the dark right
    img[body] = shade(body, ramp, bevel=1.5, tex=0.18, seed=seed, base=base, tilt=-0.32 * across)[body]
    if flutes:
        for k in range(1, flutes):
            fx = cx - rx + 2 * rx * k / flutes
            img[body & on(x, fx) & (y > y_top + ry) & (y < y_bot)] = "d" if abs(fx - cx) < rx * 0.7 else "b"
    top = ellipse_mask(cx, y_top, rx, ry)
    img[top] = shade(top, top_ramp or ["d", "e", "f", "F", "h"], bevel=1.5, base=0.62, tilt=-0.15 * (x - cx) / rx)[top]
    return body, top

# ---- a standing stone: front face, a lit top, a darker side face, a rune glowing down the front ---------------------
def standing_stone():
    img = canvas(32, 58); x, y = grid()
    front = poly_mask([(4, 16), (18, 13), (19, 54), (3, 55)])
    side = poly_mask([(18, 13), (26, 9), (27, 49), (19, 54)])
    top = poly_mask([(4, 16), (11, 9), (17, 5), (26, 9), (18, 13)])
    top &= ~poly_mask([(15, 2), (30, 2), (30, 12), (21, 8)])  # a chip off the top corner
    side &= ~poly_mask([(21, 8), (30, 2), (30, 18), (25, 14)])
    img[side] = shade(side, ["a", "b", "c", "d"], bevel=1.5, tex=0.3, seed=24, base=0.45)[side]
    img[front] = shade(front, STONE, bevel=2, tex=0.32, seed=21, base=0.48)[front]
    img[top] = shade(top, ["e", "f", "g", "h"], bevel=1, tex=0.2, seed=23, base=0.55)[top]
    img[(front | side) & (y > 47) & (vnoise(3, 22) > 0.5)] = "u"  # lichen at the foot
    rune = ["w", "v", "v", "w"] if SMALL else ["..w..", ".v.v.", "v.w.v", ".v.v.", "..v..", ".vwv.", "..v.."]
    put(img, rune, 11, 30, center=True)
    return spec("standing-stone", img, [11, 54], variants={"lit": {}, "far": {"e": "#45404d", "f": "#6d6488", "g": "#8a8296", "h": "#8b95a3", "w": "#2e5a55", "v": "#16302e", "u": "#16302e"}})

# ---- a relic pedestal: a round capital with the relic's teal rest, a fluted shaft, a round plinth ---------------------
def pedestal():
    img = canvas(46, 68); x, y = grid(); cx = 23
    cylinder(img, cx, 54, 60, 19, 4.5, 45, base=0.45)                                  # plinth
    cylinder(img, cx, 20, 54, 10, 2.5, 42, base=0.5, flutes=5)                          # shaft
    cylinder(img, cx, 17, 21, 13, 3.2, 46, base=0.5)                                    # echinus
    cap_body, cap_top = cylinder(img, cx, 10, 15, 20, 5, 43, base=0.55)                 # capital
    img[ellipse_mask(cx, 10, 12, 3)] = "u"; img[ellipse_mask(cx, 10, 12, 3) & ~ellipse_mask(cx, 10.6, 10.5, 2.4)] = "v"  # the rest
    img[cap_top & ~poly_mask([(0, 0), (37, 0), (31, 16), (0, 16)]) & (vnoise(3, 47) > 0.55)] = "d"  # wear on the far edge
    crack_line(img, (np.abs(x - cx) <= 10) & (y > 24) & (y < 52), cx + 2, 26, cx - 4, 48, 44)
    return spec("pedestal", img, [cx, 60], variants={"open": {}, "locked": {"E": "#1a1229", "d": "#241a3a", "e": "#353a44", "f": "#45404d", "F": "#45404d", "h": "#6d6488", "v": "#241a3a", "u": "#1a1229", "b": "#1a1229", "g": "#45404d"}})

# ---- the Deck's lectern: a book rest tilted toward us (its top and its thick front lip), a round column, a plinth ------
def lectern():
    img = canvas(72, 70); x, y = grid(); cx = 36
    cylinder(img, cx, 54, 60, 26, 6, 13, base=0.42)                                    # plinth
    cylinder(img, cx, 46, 54, 18, 4.5, 14, base=0.5)                                   # step
    col, _ = cylinder(img, cx, 24, 46, 8, 2.5, 15, base=0.5)                            # column
    eye = [".X.", "XKX", ".X."] if SMALL else ["..XXX..", ".XWKWX.", "XWKKKWX", ".XWKWX.", "..XXX.."]
    put(img, eye, cx - 1, 35, center=True)
    lip = poly_mask([(6, 22), (66, 22), (66, 27), (6, 27)])
    rest = poly_mask([(12, 6), (60, 6), (66, 22), (6, 22)])
    img[lip] = shade(lip, ["b", "E", "d"], bevel=1, base=0.5, tilt=-0.2 * (x - cx) / 30)[lip]
    img[rest] = shade(rest, COOL, bevel=2, tex=0.18, seed=16, base=0.62, tilt=-0.18 * (x - cx) / 30 + 0.12 * (y - 6) / 16)[rest]
    img[rest & on(y, 21)] = "H"; img[lip & on(y, 26)] = "b"  # a gold edge along the front of the rest
    img[rest & (np.abs(x - cx) < 9) & (y > 9) & (y < 19) & (((np.floor(x * S) + np.floor(y * S)) % 3) == 0)] = "d"  # carved lattice
    return spec("lectern", img, [cx, 60])

# ---- the Journal's tome, lying open on its stand in perspective: far edge narrower, writing tightening away ----------
def tome():
    img = canvas(124, 70); x, y = grid()
    far, near = 10, 44  # page rows: far edge, near edge
    L = lambda yy: 14 + (yy - far) / (near - far) * (5 - 14)      # left page edge at row yy
    Rr = lambda yy: 106 + (yy - far) / (near - far) * (118 - 106)
    cover = poly_mask([(12, far - 3), (108, far - 3), (120, near + 8), (3, near + 8)])
    img[cover] = shade(cover, LEATHER, bevel=2, tex=0.22, seed=51, base=0.5)[cover]
    edge = poly_mask([(5, near + 1), (118, near + 1), (119, near + 5), (4, near + 5)])  # the page block's front edge
    img[edge] = "s"; img[edge & ((np.floor(y * S) % 2) == 0)] = "P"
    for side in (0, 1):
        pg = poly_mask([(L(far), far), (60, far + 2), (60, near + 2), (L(near), near)] if side == 0 else [(60, far + 2), (Rr(far), far), (Rr(near), near), (60, near + 2)])
        u = np.clip(np.abs(x - 60) / 50, 0, 1)
        img[pg] = shade(pg, ["s", "P", "P", "P"], bevel=2, base=0.72, tilt=-0.25 * (1 - u) + 0.08 * (side * 2 - 1))[pg]
        yy, k = far + 5.0, 0
        while yy < near - 2:  # writing: rows closer together toward the far edge
            x0 = (L(yy) + 4) if side == 0 else 64; x1 = (58 if side == 0 else Rr(yy) - 4) - (k * 7 % 11)
            img[pg & on(y, yy) & (x > x0) & (x < x1) & (((np.floor(x * S) * 7 + k * 3) % 11) != 0)] = "r"
            yy += 3.0 + 2.5 * (yy - far) / (near - far); k += 1
        if side == 0:
            ill = poly_mask([(L(far + 4) + 3, far + 4), (L(far + 4) + 12, far + 4), (L(far + 13) + 12, far + 13), (L(far + 13) + 3, far + 13)])
            img[ill] = "W"; img[ill & (np.abs(x - (L(far + 8) + 8)) < 2.5)] = "X"
    img[poly_mask([(58.5, far + 1), (61.5, far + 1), (61.5, near + 3), (58.5, near + 3)])] = "K"  # the gutter
    for cx0, cy0 in [(10, far - 2), (110, far - 2), (5, near + 6), (118, near + 6)]:
        put(img, ["IH"] if SMALL else ["IIH", "HH."], cx0, cy0, center=True)
    ribbon = poly_mask([(57, near + 4), (61, near + 4), (62, near + 18), (59, near + 15), (56, near + 18)])
    img[ribbon] = "N"; img[ribbon & (x > 59)] = "M"
    return spec("tome", img, [60, near + 4])

# ---- the Journal's rock: the reading stand's top is tilted to take the tome --------------------------------------------
def journal_rock():
    img = canvas(150, 100); x, y = grid(); cx, stand_top = 75, 22
    rock_under(img, [(22, 58), (128, 58)], [(124, 66), (104, 80), (84, 94), (64, 84), (40, 78), (26, 68)], 111)
    slab_top(img, cx, 56, 54, 10, 6, 112, ramp=["b", "E", "d", "e", "f"], base=0.45, seams=30)
    crystal(img, 92, 84, 12, 3, 1.3)
    body, _ = cylinder(img, cx, stand_top + 8, 56, 11, 3, 113, base=0.5)
    img[body & on(y, 46)] = "d"
    top = poly_mask([(cx - 20, stand_top), (cx + 20, stand_top), (cx + 22, stand_top + 8), (cx - 22, stand_top + 8)])
    img[top] = shade(top, COOL, bevel=1.5, base=0.62, tilt=-0.15 * (x - cx) / 20)[top]
    img[top & on(y, stand_top + 7)] = "d"
    rune = [".v.", "vwv"] if SMALL else ["..v..", ".vwv.", "..v.."]
    put(img, rune, cx - 1, 40, center=True)
    return spec("journal-rock", img, [cx, stand_top + 4])

# ---- the Relics' slab: its broken columns are round stumps now --------------------------------------------------------
def relic_slab():
    img = canvas(290, 116); x, y = grid(); cx, top = 145, 60
    rock_under(img, [(30, top + 14), (262, top + 14)], [(250, 82), (210, 96), (170, 112), (130, 100), (96, 106), (60, 90), (38, 80)], 121, base=0.4)
    disc, face = slab_top(img, cx, top, 124, 14, 10, 122, base=0.54, seams=16)
    img[disc & ((np.floor(x * S) % max(3, px(24))) == 0) & (y < top + 12)] = "d"
    for sx, h, sd in [(36, 40, 123), (256, 58, 124)]:
        cylinder(img, sx, top - 2, top + 4, 13, 3.5, sd + 10, base=0.55)               # the base ring
        body, ctop = cylinder(img, sx, top - h, top - 2, 9, 2.5, sd, base=0.5, flutes=4)
        snap = poly_mask(jag([(sx - 12, top - h - 8), (sx + 12, top - h - 8), (sx + 12, top - h + 3), (sx - 12, top - h - 2)], sd, 2))
        img[snap & (body | ctop)] = "."                                                 # snapped off at a slant
        img[body & (np.abs(y - (top - h + 1 + (x - sx) * -0.2)) < 1.0 / S)] = "f"         # the broken lip, lit
    left_break = poly_mask(jag([(0, top - 20), (20, top - 20), (16, top + 6), (24, top + 30), (0, top + 30)], 125, 3))
    img[left_break & ~ellipse_mask(36, top, 14, 10) & (y > top - 6)] = "."
    return spec("relic-slab", img, [cx, top])

# ---- the Rift Gate's horns: each with thickness (a darker side face behind it), drawn left and right ------------------
def gate_arch():
    def quad(p0, p1, p2, n=40):
        return [((1 - t) ** 2 * p0[0] + 2 * (1 - t) * t * p1[0] + t * t * p2[0], (1 - t) ** 2 * p0[1] + 2 * (1 - t) * t * p1[1] + t * t * p2[1]) for t in [i / n for i in range(n + 1)]]
    W, D = 104, (10, -5)  # canvas width; the side face shows this far right and up
    for name, mirror in (("gate-arch-l", False), ("gate-arch-r", True)):
        img = canvas(W, 248); x, y = grid()
        outer = quad((46, 236), (2, 116), (58, 6)); inner = quad((76, 18), (40, 116), (78, 236))
        pts = outer + inner
        if mirror: pts = [(92 - px_, py_) for px_, py_ in pts]
        front = poly_mask(pts)
        back = poly_mask([(px_ + D[0], py_ + D[1]) for px_, py_ in pts]) & ~front
        img[back] = shade(back, ["a", "b", "c", "d"], bevel=2, tex=0.25, seed=36, base=0.42)[back]
        img[front] = shade(front, STONE, bevel=5, tex=0.3, seed=31, base=0.5)[front]
        d = dist_u(front)
        runes = front & (d > 4) & (((y // 22) % 2) == 0) & ((y % 22) < max(3, 1.0 / S)) & (vnoise(3, 33) > 0.35)
        img[runes] = "v"
        img[back & ((np.floor(y * S) % max(3, px(22))) == 0)] = "a"  # block joints wrap round the side
        boss = [".J.", "JHJ", ".J."] if SMALL else ["..JJ..", ".JIIJ.", "JIHHIJ", ".JIIJ.", "..JJ.."]
        put(img, boss, (60 if not mirror else 32), 13, center=True)
        spec(name, img, [0, 0])

for f in (altar, lectern, standing_stone, gate_arch, sigil_star, pedestal, tome, asteroids, void_icon, card_back, void_ball, blob, forms, puffs, rite_isle, gate_dais, sigil_disc, journal_rock, relic_slab):
    f()
print("ok", OUT, "scale", S)
