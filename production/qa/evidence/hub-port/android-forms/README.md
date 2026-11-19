# Android check: blob forms in the hub and the Pool

Checked on November 19, 2026 on the 25053PC47G (Android 16, 2772 × 1280, landscape) with debug build 0.2.0 (464802), installed over 464676 with `adb install -r`.

The player's save was copied off the phone first (SHA-256 `bc08c98c82faeffd038bb4fb03e7d8549550c15cae1c600b9d4c67faee95dd0d`). The test used that save with its pool replaced by two Envy cards and one Void Bolt, so the pool leans to Envy.

| Step | Observed | Screenshot |
| --- | --- | --- |
| Launch | The hub shows the green Serpent on the altar. | [01-hub.png](01-hub.png) |
| Tap the Deck station | The Pool panel shows the Serpent sprite, "Serpent" and its trait. | [02-pool.png](02-pool.png) |
| Tap the Envy row twice | The pool falls to 1 (the Void Bolt); the panel shows the shapeless blob, "Shapeless", "no trait". The save's loadout read back as `{"|1": 1}`. | [03-pool-shapeless.png](03-pool-shapeless.png) |
| Close the screen | The hub's blob is now shapeless. | [04-hub-shapeless.png](04-hub-shapeless.png) |

The game's log had no Godot error or crash; its four "error" lines came from the phone's system (MIUI preload and ActivityThread). With the game stopped, the original save was copied back and its SHA-256 matched the backup.
