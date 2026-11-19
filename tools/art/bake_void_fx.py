# Bakes the lobby's void effects, our own pixelfx effects in tools/art/fx/arcane/, rendered by pixelfx (tools/pixelfx,
# MIT) and snapped to the game's 8 void violets:
#   rite-crack  void-crack.js  the Void Rite's crack in space-time with void seeping out (a loop)
#   void-pop    void-pop.js    a tapped void ball bursting into puffs (a one-shot)
#
# Each pixel takes the void colour nearest its brightness (the palette is one violet ramp, so brightness picks the
# step). Opaque pixels stay; translucent ones (the seeping wisps) stay where their alpha beats a 4x4 ordered dither,
# the same stipple the lobby's sky and stone use, and only from the third step up (fainter glow is left to the sky).
# Frames are cropped to the union of what every frame draws, so the pivot moves with the crop.
# Run from the repo root: python tools/art/bake_void_fx.py [name ...]   (needs Node for pixelfx; no names = all)
import json, os, shutil, subprocess, sys
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
PFX = os.path.join(ROOT, "tools", "pixelfx")
FX = os.path.join(ROOT, "tools", "art", "fx", "arcane")
OUT = os.path.join(ROOT, "assets", "fx")
EFFECTS = {"rite-crack": "void-crack", "void-pop": "void-pop"}  # sheet name -> effect file (arcane/<file>.js)
VOID = ["#07040d", "#1a1229", "#2a1636", "#4a3a7a", "#7b5bdf", "#b9a6ff", "#d8c8ff", "#efe6ff"]
OPAQUE = 0.98      # alpha at or above this is solid
FIRST_KEPT = 2     # translucent pixels darker than this step are dropped
COLUMNS = 12
BAYER = (np.array([[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]) + 0.5) / 16


def luma(rgb):
    return rgb[..., 0] * 0.3 + rgb[..., 1] * 0.59 + rgb[..., 2] * 0.11


def bake(name, fx):
    shutil.copy(os.path.join(FX, fx + ".js"), os.path.join(PFX, "src", "fx", "arcane", fx + ".js"))
    subprocess.run(["node", "tools/render.mjs", "arcane/" + fx, "--sheet"], cwd=PFX, check=True, capture_output=True)
    rec = os.path.join(PFX, "records", "arcane", fx)
    meta = json.load(open(os.path.join(rec, fx + ".json"), encoding="utf-8"))["meta"]["pixelfx"]
    sheet = np.asarray(Image.open(os.path.join(rec, fx + ".png")).convert("RGBA"), dtype=float) / 255
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
    Image.fromarray(out).save(os.path.join(OUT, name + ".png"))
    info = {
        "source": f"tools/art/fx/arcane/{fx}.js rendered by pixelfx {json.load(open(os.path.join(PFX, 'package.json')))['version']} "
                  "(MIT, https://github.com/patch-gg/pixelfx); snapped to the 8 void violets with ordered-dither translucency by tools/art/bake_void_fx.py",
        "frame_size": [int(w), int(h)], "columns": COLUMNS, "frames": count, "fps": meta["fps"], "loop": bool(meta.get("loop", True)),
        "pivot": [int(meta["pivot"][0] - x0), int(meta["pivot"][1] - y0)],
    }
    json.dump(info, open(os.path.join(OUT, name + ".json"), "w", encoding="utf-8"), indent=1)
    print(name, f"{w}x{h}", count, "frames, pivot", info["pivot"], "loop" if info["loop"] else "once")


if __name__ == "__main__":
    for name, fx in EFFECTS.items():
        if len(sys.argv) < 2 or name in sys.argv[1:]:
            bake(name, fx)
