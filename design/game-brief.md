# Game Brief: Voidwalker

## In your words

From `design/concepts/voidwalker-concept-notes.md` (built from these notes and the mindmap `voidwalker-mindmap.mm`):

> 2D topdown deckbuilder Void monster shooter with abilities based on seven sins and seven virtues to represent elements.
> Grid under the hood. Most movements are not based on grid but free. Some entities might be based on grid.
> The player glides through the 2D void terrain (like outer space), sometimes comes across with small/big asteroids, some planets, some ruins of lost mankinds/civilizations, some spacecraft intact/ruined (either way, devoid of living things).
> The universe is consistent of Sins and Virtues each representing nature and works of universe: e.g. patience = ice/coldness; wrath = fire/heat.
> The player character is a blob of void born from the void itself. It gained consciousness out of nowhere, it wants to know why it is there and go on an endless journey in space. Nearby it finds a source of power: one of the innate natures (randomize on mvp).
> Seven sins and seven virtues: we will call them innate natures.
> If two opposite natures are combined: it creates void = nothing.
> If two non-opposite natures are combined: it creates a new nature: suggest a list of nature with all combinations sin+sin or sin+virtue or virtue+virtue, except of opposites.
> The player can collect each nature from the adventure, and when combined, shaped different: a void blob of dragon, a void blob of bird, a void blob of a rat, a void blob of a humanoid, etc. The character changes based on the deckbuild.
> Void energy is spread across the open world map and can be obtained through killing other entities and destroying objects and matters, and by consuming the energy the player can gacha out a number of cards, representing collectible natures.
> Collectible natures mean those that are unlocked from the adventure. Innate natures are spread evenly across the universe.
> Remove feature overload if you think it is overdone. I am open to suggestions. The idea is based on voidwalker mindmap.
"I would like to take inspiration from a game Ball x Pit for stages but with the innate natures with deckbuilding instead of the town"

1–8 (stage, shooting, deck, natures, between stages, look, difficulty, slice): "you pick, go with the usual for fun"

Change: "Not only Ybasogs approaches but space debris, ruin and asteroids."

Change: "Leave rooms for 3-nature combinations and 4-nature combinations in the future updates (big numbers)"

After play: "Cards/innate natures are discovered in adventures as achievement rewards. But you described: "Wrath/.. joins the ", deck types are supposed to be "joined" between adventure sessions. Now let's add all innate natures and all combos. Many combos are not just combined powers. They can be unique. Be creative. The player character shapes can be also very unique: they give certain stat and effect benefits/traits. Raise the void costs to balance the economy."

After play: "Make gachability and deckbuilding specific to this game: collections are fun and need to be rewarding -- preparation before adventures which can be fun and replayability matters."

After play: "Achievements should be hard to get."

After play: "We will eventually move to Godot .net but now stay on browser. Now give Ybasogs more type variations (same type but different variants) -- at least 3 and more types -- at least 30 including what we have now -- including mini bosses and bosses."

After play: "Let some ybasog have an innate nature. Opposite nature are more effective. Same natures are less effective."

After play: "Make sure to create good points in time where you start confronting each type. There should be times when there are more innate ones or more void ones. Make the spawning more procedural. Make more debris and ruin types and variants too."

After play: "Ybasogs are based on Cthulhu mythos inspirations so you can take more references from Lovecraftian horrors"

After play: "Make the gameplay view (perspective) much larger, and adjust all elements behaviors to the enlarged view. Make the adventure UI simpler with less words / wordy = less fun."

After play: "Our game is roguelite. But we still want a HUB where the cards are prepared in deck. Just like the town in BALL x PIT. Just that there is no building but just card deck preps and gacha. The card upgrades and merge between stages is not that. There should be also some progression/upgrades/differences between adventure sessions. Make it less like some flash game or a simple browser game."

After play (answers): merging and upgrading between stages, run only; progression: permanent upgrades, unlocks, adventure choices, "Journal and Quests and Loots (artifacts and logs from lost civilizations)"; feel: hub presentation, combat feel, sound and music, art: "We use pixel with type/variant-limited and specific colors to specific things for better visibilities of different things. Make enemy and player effects and projectiles distinctive. Just to add, what end game stuff should you suggest?"

**Pitch:** A blob born of the void holds the edge of a rift as Ybasogs, asteroids and wrecked ruins pour down, and its deck of sin and virtue cards is its ammo: every ricocheting bolt is the next nature in the deck. *(provisional)*

**Genre:** survivors-like × roguelike deckbuilder (vertical rift shooter)

