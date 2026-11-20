# Bakes the adventure's pixelfx effects (our own, in tools/art/fx/<category>/, and pixelfx's own where one fits) into
# sprite sheets for the Godot rift.
# Each effect is copied into the pixelfx checkout (tools/pixelfx, MIT, https://github.com/patch-gg/pixelfx), rendered
# there with `tools/render.mjs <id> --sheet` (with the options below), and every pixel is snapped to the nearest colour
# of the game palette (src/data/game_data.json). Opaque pixels stay; translucent ones (glow, vapour) stay where their
# alpha beats a 4x4 ordered dither, the stipple the lobby uses. Frames are cropped to the union of what every frame draws,
# so the pivot moves with the crop.
#
# Output: assets/sprites/rift/<sheet>.png, and the sheet's entry in assets/sprites/rift/rift-sheets.json (frames, fps,
# loop, pivot), alongside the other rift sheets.
# Run from the repo root: python tools/art/bake_rift_fx.py [sheet ...]   (needs Node; no names = all)
import json, os, shutil, subprocess, sys
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
PFX = os.path.join(ROOT, "tools", "pixelfx")
SRC = os.path.join(ROOT, "tools", "art", "fx")
OUT = os.path.join(ROOT, "assets", "sprites", "rift")
MANIFEST = os.path.join(OUT, "rift-sheets.json")
VOIDRAMP = ["#1a1229", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff", "#ffffff"]
BILERAMP = ["#3a0a1c", "#6f7a1e", "#b5c43a", "#c8e04a", "#d6e25a", "#e8e6c0", "#ffffff"]
METAL = ["#07040d", "#241a3a", "#353a44", "#45404d", "#8b95a3", "#8a8296", "#cfc6e6", "#d8f4ff"]
BOLT_STEPS = ["#555555", "#aaaaaa", "#ffffff"]  # the bolt keeps three grey steps; the game paints them per card
# sheet name -> (effect id under tools/art/fx or pixelfx's own, options, keep):
#   options is one dict (an animation laid out in reading order, 12 to a row) or a list of dicts (one row each,
#   a frame per column: headings, variants); keep is the palette pixels are snapped to (the game's by default).
SHEETS = {
    "pod": ("critters/ybasog-pod", {}, None),
    "pod-crack": ("critters/ybasog-pod", {"crack": 1}, None),
    "pod-burst": ("spawn/pod-burst", {}, None),
    # rows: heading d (0..15, d * 2) and dive (+1)
    "grazer": ("critters/grazer", [{"dir": d, "dive": v} for d in range(16) for v in (0, 1)], None),
    # rows: calm, gaping before a spit
    "feeder": ("critters/feeder", [{"gape": 0}, {"gape": 1}], None),
    # rows: plain, void crystal, scrap metal, both; columns: twelve steps of one turn
    **{f"rock-{i}": ("nature/asteroid", [{"seed": i + 1, "void": v, "scrap": sc} for v, sc in ((0, 0), (1, 0), (0, 1), (1, 1))], None) for i in range(3)},
    "slab": ("scifi/debris-slab", {}, None),
    "scrap": ("pickups/scrap-bit", {}, None),
    "rock-break": ("nature/rock-break", {}, None),
    "slab-break": ("nature/rock-break", {"palette": METAL, "seed": 5}, None),
    # rows: heading d (0..15, d * 2) and a fused card's rings (+1)
    "bolt": ("projectiles/void-bolt", [{"dir": d, "rings": r} for d in range(16) for r in (0, 2)], BOLT_STEPS),
    # rows: heading d (0..7)
    "glob": ("projectiles/bile-glob", [{"dir": d} for d in range(8)], None),
    # pixelfx's own hit spark, recoloured: a bolt striking (void), a glob bursting on the blob (bile)
    # rows: the blow's direction, k * 45 degrees (screen angle, y down: row = round(atan2(vy, vx) / 45 deg) mod 8)
    "hit-spark": ("impacts/hit-spark", [{"palette": VOIDRAMP, "height": 0, "aim": 0, "angle": k * 0.785398} for k in range(8)], None),
    "glob-splash": ("impacts/hit-spark", [{"palette": BILERAMP, "height": 0, "aim": 0, "seed": 9, "angle": k * 0.785398} for k in range(8)], None),
}
COLUMNS = 12
BAYER = (np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]) + 0.5) / 16
PALETTE = json.load(open(os.path.join(ROOT, "src", "data", "game_data.json"), encoding="utf-8"))["palette"]
PAL = np.array([[int(h[i:i + 2], 16) for i in (1, 3, 5)] for h in PALETTE], float)


