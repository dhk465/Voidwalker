# Voidwalker

Voidwalker (also known as Virtuousins) is a top-down deckbuilding shooter. You play a blob born in the void. Every shot fires the next card in your deck, and every bolt can ricochet off a wall, an asteroid, or a Ybasog hunting you through the rift.

At the Rift Altar you choose which cards you want to find. Your first adventure starts with a single Void Bolt. Pick up cards as you fight, then decide at a Rift Rest whether to strengthen a card or fuse two natures. The seven sins and seven virtues give you 14 natures to work with. Wrath is fire, Patience is ice, and those two cancel each other out. Other pairs can produce one of 84 fusions.

![A hunt in the Voidwalker browser prototype](prototypes/voidwalker/evidence/iteration-8-hunt.png)

## Play the browser adventure

Clone or download the repository, then open `prototypes/voidwalker/index.html` in a desktop browser. The prototype runs directly from the checkout. Press **Enter** to begin, move with **WASD** or the **arrow keys**, aim with the mouse, and hold the **left mouse button** to fire. **Q** toggles auto-fire and **Space** pauses. The game reference covers progression, enemy families, and fusion recipes. [Game reference](design/voidwalker-reference.md)

## Build and run the Godot port

The Godot build currently opens at the Rift Altar with a playable demo collection. Its six stations let you manage your pool, draw cards, buy sigils, equip relics, read the journal, and choose a rift. Combat and the three-stage adventure are playable in the browser prototype.

Install the **.NET 10 SDK** and the **Godot 4.8 beta 4 .NET editor**. Run these commands from the repository root in Bash or Git Bash. Set `GODOT` to your .NET editor executable. If it is already on your path as `godot`, the example works as written.

```bash
export GODOT=godot
dotnet build Voidwalker.sln
"$GODOT" --headless --path . --import
"$GODOT" --path .
```

## Test the port

Run the rules and save regression tests from the repository root. The suite checks card draws against fixtures from the browser game and exercises save preservation and the reset flow.

```bash
dotnet test Voidwalker.sln
```

After building and importing, launch each station headlessly to check that its scene and resources load. Check the output for engine errors as well as the process exit status.

```bash
for station in deck rite sigils relics gate journal; do
  "$GODOT" --headless --path . --quit-after 8 -- "--open=$station" || exit 1
done
```

## Build for Android

Install the matching **Godot 4.8 beta 4 Mono export templates** and configure Godot’s Android export settings with **JDK 17** and the **Android SDK**. The tested setup used SDK 36. With `GODOT` set as above, build the signed ARM64 debug APK from Bash or Git Bash.

```bash
bash tools/build/android_export.sh
```

The APK is written to `build/android/Voidwalker-debug.apk`. To install it on a connected test device with USB debugging enabled, run `adb install -r build/android/Voidwalker-debug.apk`. The device test report records all six touch flows, including removal of the sixteenth pool stack and a confirmed reset that survived a cold restart. [Android smoke-test results and screenshots](production/qa/evidence/hub-port/android-stations/README.md)

## Credits

Voidwalker uses **Claude Code Game Studios** by Donchitos as its development template. The original MIT license is included in this repository. [Claude Code Game Studios](https://github.com/Donchitos/Claude-Code-Game-Studios) · [License](LICENSE)
