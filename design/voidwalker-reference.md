# Voidwalker: Game Reference

*Updated 2026-11-19 from `prototypes/voidwalker/config.js` (game data and tuning), the Godot port (`src/`) and the project history. Each section names its source: **Godot port** once that system has been migrated, **browser build** until then. Section 18 tracks the migration. Numbers marked browser come from `config.js`; the port reads the same game data from `src/data/game_data.json`.*

**Builds:** the browser game `prototypes/voidwalker/index.html` (the whole game) and the Godot 4.8 .NET port (the Rift Altar hub so far; see section 18 and the README) · **Brief:** `design/game-brief.md` · **Checkpoints:** tag `one-shot/voidwalker`, commit `96f41a9` (iteration 9, the last browser iteration), commit `20b1e1b` (the port's lobby islands)

## Contents
- 1. The game in one page
- 2. Controls
- 3. The adventure
- 4. The Rift Altar
- 5. The economy
- 6. The fourteen natures
- 7. The forms and their traits
- 8. The 84 fusions
- 9. Effects glossary
- 10. Discoveries (achievements)
- 11. Omens and depth
- 12. Ybasogs and natured Ybasogs
- 13. Falling matter
- 14. Stages and waves
- 15. The save
- 16. Every tuning value
- 17. Development history and changelog
- 18. Migration to Godot
- 19. Known gaps and next steps

## 1. The game in one page

A blob born of the void roams a vertical rift while Ybasogs (an organic hive out of Lovecraftian horror) and falling matter (asteroids, space wrecks, ruins) pour in, and whole families hunt it from the walls, the deep and the dark above. Its **deck of nature cards is its ammunition**: each bolt fired is the next card, and bolts ricochet off the walls, the floor, the Ybasogs and the matter. The fourteen **innate natures** are the seven sins and seven virtues, each an element. Two natures fuse into one of **84 fusions**; two **opposites annul** into void.

The loop: at the **Rift Altar**, a hub floating in the void, grow the collection with the **Void Rite** (gacha), choose the **pool** of cards an adventure can find, buy **sigils**, equip **relics** and pick a rift at the **Rift Gate**. Then **adventure**: you begin with **one void card** and find the rest of your deck in the rift (drops, a pick after each wave, an offer at the Rift Rest), through three stages that end on The Mother. Void energy is **banked** win or lose. Natures are **discovered by achievements**. The deck's most common nature shapes the blob into one of 15 forms.

Survivors-like × roguelite deckbuilder. The stage shape takes inspiration from Ball x Pit; its town is replaced by the Altar, with no buildings, only preparation.

## 2. Controls

| Where | Input | Does |
|---|---|---|
| Title | Enter, Space or click | Begin (the first time) or go to the Altar |
| Adventure | WASD or arrow keys | Move anywhere in the rift |
| Adventure | Mouse | Aim in any direction |
| Adventure | Hold left mouse button | Fire |
| Adventure | Q | Auto-fire on or off |
| Adventure | Space, P or Esc | Pause |
| Wave-clear pick | Click, or 1 to 3; Esc | Take a card (or temper); skip |
| Rift Rest | Mouse; Enter | Merge, temper, take the found card, choose a path; onward |
| Anywhere | M | Mute |
| Altar | Mouse, or 1 to 6; Enter at the Gate | Visit a station; enter the rift |
| End screen | Enter, R or click | Return to the Altar |

## 3. The adventure

- **Energy is life.** Start at 20 plus the form's bonus. A bite costs 2, a glob 1, a body-check from matter 1, a gaze beam, lash or tidal wall 2, a ring 1. After a hit you are safe for 0.8 s. Clearing a stage restores 5. Nothing costs energy by slipping past the bottom; it only spoils a sealed wave.
- **The whole rift is yours.** The blob moves anywhere and aims in any direction; bolts ricochet off the walls, the top and the floor.
- **Bolts:** one every 0.17 s at 640 px/s, 4 ricochets before fading. Base damage 1; +50% per level above I; fused cards +30%.
- **The deck is the magazine, and it starts small.** Every adventure begins with **one void card** (plus a card of your form's nature with the Inborn sigil, and a card chosen at the Gate with the Chosen sigil). An empty draw pile reshuffles the discard (0.35 s without fire), so a small deck fires slowly and each found card speeds it up. The next bolts show bottom left; a card mark shows the deck's size.
- **Void energy** motes drop from kills and broken matter and fly to you within 120 px, and all at once when a wave ends.
- **Structure:** 3 stages of 3 waves; stage 2 ends on The Choir, The Hollow Eye or The Drowned King, stage 3 on The Mother. Win when she falls; lose when energy reaches 0.
- **The first adventure** begins alone in the dark: a firefly whispers "take it" beside a crack of light. Touching it grants a random innate nature of the 14 and a pool of four Void Bolts and two cards of that nature; you start with one Void Bolt and find the rest.

### Finding cards

The cards you find come from your **pool**, the loadout chosen at the Altar. Each one joins the deck at once. Chances are multiplied by `card_drop` (1).

| Source | Chance |
|---|---|
| Plain Ybasog | 0.5% |
| Natured Ybasog | 4% |
| Mini-boss | 60% |
| Boss | 100% |
| Broken wreck | 10% |
| Broken ruin piece | 15% |
| After waves 1 and 2 | Take one of three pool cards, or skip; 30% of the time one offer is "temper a card +1" instead |
| Rift Rest | One found card, free to take |
| Trial card | 2% of found cards are from *outside* your pool: marked TRIAL, kept for this adventure only, never added to the collection |

### The Rift Rest

Between stages: 2 rites to **merge** two of this adventure's cards (a new fusion fills the Codex and pays 15; opposites annul for 8 back; twins rise a level, up to 3) or **temper** one (+1 level). All of it lasts this adventure only. Take the found card, then choose the next stage's path:

- **Sunken Path**: More ruins, relics and logs.
- **Hive Path**: More Ybasogs, +25% void energy.
- **Storm Path**: More rocks and comets, +1 ricochet.
- **Tide Path**: Nature surges every wave; natured drop double.

### Stages and waves

- Stage 1, wave 1 is fixed: an asteroid and three pods, more pods, a slab of debris at 27 s with three grazers behind it, pods to finish.
- Every other wave opens with a debut of kinds not yet met, then beats (formations, swarms, rockfalls, wreck fields, ruin walls, elite pairs, lulls); see the Bestiary.
- Each wave rolls a nature tide: a void tide, a mixed tide or a nature surge.

## 4. The Rift Altar

*Source: Godot port (migrated 2026-11-19). The browser build's Altar has the same rules; its differences are listed at the end of this section.*

The hub: the altar and the six stations each float on their own island in deep space. No buildings, only preparation.

- **The altar** holds the blob in the pool's dominant form, with the deck's card backs above it on a lectern.
- **The Void Rite** is a crack in space-time with void seeping out. **Void balls** leave the crack, drift on smooth curves through one to three stops, each at its own pace and on its own depth lane (some pass behind the islands), and are drawn into the Rift Gate. A Rite draw releases a burst of them. Tap a ball and it darts away or bursts.
- **The Rift Gate** holds a rift painted in the chosen rift's nature, turning counter-clockwise as it drains.
- **Sigils** is a constellation over a cracked observatory disc, **Relics** a ruined colonnade whose pedestals carry the equipped relics, and the **Journal** a tome open on a reading stand.
- **The look:** a 640×360 game area scaled by whole numbers (4× on a wide phone, 2× in a 1280×720 window), the Spleen pixel font, code-drawn sprites, blinking stars and far asteroids drifting by.

| Station | What you do |
|---|---|
| **Pool** | Choose the cards this adventure can find (6 to 12, more with Breadth); tap a row to remove one. See the form the pool gives and its trait. |
| **Void Rite** | The gacha, drawing only from discovered natures, with a reveal ceremony. One rite 30, ten rites 270. Focus on one nature for +10 per draw: 50% of draws land on it. Rare (level II) 20%; mythic (an already-fused card, written into the Codex) 7%; a mythic is certain within 10 draws. New kinds are marked NEW. |
| **Rift Gate** | Choose one of two rifts (three after a win), each with an omen, a prevailing nature and a boon; choose the depth; with the Chosen sigil, choose the first card. Enter. |
| **Sigils** | Permanent upgrades bought with void energy, and the unlocks list. |
| **Relics** | 16 relics found in adventures (ruins, mini-bosses, bosses); equip them in slots (1, a second unlocked by finding 3, a third with Shrine). |
| **Journal** | Quests, the 16 logs found in wrecks and ruins, the 84-cell Codex, the Bestiary and the natures with their achievements. RESET TO DEMO (tap twice) returns the collection to the demo. |

### Controls

| Where | Input | Does |
|---|---|---|
| Hub, touch | Tap a station, then tap it again | Light it, then open its screen |
| Hub, desktop | Hover, then click | Light a station, open its screen |
| Hub | Tap or click a void ball | It darts away, or bursts (40%) |
| A screen | Back button or Escape | Close it |
| Journal, Natures tab | RESET TO DEMO, twice | Reset the collection to the demo |

### Hub tuning (`src/data/hub_config.tres`)

| Value | Now | What it does |
|---|---|---|
| `FlowAmbient` | 4 | Void balls the crack keeps in the air on its own |
| `FlowMax` | 9 | The most balls at once, Rite draws included |
| `FlowGapMin`, `FlowGapMax` | 1.2, 3.2 s | Time between the crack's own balls |
| `FlowBurstGap` | 0.35 s | Time between balls released by a Rite draw |
| `FlowBoundsMin`, `FlowBoundsMax` | (70, 80), (570, 320) | The area balls wander in, in hub pixels |
| `FlowStopsMin`, `FlowStopsMax` | 1, 3 | Waypoints a ball's curve passes near before it turns to the rift |
| `FlowSpeedMin`, `FlowSpeedMax` | 15, 50 px/s | The slowest and fastest pace; each ball has its own |
| `FlowMinHop`, `FlowMaxHop` | 80, 200 px | Distance from one stop to the next |
| `FlowSway` | 0.25 | How far a ball's speed swells and ebbs round its pace |
| `FlowSpiralRadius`, `FlowSpiralTurns` | 26 px, 1.5 | The spiral into the rift |
| `FlowDartDistance`, `FlowDartTime` | 40 px, 0.45 s | A tapped ball's dart |
| `FlowTapRadius` | 10 px | How close a tap must land to touch a ball |
| `FlowPopChance` | 0.4 | Chance a tapped ball bursts instead of darting |
| `TrailLifetime`, `TrailAmount`, `TrailLift` | 2 s, 60, 14 px/s | Each ball's trail: how long a puff lives, puffs at once, how fast they rise |
| `StarDensity` | 0.07 | Share of the sky's 7×7 cells that hold a star |
| `StarBlinkMin`, `StarBlinkMax` | 6, 18 s | A star's blink cycle |
| `AsteroidDrift` | 3 px/s | How fast the far asteroids drift |

### In the browser build

The browser Altar keeps the same stations and rules, drawn at 1280×720: void flames with trails flow round one floating altar, hovering lights a station, keys 1 to 6 open them, Enter at the Gate enters the rift, and the Journal's "Forget all" wipes the save.

### Sigils

| Sigil | Gives | Costs by level | Needs |
|---|---|---|---|
| **Vessel** | +3 max energy | 60 / 140 / 300 |  |
| **Reach** | +50 px void energy pull | 50 / 120 / 250 |  |
| **Swiftness** | +8% speed | 80 / 180 |  |
| **Ricochet** | +1 ricochet | 200 / 450 |  |
| **Resonance** | +0.1 damage to opposites | 120 / 260 / 420 |  |
| **Warding** | begin with a ward | 120 / 280 |  |
| **Recovery** | +3 energy between stages | 90 / 220 |  |
| **Rites** | +1 rite at each Rift Rest | 250 / 500 |  |
| **Breadth** | +2 cards in the pool | 100 / 250 |  |
| **Favor** | +2% mythic chance at the Rite | 150 / 300 / 500 |  |
| **Keeping** | keep +10% void after a fall | 70 / 160 / 320 |  |
| **Shrine** | +1 relic slot | 400 |  |
| **Inborn** | begin with your form's nature card | 150 |  |
| **Chosen** | begin with a card chosen at the Gate | 300 | after: The Chosen sigil |

**Unlocks:**

- A third rift at the Gate: 1 wins
- Darker omens: 4 runs
- A second relic slot: 3 relics
- Deeper rifts: 1 wins
- The Chosen sigil: 1 wins

### Rifts and boons

- **Relic-rich**: Relics and logs turn up twice as often.
- **Rich veins**: +30% void energy.
- **Teeming**: One more new kind each wave; more natured Ybasogs.
- **Still**: No boon, and no added danger.

**All 16 relics**

| Relic | Gives |
|---|---|
| **Drowned Bell** | A ward at the start of each stage. |
| **Star-Map Shard** | +15% void energy. |
| **Hollow Lens** | Warnings last 0.4 s longer. |
| **Chitin Charm** | Shells no longer halve your damage. |
| **First Ember** | Nature effects last 20% longer. |
| **Tide Stone** | +0.2 to opposite-nature damage. |
| **Pilgrim's Lamp** | +80 px void energy pull. |
| **Broken Crown** | +10% bolt damage. |
| **Hive Tooth** | +10% fire rate. |
| **Vial of Black Salt** | +3 max energy. |
| **Mirror Scale** | 15% of globs pass through you. |
| **Keel Nail** | +1 ricochet. |
| **Brass Orrery** | +3 energy between stages. |
| **Idol's Eye** | Relics and logs turn up more often. |
| **Sealed Psalm** | One more rite at each Rift Rest. |
| **Void Pearl** | Keep 15% more void energy after a fall. |

**All 12 quests**

| Quest | Goal | Reward |
|---|---|---|
| **The Brood Ends** | Fell the Mother | 150 void |
| **Silence the Hymn** | Fell the Choir | 100 void |
| **Close the Eye** | Fell the Hollow Eye | 100 void |
| **Wardens and Knights** | Fell 6 mini-bosses | 120 void |
| **Seven and Seven** | Discover 6 natures | 200 void |
| **Archivist** | Find 6 logs | 100 void |
| **Reliquary** | Find 4 relics | 1 relic |
| **Deeper** | Win at depth 1+ | 250 void |
| **Naturalist** | Meet 20 Ybasog kinds | 150 void |
| **Opposites** | Slay 100 with opposites | 150 void |
| **Untouched** | Clear a stage unhurt | 1 relic |
| **Codex Keeper** | Record 12 fusions | 250 void |

## 5. The economy

| Source or sink | Void energy |
|---|---|
| Kills and broken matter | each kind's drop (Bestiary), +2 if natured |
| Banked after a win | all gathered × omen × boon × depth, + 100 |
| Banked after a fall | 60% of it (more with Keeping, Void Pearl) |
| Discovering a nature | +40, and 2 cards |
| First time making a fusion | +15 |
| Annulling opposites at the Rest | +8 |
| Quests | 100 to 250, or a relic |
| One rite / ten rites | −30 / −270, +10 each when focused |
| Sigils | 50 to 500 a level |

## 6. The fourteen natures

Opposite pairs annul when fused: Humility and Pride, Charity and Greed, Patience and Wrath, Envy and Kindness, Chastity and Lust, Gluttony and Temperance, Diligence and Sloth.

| Nature | Kind | Element | Opposite | Bolt effect | Damage I / II / III |
|---|---|---|---|---|---|
| **Pride** | sin | radiance | Humility | Double damage to the unhurt. x2 damage to a target at full life | 1 / 1.5 / 2 |
| **Greed** | sin | gravity | Charity | Pulls nearby Ybasogs to the hit. pulls Ybasogs within 110 px toward the hit for 0.6 s | 1 / 1.5 / 2 |
| **Wrath** | sin | fire | Patience | Burns what it hits. burns 1.6/s for 2 s | 1 / 1.5 / 2 |
| **Envy** | sin | venom | Kindness | Venom that stacks. venom: 0.6/s per stack, up to 5 stacks, 3 s | 1 / 1.5 / 2 |
| **Lust** | sin | allure | Chastity | Charms: the Ybasog turns on its kin. charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot | 1 / 1.5 / 2 |
| **Gluttony** | sin | hunger | Temperance | Grows with every hit; kills drop more. +0.3 damage and grows after every hit; its kills drop +1 void energy | 1 / 1.5 / 2 |
| **Sloth** | sin | slumber | Diligence | Puts what it hits to sleep. sleep 1.2 s: no moving, no attacking, pods do not hatch | 1 / 1.5 / 2 |
| **Humility** | virtue | stone | Pride | A heavy stone; cracks matter. heavy: x1.4 damage, x2 more against falling matter, 20% slower | 1.4 / 2.1 / 2.8 |
| **Charity** | virtue | grace | Greed | Splits on hit; a kill heals you. splits into 2 shards (0.6 damage each); a kill heals 1 energy | 1 / 1.5 / 2 |
| **Patience** | virtue | ice | Wrath | Freezes: slows what it hits. slows 55% for 2.2 s | 1 / 1.5 / 2 |
| **Kindness** | virtue | growth | Envy | Leaves a patch of brambles. bramble patch (55 px): 1.3/s for 3 s | 1 / 1.5 / 2 |
| **Chastity** | virtue | purity | Lust | Pierces straight through. passes through 3 targets | 1 / 1.5 / 2 |
| **Temperance** | virtue | wind | Gluttony | A gust that knocks them back. knocks the target back 70 px; a diving grazer stops diving | 1 / 1.5 / 2 |
| **Diligence** | virtue | lightning | Sloth | Lightning leaps to two more. lightning leaps to 2 more within 170 px at 70% damage | 1 / 1.5 / 2 |
| Void Bolt | plain | void | none | A plain bolt of void. | 1 / 1.5 / 2 |

## 7. The forms and their traits

The nature most present in the deck (card levels count; ties go to your innate nature) shapes the blob for the whole adventure. A deck of only Void Bolts stays shapeless.

| Form | Nature | Trait | Exact values |
|---|---|---|---|
| Shapeless blob | none | none | none |
| Lion | Pride | +20% bolt damage, -3 max energy | damage 1.2, max_energy -3 |
| Rat | Greed | pulls void energy from afar, +25% void energy | magnet 120, mote_bonus 0.25 |
| Dragon | Wrath | +15% fire rate, bites hurt 1 more | fire_rate 1.15, bite 1 |
| Serpent | Envy | +1 ricochet, effects last 25% longer | bounces 1, status 1.25 |
| Moth | Lust | half of biting Ybasogs are charmed instead | contact_charm 0.5, speed 1.1 |
| Toad | Gluttony | +6 max energy, 15% slower | max_energy 6, speed 0.85 |
| Snail | Sloth | Ybasogs near you slow by 30%, 25% slower | aura 0.3, speed 0.75 |
| Tortoise | Humility | hits hurt 1 less, +2 max energy, 15% slower | armor 1, max_energy 2, speed 0.85 |
| Dove | Charity | heal 2 after each wave, 15% faster | wave_heal 2, speed 1.15 |
| Wanderer | Patience | longer safety after hits, instant reshuffle | iframes 0.5, quick_shuffle 1 |
| Stag | Kindness | regrows energy steadily | regen 0.1 |
| Swan | Chastity | 40% of globs pass through you | glob_dodge 0.4 |
| Manta | Temperance | 30% faster, longer safety after hits | speed 1.3, iframes 0.2 |
| Bee | Diligence | +30% fire rate, -15% bolt damage | fire_rate 1.3, damage 0.85 |

Trait keys: damage and fire_rate multiply; max_energy adds; magnet adds px; mote_bonus is the chance of an extra void energy per mote; bite adds to bite damage; bounces adds ricochets; status multiplies effect durations; contact_charm is the chance a biting Ybasog is charmed instead; speed multiplies movement; aura slows Ybasogs within 160 px; armor reduces each hit (never below 1); wave_heal heals after every wave; iframes adds safety time; quick_shuffle removes the reshuffle pause; regen is energy per second; glob_dodge is the chance a glob passes through.

## 8. The 84 fusions

Every fused card: x1.3 damage, drawn with rings around its bolt. Damage shown for levels I / II / III. Groups: sin with sin (21), virtue with virtue (21), sin with virtue (42; the 7 opposite pairs never fuse).

### Sin with sin

| Fusion | Natures | What it does | Exact effects | Damage I / II / III |
|---|---|---|---|---|
| **Adoration** | Lust + Pride | The charmed who die rise as your wisps. | charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot; charmed Ybasogs that die rise as your wisps for 6 s (ram for 3) | 1.3 / 1.95 / 2.6 |
| **Ardor** | Lust + Wrath | Each kill quickens your fire. | a kill quickens your fire (interval x0.6) for 3 s; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Covet** | Envy + Greed | Binds two Ybasogs: they share their wounds. | binds the target to its nearest kin for 4 s: 50% of damage is shared | 1.3 / 1.95 / 2.6 |
| **Event Horizon** | Greed + Sloth | A slow orb that collapses into a black hole. | a slow orb (half speed) that collapses into a black hole (130 px, 1.6 s) after 1 s or on impact; its core swallows pods and grazers | 1.3 / 1.95 / 2.6 |
| **Gilded Maw** | Gluttony + Pride | Swallows any Ybasog below a third of its life. | kills any Ybasog (not the Mother) under 35% life; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Glut** | Gluttony + Sloth | A slow boulder that pierces and grows. | passes through 3 targets; +0.3 damage and grows after every hit; its kills drop +1 void energy; heavy: x1.4 damage, x2 more against falling matter, 20% slower; huge and 40% slower | 1.82 / 2.73 / 3.64 |
| **Hoard** | Gluttony + Greed | Kills drop triple void energy. | kills drop x3 void energy; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Hunger** | Greed + Wrath | A burning gravity well. | burning gravity well (95 px, 1.4 s, 2.2/s); burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Indulgence** | Gluttony + Lust | A second helping fires right behind it. | a second copy fires 0.12 s later; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Midas** | Greed + Pride | The slain burst into golden shards. | kills burst into 5 golden shards and drop +2 void energy | 1.3 / 1.95 / 2.6 |
| **Obsession** | Envy + Lust | Hunts the nearest Ybasog and never lets go. | steers toward the nearest Ybasog; venom: 0.6/s per stack, up to 5 stacks, 3 s | 1.3 / 1.95 / 2.6 |
| **Parasite** | Envy + Gluttony | Venom; the damage feeds you. | heals 15% of the damage it deals; venom: 0.6/s per stack, up to 5 stacks, 3 s | 1.3 / 1.95 / 2.6 |
| **Reverie** | Lust + Sloth | Lulls every Ybasog nearby to sleep. | sleeps every Ybasog within 150 px for 1.56 s | 1.3 / 1.95 / 2.6 |
| **Rot** | Envy + Sloth | Kills release a cloud of rot. | kills leave a venom cloud (70 px, 3 s); venom: 0.6/s per stack, up to 5 stacks, 3 s | 1.3 / 1.95 / 2.6 |
| **Siren** | Greed + Lust | A wide pull, and the pulled are charmed. | a wide pull (1.8x radius, 1.5x strength); charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot | 1.3 / 1.95 / 2.6 |
| **Smolder** | Sloth + Wrath | Clings, smoulders, then erupts. | clings to the target and erupts after 0.9 s: 3 damage in 70 px; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Spite** | Envy + Wrath | Marks a Ybasog; it bursts for triple a moment later. | marks the target: after 1.5 s it bursts for x3 the hit's damage | 1.3 / 1.95 / 2.6 |
| **Supernova** | Pride + Wrath | A ring of burning shards on impact. | a ring of 6 shards; burns 1.6/s for 2 s. Shards carry: burn | 1.3 / 1.95 / 2.6 |
| **Throne** | Pride + Sloth | Circles you as a guard, then launches. | circles the blob for 1.6 s as a shield, then launches along the aim; x2 damage to a target at full life | 1.3 / 1.95 / 2.6 |
| **Vanity** | Envy + Pride | A mirror-twin strikes from the far wall. | a twin bolt launches from the mirror point across the rift; x2 damage to a target at full life | 1.3 / 1.95 / 2.6 |
| **Wildfire** | Gluttony + Wrath | Kills erupt in a ring of fire. | kills erupt in 8 shards; burns 1.6/s for 2 s. Shards carry: burn | 1.3 / 1.95 / 2.6 |

### Virtue with virtue

| Fusion | Natures | What it does | Exact effects | Damage I / II / III |
|---|---|---|---|---|
| **Alms** | Charity + Humility | Kills grant a ward that blocks one hit. | a kill grants a ward that blocks one hit (up to 3); a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Blizzard** | Patience + Temperance | A storm of frost that freezes as it strikes. | a storm cloud (95 px, 3 s) striking a Ybasog every 0.35 s for 1.2; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Blossom** | Charity + Kindness | The slain bloom into healing petals. | kills drop 2 petals that heal 1 each; a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Bulwark** | Diligence + Humility | Raises a stone wall that stops globs and Ybasogs. | raises a stone wall (140 px) for 4 s that stops globs and Ybasogs | 1.3 / 1.95 / 2.6 |
| **Clarity** | Chastity + Temperance | A beam of pure light, instant and endless. | an instant beam to the rift's edge, x1.5 damage to everything on the line (no bolt) | 1.3 / 1.95 / 2.6 |
| **Evergreen** | Kindness + Patience | Frost brambles that slow and wound. | bramble patch (55 px): 1.3/s for 3 s; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Glacier** | Humility + Patience | A frost shockwave runs the whole row. | a shockwave along the whole row (+/-38 px) for x1.2 the hit's damage; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Halo** | Charity + Chastity | A seeking halo; kills heal. | steers toward the nearest Ybasog; a kill heals 1 energy; x2 damage to a target at full life | 1.3 / 1.95 / 2.6 |
| **Hedgerow** | Humility + Kindness | Brambles burst wide and scatter seeds. | bramble patch (55 px): 1.3/s for 3 s; splits into 2 shards (0.6 damage each); brambles 1.7x wider. Shards carry: thorns | 1.3 / 1.95 / 2.6 |
| **Hive** | Diligence + Kindness | Releases bees that hunt on their own. | splits into 2 shards (0.6 damage each); lightning leaps to 2 more within 170 px at 70% damage. Shards carry: homing | 1.3 / 1.95 / 2.6 |
| **Hourglass** | Diligence + Patience | Stops a Ybasog, then shatters it for triple. | sleep 1.2 s: no moving, no attacking, pods do not hatch; marks the target: after 1.5 s it bursts for x3 the hit's damage | 1.3 / 1.95 / 2.6 |
| **Lily** | Chastity + Kindness | Three seeds that root as brambles. | splits into 3 shards; a kill heals 1 energy. Shards carry: thorns | 1.3 / 1.95 / 2.6 |
| **Mercy** | Charity + Patience | Shatters into ice shards; kills heal. | bursts into 4 shards in a wide fan; slows 55% for 2.2 s; a kill heals 1 energy. Shards carry: chill | 1.3 / 1.95 / 2.6 |
| **Oath** | Chastity + Diligence | Keeps going: every ricochet makes it stronger. | +4 ricochets; every wall ricochet adds +0.35 damage and size; passes through 3 targets | 1.3 / 1.95 / 2.6 |
| **Penance** | Chastity + Humility | Pierces and washes away nearby globs. | passes through 3 targets; destroys enemy globs within 140 px | 1.3 / 1.95 / 2.6 |
| **Pollen** | Kindness + Temperance | Plants a flower the grazers dive at. | plants a flower decoy for 4 s: grazers dive at it, feeders spit at it, Ybasogs that touch it die; bramble patch (55 px): 1.3/s for 3 s | 1.3 / 1.95 / 2.6 |
| **Providence** | Charity + Diligence | Chain lightning; kills heal. | lightning leaps to 2 more within 170 px at 70% damage; a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Sandstorm** | Humility + Temperance | Leaves a scouring storm of grit. | a storm cloud (95 px, 3 s) striking a Ybasog every 0.35 s for 1.2; heavy: x1.4 damage, x2 more against falling matter, 20% slower | 1.82 / 2.73 / 3.64 |
| **Snowdrop** | Chastity + Patience | A frozen lance through the line. | passes through 3 targets; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Tempest** | Diligence + Temperance | Lightning on the wind; flings them back. | lightning leaps to 2 more within 170 px at 70% damage; knocks the target back 70 px; a diving grazer stops diving | 1.3 / 1.95 / 2.6 |
| **Zephyr** | Charity + Temperance | A soft wind slows every Ybasog in the rift. | slows every Ybasog in the rift 45% for 1.6 s | 1.3 / 1.95 / 2.6 |

### Sin with virtue

| Fusion | Natures | What it does | Exact effects | Damage I / II / III |
|---|---|---|---|---|
| **Allure** | Lust + Temperance | A lure the Ybasogs cannot resist. | plants a flower decoy for 4 s: grazers dive at it, feeders spit at it, Ybasogs that touch it die; a wide pull (1.8x radius, 1.5x strength) | 1.3 / 1.95 / 2.6 |
| **Arc Forge** | Diligence + Wrath | Lightning that sets them alight. | lightning leaps to 2 more within 170 px at 70% damage; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Comet** | Pride + Temperance | Calls a comet down on what it hits. | a comet lands 0.55 s later: 4 damage in 65 px, and burns | 1.3 / 1.95 / 2.6 |
| **Compound** | Diligence + Greed | Interest grows with every ricochet. | +4 ricochets; every wall ricochet adds +0.35 damage and size; kills drop x3 void energy | 1.3 / 1.95 / 2.6 |
| **Crusade** | Chastity + Wrath | A burning lance through the line. | passes through 3 targets; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Daydream** | Chastity + Sloth | Drifts around you, then puts them to sleep. | circles the blob for 1.6 s as a shield, then launches along the aim; sleep 1.2 s: no moving, no attacking, pods do not hatch | 1.3 / 1.95 / 2.6 |
| **Devotion** | Humility + Lust | Charms; kills grant a ward. | charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot; a kill grants a ward that blocks one hit (up to 3) | 1.3 / 1.95 / 2.6 |
| **Diamond** | Patience + Pride | A crystal that bursts into blinding shards. | bursts into 4 shards in a wide fan; x2 damage to a target at full life. Shards carry: radiant | 1.3 / 1.95 / 2.6 |
| **Doldrums** | Sloth + Temperance | The wind dies; the rift slows to a crawl. | slows every Ybasog in the rift 45% for 1.6 s; sleep 1.2 s: no moving, no attacking, pods do not hatch | 1.3 / 1.95 / 2.6 |
| **Ember Bloom** | Kindness + Wrath | Burning brambles. | bramble patch (55 px): 1.3/s for 3 s; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Embrace** | Charity + Lust | Charms, and the damage feeds you. | charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot; heals 15% of the damage it deals | 1.3 / 1.95 / 2.6 |
| **Famine** | Gluttony + Humility | A hungry shockwave across the row. | a shockwave along the whole row (+/-38 px) for x1.2 the hit's damage; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Fast** | Chastity + Gluttony | Pierces, and finishes the weak. | passes through 3 targets; kills any Ybasog (not the Mother) under 35% life | 1.3 / 1.95 / 2.6 |
| **Feast** | Charity + Gluttony | Kills drop a feast of void energy and heal. | kills drop x3 void energy; a kill heals 1 energy; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Fervor** | Diligence + Lust | Lightning; each kill quickens your fire. | a kill quickens your fire (interval x0.6) for 3 s; lightning leaps to 2 more within 170 px at 70% damage | 1.3 / 1.95 / 2.6 |
| **Firestorm** | Temperance + Wrath | A storm that rains fire. | a storm cloud (95 px, 3 s) striking a Ybasog every 0.35 s for 1.2; burns 1.6/s for 2 s | 1.3 / 1.95 / 2.6 |
| **Frostbite** | Envy + Patience | Cold venom: slows and rots. | venom: 0.6/s per stack, up to 5 stacks, 3 s; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Grindstone** | Diligence + Gluttony | Grinds harder with every ricochet. | +4 ricochets; every wall ricochet adds +0.35 damage and size; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Harvest** | Greed + Kindness | Kills drop triple void energy and heal. | kills drop x3 void energy; a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Hibernate** | Gluttony + Patience | Cold sleep, and it grows as it feeds. | sleep 1.2 s: no moving, no attacking, pods do not hatch; slows 55% for 2.2 s; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Largesse** | Charity + Pride | Splits into three bolts of natures you know. | splits into 3 bolts of random natures you have discovered (70% damage) | 1.3 / 1.95 / 2.6 |
| **Laurel** | Kindness + Pride | Finishes the weakened; kills heal. | kills any Ybasog (not the Mother) under 35% life; a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Lichen** | Kindness + Sloth | Sleepy brambles. | bramble patch (55 px): 1.3/s for 3 s; sleep 1.2 s: no moving, no attacking, pods do not hatch | 1.3 / 1.95 / 2.6 |
| **Lodestone** | Greed + Humility | A heavy magnet; calls all void energy to you. | pulls Ybasogs within 110 px toward the hit for 0.6 s; heavy: x1.4 damage, x2 more against falling matter, 20% slower; calls all void energy on screen to you for 2 s | 1.82 / 2.73 / 3.64 |
| **Longing** | Lust + Patience | A long, cold charm. | charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot; slows 55% for 2.2 s; the charm lasts twice as long | 1.3 / 1.95 / 2.6 |
| **Magma** | Humility + Wrath | Molten rock; kills erupt. | heavy: x1.4 damage, x2 more against falling matter, 20% slower; kills erupt in 8 shards; burns 1.6/s for 2 s. Shards carry: burn | 1.82 / 2.73 / 3.64 |
| **Miasma** | Envy + Temperance | A storm of rot. | a storm cloud (95 px, 3 s) striking a Ybasog every 0.35 s for 1.2; venom: 0.6/s per stack, up to 5 stacks, 3 s | 1.3 / 1.95 / 2.6 |
| **Mire** | Humility + Sloth | A heavy mire that holds them fast. | stasis field (105 px, 1.8 s) slowing 85%; heavy: x1.4 damage, x2 more against falling matter, 20% slower | 1.82 / 2.73 / 3.64 |
| **Moss** | Envy + Humility | A heavy venom stone that raises a wall. | venom: 0.6/s per stack, up to 5 stacks, 3 s; heavy: x1.4 damage, x2 more against falling matter, 20% slower; raises a stone wall (140 px) for 4 s that stops globs and Ybasogs | 1.82 / 2.73 / 3.64 |
| **Orchard** | Gluttony + Kindness | The slain fall as healing fruit. | kills drop 2 petals that heal 1 each; +0.3 damage and grows after every hit; its kills drop +1 void energy | 1.3 / 1.95 / 2.6 |
| **Permafrost** | Patience + Sloth | The whole rift freezes for a moment. | slows every Ybasog in the rift 45% for 1.6 s; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Poisoned Gift** | Charity + Envy | Splits into poisoned shards. | splits into 2 shards (0.6 damage each); venom: 0.6/s per stack, up to 5 stacks, 3 s. Shards carry: venom | 1.3 / 1.95 / 2.6 |
| **Respite** | Charity + Sloth | Lulls them to sleep; kills heal. | sleeps every Ybasog within 150 px for 1.56 s; a kill heals 1 energy | 1.3 / 1.95 / 2.6 |
| **Rivalry** | Diligence + Envy | A jealous twin strikes from the far wall. | a twin bolt launches from the mirror point across the rift; venom: 0.6/s per stack, up to 5 stacks, 3 s | 1.3 / 1.95 / 2.6 |
| **Rose** | Kindness + Lust | Thorny brambles that charm. | bramble patch (55 px): 1.3/s for 3 s; charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot | 1.3 / 1.95 / 2.6 |
| **Seraph** | Chastity + Pride | A seeking light that passes through. | steers toward the nearest Ybasog; passes through 3 targets; x2 damage to a target at full life | 1.3 / 1.95 / 2.6 |
| **Stasis** | Greed + Patience | A freezing field that holds them. | stasis field (105 px, 1.8 s) slowing 85%; slows 55% for 2.2 s | 1.3 / 1.95 / 2.6 |
| **Tithe** | Chastity + Greed | Takes a tenth of a Ybasog's life with each hit. | +10% of the target's current life as extra damage; passes through 3 targets | 1.3 / 1.95 / 2.6 |
| **Truth** | Chastity + Envy | Washes globs away; finishes the weak. | destroys enemy globs within 140 px; venom: 0.6/s per stack, up to 5 stacks, 3 s; kills any Ybasog (not the Mother) under 35% life | 1.3 / 1.95 / 2.6 |
| **Whirlpool** | Greed + Temperance | A spinning vortex that grinds them down. | a spinning vortex (95 px, 1.4 s) that pulls and grinds (1.54/s); knocks the target back 70 px; a diving grazer stops diving | 1.3 / 1.95 / 2.6 |
| **Zeal** | Charity + Wrath | Bursts into three burning shards. | splits into 3 shards; burns 1.6/s for 2 s; a kill heals 1 energy. Shards carry: burn | 1.3 / 1.95 / 2.6 |
| **Zenith** | Diligence + Pride | Flies out, then returns to strike again. | turns back after 0.55 s and returns to you, striking again; x2 damage to a target at full life | 1.3 / 1.95 / 2.6 |

### Fusion grid

Row nature + column nature. "void" marks opposites.

| | Prid | Gree | Wrat | Envy | Lust | Glut | Slot | Humi | Char | Pati | Kind | Chas | Temp | Dili |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| **Pride** | level up | Midas | Supernova | Vanity | Adoration | Gilded Maw | Throne | void | Largesse | Diamond | Laurel | Seraph | Comet | Zenith |
| **Greed** | Midas | level up | Hunger | Covet | Siren | Hoard | Event Horizon | Lodestone | void | Stasis | Harvest | Tithe | Whirlpool | Compound |
| **Wrath** | Supernova | Hunger | level up | Spite | Ardor | Wildfire | Smolder | Magma | Zeal | void | Ember Bloom | Crusade | Firestorm | Arc Forge |
| **Envy** | Vanity | Covet | Spite | level up | Obsession | Parasite | Rot | Moss | Poisoned Gift | Frostbite | void | Truth | Miasma | Rivalry |
| **Lust** | Adoration | Siren | Ardor | Obsession | level up | Indulgence | Reverie | Devotion | Embrace | Longing | Rose | void | Allure | Fervor |
| **Gluttony** | Gilded Maw | Hoard | Wildfire | Parasite | Indulgence | level up | Glut | Famine | Feast | Hibernate | Orchard | Fast | void | Grindstone |
| **Sloth** | Throne | Event Horizon | Smolder | Rot | Reverie | Glut | level up | Mire | Respite | Permafrost | Lichen | Daydream | Doldrums | void |
| **Humility** | void | Lodestone | Magma | Moss | Devotion | Famine | Mire | level up | Alms | Glacier | Hedgerow | Penance | Sandstorm | Bulwark |
| **Charity** | Largesse | void | Zeal | Poisoned Gift | Embrace | Feast | Respite | Alms | level up | Mercy | Blossom | Halo | Zephyr | Providence |
| **Patience** | Diamond | Stasis | void | Frostbite | Longing | Hibernate | Permafrost | Glacier | Mercy | level up | Evergreen | Snowdrop | Blizzard | Hourglass |
| **Kindness** | Laurel | Harvest | Ember Bloom | void | Rose | Orchard | Lichen | Hedgerow | Blossom | Evergreen | level up | Lily | Pollen | Hive |
| **Chastity** | Seraph | Tithe | Crusade | Truth | void | Fast | Daydream | Penance | Halo | Snowdrop | Lily | level up | Clarity | Oath |
| **Temperance** | Comet | Whirlpool | Firestorm | Miasma | Allure | void | Doldrums | Sandstorm | Zephyr | Blizzard | Pollen | Clarity | level up | Tempest |
| **Diligence** | Zenith | Compound | Arc Forge | Rivalry | Fervor | Grindstone | void | Bulwark | Providence | Hourglass | Hive | Oath | Tempest | level up |

**Fusion rules:** the union of both cards' natures is looked up in the recipe table (keyed by natures sorted and joined with "+"). An opposite pair anywhere in the set is void. Twins (same natures) rise a level. A set with no recipe is "unstable, not yet known" and nothing is spent. With 14 natures a 3-nature set has 364 possible combinations and a 4-nature set 1,001; they are planned as data in the same table (see section 19).

## 9. Effects glossary

| Effect id | Meaning with current values |
|---|---|
| `radiant` | x2 damage to a target at full life |
| `pull` | pulls Ybasogs within 110 px toward the hit for 0.6 s |
| `burn` | burns 1.6/s for 2 s |
| `venom` | venom: 0.6/s per stack, up to 5 stacks, 3 s |
| `charm` | charm 2.5 s: the Ybasog turns on its kin (3/s on contact) and is not shot |
| `devour` | +0.3 damage and grows after every hit; its kills drop +1 void energy |
| `sleep` | sleep 1.2 s: no moving, no attacking, pods do not hatch |
| `stone` | heavy: x1.4 damage, x2 more against falling matter, 20% slower |
| `split` | splits into 2 shards (0.6 damage each) |
| `heal` | a kill heals 1 energy |
| `chill` | slows 55% for 2.2 s |
| `thorns` | bramble patch (55 px): 1.3/s for 3 s |
| `pierce` | passes through 3 targets |
| `gust` | knocks the target back 70 px; a diving grazer stops diving |
| `chain` | lightning leaps to 2 more within 170 px at 70% damage |
| `well` | burning gravity well (95 px, 1.4 s, 2.2/s) |
| `field` | stasis field (105 px, 1.8 s) slowing 85% |
| `shatter` | bursts into 4 shards in a wide fan |
| `split3` | splits into 3 shards |
| `nova` | a ring of 6 shards |
| `mirror` | a twin bolt launches from the mirror point across the rift |
| `thrall` | charmed Ybasogs that die rise as your wisps for 6 s (ram for 3) |
| `execute` | kills any Ybasog (not the Mother) under 35% life |
| `orbit` | circles the blob for 1.6 s as a shield, then launches along the aim |
| `hoard` | kills drop x3 void energy |
| `tether` | binds the target to its nearest kin for 4 s: 50% of damage is shared |
| `lure` | a wide pull (1.8x radius, 1.5x strength) |
| `singularity` | a slow orb (half speed) that collapses into a black hole (130 px, 1.6 s) after 1 s or on impact; its core swallows pods and grazers |
| `doom` | marks the target: after 1.5 s it bursts for x3 the hit's damage |
| `frenzy` | a kill quickens your fire (interval x0.6) for 3 s |
| `killburst` | kills erupt in 8 shards |
| `mine` | clings to the target and erupts after 0.9 s: 3 damage in 70 px |
| `homing` | steers toward the nearest Ybasog |
| `leech` | heals 15% of the damage it deals |
| `spore` | kills leave a venom cloud (70 px, 3 s) |
| `echo` | a second copy fires 0.12 s later |
| `lullaby` | sleeps every Ybasog within 150 px for 1.56 s |
| `aegis` | a kill grants a ward that blocks one hit (up to 3) |
| `quake` | a shockwave along the whole row (+/-38 px) for x1.2 the hit's damage |
| `wall` | raises a stone wall (140 px) for 4 s that stops globs and Ybasogs |
| `storm` | a storm cloud (95 px, 3 s) striking a Ybasog every 0.35 s for 1.2 |
| `bloom` | kills drop 2 petals that heal 1 each |
| `timewarp` | slows every Ybasog in the rift 45% for 1.6 s |
| `decoy` | plants a flower decoy for 4 s: grazers dive at it, feeders spit at it, Ybasogs that touch it die |
| `beam` | an instant beam to the rift's edge, x1.5 damage to everything on the line (no bolt) |
| `snowball` | +4 ricochets; every wall ricochet adds +0.35 damage and size |
| `meteor` | a comet lands 0.55 s later: 4 damage in 65 px, and burns |
| `boomerang` | turns back after 0.55 s and returns to you, striking again |
| `prism` | splits into 3 bolts of random natures you have discovered (70% damage) |
| `magnet` | calls all void energy on screen to you for 2 s |
| `tithe` | +10% of the target's current life as extra damage |
| `vortex` | a spinning vortex (95 px, 1.4 s) that pulls and grinds (1.54/s) |
| `gild` | kills burst into 5 golden shards and drop +2 void energy |
| `cleanse` | destroys enemy globs within 140 px |
| `big` | huge and 40% slower |
| `long` | the charm lasts twice as long |
| `wide` | brambles 1.7x wider |

Status marks drawn on targets: burn (red arc), chill (blue arc), venom (green drops, one per stack), sleep (z), charm (pink heart), doom (pink ring filling), tether (dashed gold line).

## 10. Discoveries (achievements)

Your innate nature is known from the start; each other nature is discovered when its counter reaches the goal. Counters add up across all adventures and are saved.

| Nature | Achievement | Counter |
|---|---|---|
| Pride | Slay 1,500 Ybasogs | `kills` >= 1500 |
| Greed | Gather 2,500 void energy | `motes` >= 2500 |
| Wrath | Slay 3 Ybasogs within half a second, 30 times | `multikill` >= 30 |
| Envy | Slay 200 Ybasogs with bolts that ricocheted twice | `ricochets` >= 200 |
| Lust | Slay 150 Ybasogs close to your body | `closekills` >= 150 |
| Gluttony | Slay 400 pods before they hatch | `pods` >= 400 |
| Sloth | Slay 300 Ybasogs while standing still | `stillkills` >= 300 |
| Humility | Break 150 asteroids | `asteroids` >= 150 |
| Charity | Clear 15 waves with nothing escaping | `sealedwave` >= 15 |
| Patience | Clear 8 waves without being hurt | `cleanwave` >= 8 |
| Kindness | Clear 5 waves on 5 energy or less | `lowwave` >= 5 |
| Chastity | Shatter 40 ruins | `ruins` >= 40 |
| Temperance | Break 150 pieces of space debris | `debris` >= 150 |
| Diligence | Slay 120 feeders | `feeders` >= 120 |

## 11. Omens and depth

After every adventure the Rift Gate offers new rifts (two, three after a win). Each names an omen, a prevailing nature (most natured Ybasogs carry it, and it is weak to its opposite) and a boon, so the pool can be prepared for it. The first rift is always calm.

| Omen | Effect | Data |
|---|---|---|
| **A Quiet Drift** | Nothing stirs but the Ybasogs. | none |
| **The Brood Wakes** | Half again as many pods; void energy +25%. | pods 1.5, void 1.25 |
| **Rockfall** | Twice the asteroids, and they drop twice the void energy. | rocks 2, rockVoid 2 |
| **Ruinwake** | Ruins drift from the first stage and drop triple. | ruinsEarly 1, ruinVoid 3 |
| **The Hungry Dark** | Feeders spit faster; every kill drops one more. | spit 0.7, killVoid 1 |
| **Eclipse** | Bolts ricochet once more; Ybasogs have 20% more life. | bounces 1, hp 1.2 |
| **Resonance** | Nature effects last half again as long. | status 1.5 |

**Depth:** a win opens the next depth. Each depth adds 25% Ybasog life and 20% void energy; choose any depth up to the deepest opened.

## 12. Ybasogs

The hive draws on Lovecraftian horror: flesh, clusters of eyes, tendrils and hooded choirs, in original creatures. 58 kinds: 48 in 12 families of four variants, 6 mini-bosses and 4 bosses. Since iteration 6 the reachers, the deep, the jellies and the gaunts hunt you across the whole rift; hunters tire and leave after 24 s, and reachers withdraw into the wall after 13 s. Each kind appears first in the stage shown (one stage earlier from depth 2).

| Family | Ybasog | Life | Speed | From stage | Behaviour | Void |
|---|---|---|---|---|---|---|
| Pods | **Pod** | 3 | 30 px/s | 1 | Drifts down and hatches a grazer. | 2 |
| Pods | **Husk Pod** | 8 | 24 px/s | 1 | An armoured shell halves damage until it cracks; hatches a horned grazer. | 4 |
| Pods | **Clutch Pod** | 4 | 28 px/s | 2 | Bursts into three mites when it dies or hatches. | 3 |
| Pods | **Bloat Pod** | 3 | 22 px/s | 2 | Swollen with bile: bursts into a ring of globs when killed. | 3 |
| Grazers | **Grazer** | 4 | 44 px/s | 1 | Dives at you once it passes the middle. | 3 |
| Grazers | **Swift Grazer** | 3 | 70 px/s | 1 | Small and quick; dives early. | 3 |
| Grazers | **Stalker** | 5 | 40 px/s | 2 | Weaves as it falls and turns once mid-dive. | 4 |
| Grazers | **Horned Grazer** | 12 | 36 px/s | 2 | Tough; plunges straight down. | 5 |
| Feeders | **Feeder** | 7 | 26 px/s | 1 | Hovers and spits a glob at you. | 5 |
| Feeders | **Spitter** | 8 | 24 px/s | 2 | Three mouths: a spread of three globs. | 6 |
| Feeders | **Lobber** | 7 | 24 px/s | 2 | Lobs a sac that bursts into droplets above you. | 6 |
| Feeders | **Sniper** | 6 | 30 px/s | 3 | Aims a line, then fires a fast glob along it. | 6 |
| Eyes | **Watcher** | 6 | 30 px/s | 2 | Marks your column, then burns it with its gaze. | 5 |
| Eyes | **Blinker** | 5 | clings to a wall | 2 | Vanishes and reappears, spitting after each blink. | 5 |
| Eyes | **Weeper** | 6 | 20 px/s | 1 | Drips tears that fall straight down. | 4 |
| Eyes | **Oracle** | 7 | 26 px/s | 2 | Many eyes, one will: its globs seek you. | 5 |
| Devourers | **Devourer** | 9 | 40 px/s | 2 | Chases and eats your void energy, growing as it feeds. | 6 |
| Devourers | **Gorger** | 14 | 20 px/s | 2 | Eats falling matter it touches to heal and grow. | 7 |
| Devourers | **Maw** | 10 | 26 px/s | 3 | Its open mouth swallows bolts from below: strike it from the sides or bank shots off the top. | 6 |
| Devourers | **Void Leech** | 5 | 70 px/s | 2 | Latches on and drains you until shot off. | 4 |
| Deacons | **Warder** | 10 | 22 px/s | 2 | Shields Ybasogs around it. | 8 |
| Deacons | **Mender** | 9 | 22 px/s | 2 | Heals Ybasogs around it. | 8 |
| Deacons | **Herald** | 11 | 20 px/s | 3 | Calls pods out of the dark. | 8 |
| Deacons | **Cantor** | 9 | 22 px/s | 2 | Sings a ring that spreads across the rift. | 8 |
| Mites | **Mite** | 1 | 80 px/s | 1 | A flickering swarm. | 1 |
| Mites | **Lancer** | 1.5 | 60 px/s | 2 | Hovers, then darts at you. | 1 |
| Mites | **Thief Mite** | 1 | 90 px/s | 2 | Steals void energy on touch and flees. | 1 |
| Mites | **Biter Gnat** | 1 | 120 px/s | 1 | Circles you, then bites. | 1 |
| Wall crawlers | **Creeper** | 7 | 35 px/s | 2 | Crawls down a rift wall, spitting across. | 5 |
| Wall crawlers | **Lasher** | 12 | 30 px/s | 3 | Clings to a wall and sweeps a tendril across the rift. | 7 |
| Wall crawlers | **Weaver** | 8 | 30 px/s | 2 | Spins webs across the rift that catch bolts. | 6 |
| Wall crawlers | **Skitterer** | 7 | 90 px/s | 1 | Runs along the rift floor, spitting up at you. | 5 |
| Reachers | **Grasping Limb** | 6 | clings to a wall | 1 | Reaches from a wall at your height, then lunges. | 4 |
| Reachers | **Suckermouth** | 8 | clings to a wall | 2 | Clings to a wall and drags you toward its mouth. | 5 |
| Reachers | **Thrasher** | 10 | clings to a wall | 2 | Hangs from the top and sweeps down at you. | 6 |
| Reachers | **Stalk Eye** | 5 | clings to a wall | 1 | Peers from a wall on a stalk and fires at you. | 4 |
| The deep | **Shambler** | 9 | 42 px/s | 1 | A drowned thing that walks straight at you. | 5 |
| The deep | **Croaker** | 8 | 50 px/s | 2 | Keeps its distance and croaks out rings. | 5 |
| The deep | **Harpooner** | 7 | 60 px/s | 2 | Aims a line at you, then throws. | 6 |
| The deep | **Brine Priest** | 10 | 30 px/s | 3 | Marks where you stand; the brine there slows you. | 7 |
| Jellies | **Polyp** | 6 | 34 px/s | 1 | Drifts after you and stings on touch, unharmed. | 4 |
| Jellies | **Siphon Jelly** | 7 | 30 px/s | 2 | Drains your void energy while you are near. | 6 |
| Jellies | **Lantern Jelly** | 9 | 24 px/s | 2 | Its light bends your bolts toward it. | 6 |
| Jellies | **Man-o-War** | 8 | 28 px/s | 3 | Trails stinging tendrils far below it. | 6 |
| Gaunts | **Faceless Gaunt** | 6 | 250 px/s | 1 | Swoops at you in long arcs. | 5 |
| Gaunts | **Harrier** | 4 | 520 px/s | 2 | Marks your row, then strafes across it. | 4 |
| Gaunts | **Shrieker** | 7 | 60 px/s | 2 | Screams a ring that throws you back. | 5 |
| Gaunts | **Carrion Wing** | 6 | 120 px/s | 2 | Flies above you and drops bile bombs. | 5 |
| Mini-bosses | **Brood Warden** | 90 | 30 px/s | 1 | A shelled brood-carrier that sows pods and mites. | 25 |
| Mini-bosses | **Hive Knight** | 80 | 30 px/s | 1 | An armoured grazer that charges again and again. | 25 |
| Mini-bosses | **Gazer Prime** | 70 | 30 px/s | 2 | Burns three columns at once; five when hurt. | 25 |
| Mini-bosses | **Gorging Behemoth** | 110 | 26 px/s | 2 | Eats falling matter and hurls it back at you. | 30 |
| Mini-bosses | **Tide Matron** | 95 | 30 px/s | 2 | Calls the drowned and sings rings of brine. | 25 |
| Mini-bosses | **Gaunt Lord** | 85 | 220 px/s | 2 | A great faceless swooper that calls harriers. | 25 |
| Bosses | **The Choir** · Pride | 260 | 30 px/s | 2 | Three hooded heads behind orbiting wards; sings rings of globs and calls deacons. | 40 |
| Bosses | **The Hollow Eye** · Envy | 240 | 30 px/s | 2 | A vast eye that blinks across the rift and burns your column. | 40 |
| Bosses | **The Mother** · Gluttony | 320 | 40 px/s | 3 | The hive itself: pods, grazers, spreads of globs, hurled matter. | 30 |
| Bosses | **The Drowned King** · Sloth | 250 | 30 px/s | 2 | Dreaming in the deep: tidal walls, limbs from the rift walls, the drowned at his call. | 40 |

Life x (1 + 0.35 per later stage) x omen x depth; speed x (1 + 0.1 per later stage). Mini-bosses and bosses are not slowed, charmed, slept, pulled or executed; touching one costs 3 energy. Gaze beams and lashes are telegraphed for 1 s and cost 2.

### Natured Ybasogs

Some Ybasogs carry an innate nature, shown by an aura in its colour and its sigil. A natured Ybasog has 30% more life and drops 2 more void energy. **A bolt holding the opposite nature deals x1.6** ("WEAK"); **a bolt holding the same nature deals x0.5** ("RESISTS"); burns and venom follow the same multiplier. A fused bolt counts every nature it holds.

- **How many:** 20% of stage 1 Ybasogs, +12% per later stage, in a mixed tide.
- **Prevailing nature:** each adventure has one, shown with each rift at the Rift Gate; 60% of natured Ybasogs carry it (85% in a surge).
- **Tides:** each wave rolls a void tide (5% natured), a mixed tide, or a nature surge (70% natured). Surges grow more common: stage 1 void 40% / mixed 50% / surge 10%; stage 2 25 / 50 / 25; stage 3 15 / 45 / 40.
- **Bosses** have fixed natures: The Choir is Pride, The Hollow Eye is Envy, The Drowned King is Sloth, The Mother is Gluttony. Mini-bosses always carry a nature (60% the prevailing one).

## 13. Falling matter

| Family | Matter | Life | Speed | From stage | Behaviour | Void |
|---|---|---|---|---|---|---|
| Rocks | **Asteroid** | 4 | 36 px/s | 1 | Jagged rock; bolts ricochet off it. | 3 |
| Rocks | **Big Asteroid** | 11 | 30 px/s | 1 | Splits into two chunks. | 5 |
| Rocks | **Chunk** (from breaking) | 2.4 | 40 px/s |  | A broken piece of rock. | 1 |
| Rocks | **Iron Asteroid** | 22 | 24 px/s | 2 | Dense ore: tough, and rich in void energy. | 9 |
| Rocks | **Crystal Asteroid** | 5 | 34 px/s | 2 | Shatters into six shards that fly as your bolts. | 4 |
| Rocks | **Comet** | 3 | 230 px/s | 1 | Dangerously fast, with rare materials; a mark at the top warns of it. | 6 |
| Rocks | **Cinder Rock** | 4 | 34 px/s | 2 | Bursts in fire when broken, burning Ybasogs and you nearby. | 3 |
| Wrecks | **Hull Plate** | 3 | 105 px/s | 1 | A fast, spinning plate of hull. | 2 |
| Wrecks | **Debris Slab** | 6 | 40 px/s | 1 | A wide, slow slab that shelters what falls behind it. | 2 |
| Wrecks | **Hull Section** | 20 | 22 px/s | 2 | A long piece of a dead ship, dark windows in a row. | 6 |
| Wrecks | **Solar Panel** | 1.5 | 70 px/s | 1 | Fragile; breaks into a spray of scrap. | 2 |
| Wrecks | **Scrap** (from breaking) | 1 | 120 px/s |  | Shards of a panel. | 0 |
| Wrecks | **Engine Core** | 6 | 30 px/s | 2 | Explodes when broken, hurting only Ybasogs. | 4 |
| Wrecks | **Cargo Pod** | 8 | 30 px/s | 1 | Sealed cargo, full of void energy. | 14 |
| Wrecks | **Antenna Spar** | 3 | 60 px/s | 2 | A long spinning mast with a dish. | 2 |
| Ruins | **Ruin Wall** | 26 | 17 px/s | 2 | A carved wall, with two pods sheltering behind it. | 8 |
| Ruins | **Fallen Pillar** | 18 | 20 px/s | 2 | A fluted column, falling upright. | 6 |
| Ruins | **Broken Arch** | 12 | 18 px/s | 3 | Two legs and a lintel: bolts slip through the gap. | 4 |
| Ruins | **Sunken Idol** | 30 | 16 px/s | 3 | The carved head of a drowned god, its beard of tentacles. | 12 |
| Ruins | **Glyph Stone** | 10 | 22 px/s | 2 | Breaking its glyph grants you a ward. | 5 |
| Ruins | **Shrine** | 12 | 18 px/s | 2 | A small lit shrine; breaking it heals you 4. | 5 |
| Ruins | **Monolith** | 40 | 14 px/s | 3 | A black slab that drinks light: bolts never ricochet off it. | 15 |

Matter body-checks the blob and takes burn, chill and venom; it ignores sleep, charm and pulls. Matter that slips past costs nothing; a drop-carrying piece spoils a sealed wave.

## 14. Stages and waves

- **Stage 1, wave 1** is fixed: an asteroid and pods, a debris slab with three grazers behind it. It introduces pods, grazers, asteroids and the slab.
- **Every other wave** opens with a **debut**: 1 Ybasog kind not yet met this adventure (never-met kinds first, earliest stage first), shown alone, and one new kind of matter. The first meeting ever shows a name card. The wave then plays **3 + 1 per later wave and stage beats**, drawn from kinds already met: trickle, line, V formation, column, flank, swarm, rockfall, wreck field, ruin wall, elite pair, lull.
- **Mini-bosses** close stage 1 wave 3 and wave 2 of stages 2 and 3. **Bosses:** The Choir, The Hollow Eye or The Drowned King end stage 2; The Mother ends stage 3.
- About 9 new kinds are met in one adventure, so the bestiary (counted at the Altar) fills over several adventures.
- **Between waves 1 and 2, and 2 and 3,** a pick: take one of three pool cards (sometimes "temper a card +1"), or skip. After wave 3 comes the Rift Rest (section 3).

## 15. The save

*Source: Godot port (migrated 2026-11-19); the browser build keeps its own save.*

**Godot port:** `user://save.json` (on Windows `%APPDATA%\Godot\app_userdata\Voidwalker\save.json`), written whole through a temporary file. It holds the same collection as the browser save. A first launch starts the demo collection. A save that cannot be read is left untouched, the HUD shows SAVE PROTECTED, and that session's changes are not kept. RESET TO DEMO on the Journal's Natures tab (tap twice) writes the demo collection. Developer launch options are in the README.

**Browser build:** kept in the browser (`localStorage`, key `voidwalker.save.v1`): innate nature, discovered natures, collection, pool, fusions found, achievement counters, banked void energy, adventures and wins, depth, rite pity, sigils, relics and slots, logs, quests, the rifts offered and the chosen first card. Test addresses: `?demo=1` (a sample collection, never saved), `?nosave=1`, `?scene=altar&tab=gate`, `?scene=figures` (every Ybasog), `?stage=2&wave=1&autofire=1`, `?rest=1`, `?seed=N`.

## 16. Every tuning value

All 131 values the game reads live, from `prototypes/voidwalker/config.js`.

| Key | Value | Range | Step |
|---|---|---|---|
| `seed` | 1 | | |
| `volume` | 0.5 | 0 to 1 | 0.05 |
| `rite_cost` | 30 | 0 to 200 | 5 |
| `grand_rite_cost` | 270 | 0 to 2000 | 10 |
| `focus_cost` | 10 | 0 to 100 | 5 |
| `focus_share` | 0.5 | 0 to 1 | 0.05 |
| `rare_chance` | 0.2 | 0 to 1 | 0.01 |
| `mythic_chance` | 0.07 | 0 to 1 | 0.01 |
| `mythic_pity` | 10 | 1 to 50 | 1 |
| `void_refund` | 8 | 0 to 100 | 1 |
| `rest_actions` | 2 | 0 to 6 | 1 |
| `relic_chance` | 1 | 0 to 5 | 0.1 |
| `hitstop` | 0.06 | 0 to 0.3 | 0.01 |
| `music_volume` | 0.35 | 0 to 1 | 0.05 |
| `first_fusion_bonus` | 15 | 0 to 200 | 5 |
| `discovery_bonus` | 40 | 0 to 300 | 5 |
| `discovery_copies` | 2 | 1 to 5 | 1 |
| `loss_keep` | 0.6 | 0 to 1 | 0.05 |
| `win_bonus` | 100 | 0 to 1000 | 10 |
| `depth_void` | 0.2 | 0 to 1 | 0.05 |
| `depth_hp` | 0.25 | 0 to 1 | 0.05 |
| `deck_min` | 6 | 1 to 20 | 1 |
| `deck_max` | 12 | 4 to 30 | 1 |
| `max_level` | 3 | 1 to 6 | 1 |
| `stage_heal` | 5 | 0 to 20 | 1 |
| `player_speed` | 470 | 150 to 600 | 10 |
| `max_energy` | 20 | 5 to 60 | 1 |
| `iframes` | 0.8 | 0 to 3 | 0.1 |
| `hit_damage` | 2 | 0 to 10 | 1 |
| `glob_damage` | 1 | 0 to 10 | 1 |
| `bump_damage` | 1 | 0 to 10 | 1 |
| `magnet_radius` | 120 | 20 to 400 | 10 |
| `mote_speed` | 560 | 100 to 1200 | 20 |
| `fire_interval` | 0.17 | 0.05 to 0.6 | 0.01 |
| `bolt_speed` | 640 | 200 to 1200 | 20 |
| `bolt_bounces` | 4 | 0 to 12 | 1 |
| `bolt_damage` | 1 | 0.2 to 5 | 0.1 |
| `fused_bonus` | 0.3 | 0 to 2 | 0.05 |
| `level_bonus` | 0.5 | 0 to 2 | 0.05 |
| `card_drop` | 1 | 0 to 5 | 0.1 |
| `card_plain` | 0.005 | 0 to 0.2 | 0.005 |
| `card_natured` | 0.04 | 0 to 0.5 | 0.01 |
| `card_mini` | 0.6 | 0 to 1 | 0.05 |
| `card_boss` | 1 | 0 to 1 | 0.05 |
| `card_wreck` | 0.1 | 0 to 1 | 0.01 |
| `card_ruin` | 0.15 | 0 to 1 | 0.01 |
| `trial_chance` | 0.02 | 0 to 0.5 | 0.01 |
| `pick_temper` | 0.3 | 0 to 1 | 0.05 |
| `reshuffle_time` | 0.35 | 0 to 2 | 0.05 |
| `burn_dps` | 1.6 | 0 to 10 | 0.1 |
| `burn_time` | 2 | 0 to 6 | 0.1 |
| `chill_slow` | 0.55 | 0 to 0.95 | 0.05 |
| `chill_time` | 2.2 | 0 to 6 | 0.1 |
| `pull_radius` | 110 | 20 to 300 | 5 |
| `pull_strength` | 240 | 0 to 800 | 10 |
| `pull_time` | 0.6 | 0 to 3 | 0.1 |
| `split_count` | 2 | 1 to 6 | 1 |
| `heal_on_kill` | 1 | 0 to 5 | 0.5 |
| `shard_damage` | 0.6 | 0.1 to 3 | 0.1 |
| `radiant_mult` | 2 | 1 to 5 | 0.1 |
| `stone_mult` | 1.4 | 1 to 4 | 0.1 |
| `stone_matter` | 2 | 1 to 5 | 0.1 |
| `venom_dps` | 0.6 | 0 to 5 | 0.1 |
| `venom_stacks` | 5 | 1 to 12 | 1 |
| `venom_time` | 3 | 0 to 10 | 0.5 |
| `thorns_radius` | 55 | 10 to 200 | 5 |
| `thorns_dps` | 1.3 | 0 to 10 | 0.1 |
| `thorns_time` | 3 | 0 to 10 | 0.5 |
| `charm_time` | 2.5 | 0 to 10 | 0.5 |
| `charm_dps` | 3 | 0 to 10 | 0.5 |
| `pierce_count` | 3 | 1 to 10 | 1 |
| `devour_grow` | 0.3 | 0 to 2 | 0.05 |
| `gust_push` | 70 | 0 to 300 | 5 |
| `sleep_time` | 1.2 | 0 to 5 | 0.1 |
| `chain_count` | 2 | 1 to 6 | 1 |
| `chain_range` | 170 | 30 to 400 | 10 |
| `chain_share` | 0.7 | 0 to 2 | 0.05 |
| `well_radius` | 95 | 20 to 300 | 5 |
| `well_time` | 1.4 | 0 to 5 | 0.1 |
| `well_dps` | 2.2 | 0 to 10 | 0.1 |
| `field_radius` | 105 | 20 to 300 | 5 |
| `field_time` | 1.8 | 0 to 5 | 0.1 |
| `field_slow` | 0.85 | 0 to 1 | 0.05 |
| `execute_line` | 0.35 | 0 to 1 | 0.05 |
| `doom_time` | 1.5 | 0.1 to 5 | 0.1 |
| `doom_mult` | 3 | 0 to 10 | 0.5 |
| `storm_radius` | 95 | 20 to 300 | 5 |
| `storm_time` | 3 | 0.5 to 10 | 0.5 |
| `storm_zap` | 1.2 | 0 to 10 | 0.1 |
| `meteor_damage` | 4 | 0 to 20 | 0.5 |
| `meteor_radius` | 65 | 10 to 200 | 5 |
| `mine_damage` | 3 | 0 to 20 | 0.5 |
| `mine_radius` | 70 | 10 to 200 | 5 |
| `quake_damage` | 1.2 | 0 to 10 | 0.1 |
| `wall_time` | 4 | 0.5 to 12 | 0.5 |
| `timewarp_time` | 1.6 | 0 to 6 | 0.1 |
| `timewarp_slow` | 0.45 | 0 to 0.95 | 0.05 |
| `thrall_time` | 6 | 0 to 20 | 0.5 |
| `frenzy_time` | 3 | 0 to 10 | 0.5 |
| `frenzy_rate` | 0.6 | 0.1 to 1 | 0.05 |
| `leech_share` | 0.15 | 0 to 1 | 0.01 |
| `decoy_time` | 4 | 0 to 12 | 0.5 |
| `ward_max` | 3 | 1 to 10 | 1 |
| `pod_hatch` | 9 | 1 to 30 | 0.5 |
| `grazer_dive_line` | 0.45 | 0.1 to 0.8 | 0.05 |
| `glob_speed` | 170 | 40 to 500 | 10 |
| `mother_hp` | 320 | 20 to 2000 | 10 |
| `mother_spawn` | 3.2 | 0.5 to 10 | 0.1 |
| `mother_spread` | 3.6 | 0.5 to 10 | 0.1 |
| `mother_volley` | 7 | 1 to 20 | 1 |
| `telegraph_time` | 1 | 0.2 to 3 | 0.1 |
| `beam_damage` | 2 | 0 to 10 | 1 |
| `pulse_speed` | 210 | 60 to 600 | 10 |
| `pulse_damage` | 1 | 0 to 5 | 1 |
| `brine_slow` | 0.5 | 0 to 0.9 | 0.05 |
| `brine_time` | 5 | 1 to 12 | 0.5 |
| `hunt_time` | 24 | 8 to 60 | 1 |
| `reach_time` | 13 | 4 to 30 | 1 |
| `leech_drain` | 1.3 | 0.3 to 5 | 0.1 |
| `pull_speed` | 95 | 0 to 300 | 5 |
| `shriek_push` | 520 | 0 to 1500 | 20 |
| `nature_chance` | 0.2 | 0 to 1 | 0.05 |
| `nature_chance_step` | 0.12 | 0 to 0.5 | 0.01 |
| `prevailing_share` | 0.6 | 0 to 1 | 0.05 |
| `opposite_mult` | 1.6 | 1 to 4 | 0.05 |
| `same_mult` | 0.5 | 0 to 1 | 0.05 |
| `stage_hp_scale` | 0.35 | 0 to 2 | 0.05 |
| `stage_speed_scale` | 0.1 | 0 to 1 | 0.02 |
| `beats_base` | 3 | 1 to 12 | 1 |
| `beats_step` | 1 | 0 to 6 | 1 |
| `debuts_per_wave` | 1 | 0 to 3 | 1 |

## 17. Development history and changelog

### Timeline

| When | Step | Result |
|---|---|---|
| 2026-11-18, evening | `/start` onboarding | Existing work found: `design/concepts/voidwalker-concept-notes.md` and `voidwalker-mindmap.mm`. `design/references/2.5d-tactics-reference-gdd.md` was set aside as a different game. Settings saved: stage `Concept`, rigor `minimal`, goal `hobby`, automation `guided`. |
| 2026-11-18, evening | Design direction | You chose stages inspired by Ball x Pit, with the innate natures and deckbuilding in place of the town. |
| 2026-11-18, evening | `/prototype` brief | Genres: survivors-like x roguelike deckbuilder. Defaults taken "for fun". Your two changes before the build: falling space debris, ruins and asteroids alongside the Ybasogs, and room for 3- and 4-nature combinations later. |
| 2026-11-18, 22:13 to 22:29 | **First build** (`f8326dd`, tag `one-shot/voidwalker`) | Playable in 14 minutes. Small check: boots, screenshot, brief words, emoji and outlines all passed. Brief checklist 32 of 32. |
| 2026-11-18, 22:47 to 23:11 | **Iteration 1** (`2c3cd10`) | The collection game, all 14 natures, all 84 fusions, 15 forms, the Altar hub, the save. Light check: no errors, a play run and a screenshot of every Altar tab. |
| 2026-11-18, 23:15 | This reference | Generated from `config.js`, then published as the Voidwalker Codex page. |
| 2026-11-18, 23:22 to 23:30 | **Iteration 2** (`0053f63`) | Achievements made hard to get. |
| 2026-11-18, 23:32 to 23:52 | **Iteration 3** (`708c5d9`) | 34 Ybasog kinds, natured Ybasogs, nature tides, procedural waves with debuts, 20 kinds of matter. |
| 2026-11-19 | **Iteration 4** (`1d5e28a`) | The rift widened to fill the screen; the adventure HUD made wordless. |
| 2026-11-19 | **Iteration 5** (`9c8ad60`) | The hub became a place with six stations; Rift Rest; sigils, relics, logs, quests, rifts; pixel look, combat feel and a score. |
| 2026-11-19 | **Iteration 6** (`f6c1dd9`) | Free roam, no leak damage, 23 new kinds and The Drowned King. Checked with a headless harness: each new kind alone for 30 s, three full seeded runs to a win, no errors. |
| 2026-11-19 | Chapter II plan (`7edf2ca`) | `design/chapter-2-the-silent-foundry.md`: a civilization destroyed by its own AI, 34 machine kinds. Not built. |
| 2026-11-19 | **Iteration 7** (`ef7b610`) | The deck is found in the adventure; Inborn and Chosen sigils; trial cards; full 1280×720. Three full runs (one from a new save) won with no errors. |
| 2026-11-19 | **Iteration 8** (`e31844b`) | Pixel-perfect rendering, the Spleen pixel font, hard edges, bolts leave the view, pixelfx void flames in the hub. |
| 2026-11-19 | **Iteration 9** (`96f41a9`) | Void-trail flames, a deep-space shader, a shader highlight on hover, still blinking stars, code-drawn lobby sprites. The last browser iteration. |
| 2026-11-19 | Godot set up (`08cd289`, `8085c9d`) | Godot 4.8 beta 4 .NET, 640×360 base resolution, whole-number scaling, landscape. |
| 2026-11-19 | Hub port (`da181db`, `4416be3`) | The Rift Altar and its six station screens in C#, with the save. Built and installed on an Android phone; all six stations passed by touch. |
| 2026-11-19 | Forms, relics, phone scaling (`4f71af7`, `00703ce`, `87f074a`) | The blob's 15 forms drawn and shown on the altar and the Pool screen; relics over the pedestals; bigger pixels and fonts on phones. Checked on the phone. |
| 2026-11-19 | Gate rift, Rite crack (`784e8a8`, `e1c4803`) | The Gate's rift and the Rite's crack in space-time as pixelfx effects. |
| 2026-11-19 | Lobby islands (`20b1e1b`) | Each station on its own island, faceted rock, depth redraws, void balls drifting from the crack to the rift, tap to dodge or burst. Checked in desktop Godot. |

### Changelog

#### The Godot port: the Rift Altar hub (`08cd289` to `20b1e1b`)

The port moves the game to Godot 4.8 beta 4 .NET (C#), mobile-first, one system at a time. The browser build stays the full game until the adventure is ported. Section 18 covers what the port holds now.

- **Added** the Rift Altar with its six station screens (Pool, Void Rite, Rift Gate, Sigils, Relics, Journal) and the save. The rules are plain C# shared with the tests, checked against fixtures from the browser game.
- **Added** an Android build. The six stations passed by touch on a phone, with the save kept safe.
- **Added** the blob's 15 forms, drawn in code. The altar and the Pool screen show the pool's dominant form, and equipped relics float over their pedestals.
- **Changed** the scaling for phones: bigger pixels and fonts, a camera that frames the hub, and screens that page their content when it does not fit.
- **Changed** the Rift Gate into a pixelfx rift painted in the chosen rift's nature, turning counter-clockwise as it drains. The Void Rite became a crack in space-time with void seeping out.
- **Added** a floating island for each station, faceted rock under the islands and the altar, and stones, pedestals, lectern and tome redrawn with depth.
- **Changed** the hub's void flames into void balls. Each one leaves the Rite's crack, drifts on a smooth curve through one to three stops at its own pace and on its own depth lane, and is drawn into the rift. A Rite draw releases a burst. Tapping a ball makes it dart away or burst.

#### Iteration 9: void trails, deep space, the hub lit on hover (`96f41a9`)

Your words for this iteration were not recorded in this reference. The changes are from the brief's Features list.

- **Changed** the hub's flames into balls with real tails from a new pixelfx effect: each ball sheds flame that cools from lavender to deep violet and rises. The four altar pillars and their fire are gone.
- **Added** a deep-space background from a fragment shader: a drifting violet nebula with dark dust lanes and a teal vein, in exact palette colours with a 4×4 ordered dither.
- **Changed** hovering a station: the object itself lights up with a pale-gold outline, a breathing ring and a sweep of light, in place of a dashed circle. Stations carry no names; keys 1 to 6 still open them.
- **Changed** the stars to hold still, each brightening and fading on its own 6 to 18 s cycle.
- **Changed** the lobby's props to code-drawn pixel sprites.

#### Iteration 8: pixel-perfect (`e31844b`)

Your words for this iteration were not recorded in this reference. The changes are from the brief's Features list.

- **Changed** the rendering to pixel-perfect: one pixel is one colour, every pixel is the same size, no blur or glow, whole-number scaling only, screen shake in whole pixels. A small rasteriser fills every shape at pixel centres, so no edge is blended.
- **Changed** the text to Spleen, a pixel font by Frederic Cambus (BSD-2-Clause), at its native sizes.
- **Changed** bolts to leave the view past the top or the bottom instead of bouncing, except orbit guards and boomerangs. Lodestone (Greed + Humility) now flies out and swings back.
- **Changed** the hub's fireflies into pixelfx spirit flames flowing round the altar, and the Void Rite's crack into a narrow pixelfx void rift.

#### Iteration 7: the deck is found in the adventure (`ef7b610`)

Your words: *"There are too many cards (power) from the beginning. You purchase and gacha cards in preparation but you collect those cards in adventure. You do not use all the cards in your deck from the beginning of a new adventure session."* Your answers: start with 1 void card and upgrade cards temporarily in the adventure; permanent upgrades to start with an innate card or two; cards from kills, wave-clear picks, the Rift Rest and broken matter at different chances; the loadout as the pool, and very rarely a card from outside it, for that adventure only. And: *"we need also to increase the resolution of the game."*

- **Changed** every adventure to begin with one void card; the Altar's deck became the **pool** the adventure's cards come from.
- **Added** card drops (plain 0.5%, natured 4%, mini-boss 60%, boss always, wreck 10%, ruin 15%), a pick of one of three after waves 1 and 2 (sometimes "temper +1"), and a found card at the Rift Rest.
- **Added** trial cards: 2% of found cards come from outside the pool, marked TRIAL, for that adventure only.
- **Added** sigils Inborn (begin with your form's nature card) and Chosen (begin with a card picked at the Gate; unlocked by a win).
- **Changed** merging at the Rest: allowed down to a single card.
- **Changed** the screen to full 1280×720, same palette.
- **Fixed** during the check: the Sigils page overflowed into the unlocks list, and badge text was hard to read.

#### Iteration 6: free roam and the hunters (`f6c1dd9`)

Your words for this iteration were not recorded in the brief; it was begun in an earlier session and finished from its config changes.

- **Changed** movement: the blob roams the whole rift and aims anywhere; bolts bounce off the floor. Nothing drains energy by slipping past the bottom.
- **Added** 23 Ybasog kinds: a fourth variant for every older family (Oracle, Void Leech, Cantor, Biter Gnat, Skitterer); four families that hunt you, reachers (Grasping Limb, Suckermouth, Thrasher, Stalk Eye), the deep (Shambler, Croaker, Harpooner, Brine Priest), jellies (Polyp, Siphon Jelly, Lantern Jelly, Man-o-War) and gaunts (Faceless Gaunt, Harrier, Shrieker, Carrion Wing); mini-bosses Tide Matron and Gaunt Lord; and a third stage 2 boss, The Drowned King.
- **Added** spreading rings, slowing brine pools, latching leeches, bolt-bending lanterns, row strafes and tidal walls; hunters tire and leave after a while.
- **Changed** the Stag (regrows energy) and the Manta (longer safety) traits, which had leaned on the old band and leaks.

#### Iteration 5: the hub as a place, and progression (`9c8ad60`)

Your words: *"Our game is roguelite. But we still want a HUB where the cards are prepared in deck. Just like the town in BALL x PIT. Just that there is no building but just card deck preps and gacha. The card upgrades and merge between stages is not that. There should be also some progression/upgrades/differences between adventure sessions. Make it less like some flash game or a simple browser game."*

- **Changed** the Altar into a place: a floating altar with six stations (Deck, Void Rite, Rift Gate, Sigils, Relics, Journal), and a reveal ceremony for the Rite.
- **Added** the Rift Rest between stages (merge and temper for that adventure only, then a path); the Forge's fusing moved there.
- **Added** sigils, unlocks, a choice of rifts with boons, 16 relics in slots, 16 logs and 12 quests in the Journal.
- **Changed** the look to pixels on a limited palette with a pixel font, and added hit-stop, impact flashes, layered sound and a score that follows the scene.

#### Iteration 4: the wide rift (`1d5e28a`)

Your words: *"Make the gameplay view (perspective) much larger, and adjust all elements behaviors to the enlarged view. Make the adventure UI simpler with less words / wordy = less fun."*

- **Changed** the rift to fill the screen, with spawns and behaviours spread to the wider field.
- **Changed** the adventure HUD to marks, not words: an energy bar, a void count, stage and wave pips, the tide mark, the next bolts as sigils, the form.

#### Iteration 3: the hive grows (`708c5d9`)

Your words: *"We will eventually move to Godot .net but now stay on browser. Now give Ybasogs more type variations (same type but different variants) -- at least 3 and more types -- at least 30 including what we have now -- including mini bosses and bosses."* · *"Let some ybasog have an innate nature. Opposite nature are more effective. Same natures are less effective."* · *"Make sure to create good points in time where you start confronting each type. There should be times when there are more innate ones or more void ones. Make the spawning more procedural. Make more debris and ruin types and variants too."* · *"Ybasogs are based on Cthulhu mythos inspirations so you can take more references from Lovecraftian horrors"*

- **Added** 30 Ybasog kinds (34 in all): 8 families of 3 or 4 variants each, plus 4 mini-bosses and 2 new bosses, The Choir and The Hollow Eye. All are drawn as Lovecraftian flesh, eyes and tendrils, each kind with its own outline and behaviour (telegraphed gaze columns, tendril lashes, webs that catch bolts, a maw that swallows bolts from below, thieves, healers and shielders).
- **Added** natured Ybasogs: an aura and sigil, ×1.6 damage from the opposite nature and ×0.5 from the same, and a prevailing nature for each adventure shown at the Altar.
- **Added** nature tides for each wave: void tide, mixed tide, nature surge.
- **Changed** wave building to a director: a debut of kinds not yet met, with a name card the first time ever, then weighted beats such as formations, flanks, swarms, rockfalls, wreck fields, ruin walls, elite pairs and lulls. Mini-bosses close waves in every stage.
- **Added** 15 kinds of falling matter (20 in all, in rock, wreck and ruin families), several with break effects.
- **Added** a Ybasogs-met count at the Altar, a test option (`?stage=&wave=&autofire=1`), and Godot .NET in the brief as the later engine.
- **Fixed** during the check: the debut card's drawing function shared a name with the card drawer and crashed the first run.

#### Iteration 2: achievements made hard to get (`0053f63`)

Your words: *"Achievements should be hard to get."*

- **Changed** every discovery goal, raising each 5 to 30 times so most natures take several adventures. Examples: Pride needs 1,500 kills (was 80), Greed 2,500 void energy (was 120), Patience 8 waves without being hurt (was 1), Chastity 40 ruins (was 2).
- **Note:** the Void Rite draws only from discovered natures, so early adventures draw mostly the innate nature, and fusions open up only once a second nature is found.

#### Iteration 1: the Rift Altar collection, all 14 natures and 84 fusions (`2c3cd10`)

Your words: *"Cards/innate natures are discovered in adventures as achievement rewards... deck types are supposed to be "joined" between adventure sessions. Now let's add all innate natures and all combos. Many combos are not just combined powers. They can be unique. Be creative. The player character shapes can be also very unique: they give certain stat and effect benefits/traits. Raise the void costs to balance the economy."* And: *"Make gachability and deckbuilding specific to this game: collections are fun and need to be rewarding -- preparation before adventures which can be fun and replayability matters."*

- **Added** all 14 innate natures (was 4), each with its own effect and sigil.
- **Added** all 84 two-nature fusions (was 4). Most have a mechanic of their own (black hole, comet, mirror-twin, beam, stone wall, decoy, thralls, storms, orbit guard, boomerang, tether, doom mark, execute, prism and more), not just two effects added together.
- **Added** 15 forms (was 4): shapeless plus a lion, rat, dragon, serpent, moth, toad, snail, tortoise, dove, wanderer, stag, swan, manta and bee, each with a trait that helps and costs.
- **Changed** how cards are gained: no more picks between waves. Natures are discovered by achievements whose progress carries across adventures. Cards join the collection, then the deck, only at the Altar.
- **Changed** the Rift Altar from a between-stages stop to the hub before every adventure, with five tabs: Prepare, Void Rite, Forge, Codex, Discoveries.
- **Added** the Void Rite gacha: draws from discovered natures only, a focus option, rare and mythic odds, a pity guarantee and NEW marks.
- **Added** to the Forge: twins level up to III, opposites annul with a refund, and a first-time fusion pays a bounty and fills the 84-cell Codex. A Void Bolt melts into a nature card.
- **Added** omens (7) and depth, so each adventure differs and the deck is prepared for it.
- **Added** a save in the browser, with "Forget everything".
- **Changed** the economy. Costs went up: rite 6 to 30 (ten for 270), fuse 3 to 20, focus +10. Rewards: annul refund 8, discovery +40, first fusion +15, win +100, 60% kept after a fall.
- **Fixed** during the check: card names showed "Void Bolt 1" (level I is now unnamed), "in deck" labels overlapped the next row of cards, and the rite focus row overlapped its description. A pod-hatching condition that would never fire was corrected before the first run.

#### First build: the slice (`f8326dd`, tag `one-shot/voidwalker`)

- One adventure of 3 stages x 3 waves in a vertical rift, ending on the Mother.
- The deck as the magazine: each bolt is the next card, ricocheting off the walls, enemies and matter.
- 4 natures (Wrath, Patience, Greed, Charity) and their 4 fusions (Hunger, Zeal, Stasis, Mercy); opposites annul.
- Pick 1 of 3 cards or skip after each wave; a Rift Altar between stages (gacha, fuse, annul).
- 4 Ybasogs (pod, grazer, feeder, Mother) and 3 kinds of falling matter (asteroids that split, debris, ruins with pods behind them).
- The firefly intro and a random innate nature; 4 blob forms.
- Title, pause, game over, results and mute; Web Audio tones; every value in `config.js`.
- Fixed during the small check: plain rectangles in the background redrawn as broken shapes, and the first wave retimed so the debris slab and grazers arrive near 0:40.

### Decisions log

| Decision | Why | Who |
|---|---|---|
| The deck starts as one void card; the loadout is the pool | Your note after iteration 6: too much power from the start | you; the drop chances are guessed |
| Free roam, no leak damage | Iteration 6, finished from its config | earlier session |
| A hub with stations, sigils, relics and the Rift Rest | Your note: progression between adventures, less like a flash game | you; the contents are guessed |
| Port to Godot 4 .NET, mobile-first, the hub first | The browser build proved the fun; Android now, iOS later | you |
| Lobby art drawn in code, not by an image model | Your choice for the forms, then kept for the lobby | you |
| Void balls drift slowly with quick moves kept for taps; a tap bursts or dodges | Your notes in the lobby reviews | you; the burst chance (40%) is guessed |
| Browser game first, engine later | No engine set; the one-shot proves the fun before an engine is chosen | the workflow |
| Rigor `minimal` | No scope signals at the start; `design/game-brief.md` is the design record | guessed, you saved it |
| Stage shape like Ball x Pit; the town replaced by the Altar | Your direction | you |
| Ybasogs, debris, ruins and asteroids all descend | Your change to the brief | you |
| Fusion table keyed by any set of natures | Room for 3- and 4-nature combinations | you |
| Cards join only between adventures, found by achievements | Your note after the first build | you |
| Collection, gacha and preparation as the meta loop; omens and depth for replay | Your note: collections rewarding, preparation fun, replayability | you; the omen and depth design is guessed |
| A fall keeps 60% of the void energy | So a lost adventure still feeds the collection | guessed |
| Fusion names and mechanics, form traits, achievement goals | "Be creative" | guessed |

### Files

| File | What it is |
|---|---|
| `prototypes/voidwalker/index.html` | The game (Canvas 2D, Web Audio, no libraries) |
| `prototypes/voidwalker/config.js` | Every tuning value, plus the natures, recipes, discoveries and omens as data |
| `design/game-brief.md` (copy in `prototypes/voidwalker/brief.md`) | The one-page brief: your words, Features, systems, scope |
| `prototypes/voidwalker/evidence/` | Check results and screenshots (`iteration-6-figures.png` shows every Ybasog; `iteration-7-*.png` the pick, Rest, Gate and Sigils screens) |
| `design/chapter-2-the-silent-foundry.md` | The Chapter II plan (not built) |
| `design/concepts/` | Your original concept notes and mindmap |
| `design/voidwalker-reference.md` | The same reference as text |
| `README.md` | How to build, run, test-play as a developer, test and export the Godot port |
| `project.godot`, `Voidwalker.sln` | The Godot port's project and its C# solution |
| `src/core/` | The port's rules in plain C# (game data, the save's model, the Altar's rules, void-ball paths) |
| `src/hub/`, `src/ui/` | The hub scene and the six station screens |
| `src/data/game_data.json`, `src/data/hub_config.tres` | The game data (shared with the browser build) and the hub's tuning values |
| `tests/unit/Rules/` | The port's tests (`dotnet test Voidwalker.sln`) |
| `tools/art/` | The code-drawn lobby art and the pixelfx bakes |
| `production/qa/evidence/` | The port's check screenshots and device results |

## 18. Migration to Godot

The game moves from the browser build to Godot 4.8 beta 4 .NET (C#), mobile-first, one system at a time. When a system is migrated, its section here switches its source to the Godot port; until then the browser build is the source. How to build, run, test-play and export the port: `README.md`.

| System | Source now | Migrated |
|---|---|---|
| Game data (natures, recipes, sigils, relics, logs, quests, rifts) | Godot port (`src/data/game_data.json`, shared) | 2026-11-19 |
| The Rift Altar hub and its six stations (section 4) | Godot port | 2026-11-19 |
| The save (section 15) | Godot port (its own save) | 2026-11-19 |
| The blob's 15 forms, as hub and Pool sprites (section 7) | Godot port (sprites); traits still apply only in the browser adventure | 2026-11-19 |
| The adventure: the rift, bolts and the deck (section 3) | Browser build | not yet |
| Ybasogs and falling matter (sections 12, 13) | Browser build | not yet |
| Stages, waves, tides and omens in play (sections 11, 14) | Browser build | not yet |
| The Rift Rest (section 3) | Browser build | not yet |
| Discoveries counted in play (section 10) | Browser build (the port shows progress) | not yet |
| Sound and score | Browser build | not yet |

## 19. Known gaps and next steps

**Not checked yet (browser):** these were tested only by a headless harness. Your play is the check.

- How the start feels with one card, and whether the drop chances fill the deck at a good pace.
- The first adventure: its pool is only six cards, and the harness took far more damage than in later runs.
- Difficulty with hunters chasing across the whole rift, and how readable the new families are.
- The Drowned King, the Tide Matron and the Gaunt Lord in real play.
- Whether the economy's pace feels right over several adventures.

**Later, by the brief:**

- Chapter II, the Silent Foundry: add it as a feature (plan in `design/chapter-2-the-silent-foundry.md`).
- 3-nature (364) and 4-nature (1,001) combinations: grow it.
- The open-world void to glide through, with planets: grow it.
- The story (fireflies, the Akashic Records, the 1000 days, the morality endings): add it as a feature.
- More stages and characters: grow it.
- The Godot port: the adventure (the rift, combat, Ybasogs, falling matter, the Rift Rest) next, then iOS.

**Open in the port:**

- The serpent and swan forms show no eyes at the hub's size.
- About 400 colours on screen; no global palette snap yet.
- The lobby islands are checked in desktop Godot only, not yet on the phone.

**Next:** play several adventures, then say what to change.

