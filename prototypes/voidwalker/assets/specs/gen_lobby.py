# PROTOTYPE - NOT FOR PRODUCTION
# Writes the lobby's PixelSpecs (codeart2d.pixelspec.v1): code-drawn pixel art, no image model.
# Big props are shaded by a small pixel "shader": a hard mask, a bevel from the distance to its edge, light from the
# top-left, a little value noise for stone, then exact palette steps chosen with a 4x4 ordered dither. Small details
# (runes, clasps, page lines, inlays) are placed pixel by pixel on top. Run: python gen_lobby.py <out-dir>
import json, math, os, sys
import numpy as np

OUT = sys.argv[1] if len(sys.argv) > 1 else os.path.dirname(os.path.abspath(__file__))
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

def grid(w, h):
    y, x = np.mgrid[0:h, 0:w]
    return x + 0.5, y + 0.5

def poly_mask(w, h, pts):
    """Even-odd fill of a polygon at pixel centres."""
    x, y = grid(w, h); m = np.zeros((h, w), bool); n = len(pts)
    for i in range(n):
        x0, y0 = pts[i]; x1, y1 = pts[(i + 1) % n]
        if y0 == y1: continue
        cond = ((y0 <= y) & (y < y1)) | ((y1 <= y) & (y < y0))
        xc = x0 + (y - y0) * (x1 - x0) / (y1 - y0)
        m ^= cond & (x < xc)
    return m

def ellipse_mask(w, h, cx, cy, rx, ry):
    x, y = grid(w, h); return ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1

def dist_in(m, cap=64):
    """Chamfer distance (px) from each inside pixel to the nearest outside pixel."""
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

def vnoise(w, h, scale, seed):
    rng = np.random.default_rng(seed); gw, gh = int(w / scale) + 3, int(h / scale) + 3; g = rng.random((gh, gw))
    x, y = grid(w, h); fx, fy = x / scale, y / scale; ix, iy = fx.astype(int), fy.astype(int); tx, ty = fx - ix, fy - iy
    tx, ty = tx * tx * (3 - 2 * tx), ty * ty * (3 - 2 * ty)
    a, b, c, d = g[iy, ix], g[iy, ix + 1], g[iy + 1, ix], g[iy + 1, ix + 1]
    return a + (b - a) * tx + (c - a) * ty + (a - b - c + d) * tx * ty

def shade(m, ramp, bevel=4.0, tex=0.0, seed=1, base=0.55, light=(-0.6, -0.75, 0.6), tilt=None):
    """The pixel shader: bevel normals from the edge distance, lambert light, noise, dithered ramp steps."""
    h, w = m.shape; dd = dist_in(m); hgt = np.clip(dd / bevel, 0, 1); hgt = hgt * hgt * (3 - 2 * hgt)
    gy, gx = np.gradient(hgt); L = np.array(light) / np.linalg.norm(light)
    nz = np.full_like(gx, 0.35); nl = np.sqrt(gx ** 2 + gy ** 2 + nz ** 2)
    lam = (-gx * L[0] - gy * L[1] + nz * L[2]) / nl  # the surface tips up toward the edge
    v = base + 0.55 * (lam - 0.45)
    if tilt is not None: v = v + tilt
    if tex: v = v + tex * (vnoise(w, h, 6, seed) - 0.5) + tex * 0.5 * (vnoise(w, h, 2.5, seed + 7) - 0.5)
    v = np.clip(v, 0, 0.999) * (len(ramp) - 1)
    y, x = np.mgrid[0:h, 0:w]; i = np.floor(v).astype(int); i = i + ((v - i) > BAYER[y % 4, x % 4])
    i = np.clip(i, 0, len(ramp) - 1)
    out = np.full((h, w), ".", object); out[m] = np.array(ramp, object)[i[m]]
    return out

