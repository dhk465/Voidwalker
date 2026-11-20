# The adventure's core loop in Godot: evidence, 2026-11-19

Checked in desktop Godot 4.8 beta 4 .NET on this PC. It hasn't been checked on a phone; the user's rule is to test on the PC unless they ask for a device test. Your save was backed up first, the check runs banked into it, and then it was restored. Its SHA-256 (`7af439cb…`) matched before and after.

| File | What it shows |
|---|---|
| `rift-play-and-return-sheet.png` | The full check sheet, each frame at 2×: the 640×360 window at 29.5 s (stage 1, wave 1) and at 62 s (wave 2: pods and the asteroid rockfall), a phone-shaped 693×320 window at 45 s (a grazer diving), and the Rift Altar after `--return-at-end` |
| `rift-wave2-640x360.png`, `rift-phone-693x320.png` | Two of those frames at their real size |
| `rift-end-stage-cleared-640x360.png` | The end screen after a whole stage on autoplay: STAGE CLEARED, 24 kills, 119 void banked |
| `gate-enter-640x360.png` | The Gate with ENTER THE RIFT enabled |
| `rift-sprites-x6.png` | The new code-drawn rift sprites (pod, grazer, feeder, three asteroids, the slab, 15 blob forms at rift size) at 6× |

## What was run

- `dotnet build Voidwalker.sln`: 0 warnings and 0 errors. `dotnet test`: 57 of 57 passed, including the 14 new tests in `RiftRunTests`.
- `Rift.tscn -- --autoplay --speed=6 --shot-at=end`, twice. Both runs reached StageCleared at stage 1, wave 3 with no engine errors (24 kills and 119 void, then 37 kills and 129 void).
- `-- --autoplay --speed=6 --return-at-end`: the end screen hands back to `Hub.tscn`, and the hub draws with the banked void energy.
- Headless `--open=` for all six stations: no errors.

## Not checked

- Real touch on a phone: the drag stick and auto-aim. The code is in place but only ran through autoplay.
- The mouse firing and the pause screen by hand. No input automation exists for them yet.
- Sound. The port has no audio yet.

## Rework after review, 2026-11-19

You said: *"Proper animations and movements, directions were not implemented for adventures, e.g. Grazers do not face the player character before walking forward. The projectiles are too small. The ybasog sprites are smudged."* The earlier images in this folder (`rift-*.png`) show the first pass, before this rework.

| File | What it shows |
|---|---|
| `rework-sheets-x5.png` | The new sheets at 5x: the pod (pulse and crack), the feeder (sway and gape), the slab (flicker), three rocks through their turns, and the grazer in every other heading with its walk frames, then with red dive eyes |
| `rework-grazer-turn-then-charge.png` | 16 frames from 37.4 s to 39.4 s, 3x: the grazers fall facing down; at the dive line one stops, turns to face the blob, its eyes go red, and it charges along that heading |
| `rework-play-640x360-x2.png` | The 640x360 window at 38.8 s and 52 s, 2x: grazers turning, pods, the bigger bolts with short trails, a feeder and a glob |
| `rework-phone-693x320.png` | The phone-shaped window at 29.8 s |

What I ran:

- Tests: 59 of 59 pass. The three new ones (a grazer stops and turns before charging, it charges along its heading, rocks spin) were run first against the old code and failed.
- Build: 0 warnings and 0 errors.
- A full stage on autoplay at 6x: StageCleared (38 kills, 144 void banked) with no engine errors.
- A 50-second recording at 30 fps: no errors.
- Your save was backed up first, the check runs banked into it, and then it was restored. Its hash, `7af439cb…`, matched before and after.
