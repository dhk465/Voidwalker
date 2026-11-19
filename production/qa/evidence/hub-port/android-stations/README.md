# Android station smoke test

All six hub stations passed touch testing on November 19, 2026. The device was a 25053PC47G running Android 16 (API 36) at 2772 × 1280 in landscape. The installed ARM64 debug APK was Voidwalker 0.2.0 (464676). Its SHA-256 matched the signed build artifact, `6a5a856e977664f248754438727f5e166a4375b85a9da3dee32de51bcebd9ce7`.

The test used a separate game profile with two Breadth upgrades, 16 distinct equipped stacks, and 930 void. Each check used Android touch input and read back the resulting test save. [Six station screenshots](six-stations.png)

| Station | Action | Observed result |
| --- | --- | --- |
| Pool | Tap Hunger, the last of 16 visible stacks. | The pool fell to 15 and the Hunger stack was removed from the saved loadout. [Pool after removal](02-pool-removed.png) |
| Void Rite | Draw one card with Wrath focused. | The draw counter increased by one and the bank fell from 930 to 890. |
| Sigils | Buy the next Vessel upgrade. | Vessel reached level 2 and the bank fell from 890 to 750. |
| Relics | Tap Drowned Bell. | The saved equipment list included the Bell alongside the Crown. |
| Rift Gate | Select the second rift and increase depth. | The save recorded the second rift and depth 1. |
| Journal | Open Natures and tap Reset to Demo twice. | After the first tap the test save matched its previous bytes. The second tap restored the first-launch collection. [First tap](08-reset-armed.png) · [Confirmed reset](09-reset-confirmed.png) |

After a cold restart, the restored collection matched the first-launch demo and could draw a card. Both game processes completed the checked flows without a Godot error or fatal exception in their logs. With the game stopped, the original save was restored and its SHA-256 matched the pre-install backup.

## Build and test from a clean checkout

Use the Godot 4.8 beta 4 .NET editor, its matching Mono export templates, .NET 10, Git Bash or Bash, and a Godot Android export environment with JDK 17 and the Android SDK. Set `GODOT` to the editor executable if the configured path and the `godot` command are unavailable. From the repository root, run

```sh
dotnet build Voidwalker.sln
dotnet test Voidwalker.sln
bash tools/build/android_export.sh
```

The Android script generates the local export preset and build metadata, then writes `build/android/Voidwalker-debug.apk`. Its time-based version code identifies each rebuild. For a desktop smoke check, use the same editor after resource import and open each station by its ID.

```sh
"$GODOT" --headless --path . --import
for station in deck rite sigils relics gate journal; do
  "$GODOT" --headless --path . --quit-after 8 -- "--open=$station"
done
```