def put(img, rows, ox, oy):
    for j, r in enumerate(rows):
        for i, ch in enumerate(r):
            if ch != "." and 0 <= oy + j < img.shape[0] and 0 <= ox + i < img.shape[1]: img[oy + j, ox + i] = ch

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
    sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "..", "..", ".claude", "skills", "codeart2d", "scripts"))
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
    sp = {"schema": "codeart2d.pixelspec.v1", "name": name, "canvas": [w, h], "anchor_px": anchor,
          "palette": {k: PAL[k] for k in sorted(set(used) | ({"o"} if outline else set()))},
          "layers": [{"name": "body", "origin": [0, 0], "segments": segments(img)}]}
    if variants: sp["variants"] = {k: {c: v for c, v in o.items() if c in sp["palette"]} for k, o in variants.items()}
    if outline: sp["outline"] = {"mode": "solid", "color": "o"}
    with open(os.path.join(OUT, name + ".pixelspec.json"), "w", encoding="utf-8") as f: json.dump(sp, f, indent=1)
    return sp

def blank(w, h): return np.full((h, w), ".", object)

# ---- the floating altar: a carved stone disc on a rough island of earth and rock, crystals hanging below ----------
def altar():
    W, H = 704, 262; cx, top = 352, 82  # the disc's centre; canvas origin sits at (288, 433) in the hub
    img = blank(W, H)
    under = poly_mask(W, H, [(26, 92), (678, 92), (546, 196), (420, 236), (352, 254), (286, 236), (160, 198)])
    yy, xx = np.mgrid[0:H, 0:W]  # lumps along the lower edge: rocks, but a smooth outline
    under &= ~((dist_in(under) < 4) & (np.sin(xx / 7.0) * np.sin(yy / 5.0 + xx / 31.0) > 0.55))
    img[under] = shade(under, EARTH, bevel=10, tex=0.22, seed=4, base=0.42, light=(-0.4, -0.9, 0.5), tilt=0.12 * np.sin(np.mgrid[0:H, 0:W][1] / 37.0) - 0.002 * (np.mgrid[0:H, 0:W][0] - 92))[under]
    y, x = np.mgrid[0:H, 0:W]
    strata = under & (np.abs(((y + 4 * np.sin(x / 29.0)) % 16) - 8) < 0.9) & (vnoise(W, H, 14, 9) > 0.35)
    img[strata] = "p"
    for cxr, ln in [(250, 22), (330, 34), (402, 26), (470, 18), (204, 14)]:  # teal crystals hanging off the bottom
        for k in range(ln):
            half = max(1, (ln - k) // 6)
            yy = int(200 + (abs(cxr - 352) < 90) * 26) + k
            for dx in range(-half, half + 1):
                if 0 <= yy < H: img[yy, cxr + dx] = "w" if dx < 0 else ("v" if dx == 0 else "u")
    side = ellipse_mask(W, H, cx, top + 14, 335, 72) & ~ellipse_mask(W, H, cx, top, 335, 72) & (y > top)
    img[side] = shade(side, STONE, bevel=3, tex=0.25, seed=5, base=0.28)[side]
    img[side & (x % 9 == 0)] = "b"  # carved blocks round the rim
    disc = ellipse_mask(W, H, cx, top, 335, 72)
    radial = ((x - cx) / 335.0) ** 2 + ((y - top) / 72.0) ** 2
    tilt = -0.18 * (x - cx) / 335.0 - 0.22 * (y - top) / 72.0  # lit from the top-left across the disc
    img[disc] = shade(disc, COOL, bevel=6, tex=0.18, seed=6, base=0.5, tilt=tilt)[disc]
    ang = np.arctan2((y - top) / 72.0, (x - cx) / 335.0)
    seams = disc & (radial > 0.12) & (np.abs(((ang / (2 * math.pi) * 24) % 1) - 0.5) > 0.47)
    img[seams] = "d"
    for rr in (0.36, 0.12):  # rings carved into the floor
        img[disc & (np.abs(radial - rr) < 0.008)] = "d"
    ring = disc & (np.abs(np.sqrt(radial) - 0.8) < 0.025)  # the teal inlay that glows in the floor
    img[ring] = np.where(y[ring] < top, "v", "u")
    img[disc & (np.abs(np.sqrt(radial) - 0.8) < 0.007) & (y < top - 30)] = "w"
    dais = ellipse_mask(W, H, cx, top - 2, 110, 24)
    img[dais] = shade(dais, COOL, bevel=3, base=0.62, tilt=-0.1 * (y - top) / 24.0)[dais]
    return spec("altar", img, [cx, top])

# ---- the Deck's lectern: a carved stone stand with a slanted top --------------------------------------------------
def lectern():
    W, H = 64, 50; img = blank(W, H)
    body = poly_mask(W, H, [(4, 6), (60, 6), (50, 40), (14, 40)]) | poly_mask(W, H, [(10, 40), (54, 40), (58, 48), (6, 48)])
    img[body] = shade(body, COOL, bevel=4, tex=0.2, seed=11, base=0.48)[body]
    topf = poly_mask(W, H, [(2, 2), (62, 2), (60, 8), (4, 8)])
    img[topf] = shade(topf, COOL, bevel=2, base=0.75)[topf]
    put(img, ["..vvv..", ".v...v.", "v..w..v", ".v...v.", "..vvv.."], 29, 18)  # a rune of the void, inlaid
    img[40, 12:52] = "d"
    return spec("lectern", img, [32, 48])

# ---- a standing stone of the Rite, with a carved rune; dim for the back of the ring --------------------------------
def standing_stone():
    W, H = 22, 34; img = blank(W, H)
    m = poly_mask(W, H, [(4, 6), (8, 2), (14, 2), (18, 8), (19, 32), (3, 32)])
    img[m] = shade(m, STONE, bevel=3, tex=0.3, seed=21, base=0.5)[m]
    put(img, ["..w..", ".v.v.", "v.w.v", ".v.v.", "..v..", "..v.."], 8, 10)
    return spec("standing-stone", img, [11, 32], variants={"lit": {}, "far": {"e": "#45404d", "f": "#6d6488", "g": "#8a8296", "h": "#8b95a3", "w": "#2e5a55", "v": "#16302e"}})

# ---- the Rift Gate's arch, one horn (the right one is the same, mirrored) -----------------------------------------
def gate_arch():
    W, H = 92, 244; img = blank(W, H)
    def quad(p0, p1, p2, n=40):
        return [((1 - t) ** 2 * p0[0] + 2 * (1 - t) * t * p1[0] + t * t * p2[0], (1 - t) ** 2 * p0[1] + 2 * (1 - t) * t * p1[1] + t * t * p2[1]) for t in [i / n for i in range(n + 1)]]
    # in the hub this horn spans x 875..967 and y 288..532 (the left one); points are local to that box
    outer = quad((46, 232), (2, 112), (58, 2)); inner = quad((76, 14), (40, 112), (78, 232))
    m = poly_mask(W, H, outer + inner)
    img[m] = shade(m, STONE, bevel=5, tex=0.3, seed=31, base=0.48)[m]
    y, x = np.mgrid[0:H, 0:W]; d = dist_in(m)
    runes = m & (d > 4) & (((y // 22) % 2) == 0) & ((y % 22) < 3) & (vnoise(W, H, 3, 33) > 0.35)
    img[runes] = "v"
    put(img, ["..JJ..", ".JIIJ.", "JIHHIJ", ".JIIJ.", "..JJ.."], 54, 6)  # a gold boss at the tip
    return spec("gate-arch", img, [0, 0])

# ---- a star of the Sigils constellation: bought (lit) or not yet ---------------------------------------------------
def sigil_star():
    img = blank(11, 11)
    put(img, [".....", "..w..", ".wZw.", "..w..", "....."], 3, 3)
    put(img, ["....J....", "....J....", "...JIJ...", "JJIIZIIJJ", "...JIJ...", "....J....", "....J...."], 1, 2)
    return spec("sigil-star", img, [5, 5], variants={"lit": {}, "dark": {"J": "#3b3350", "I": "#4a3a7a", "Z": "#6d6488", "w": "#4a3a7a"}})

# ---- a relic pedestal: open (stone) or still locked (sunk in shadow) ------------------------------------------------
def pedestal():
    W, H = 44, 50; img = blank(W, H)
    m = poly_mask(W, H, [(4, 2), (40, 2), (40, 10), (34, 12), (32, 40), (38, 42), (40, 48), (4, 48), (6, 42), (12, 40), (10, 12), (4, 10)])
    img[m] = shade(m, COOL, bevel=3, tex=0.2, seed=41, base=0.5)[m]
    y, x = np.mgrid[0:H, 0:W]
    img[m & (y >= 14) & (y <= 38) & ((x == 16) | (x == 22) | (x == 28))] = "d"  # fluting
    img[m & (y == 11)] = "d"
    put(img, ["vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"], 6, 3)
    return spec("pedestal", img, [22, 48], variants={"open": {}, "locked": {"E": "#1a1229", "d": "#241a3a", "e": "#353a44", "f": "#45404d", "F": "#45404d", "h": "#6d6488", "v": "#241a3a"}})

# ---- the Journal: an open tome, leather and gold, its pages written --------------------------------------------------
def tome():
    W, H = 112, 64; img = blank(W, H)
    cover = poly_mask(W, H, [(2, 10), (54, 4), (58, 6), (110, 4), (110, 56), (58, 60), (54, 60), (2, 56)])
    img[cover] = shade(cover, LEATHER, bevel=3, tex=0.25, seed=51, base=0.5)[cover]
    for side, (x0, x1) in enumerate([(6, 54), (58, 106)]):
        pg = poly_mask(W, H, [(x0, 10 if side == 0 else 6), (x1, 6 if side == 0 else 10), (x1, 54), (x0, 54)])
        y, x = np.mgrid[0:H, 0:W]
        curl = 0.15 * np.cos((x - (x0 + x1) / 2) / (x1 - x0) * math.pi)
        img[pg] = shade(pg, ["s", "P", "P", "P"], bevel=3, base=0.7, tilt=curl)[pg]
        for k in range(8):  # lines of writing, ragged at the end
            yy = 16 + k * 4; ln = (x1 - x0 - 10) - (k * 7 % 13)
            for xx in range(x0 + 5, x0 + 5 + ln):
                if (xx * 7 + k * 3) % 11 != 0: img[yy, xx] = "r"
    img[6:58, 55:57] = "K"  # the spine
    put(img, ["JJJ", "JIJ", "JHJ", "JIJ", "JJJ"], 107, 27)  # a gold clasp
    put(img, ["IJI", "JHJ"], 3, 50)
    return spec("tome", img, [56, 32])

# ---- far asteroids: three rough rocks, lit from the top-left --------------------------------------------------------
def asteroids():
    out = []
    for k, (w, h, seed) in enumerate([(44, 38, 61), (62, 50, 62), (84, 66, 63)]):
        img = blank(w, h); y, x = np.mgrid[0:h, 0:w]; cx, cy = w / 2, h / 2
        ang = np.arctan2(y - cy, x - cx); r = np.hypot((x - cx) / (w / 2 - 2), (y - cy) / (h / 2 - 2))
        lump = 1 + 0.12 * np.sin(ang * 3 + seed) + 0.08 * np.sin(ang * 7 + seed * 2)
        m = r < 0.8 * lump
        img[m] = shade(m, ["a", "b", "E", "d", "e"], bevel=6, tex=0.35, seed=seed, base=0.45)[m]
        rng = np.random.default_rng(seed)
        for _ in range(1 + k):  # craters
            ccx, ccy, rr = rng.uniform(0.3, 0.7) * w, rng.uniform(0.3, 0.7) * h, rng.uniform(2.5, 4 + k * 1.5)
            dd = np.hypot(x - ccx, y - ccy); cr = m & (dd < rr) & (dist_in(m) > 3)
            img[cr] = "E"; img[cr & ((x - ccx) + (y - ccy) < -rr * 0.2)] = "b"  # the bowl, darker where the rim shades it
            img[m & (np.abs(dd - rr) < 0.8) & ((x - ccx) + (y - ccy) > rr * 0.5)] = "e"
        out.append(spec(f"asteroid-{k}", img, [w // 2, h // 2]))
    return out

# ---- the void-energy icon in the top bar: a faceted crystal --------------------------------------------------------
def void_icon():
    img = blank(15, 23); cx, cy = 7, 11
    for yy in range(23):
        for xx in range(15):
            dx, dy = xx + 0.5 - (cx + 0.5), yy + 0.5 - (cy + 0.5)
            if abs(dx) / 6.2 + abs(dy) / 10.2 <= 1:
                img[yy, xx] = ("Z" if dx < 0 and dy < 0 and abs(dx) + abs(dy) < 6 else "Y") if dy < 0 and dx <= 0 else ("X" if dy < 0 else ("W" if dx <= 0 else "V"))
    img[cy, 1:14] = np.where(img[cy, 1:14] != ".", "X", ".")
    return spec("void-icon", img, [7, 11])

for f in (altar, lectern, standing_stone, gate_arch, sigil_star, pedestal, tome, asteroids, void_icon):
    f()
print("ok", OUT)
