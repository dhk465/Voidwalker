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

def spec(name, img, anchor, variants=None, outline=True):
    if outline: img = clean_corners(img)
    h, w = img.shape
    used = sorted(set(ch for ch in img.flatten() if ch != "."))
    sp = {"schema": "codeart2d.pixelspec.v1", "name": name, "canvas": [w, h], "anchor_px": [px(anchor[0]), px(anchor[1])],
          "palette": {k: PAL[k] for k in sorted(set(used) | ({"o"} if outline else set()))},
          "layers": [{"name": "body", "origin": [0, 0], "segments": segments(img)}]}
    if variants: sp["variants"] = {k: {c: v for c, v in o.items() if c in sp["palette"]} for k, o in variants.items()}
    if outline: sp["outline"] = {"mode": "solid", "color": "o"}
    with open(os.path.join(OUT, name + ".pixelspec.json"), "w", encoding="utf-8") as f: json.dump(sp, f, indent=1)
    return sp

# ---- the floating altar: a carved stone disc on a rough island of earth and rock, crystals hanging below ----------
def altar():
    W, H = 704, 262; cx, top = 352, 82  # the disc's centre; the canvas origin sits at (288, 433) in the 1280x720 hub
    img = canvas(W, H); x, y = grid()
    under = poly_mask([(26, 92), (678, 92), (546, 196), (420, 236), (352, 254), (286, 236), (160, 198)])
    under &= ~((dist_u(under) < 4) & (np.sin(x / 7.0) * np.sin(y / 5.0 + x / 31.0) > 0.55))  # lumps: rocks, but a smooth outline
    img[under] = shade(under, EARTH, bevel=10, tex=0.22, seed=4, base=0.42, light=(-0.4, -0.9, 0.5), tilt=0.12 * np.sin(x / 37.0) - 0.002 * (y - 92))[under]
    strata = under & (np.abs(((y + 4 * np.sin(x / 29.0)) % 16) - 8) < max(0.9, 0.7 / S)) & (vnoise(14, 9) > 0.35)
    img[strata] = "p"
    for cxr, ln in [(250, 22), (330, 34), (402, 26), (470, 18), (204, 14)]:  # teal crystals hanging off the bottom
        y0 = 200 + (abs(cxr - 352) < 90) * 26
        tip = poly_mask([(cxr - max(3, ln / 6), y0), (cxr + max(3, ln / 6), y0), (cxr, y0 + ln)])
        img[tip & (x < cxr - 0.5 / S)] = "w"; img[tip & (np.abs(x - cxr) <= 0.5 / S)] = "v"; img[tip & (x > cxr + 0.5 / S)] = "u"
    side = ellipse_mask(cx, top + 14, 335, 72) & ~ellipse_mask(cx, top, 335, 72) & (y > top)
    img[side] = shade(side, STONE, bevel=3, tex=0.25, seed=5, base=0.28)[side]
    img[side & ((np.floor(x * S) % max(3, px(9))) == 0)] = "b"  # carved blocks round the rim
    disc = ellipse_mask(cx, top, 335, 72)
    radial = ((x - cx) / 335.0) ** 2 + ((y - top) / 72.0) ** 2
    tilt = -0.18 * (x - cx) / 335.0 - 0.22 * (y - top) / 72.0  # lit from the top-left across the disc
    img[disc] = shade(disc, COOL, bevel=6, tex=0.18, seed=6, base=0.5, tilt=tilt)[disc]
    ang = np.arctan2((y - top) / 72.0, (x - cx) / 335.0)
    seam_w = 0.03 / S  # a seam stays about one pixel wide at any scale
    seams = disc & (radial > 0.12) & (np.abs(((ang / (2 * math.pi) * 24) % 1) - 0.5) > 0.5 - seam_w)
    img[seams] = "d"
    for rr in (0.36, 0.12):  # rings carved into the floor
        img[disc & (np.abs(radial - rr) < 0.008 / S)] = "d"
    ring = disc & (np.abs(np.sqrt(radial) - 0.8) < 0.025)  # the teal inlay that glows in the floor
    img[ring] = np.where(y[ring] < top, "v", "u")
    img[disc & (np.abs(np.sqrt(radial) - 0.8) < 0.007 / S) & (y < top - 30)] = "w"
    dais = ellipse_mask(cx, top - 2, 110, 24)
    img[dais] = shade(dais, COOL, bevel=3, base=0.62, tilt=-0.1 * (y - top) / 24.0)[dais]
    return spec("altar", img, [cx, top])

