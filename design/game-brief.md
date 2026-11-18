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

**Pitch:** A blob born of the void holds the edge of a rift as Ybasogs, asteroids and wrecked ruins pour down, and its deck of sin and virtue cards is its ammo: every ricocheting bolt is the next nature in the deck. *(provisional)*

**Genre:** survivors-like × roguelike deckbuilder (vertical rift shooter)

**Controls:** WASD to move inside the bottom band, mouse to aim, hold the left button to fire, Space to pause. *(provisional)*

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
- Fire your deck's bolts at descending Ybasogs and matter; kills and broken rock drop void energy.
- Clear a wave, then take or skip one of three nature cards.
- After the stage, at the Rift Altar (instead of the town), spend energy on gacha draws, fuse two cards, or void two opposites to trim the deck.
- Into the next, harder stage; win by beating the Mother, lose when energy hits 0.
- Leads: survivors-like leads the horde, movement and in-stage picks; the deckbuilder leads the deck, its draw order, fusion and removal. *(guessed)*

## Goal & fail state
- Win: clear 3 stages and defeat the Mother. *(provisional)*
- Fail: energy reaches 0 from hits or from things reaching the bottom; the run ends. *(provisional)*

## Features
- [x] One full run: 3 stages of waves, the run ending on a boss
- [x] The nature deck is the magazine: each bolt is the next card, draw and discard counts shown, reshuffle when empty
- [x] 4 natures: Wrath (burns), Patience (freezes), Greed (pulls enemies in), Charity (splits on hit, heals a little)
- [x] 4 two-nature fusions: Wrath+Greed = Hunger (burning gravity well), Wrath+Charity = Zeal (splits into burning shards), Patience+Greed = Stasis (freezing field), Patience+Charity = Mercy (shatters into ice shards, heals); opposites fused make void and both cards are removed. Fusion is a recipe table keyed by any set of natures, a card records every nature in it and fused cards can be fused again, so 3- and 4-nature recipes are data later; a combination with no recipe yet reads "unstable — not yet known" and the cards are kept
- [x] Rift Altar between stages: gacha draws paid with void energy, fusing, voiding
- [x] Pick 1 of 3 or skip after each wave
- [x] 4 enemy kinds, each attacking differently: pod (slow drifter that hatches), grazer (dives), feeder (spits globs), Mother (boss: spawns pods, fires spreads)
- [x] 3 kinds of falling matter: asteroids (break and split), space debris (fast thin slabs to ricochet off), ruins (tough slow walls that shield enemies), all destructible for void energy
- [x] The blob's shape follows the deck's main nature (thin stand-in)

## Your systems
- seven sins and seven virtues as elements · thin stand-in *(guessed)*
- combining natures, opposites making void · in the slice
- deckbuilding · in the slice
- gacha with void energy · in the slice
- void energy from killing entities and destroying objects and matter · in the slice
- space debris, ruins and asteroids approaching · in the slice
- character changes with the deckbuild · thin stand-in *(guessed)*
- 3-nature and 4-nature combinations · later
- open world void map with planets · later *(guessed)*
- story: fireflies, Akashic Records, the 1000 days · later *(guessed)*

## Keep
-

## Avoid
- The town (replaced by the deckbuilding Rift Altar)

## Out of scope
- All 14 natures and their 91 two-nature fusions: grow it
- 3-nature combinations (364 with all 14) and 4-nature combinations (1,001), an opposite pair anywhere in the set still making void: grow it
- An open-world void to glide through: grow it
- The story and its endings: add it as a feature
- More stages and characters: grow it
- Unlocks saved across runs: grow it

## Build order
1. The slice: one run of 3 stages in the rift, Ybasogs and falling matter, deck-as-ammo bolts, picks after waves, the Rift Altar between stages, the Mother at the end; lose when energy hits 0.
2. More natures and two-nature fusions, two at a time; then 3-nature recipes, then 4-nature recipes.
3. Grow toward the open void.

---
**Kill condition:** if after two tuning rounds the deck order feels like noise rather than a build you made. *(guessed)*

**Who it's for / what they feel:** players who love building around combos, feeling the rush of a trimmed deck firing exactly the volley they planned. *(guessed)*

**Games you love / don't:** Love Ball x Pit (the stages).