**Controls:** WASD to move inside the bottom band, mouse to aim, hold the left button to fire, Q auto-fire, Space to pause; the Altar is played with the mouse, 1-5 for its tabs, Enter to begin. *(provisional)*

**Question:** Is building and fusing a nature deck fun when every card is a bolt you watch ricochet? *(inferred (strong))*

**Hooks:** The deck is the magazine, so its order and mix is the build. Fusing two natures makes a new one; fusing opposites makes void and wipes both. The blob takes the shape of its deck's main nature.

**Intent:**

## Look and references
- Look: glowing vector shapes on deep black void, faint star dust and slow parallax. Natures each own a colour: Wrath ember red, Patience pale ice cyan, Greed old gold, Charity soft white-green; fusions blend their two. Ybasogs are sickly violet-and-bile organic shapes with pulsing membranes. Falling matter has its own look: asteroids are jagged grey-brown rock with lit edges, debris is twisted hull plating with dead running lights, ruins are carved stone fragments with faint glyphs. Signature detail: the player is an inky blob with a glowing rim in its main nature's colour, reshaping into dragon, bird, rat or humanoid. Every figure drawn, no emoji, no plain circles. *(provisional)*
- References: Ball x Pit (the stage shape: hold the bottom while things descend, ricocheting shots, a boss to end it); Vampire Survivors (picks under pressure); Slay the Spire (take a card or skip, card removal). *(from your words, provisional)*
- View: 2D · Camera: static, one fixed vertical rift filling the window. *(provisional)*

## First minute
The blob floats alone in the dark. A firefly drifts by whispering "take it" beside a crack of light; touching it absorbs a random first nature of the four, and the blob takes its shape. A small asteroid and three pods drift down; held fire sends bolts ricocheting off the rift walls and the asteroid, which cracks and spills void-energy motes that fly to you. Around 0:40 grazers arrive behind a slab of debris you must break or bank shots around. Wave 1 ends on a pick: three nature cards, take one or skip. The next volley alternates colours: the deck, visibly at work. *(guessed)*

## Challenge
Stage 1 wave 2 (about 1:30) brings feeders spitting slow globs, grazers that dive once they reach the middle, and a big asteroid that splits when hit. Anything reaching the bottom drains energy (your HP): enemies a lot, matter a little; falling matter also blocks shots and body-checks the blob. Stage 2 adds ruins, tough slow walls that shield the enemies behind them. Stage 3 ends on the Mother, who spawns pods and fires spreading volleys. Death ends the run; a new run starts fresh. *(provisional)*

## Core loop
- At the Rift Altar, before an adventure: build the deck from your collection, read the omen, pick a depth; the deck's main nature sets your form and its trait.
- In the rift: fire the deck's bolts at Ybasogs and falling matter; kills and broken matter drop void energy; achievements discover new natures.
- After the adventure (win or fall): void energy is banked; at the Altar spend it on the Void Rite (gacha from discovered natures), fuse at the Forge to fill the 84-fusion Codex, and prepare the next deck.
- Leads: survivors-like leads the horde and movement in the rift; the deckbuilder leads the collection, the deck, fusion and removal. *(guessed)*

## Goal & fail state
- Win: clear 3 stages and defeat the Mother. *(provisional)*
- Fail: energy reaches 0 from hits or from things reaching the bottom; the run ends. *(provisional)*

