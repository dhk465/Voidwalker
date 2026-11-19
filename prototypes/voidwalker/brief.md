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

After play: "There are too many cards (power) from the beginning. You purchase and gacha cards in preparation but you collect those cards in adventure. You do not use all the cards in your deck from the beginning of a new adventure session." Answers: start with 1 void card, upgrade cards temporarily in the adventure; permanent upgrades let you start with an innate card or two (first from your form's nature, second an unlocked choice); cards come from kills, wave-clear picks, the Rift Rest and broken matter, all at different chances; the loadout is the pool, and very rarely a card outside it turns up for that adventure only. Also: raise the resolution (full 1280x720).

After play: "Make sure no vector image is used. The top and the bottom of the screen do not bounce back the bullets. Destroy the bullets when they are out of the viewport unless otherwise purposed. Maybe there can be some bullets in some combo power that fits as boomerang." Answers: pixel-perfect, "without any smudging inside a pixel. One pixel = one color. Every pixel has to be the same size"; Lodestone becomes a boomerang.

After play: "make the edges hard too. Some of fonts are distorted. Use a proper pixel font in right size without making it yourself."

After play: "Remove the fireflies. Instead use pixelfx to animate void flames to represent void energy flowing in the lobby." And: "redesign the void rite \"crack\" to be more pixel-art. with pixelfx"

After play: "Can we make the flame physical (like the flame would have tail instead of always pointing upward)?"

After play: "The changing of pixel arts direction is not a solution. Create a new one possibly using shader or trailing tail, while maintaining the pixel art feel."

After play: "remove the flame triangle (or tail if you can call it). we will keep the ball and the particle trails as now."

After play: "Remove pillars with their dirty flames"

After play: "Also remove the names like 2 Void rite, 3 rift gate and so on. No need to put text for these."

After play: "Remove the transluscent ellipses in the background. We will use shader to make it looks like deep space instead of shapes."

After play: "Highlights the objects using shader instead of circles when you hover on the objects in the lobby."

After play: "Make stars fixed in the space instead of moving downward. Instead, mke them just blinks over long duration."

After the move to Godot was decided: "If shader and other work is easier to implement using godot latest, its time to migrate." Platform: "Mobile-first design (auto aim in adventure, touch for movements and interactions + mobile friendly ui) for android now and ios later, PC possible." Performance: "Aim for 60fps (as recommended) but 120 possible".

After play: "Our game is roguelite. But we still want a HUB where the cards are prepared in deck. Just like the town in BALL x PIT. Just that there is no building but just card deck preps and gacha. The card upgrades and merge between stages is not that. There should be also some progression/upgrades/differences between adventure sessions. Make it less like some flash game or a simple browser game."

After play (answers): merging and upgrading between stages, run only; progression: permanent upgrades, unlocks, adventure choices, "Journal and Quests and Loots (artifacts and logs from lost civilizations)"; feel: hub presentation, combat feel, sound and music, art: "We use pixel with type/variant-limited and specific colors to specific things for better visibilities of different things. Make enemy and player effects and projectiles distinctive. Just to add, what end game stuff should you suggest?"

**Pitch:** A blob born of the void holds the edge of a rift as Ybasogs, asteroids and wrecked ruins pour down, and its deck of sin and virtue cards is its ammo: every ricocheting bolt is the next nature in the deck. *(provisional)*

**Genre:** survivors-like × roguelike deckbuilder (vertical rift shooter)

**Controls:** WASD to move anywhere in the rift, mouse to aim in any direction, hold the left button to fire, Q auto-fire, Space to pause; the Altar is played with the mouse, 1-5 for its tabs, Enter to begin. *(provisional)*

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
Stage 1 wave 2 (about 1:30) brings feeders spitting slow globs, grazers that dive once they reach the middle, and a big asteroid that splits when hit. Nothing is lost when a thing passes the bottom (it only spoils a sealed wave); energy (your HP) goes to hits, and falling matter blocks shots and body-checks the blob. From stage 1 on, hunters roam the whole rift after you. Stage 2 adds ruins, tough slow walls that shield the enemies behind them. Stage 3 ends on the Mother, who spawns pods and fires spreading volleys. Death ends the run; a new run starts fresh. *(provisional)*

## Core loop
- At the Rift Altar, before an adventure: build the pool (the cards this adventure can find) from your collection, read the omen, pick a depth; the deck's main nature sets your form and its trait.
- In the rift: begin with one void card (plus Inborn and Chosen cards if those sigils are owned) and grow the deck from the pool: cards drop from kills and broken matter, a pick after waves 1 and 2, one offered at the Rift Rest; fire the deck's bolts at Ybasogs and falling matter; kills and broken matter drop void energy; achievements discover new natures.
- After the adventure (win or fall): void energy is banked; at the Altar spend it on the Void Rite (gacha from discovered natures), fuse at the Forge to fill the 84-fusion Codex, and prepare the next deck.
- Leads: survivors-like leads the horde and movement in the rift; the deckbuilder leads the collection, the deck, fusion and removal. *(guessed)*

## Goal & fail state
- Win: clear 3 stages and defeat the Mother. *(provisional)*
- Fail: energy reaches 0 from hits; the run ends. *(provisional)*

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
- [x] Iteration 6: the blob roams the whole rift and aims anywhere, bolts bounce off the floor too, and nothing drains you by slipping past. 23 new kinds: Oracle (seeking globs), Void Leech (latches and drains), Cantor (rings), Biter Gnat, Skitterer (floor runner); four new families that hunt you: reachers from the rift walls (Grasping Limb, Suckermouth, Thrasher, Stalk Eye), the deep (Shambler, Croaker, Harpooner, Brine Priest and its slowing brine), jellies (Polyp, Siphon Jelly, Lantern Jelly that bends your bolts, Man-o-War), gaunts (Faceless Gaunt, Harrier, Shrieker that throws you back, Carrion Wing); mini-bosses Tide Matron and Gaunt Lord; and a third stage 2 boss, The Drowned King (tidal walls, limbs from the walls, the drowned at his call). Hunters tire and leave after a while (hunt_time)
- [x] Iteration 7: the deck starts as one void card and is found in the adventure. The loadout is now the pool; cards drop from natured Ybasogs (4%), plain ones (0.5%), mini-bosses (60%), bosses (always), wrecks (10%) and ruins (15%); after waves 1 and 2 take one of three (sometimes "temper a card +1") or skip; the Rift Rest offers one card. 2% of found cards are trials: a card outside your pool for that adventure only. New sigils Inborn (begin with your form's nature card) and Chosen (begin with a card picked at the Gate; unlocked by a win). The game draws at full 1280x720
- [x] Iteration 8: pixel-perfect: one pixel is one colour and every pixel is the same size. Text is Spleen, a pixel font by Frederic Cambus (BSD-2-Clause), drawn from its own bitmaps at its native sizes (6x12, 8x16, 12x24, 16x32), one font pixel to one screen pixel; hard edges everywhere: a small rasteriser fills every shape at pixel centres, so no edge is blended; no blur or glow; whole-number scaling only (a small window crops); screen shake in whole pixels. Bolts no longer bounce off the top or the bottom: they are gone once out of view, except orbit guards and boomerangs. Lodestone (Greed + Humility) now flies out and swings back, like Zenith. The hub's fireflies are gone: void energy flows round the altar as pixelfx spirit flames, and the Void Rite's crack is a narrow pixelfx void rift; all in the game's own violets, cut to whole pixels. The story firefly in the first adventure's intro stays
- [x] Iteration 9: the hub's flowing flames have real tails, from a new pixelfx effect, fire/void-trail: nothing is rotated; each void ball flies its own path round the altar, and its tail is the flame it sheds where it has been, left in the air to cool from lavender to deep violet and rise, with a second, thinner stream above it and embers breaking away (no flame tongues in the hub: the effect's tongues option is off; flow_trail, flow_size); the four altar pillars and their void fire are gone; the hub's stations carry no names (keys 1 to 6 still open them); the hub's background is deep space from a fragment shader (WebGL): a slowly drifting, domain-warped violet nebula with dark dust lanes and a cold teal vein, shaded only in exact palette colours stepped by a 4x4 ordered dither (worked out once on the CPU where WebGL is missing); the see-through ellipses are gone; hovering a station lights the object itself instead of a dashed circle: a shader reads the station drawn alone on a layer and gives it a 1 px pale-gold outline on its silhouette, a breathing dithered outer ring and a band of light sweeping across it (an outline alone on the CPU where WebGL is missing); the hub's stars hold still and each slowly brightens and fades on its own cycle of 6 to 18 s, in exact colours (star_blink_min, star_blink_max)
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
- Chapter II, the Silent Foundry: a civilization destroyed by its own AI; 34 machine kinds with protocols and Adaptive learning (plan: design/chapter-2-the-silent-foundry.md): add it as a feature
- More stages and characters: grow it
- Godot 4 .NET (C#) as the engine: set up 2026-11-19 (Godot 4.8 beta 4, .NET 10, Compatibility renderer); the browser build stays playable while the port goes system by system, starting with the hub
- Mobile-first: Android now, iOS later, PC possible; auto-aim in the adventure, touch to move and to do everything else, mobile-friendly UI (to be designed with the port)

## Build order
1. The slice: one run of 3 stages in the rift, Ybasogs and falling matter, deck-as-ammo bolts, picks after waves, the Rift Altar between stages, the Mother at the end; lose when energy hits 0.
2. More natures and two-nature fusions, two at a time; then 3-nature recipes, then 4-nature recipes.
3. Grow toward the open void.

---
**Kill condition:** if after two tuning rounds the deck order feels like noise rather than a build you made. *(guessed)*

**Who it's for / what they feel:** players who love building around combos, feeling the rush of a trimmed deck firing exactly the volley they planned. *(guessed)*

**Games you love / don't:** Love Ball x Pit (the stages). Ybasogs draw on Lovecraftian horror (Cthulhu mythos), in original creatures.