# ---- the Deck's lectern: a carved stone stand with a slanted top --------------------------------------------------
def lectern():
    img = canvas(64, 50)
    body = poly_mask([(4, 6), (60, 6), (50, 40), (14, 40)]) | poly_mask([(10, 40), (54, 40), (58, 48), (6, 48)])
    img[body] = shade(body, COOL, bevel=4, tex=0.2, seed=11, base=0.48)[body]
    topf = poly_mask([(2, 2), (62, 2), (60, 8), (4, 8)])
    img[topf] = shade(topf, COOL, bevel=2, base=0.75)[topf]
    rune = [".v.", "vwv", ".v."] if SMALL else ["..vvv..", ".v...v.", "v..w..v", ".v...v.", "..vvv.."]
    put(img, rune, 32, 23, center=True)  # a rune of the void, inlaid
    x, y = grid(); img[body & (on(y, 40))] = "d"
    return spec("lectern", img, [32, 48])

# ---- a standing stone of the Rite, with a carved rune; dim for the back of the ring --------------------------------
def standing_stone():
    img = canvas(22, 34)
    m = poly_mask([(4, 6), (8, 2), (14, 2), (18, 8), (19, 32), (3, 32)])
    img[m] = shade(m, STONE, bevel=3, tex=0.3, seed=21, base=0.5)[m]
    rune = ["w", "v", "v"] if SMALL else ["..w..", ".v.v.", "v.w.v", ".v.v.", "..v..", "..v.."]
    put(img, rune, 11, 15, center=True)
    return spec("standing-stone", img, [11, 32], variants={"lit": {}, "far": {"e": "#45404d", "f": "#6d6488", "g": "#8a8296", "h": "#8b95a3", "w": "#2e5a55", "v": "#16302e"}})

