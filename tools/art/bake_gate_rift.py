# Bakes the Rift Gate's rift: our gate-portal effect (tools/art/fx/arcane/gate-portal.js, a copy of pixelfx's
# arcane/portal, MIT) rendered by pixelfx (tools/pixelfx) at the gate's size, then cut to
# 7 hard brightness steps so canvas_env_rift_tint.gdshader can paint each step in an exact colour from the chosen
# rift's nature. The sheet stores a step per pixel (grey = step * 36, alpha 255; empty pixels alpha 0), not colour.
#
# Each pixel's brightness is its colour's luma times its alpha (what it shows over the dark void). Pixels kept: the
# opaque ones (the dark well stays, as step 0) and any translucent one bright enough to read. The portal's floor glow
# (the rows at its pivot) is cut: the gate stands on the altar's edge, not on a floor. Frames are cropped to the
# union of what every frame draws, so the pivot moves with the crop.
#
# Two sizes: gate-rift for the hub (between the stone horns), gate-rift-small for the Gate screen's rift cards.
# Run from the repo root: python tools/art/bake_gate_rift.py   (needs Node for pixelfx)
import json, os, shutil, subprocess, sys
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
PFX = os.path.join(ROOT, "tools", "pixelfx")
OUT = os.path.join(ROOT, "assets", "fx")
SRC = os.path.join(ROOT, "tools", "art", "fx", "arcane", "gate-portal.js")
VOID = ["#07040d", "#1a1229", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff"]  # the game's void violets
STEPS = [0.0, 0.06, 0.14, 0.26, 0.42, 0.62, 0.82]  # brightness where each of the 7 steps begins
STEP_GREY = 36  # step k is stored as grey k * 36
KEEP_ALPHA, KEEP_BRIGHT = 0.5, 0.10
COLUMNS = 12
SIZES = {"gate-rift": (18, 44, 12), "gate-rift-small": (10, 22, 4)}  # pixelfx width and height (the rift's half-axes), and how far past them motes may show


def render(width, height):
    """Renders the portal sheet with pixelfx; returns (sheet RGBA floats, pixelfx meta)."""
    # wind -1: the arms wind the way the motes fall in, a drain; flip: mirrored, so it all turns counter-clockwise
    shutil.copy(SRC, os.path.join(PFX, "src", "fx", "arcane", "gate-portal.js"))
    opts = json.dumps({"width": width, "height": height, "palette": VOID, "seed": 4, "wind": -1, "flip": True})
    subprocess.run(["node", "tools/render.mjs", "arcane/gate-portal", "--sheet", "--opts", opts], cwd=PFX, check=True, capture_output=True)
    rec = os.path.join(PFX, "records", "arcane", "gate-portal")
    meta = json.load(open(os.path.join(rec, "gate-portal.json"), encoding="utf-8"))["meta"]["pixelfx"]
    return np.asarray(Image.open(os.path.join(rec, "gate-portal.png")).convert("RGBA"), dtype=float) / 255, meta


def steps(frame, pivot, width, height, reach):
    """Step per pixel (-1 = empty), within `reach` pixels of the rift's oval box."""
    rgb, a = frame[..., :3], frame[..., 3]
    v = (0.3 * rgb[..., 0] + 0.59 * rgb[..., 1] + 0.11 * rgb[..., 2]) * a
    out = np.full(v.shape, -1, int)
    keep = (a > KEEP_ALPHA) | (v > KEEP_BRIGHT)
    for k, t in enumerate(STEPS):
        out[keep & (v >= t)] = k
    px, py = pivot
    out[py - 1:, :] = -1  # the floor glow
    top = py - (2 * height + 5) - reach  # pixelfx centres the rift height + 5 above its pivot
    out[:max(0, top), :] = -1
    out[:, :max(0, px - width - 4 - reach)] = -1; out[:, px + width + 4 + reach + 1:] = -1
    return out


def bake(name, width, height, reach):
    sheet, meta = render(width, height)
    fw, fh = meta["frameSize"]; cols, count = meta["columns"], meta["frames"]
    frames = [steps(sheet[(k // cols) * fh:(k // cols + 1) * fh, (k % cols) * fw:(k % cols + 1) * fw], meta["pivot"], width, height, reach) for k in range(count)]
    used = np.any(np.stack(frames) >= 0, axis=0)
    ys, xs = np.nonzero(used)
    x0, x1, y0, y1 = xs.min(), xs.max() + 1, ys.min(), ys.max() + 1
    w, h = x1 - x0, y1 - y0
    rows = (count + COLUMNS - 1) // COLUMNS
    out = np.zeros((rows * h, COLUMNS * w, 4), np.uint8)
    for k, f in enumerate(frames):
        c = f[y0:y1, x0:x1]; r, q = divmod(k, COLUMNS)
        tile = out[r * h:(r + 1) * h, q * w:(q + 1) * w]
        m = c >= 0
        tile[m, :3] = (c[m] * STEP_GREY)[:, None]; tile[m, 3] = 255
    Image.fromarray(out).save(os.path.join(OUT, name + ".png"))
    info = {
        "source": f"tools/art/fx/arcane/gate-portal.js (pixelfx arcane/portal, MIT, https://github.com/patch-gg/pixelfx) rendered by pixelfx "
                  f"{json.load(open(os.path.join(PFX, 'package.json')))['version']}, width {width}, height {height}, reach {reach}, seed 4, wind -1, mirrored, "
                  f"the game's void violets; cut to {len(STEPS)} brightness steps by tools/art/bake_gate_rift.py",
        "encoding": f"grey = step * {STEP_GREY}, steps 0..{len(STEPS) - 1}; colour comes from canvas_env_rift_tint.gdshader",
        "frame_size": [int(w), int(h)], "columns": COLUMNS, "frames": count, "fps": meta["fps"],
        "pivot": [int(meta["pivot"][0] - x0), int(meta["pivot"][1] - y0)],
    }
    json.dump(info, open(os.path.join(OUT, name + ".json"), "w", encoding="utf-8"), indent=1)
    print(name, f"{w}x{h}", count, "frames, pivot", info["pivot"])


if __name__ == "__main__":
    for name, (w, h, reach) in SIZES.items():
        if len(sys.argv) < 2 or name in sys.argv[1:]:
            bake(name, w, h, reach)
