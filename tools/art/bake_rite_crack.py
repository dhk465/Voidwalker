# Bakes the Void Rite's crack: our pixelfx effect tools/art/fx/arcane/void-crack.js (a crack in space-time with void
# seeping out), rendered by pixelfx (tools/pixelfx, MIT) and snapped to the game's 8 void violets.
#
# Each pixel takes the void colour nearest its brightness (the palette is one violet ramp, so brightness picks the
# step). Opaque pixels stay; translucent ones (the seeping wisps) stay where their alpha beats a 4x4 ordered dither,
# the same stipple the lobby's sky and stone use, and only from the third step up (fainter glow is left to the sky).
# Frames are cropped to the union of what every frame draws, so the pivot moves with the crop.
# Run from the repo root: python tools/art/bake_rite_crack.py   (needs Node for pixelfx)
import json, os, shutil, subprocess
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
PFX = os.path.join(ROOT, "tools", "pixelfx")
SRC = os.path.join(ROOT, "tools", "art", "fx", "arcane", "void-crack.js")
OUT = os.path.join(ROOT, "assets", "fx")
NAME = "rite-crack"
VOID = ["#07040d", "#1a1229", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff"]
OPAQUE = 0.98      # alpha at or above this is solid
FIRST_KEPT = 2     # translucent pixels darker than this step are dropped
COLUMNS = 12
BAYER = (np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]) + 0.5) / 16


def luma(rgb):
    return rgb[..., 0] * 0.3 + rgb[..., 1] * 0.59 + rgb[..., 2] * 0.11


def main():
    shutil.copy(SRC, os.path.join(PFX, "src", "fx", "arcane", "void-crack.js"))
    subprocess.run(["node", "tools/render.mjs", "arcane/void-crack", "--sheet"], cwd=PFX, check=True, capture_output=True)
    rec = os.path.join(PFX, "records", "arcane", "void-crack")
    meta = json.load(open(os.path.join(rec, "void-crack.json"), encoding="utf-8"))["meta"]["pixelfx"]
    sheet = np.asarray(Image.open(os.path.join(rec, "void-crack.png")).convert("RGBA"), dtype=float) / 255
    pal = np.array([[int(h[i:i + 2], 16) for i in (1, 3, 5)] for h in VOID]) / 255
    pal_l = luma(pal)
    fw, fh = meta["frameSize"]; cols, count = meta["columns"], meta["frames"]
    yy, xx = np.mgrid[0:fh, 0:fw]
    frames = []
    for k in range(count):
        f = sheet[(k // cols) * fh:(k // cols + 1) * fh, (k % cols) * fw:(k % cols + 1) * fw]
        step = np.abs(luma(f[..., :3])[..., None] - pal_l).argmin(-1)
        a = f[..., 3]
        keep = (a >= OPAQUE) | ((a > BAYER[yy % 4, xx % 4]) & (step >= FIRST_KEPT))
        frames.append(np.where(keep, step, -1))
    used = np.any(np.stack(frames) >= 0, axis=0)
    ys, xs = np.nonzero(used)
    x0, x1, y0, y1 = xs.min(), xs.max() + 1, ys.min(), ys.max() + 1
    w, h = x1 - x0, y1 - y0
    out = np.zeros((((count + COLUMNS - 1) // COLUMNS) * h, COLUMNS * w, 4), np.uint8)
    for k, f in enumerate(frames):
        c = f[y0:y1, x0:x1]; r, q = divmod(k, COLUMNS)
        tile = out[r * h:(r + 1) * h, q * w:(q + 1) * w]
        m = c >= 0
        tile[m, :3] = np.round(pal[c[m]] * 255); tile[m, 3] = 255
    Image.fromarray(out).save(os.path.join(OUT, NAME + ".png"))
    info = {
        "source": f"tools/art/fx/arcane/void-crack.js rendered by pixelfx {json.load(open(os.path.join(PFX, 'package.json')))['version']} "
                  "(MIT, https://github.com/patch-gg/pixelfx); snapped to the 8 void violets with ordered-dither translucency by tools/art/bake_rite_crack.py",
        "frame_size": [int(w), int(h)], "columns": COLUMNS, "frames": count, "fps": meta["fps"],
        "pivot": [int(meta["pivot"][0] - x0), int(meta["pivot"][1] - y0)],
    }
    json.dump(info, open(os.path.join(OUT, NAME + ".json"), "w", encoding="utf-8"), indent=1)
    print(NAME, f"{w}x{h}", count, "frames, pivot", info["pivot"])


if __name__ == "__main__":
    main()
