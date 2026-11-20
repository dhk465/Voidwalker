# Packs rendered lobby sprites (codeart2d render_pixelspec output) into the Godot project's assets folder.
# Each sprite gets a 3 px transparent margin (room for the selection outline and its ring, drawn by
# station_highlight.gdshader inside the sprite's own quad); anchors move with the margin. The puffs become one strip.
# Run: python tools/art/pack_lobby.py <render-dir> <specs-dir> <out-dir>
import glob, json, os, sys
from PIL import Image

RENDER, SPECS, OUT = sys.argv[1:4]
PAD = 3
os.makedirs(OUT, exist_ok=True)
man = {"schema": "voidwalker.lobby_sprites.v2", "art_source": "code", "disclosure": "code-drawn, no image model",
       "made_by": "tools/art/gen_lobby.py --scale 0.5 -> codeart2d render_pixelspec.py --strict-qc -> tools/art/pack_lobby.py",
       "pad": PAD, "sprites": {}}
puffs = []
for meta in sorted(glob.glob(os.path.join(RENDER, "*", "codeart-meta.json"))):
    name = os.path.basename(os.path.dirname(meta))
    spec = json.load(open(os.path.join(SPECS, name + ".pixelspec.json"), encoding="utf-8"))
    qa = json.load(open(meta)).get("qa", {}).get("status")
    if qa != "pass": raise SystemExit(f"{name}: QA status {qa}, not packing it")
    for png in sorted(glob.glob(os.path.join(RENDER, name, "*", "frames", "*.png"))):
        variant = os.path.normpath(png).split(os.sep)[-3]; key = name if variant == "base" else name + "-" + variant
        im = Image.open(png).convert("RGBA")
        if name.startswith("puff-"):
            puffs.append((int(name.split("-")[1]), im)); continue
        out = Image.new("RGBA", (im.width + 2 * PAD, im.height + 2 * PAD), (0, 0, 0, 0)); out.paste(im, (PAD, PAD))
        out.save(os.path.join(OUT, key + ".png"))
        ax, ay = spec["anchor_px"]
        man["sprites"][key] = {"file": key + ".png", "size": [out.width, out.height], "anchor": [ax + PAD, ay + PAD], "qa": qa}
puffs.sort()
if puffs:  # the lobby's flame puffs become one strip; the rift's set has none
    w, h = puffs[0][1].size
    strip = Image.new("RGBA", (w * len(puffs), h), (0, 0, 0, 0))
    for i, (_, im) in enumerate(puffs): strip.paste(im, (i * w, 0))
    strip.save(os.path.join(OUT, "puff-strip.png"))
    man["sprites"]["puff-strip"] = {"file": "puff-strip.png", "size": [strip.width, strip.height], "frames": len(puffs), "frame_size": [w, h]}
json.dump(man, open(os.path.join(OUT, "lobby-sprites.json"), "w"), indent=1)
print(len(man["sprites"]), "packed into", OUT)
