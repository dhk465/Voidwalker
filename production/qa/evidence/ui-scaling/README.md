# UI and camera scaling

November 19, 2026. The game now picks the largest whole-number zoom that leaves at least 533 × 300 game pixels, instead of one that fits 640 × 360. A camera frames the hub, the HUD is pinned to the screen's edges, body text is Spleen 8 × 16 and titles are 12 × 24, and every station screen lays itself out in the area it gets, paging what does not fit.

| Window | Zoom | Game area | Body text on screen |
| --- | --- | --- | --- |
| 2772 × 1280 phone (before) | 3 | 924 × 426 | 36 px, about 2 mm |
| 2772 × 1280 phone (now) | 4 | 693 × 320 | 64 px, about 3.5 mm |
| 1280 × 720 desktop | 2 | 640 × 360 | 32 px |
| 1920 × 1080 desktop | 3 | 640 × 360 | 48 px |

## Desktop captures

Each image stacks three game-pixel captures at 2×: the phone's area (693 × 320, from a 1386 × 640 window), the minimum (533 × 300, from 1066 × 600) and desktop (640 × 360, from 1280 × 720). They were taken with the hub's `-- --shot=<png>` check option; Journal tabs used `-- --open=journal --tab=<name>`.

[Hub](hub.png) · [Pool](deck.png) · [Void Rite](rite.png) · [Rift Gate](gate.png) · [Sigils](sigils.png) · [Relics](relics.png) · Journal: [Quests](journal-quests.png), [Logs](journal-logs.png), [Codex](journal-codex.png), [Bestiary](journal-bestiary.png), [Natures](journal-natures.png)

## On the phone

Debug build 0.2.0 (464827) on the 25053PC47G (Android 16, 2772 × 1280), with the player's own save. [Hub at 4×](android-hub.png). Taps went through the camera to the Journal, its Codex tab, the close button, the Rift Gate and the Relics, each opening as expected ([sheet](android-taps.png)). The game's log had no Godot error. The save's SHA-256 (`bc08c98c…dd0d`) was the same before and after.