# ---- the Rift Gate's arch, one horn (the right one is the same, mirrored) -----------------------------------------
def gate_arch():
    img = canvas(92, 244)
    def quad(p0, p1, p2, n=40):
        return [((1 - t) ** 2 * p0[0] + 2 * (1 - t) * t * p1[0] + t * t * p2[0], (1 - t) ** 2 * p0[1] + 2 * (1 - t) * t * p1[1] + t * t * p2[1]) for t in [i / n for i in range(n + 1)]]
    # in the 1280x720 hub this horn spans x 875..967 and y 288..532 (the left one); points are local to that box
    outer = quad((46, 232), (2, 112), (58, 2)); inner = quad((76, 14), (40, 112), (78, 232))
    m = poly_mask(outer + inner)
    img[m] = shade(m, STONE, bevel=5, tex=0.3, seed=31, base=0.48)[m]
    x, y = grid(); d = dist_u(m)
    runes = m & (d > 4) & (((y // 22) % 2) == 0) & ((y % 22) < max(3, 1.0 / S)) & (vnoise(3, 33) > 0.35)
    img[runes] = "v"
    boss = [".J.", "JHJ", ".J."] if SMALL else ["..JJ..", ".JIIJ.", "JIHHIJ", ".JIIJ.", "..JJ.."]
    put(img, boss, 60, 9, center=True)  # a gold boss at the tip
    return spec("gate-arch", img, [0, 0])

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

# ---- a relic pedestal: open (stone) or still locked (sunk in shadow) ------------------------------------------------
def pedestal():
    img = canvas(44, 50)
    m = poly_mask([(4, 2), (40, 2), (40, 10), (34, 12), (32, 40), (38, 42), (40, 48), (4, 48), (6, 42), (12, 40), (10, 12), (4, 10)])
    img[m] = shade(m, COOL, bevel=3, tex=0.2, seed=41, base=0.5)[m]
    x, y = grid(); half = 0.5 / S
    img[m & (y >= 14) & (y <= 38) & (on(x, 16) | on(x, 22) | on(x, 28))] = "d"  # fluting
    img[m & (on(y, 11))] = "d"
    img[m & (on(y, 4)) & (x > 6) & (x < 38)] = "v"  # the teal line where a relic rests
    return spec("pedestal", img, [22, 48], variants={"open": {}, "locked": {"E": "#1a1229", "d": "#241a3a", "e": "#353a44", "f": "#45404d", "F": "#45404d", "h": "#6d6488", "v": "#241a3a"}})

# ---- the Journal: an open tome, leather and gold, its pages written --------------------------------------------------
def tome():
    img = canvas(112, 64); x, y = grid()
    cover = poly_mask([(2, 10), (54, 4), (58, 6), (110, 4), (110, 56), (58, 60), (54, 60), (2, 56)])
    img[cover] = shade(cover, LEATHER, bevel=3, tex=0.25, seed=51, base=0.5)[cover]
    for side, (x0, x1) in enumerate([(6, 54), (58, 106)]):
        pg = poly_mask([(x0, 10 if side == 0 else 6), (x1, 6 if side == 0 else 10), (x1, 54), (x0, 54)])
        curl = 0.15 * np.cos((x - (x0 + x1) / 2) / (x1 - x0) * math.pi)
        img[pg] = shade(pg, ["s", "P", "P", "P"], bevel=3, base=0.7, tilt=curl)[pg]
        gap = 4 if not SMALL else 3 / S  # lines of writing, ragged at the end, one pixel thick at any scale
        k = 0; yy = 16.0
        while yy < 50:
            ln = (x1 - x0 - 10) - (k * 7 % 13)
            line = pg & (on(y, yy)) & (x > x0 + 5) & (x < x0 + 5 + ln) & (((np.floor(x * S) * 7 + k * 3) % 11) != 0)
            img[line] = "r"; k += 1; yy += gap
    img[cover & (x > 54.5) & (x < 57.5)] = "K"  # the spine
    clasp = ["J", "H", "J"] if SMALL else ["JJJ", "JIJ", "JHJ", "JIJ", "JJJ"]
    put(img, clasp, 108.5, 32, center=True)  # a gold clasp
    return spec("tome", img, [56, 32])

# ---- far asteroids: three rough rocks, lit from the top-left --------------------------------------------------------
def asteroids():
    out = []
    for k, (w, h, seed) in enumerate([(44, 38, 61), (62, 50, 62), (84, 66, 63)]):
        img = canvas(w, h); x, y = grid(); cx, cy = w / 2, h / 2
        ang = np.arctan2(y - cy, x - cx); r = np.hypot((x - cx) / (w / 2 - 2), (y - cy) / (h / 2 - 2))
        lump = 1 + 0.12 * np.sin(ang * 3 + seed) + 0.08 * np.sin(ang * 7 + seed * 2)
        m = r < 0.8 * lump
        img[m] = shade(m, ["a", "b", "E", "d", "e"], bevel=6, tex=0.35, seed=seed, base=0.45)[m]
        rng = np.random.default_rng(seed)
        for _ in range(1 + k):  # craters
            ccx, ccy, rr = rng.uniform(0.3, 0.7) * w, rng.uniform(0.3, 0.7) * h, rng.uniform(2.5, 4 + k * 1.5) * max(1, 0.75 / S)
            dd = np.hypot(x - ccx, y - ccy); cr = m & (dd < rr) & (dist_u(m) > 3)
            img[cr] = "E"; img[cr & ((x - ccx) + (y - ccy) < -rr * 0.2)] = "b"  # the bowl, darker where the rim shades it
            img[m & (np.abs(dd - rr) < 0.8 / S) & ((x - ccx) + (y - ccy) > rr * 0.5)] = "e"
        out.append(spec(f"asteroid-{k}", img, [w / 2, h / 2]))
    return out

# ---- the void-energy icon in the top bar: a faceted crystal --------------------------------------------------------
def void_icon():
    img = canvas(15, 23); x, y = grid(); cx, cy = 7.5, 11.5
    dx, dy = x - cx, y - cy; m = np.abs(dx) / 6.2 + np.abs(dy) / 10.2 <= 1
    img[m & (dy < 0) & (dx <= 0)] = "Y"; img[m & (dy < 0) & (dx < 0) & (np.abs(dx) + np.abs(dy) < 6)] = "Z"
    img[m & (dy < 0) & (dx > 0)] = "X"; img[m & (dy >= 0) & (dx <= 0)] = "W"; img[m & (dy >= 0) & (dx > 0)] = "V"
    img[m & (on(y, cy))] = "X"
    return spec("void-icon", img, [7.5, 11.5])

# ---- a card back, for the Deck's fan on the lectern ------------------------------------------------------------------
def card_back():
    img = canvas(52, 70); x, y = grid()
    m = poly_mask([(2, 4), (4, 2), (48, 2), (50, 4), (50, 66), (48, 68), (4, 68), (2, 66)])
    img[m] = shade(m, ["K", "b", "L", "V"], bevel=4, tex=0.15, seed=71, base=0.42)[m]
    rim = m & (dist_u(m) > 3) & (dist_u(m) < 3 + 1.0 / S)
    img[rim] = "W"
    gem = np.abs(x - 26) / 10 + np.abs(y - 35) / 16 <= 1  # the void crystal on the back
    img[gem & (x <= 26)] = "X"; img[gem & (x > 26)] = "W"; img[gem & (np.abs(x - 26) / 10 + np.abs(y - 35) / 16 > 0.75)] = "V"
    return spec("card-back", img, [26, 35])

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

for f in (altar, lectern, standing_stone, gate_arch, sigil_star, pedestal, tome, asteroids, void_icon, card_back, void_ball, blob, puffs):
    f()
print("ok", OUT, "scale", S)