## Features
- [x] One full adventure: 3 stages of waves, ending on the Mother, in a wide rift that fills the screen
- [x] A wordless adventure HUD: an energy bar, a void energy count, stage and wave pips, the tide mark, the next bolts as sigils, the form; short call-outs only
- [x] The nature deck is the magazine: each bolt is the next card, the next five shown as sigils, reshuffle when empty
- [x] All 14 innate natures, each with its own effect: Pride (double damage to the unhurt), Greed (pull), Wrath (burn), Envy (stacking venom), Lust (charm: turns on its kin), Gluttony (grows per hit), Sloth (sleep), Humility (heavy stone), Charity (split, heal), Patience (freeze), Kindness (brambles), Chastity (pierce), Temperance (gust), Diligence (chain lightning)
- [x] All 84 two-nature fusions (made at the Rift Rest or drawn as mythics), most with a unique mechanic (black hole, comet, mirror-twin, beam, walls, decoys, thralls, storms, orbit guard, boomerang and more); opposites annul; a recipe table keyed by any set of natures, so 3- and 4-nature recipes are data later; an unknown set reads "unstable, not yet known"
- [x] Natures discovered by hard achievements during adventures (most take several adventures), progress kept between adventures; a discovery adds cards to the collection at the Altar
- [x] The hub, the Rift Altar: a floating altar in the void with six stations (Deck, Void Rite, Rift Gate, Sigils, Relics, Journal); deck prep and gacha with a reveal ceremony; no buildings
- [x] Rift Rest between stages: merge and temper this adventure's cards (run only), then choose the next stage's path
- [x] Progression between adventures: Sigils (permanent upgrades), unlocks, a choice of rifts at the Gate (omen, prevailing nature, boon), relics (16, equipped in slots), logs (16) and quests (12) in the Journal
- [x] Pixel look: half-resolution, snapped to a limited palette with colours kept for kinds of thing; a pixel font; distinct enemy projectiles (crimson spiked rims) and player bolts (nature-coloured diamonds)
- [x] Combat feel and sound: hit-stop, impact flashes, a light that falls off at the edges; layered sound effects and a score that follows the hub, the stage and the boss
- [x] 15 forms of the blob (shapeless plus one per nature), each with a trait that helps and costs
- [x] Omens and depth: each adventure is read in advance; a win opens a deeper rift with more life and more void energy
- [x] The collection is saved in the browser between sessions
- [x] 34 Ybasog kinds in 8 families of variants (pods, grazers, feeders, eyes, devourers, deacons, mites, wall crawlers), plus 4 mini-bosses (Brood Warden, Hive Knight, Gazer Prime, Gorging Behemoth) and 3 bosses (The Choir or The Hollow Eye at the end of stage 2, The Mother at the end of stage 3); Lovecraftian flesh, eyes and tendrils, each kind its own outline
- [x] Natured Ybasogs: some carry an innate nature (an aura and its sigil); bolts holding the opposite nature deal more, the same nature less; each adventure has a prevailing nature shown at the Altar
- [x] Nature tides per wave: void tides (few natured), mixed, and nature surges (many, mostly the prevailing nature)
- [x] Procedural waves: debuts of kinds not yet met (a name card the first time ever), then beats (trickles, lines, V formations, columns, flanks, swarms, rockfalls, wreck fields, ruin walls, elite pairs, lulls)
- [x] 20 kinds of falling matter in 3 families: rocks (asteroid, big, iron, crystal, comet, cinder), wrecks (hull plate, slab, hull section, solar panel, engine core, cargo pod, antenna spar), ruins (wall, pillar, broken arch, sunken idol, glyph stone, shrine, monolith); several break with an effect

## Your systems
- seven sins and seven virtues as elements · in the slice
- Ybasog variants, mini-bosses and bosses · in the slice
- Ybasogs with innate natures, opposite more effective, same less · in the slice
- procedural spawning with debuts and nature tides · in the slice
- combining natures, opposites making void · in the slice
- deckbuilding · in the slice
- gacha with void energy · in the slice
- void energy from killing entities and destroying objects and matter · in the slice
- space debris, ruins and asteroids approaching · in the slice
- character changes with the deckbuild · in the slice
- natures discovered as achievement rewards · in the slice
- collection and preparation between adventures · in the slice
- 3-nature and 4-nature combinations · later
- open world void map with planets · later *(guessed)*
- story: fireflies, Akashic Records, the 1000 days · later *(guessed)*

## Keep
-

## Avoid
- The town (replaced by the deckbuilding Rift Altar)
- Cards joining the deck in the middle of an adventure (they join at the Altar, between adventures)

## Out of scope
- 3-nature combinations (364 with all 14) and 4-nature combinations (1,001), an opposite pair anywhere in the set still making void: grow it
- An open-world void to glide through: grow it
- The story and its endings: add it as a feature
- More stages and characters: grow it
- Godot 4 .NET (C#) as the engine, later: /setup-engine godot when the browser build is settled

## Build order
1. The slice: one run of 3 stages in the rift, Ybasogs and falling matter, deck-as-ammo bolts, picks after waves, the Rift Altar between stages, the Mother at the end; lose when energy hits 0.
2. More natures and two-nature fusions, two at a time; then 3-nature recipes, then 4-nature recipes.
3. Grow toward the open void.

---
**Kill condition:** if after two tuning rounds the deck order feels like noise rather than a build you made. *(guessed)*

**Who it's for / what they feel:** players who love building around combos, feeling the rush of a trimmed deck firing exactly the volley they planned. *(guessed)*

**Games you love / don't:** Love Ball x Pit (the stages). Ybasogs draw on Lovecraftian horror (Cthulhu mythos), in original creatures.