def render(fx, opts, pal):
    """Renders one option set with pixelfx; returns (frames as palette indices, meta)."""
    cat, eff = fx.split("/")
    ours = os.path.join(SRC, cat, eff + ".js")
    if os.path.exists(ours):  # our own effect: copied in; otherwise pixelfx's own, used as it is
        os.makedirs(os.path.join(PFX, "src", "fx", cat), exist_ok=True)
        shutil.copy(ours, os.path.join(PFX, "src", "fx", cat, eff + ".js"))
    cmd = ["node", "tools/render.mjs", fx, "--sheet"] + (["--opts", json.dumps(opts)] if opts else [])
    subprocess.run(cmd, cwd=PFX, check=True, capture_output=True)
    rec = os.path.join(PFX, "records", cat, eff)
    meta = json.load(open(os.path.join(rec, eff + ".json"), encoding="utf-8"))["meta"]["pixelfx"]
    sheet = np.asarray(Image.open(os.path.join(rec, eff + ".png")).convert("RGBA"), dtype=float)
    fw, fh = meta["frameSize"]; cols, count = meta["columns"], meta["frames"]
    yy, xx = np.mgrid[0:fh, 0:fw]
    frames = []
    for k in range(count):
        f = sheet[(k // cols) * fh:(k // cols + 1) * fh, (k % cols) * fw:(k % cols + 1) * fw]
        a = f[..., 3] / 255
        # a translucent pixel's colour is premultiplied over black (what it shows over the dark rift)
        rgb = f[..., :3] * np.where(a >= 0.98, 1, a)[..., None]
        idx = ((rgb[..., None, :] - pal) ** 2).sum(-1).argmin(-1)
        keep = (a >= 0.98) | ((a > BAYER[yy % 4, xx % 4]) & (rgb.max(-1) > 24))
        frames.append(np.where(keep, idx, -1))
    return frames, meta


def bake(name, fx, opts, keep):
    pal = PAL if keep is None else np.array([[int(h[i:i + 2], 16) for i in (1, 3, 5)] for h in keep], float)
    sets = opts if isinstance(opts, list) else [opts]
    rendered = [render(fx, o, pal) for o in sets]
    meta = rendered[0][1]
    used = np.any(np.stack([f for frames, _ in rendered for f in frames]) >= 0, axis=0)
    ys, xs = np.nonzero(used)
    x0, x1, y0, y1 = xs.min(), xs.max() + 1, ys.min(), ys.max() + 1
    w, h = int(x1 - x0), int(y1 - y0)
    count = len(rendered[0][0])
    # one option set: reading order, COLUMNS to a row; several: one row each, a frame per column
    cols = COLUMNS if len(sets) == 1 else count
    cells = [(k, f) for _, (frames, _) in enumerate(rendered) for k, f in enumerate(frames)]
    rows = (count + COLUMNS - 1) // COLUMNS if len(sets) == 1 else len(sets)
    out = np.zeros((rows * h, cols * w, 4), np.uint8)
    for i, (k, f) in enumerate(cells):
        r, q = divmod(k, COLUMNS) if len(sets) == 1 else (i // count, k)
        c = f[y0:y1, x0:x1]
        tile = out[r * h:(r + 1) * h, q * w:(q + 1) * w]
        m = c >= 0
        tile[m, :3] = pal[c[m]]; tile[m, 3] = 255
    Image.fromarray(out).save(os.path.join(OUT, name + ".png"))
    version = json.load(open(os.path.join(PFX, "package.json")))["version"]
    own = os.path.exists(os.path.join(SRC, *fx.split("/")[:1], fx.split("/")[1] + ".js"))
    return {"file": name + ".png", "frame": [w, h], "cols": cols, "rows": rows, "frames": count, "fps": meta["fps"],
            "loop": bool(meta.get("loop", True)), "anchor": [int(meta["pivot"][0] - x0), int(meta["pivot"][1] - y0)],
            "variants": len(sets),
            "source": (f"tools/art/fx/{fx}.js" if own else f"pixelfx {fx}") + f" rendered by pixelfx {version} (MIT), "
                      + ("kept to its grey steps" if keep else "snapped to the game palette") + " by tools/art/bake_rift_fx.py"}


if __name__ == "__main__":
    man = json.load(open(MANIFEST)) if os.path.exists(MANIFEST) else {"schema": "voidwalker.rift_sheets.v1", "sheets": {}}
    for name, (fx, opts, keep) in SHEETS.items():
        if len(sys.argv) < 2 or name in sys.argv[1:]:
            man["sheets"][name] = bake(name, fx, opts, keep)
            e = man["sheets"][name]
            print(name, f"{e['frame'][0]}x{e['frame'][1]}", e["frames"], "frames", "loop" if e["loop"] else "once", "pivot", e["anchor"])
    json.dump(man, open(MANIFEST, "w"), indent=1)
