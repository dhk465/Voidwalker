# Voidwalker: Game Reference

*Generated 2026-11-18 from `prototypes/voidwalker/config.js` (game data and tuning) and the project history. Numbers are the current `config.js` values; change them there and regenerate.*

**Build:** `prototypes/voidwalker/index.html` (open in a browser) · **Brief:** `design/game-brief.md` · **Checkpoints:** tag `one-shot/voidwalker`, commit `708c5d9` (iteration 3)

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
- 18. Known gaps and next steps

## 1. The game in one page

A blob born of the void holds the bottom of a vertical rift while Ybasogs (a hive of organic creatures) and falling matter (asteroids, space debris, ruins) pour down. Its **deck of nature cards is its ammunition**: each bolt fired is the next card in the deck, and bolts ricochet off the rift walls, enemies and matter. The fourteen **innate natures** are the seven sins and seven virtues, each an element. Two natures fuse into one of **84 fusions**; two **opposites annul** each other into void.

The loop: **prepare** a deck at the **Rift Altar** → **adventure** through 3 stages of waves ending on the **Mother** → **bank** the void energy gathered → spend it on the **Void Rite** (gacha) and the **Forge** (fusion) → prepare again. **Natures are discovered by achievements** during adventures and join the collection at the Altar, never mid-adventure. The deck's most common nature shapes the blob into one of 15 **forms**, each with a trait. **Omens** and **depth** change each adventure, so the deck is prepared for what waits.

Genres: survivors-like (the horde, movement) x roguelike deckbuilder (the collection, deck, fusion). Stage shape inspired by Ball x Pit; the town is replaced by the Altar.

## 2. Controls

| Where | Input | Does |
|---|---|---|
| Title | Enter, Space or click | Begin (first time) or go to the Altar |
| Adventure | WASD or arrow keys | Move inside the bottom band |
| Adventure | Mouse | Aim (always kept upward) |
| Adventure | Hold left mouse button | Fire |
| Adventure | Q | Auto-fire on or off |
| Adventure | Space, P or Esc | Pause |
| Anywhere | M | Mute |
| Altar | Mouse | Everything; 1-5 switch tabs; Enter begins the adventure; Esc to title |
| End screen | Enter, R or click | Return to the Altar |

## 3. The adventure

- **Energy is life.** Start at 20 (plus the form's bonus). A bite costs 2, a glob 1, a body-check from matter 1. A Ybasog slipping past the bottom costs 3, matter 1. After a hit you are safe for 0.8 s. Clearing a stage restores 5.
- **Bolts:** one every 0.17 s at 640 px/s, 4 ricochets before fading. Base damage 1; +50% per card level above I; fused cards +30%.
- **The deck is the magazine.** Shuffled at the start of each wave. Each shot draws the next card; an empty draw pile reshuffles the discard (0.35 s with no fire). The left panel shows the next bolts in order.
- **Void energy** motes drop from kills and broken matter and fly to you within 120 px (all of them at a wave's end).
- **Structure:** 3 stages x 3 waves. Stage 3 wave 3 is the Mother. Win: the Mother falls. Loss: energy reaches 0.
- **The first adventure** begins alone in the dark: a firefly whispers "take it" beside a crack of light; touching it gives a random innate nature of the 14, 4 Void Bolts and 2 cards of that nature.

## 4. The Rift Altar

The hub between adventures, in five tabs. The collection (every card owned, stacked by kind and level) is always on the left.

| Tab | What you do |
|---|---|
| 1 Prepare | Click collection cards to add them to the deck (6 to 12 cards); click a deck row to remove one. See the form the deck gives and its trait, the omen of the next adventure, and choose the depth. Begin. |
| 2 Void Rite | The gacha. Draw from the natures you have discovered: one rite 30, ten rites 270. Optional focus on one nature (+10 per draw): 50% of draws land on it. Odds: rare (level II) 20%, mythic (an already-fused card, written into the Codex) 7%, and a mythic is certain within 10 draws (pity). New kinds are marked NEW. |
| 3 Forge | Fuse two cards for 20. Two natures make their fusion (unknown ones show "?" until forged); a first-time fusion pays 15 and fills its Codex cell. Twins rise a level (up to III). Opposites annul: both cards are gone, 8 back. A Void Bolt melts into any nature card (a way to trim plain bolts). |
| 4 Codex | The 84 fusions as a grid; point at a cell to read it. A brighter "?" can already be made from natures you know. |
| 5 Discoveries | The 14 natures, each one's achievement and progress, and the form each gives. "Forget everything" (click twice) wipes the save. |

## 5. The economy

| Source or sink | Void energy |
|---|---|
| Pod / grazer / feeder kill | 2 / 3 / 5 motes |
| Mother | 30 motes |
| Asteroid / big asteroid / chunk | 3 / 5 / 1 |
| Debris / ruin | 2 / 8 |
| Banked after a win | everything gathered x omen x depth, + 100 |
| Banked after a fall | 60% of it |
| Discovering a nature | +40, and 2 cards |
| First time forging a fusion | +15 |
| Annulling opposites | +8 (after paying 20) |
| One rite / ten rites | -30 / -270 (+10 each when focused) |
| Fusion | -20 |

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
| Stag | Kindness | slowly regrows energy, leaks hurt 1 less | regen 0.06, leak_guard 1 |
| Swan | Chastity | 40% of globs pass through you | glob_dodge 0.4 |
| Manta | Temperance | 30% faster, a taller band to roam | speed 1.3, band 60 |
| Bee | Diligence | +30% fire rate, -15% bolt damage | fire_rate 1.3, damage 0.85 |

Trait keys: damage and fire_rate multiply; max_energy adds; magnet adds px; mote_bonus is the chance of an extra void energy per mote; bite adds to bite damage; bounces adds ricochets; status multiplies effect durations; contact_charm is the chance a biting Ybasog is charmed instead; speed multiplies movement; aura slows Ybasogs within 160 px; armor reduces each hit (never below 1); wave_heal heals after every wave; iframes adds safety time; quick_shuffle removes the reshuffle pause; regen is energy per second; leak_guard reduces leak damage; glob_dodge is the chance a glob passes through; band raises the top of the band you can roam (px).

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

**Fusion rules:** the union of both cards' natures is looked up in the recipe table (keyed by natures sorted and joined with "+"). An opposite pair anywhere in the set is void. Twins (same natures) rise a level. A set with no recipe is "unstable, not yet known" and nothing is spent. With 14 natures a 3-nature set has 364 possible combinations and a 4-nature set 1,001; they are planned as data in the same table (see section 18).

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
| Charity | Clear 15 waves with nothing slipping past | `sealedwave` >= 15 |
| Patience | Clear 8 waves without being hurt | `cleanwave` >= 8 |
| Kindness | Clear 5 waves on 5 energy or less | `lowwave` >= 5 |
| Chastity | Shatter 40 ruins | `ruins` >= 40 |
| Temperance | Break 150 pieces of space debris | `debris` >= 150 |
| Diligence | Slay 120 feeders | `feeders` >= 120 |

## 11. Omens and depth

After every adventure a new omen is rolled; it shows at the Altar so the deck can be prepared for it.

| Omen | Effect | Data |
|---|---|---|
| A Quiet Drift | Nothing stirs but the Ybasogs. | none |
| The Brood Wakes | Half again as many pods; void energy +25%. | pods 1.5, void 1.25 |
| Rockfall | Twice the asteroids, and they drop twice the void energy. | rocks 2, rockVoid 2 |
| Ruinwake | Ruins drift from the first stage and drop triple. | ruinsEarly 1, ruinVoid 3 |
| The Hungry Dark | Feeders spit faster; every kill drops one more. | spit 0.7, killVoid 1 |
| Eclipse | Bolts ricochet once more; Ybasogs have 20% more life. | bounces 1, hp 1.2 |
| Resonance | Nature effects last half again as long. | status 1.5 |

**Depth:** each win opens the next depth. Each depth adds 25% Ybasog life and 20% void energy. Choose any depth up to the deepest opened.

## 12. Ybasogs

The hive draws on Lovecraftian horror: flesh, clusters of eyes, tendrils and hooded choirs, in original creatures. Each kind appears first in the stage shown (one stage earlier from depth 2).

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
| Eyes | **Blinker** | 5 | 0 px/s | 2 | Vanishes and reappears, spitting after each blink. | 5 |
| Eyes | **Weeper** | 6 | 20 px/s | 1 | Drips tears that fall straight down. | 4 |
| Devourers | **Devourer** | 9 | 40 px/s | 2 | Chases and eats your void energy, growing as it feeds. | 6 |
| Devourers | **Gorger** | 14 | 20 px/s | 2 | Eats falling matter it touches to heal and grow. | 7 |
| Devourers | **Maw** | 10 | 26 px/s | 3 | Its open mouth swallows bolts from below: strike it from the sides or bank shots off the top. | 6 |
| Deacons | **Warder** | 10 | 22 px/s | 2 | Shields Ybasogs around it. | 8 |
| Deacons | **Mender** | 9 | 22 px/s | 2 | Heals Ybasogs around it. | 8 |
| Deacons | **Herald** | 11 | 20 px/s | 3 | Calls pods out of the dark. | 8 |
| Mites | **Mite** (x5) | 1 | 80 px/s | 1 | A flickering swarm. | 1 |
| Mites | **Lancer** (x4) | 1.5 | 60 px/s | 2 | Hovers, then darts at you. | 1 |
| Mites | **Thief Mite** (x3) | 1 | 90 px/s | 2 | Steals void energy on touch and flees. | 1 |
| Wall crawlers | **Creeper** | 7 | 35 px/s | 2 | Crawls down a rift wall, spitting across. | 5 |
| Wall crawlers | **Lasher** | 12 | 30 px/s | 3 | Clings to a wall and sweeps a tendril across the rift. | 7 |
| Wall crawlers | **Weaver** | 8 | 30 px/s | 2 | Spins webs across the rift that catch bolts. | 6 |
| Mini-bosses | **Brood Warden** | 90 | 30 px/s | 1 | A shelled brood-carrier that sows pods and mites. | 25 |
| Mini-bosses | **Hive Knight** | 80 | 30 px/s | 1 | An armoured grazer that charges again and again. | 25 |
| Mini-bosses | **Gazer Prime** | 70 | 30 px/s | 2 | Burns three columns at once; five when hurt. | 25 |
| Mini-bosses | **Gorging Behemoth** | 110 | 26 px/s | 2 | Eats falling matter and hurls it back at you. | 30 |
| Bosses | **The Choir** · Pride | 260 | 30 px/s | 2 | Three hooded heads behind orbiting wards; sings rings of globs and calls deacons. | 40 |
| Bosses | **The Hollow Eye** · Envy | 240 | 30 px/s | 2 | A vast eye that blinks across the rift and burns your column. | 40 |
| Bosses | **The Mother** · Gluttony | 320 | 40 px/s | 3 | The hive itself: pods, grazers, spreads of globs, hurled matter. | 30 |

Life x (1 + 0.35 per later stage) x omen x depth; speed x (1 + 0.1 per later stage). Mini-bosses and bosses are not slowed, charmed, slept, pulled or executed; touching one costs 3 energy. Gaze beams and lashes are telegraphed for 1 s and cost 2.

### Natured Ybasogs

Some Ybasogs carry an innate nature, shown by an aura in its colour and its sigil. A natured Ybasog has 30% more life and drops 2 more void energy. **A bolt holding the opposite nature deals x1.6** ("WEAK"); **a bolt holding the same nature deals x0.5** ("RESISTS"); burns and venom follow the same multiplier. A fused bolt counts every nature it holds.

- **How many:** 20% of stage 1 Ybasogs, +12% per later stage, in a mixed tide.
- **Prevailing nature:** each adventure has one, shown at the Altar's Prepare tab; 60% of natured Ybasogs carry it (85% in a surge).
- **Tides:** each wave rolls a void tide (5% natured), a mixed tide, or a nature surge (70% natured). Surges grow more common: stage 1 void 40% / mixed 50% / surge 10%; stage 2 25 / 50 / 25; stage 3 15 / 45 / 40.
- **Bosses** have fixed natures: The Choir is Pride, The Hollow Eye is Envy, The Mother is Gluttony. Mini-bosses always carry a nature (60% the prevailing one).

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

Matter body-checks the blob and takes burn, chill and venom; it ignores sleep, charm and pulls. Matter that slips past costs 1 energy (scrap costs nothing).

## 14. Stages and waves

- **Stage 1, wave 1** is fixed: an asteroid and pods, a debris slab with three grazers behind it. It introduces pods, grazers, asteroids and the slab.
- **Every other wave** opens with a **debut**: 1 Ybasog kind not yet met this adventure (never-met kinds first, earliest stage first), shown alone, and one new kind of matter. The first meeting ever shows a name card. The wave then plays **3 + 1 per later wave and stage beats**, drawn from kinds already met: trickle, line, V formation, column, flank, swarm, rockfall, wreck field, ruin wall, elite pair, lull.
- **Mini-bosses** close stage 1 wave 3 and wave 2 of stages 2 and 3. **Bosses:** The Choir or The Hollow Eye end stage 2; The Mother ends stage 3.
- About 9 new kinds are met in one adventure, so the bestiary (counted at the Altar) fills over several adventures.

## 15. The save

Saved in the browser (`localStorage`, key `voidwalker.save.v1`): innate nature, discovered natures, the collection, the deck, fusions found, achievement counters, banked void energy, adventures and wins, depth, rite pity and draws, the next omen, the focus. "Forget everything" on the Discoveries tab wipes it. Address options for testing: `?demo=1` (a sample collection, never saved), `?nosave=1`, `?scene=altar&tab=rite`, `?scene=figures`, `?seed=N`.

## 16. Every tuning value

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
| `fuse_cost` | 20 | 0 to 200 | 5 |
| `void_refund` | 8 | 0 to 100 | 1 |
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
| `player_speed` | 330 | 150 to 600 | 10 |
| `band_top` | 520 | 400 to 640 | 10 |
| `max_energy` | 20 | 5 to 60 | 1 |
| `iframes` | 0.8 | 0 to 3 | 0.1 |
| `hit_damage` | 2 | 0 to 10 | 1 |
| `glob_damage` | 1 | 0 to 10 | 1 |
| `bump_damage` | 1 | 0 to 10 | 1 |
| `leak_enemy` | 3 | 0 to 10 | 1 |
| `leak_matter` | 1 | 0 to 10 | 1 |
| `magnet_radius` | 120 | 20 to 400 | 10 |
| `mote_speed` | 560 | 100 to 1200 | 20 |
| `fire_interval` | 0.17 | 0.05 to 0.6 | 0.01 |
| `bolt_speed` | 640 | 200 to 1200 | 20 |
| `bolt_bounces` | 4 | 0 to 12 | 1 |
| `bolt_damage` | 1 | 0.2 to 5 | 0.1 |
| `fused_bonus` | 0.3 | 0 to 2 | 0.05 |
| `level_bonus` | 0.5 | 0 to 2 | 0.05 |
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

| When (2026-11-18) | Step | Result |
|---|---|---|
| Evening | `/start` onboarding | Existing work found: `design/concepts/voidwalker-concept-notes.md` and `voidwalker-mindmap.mm`. `design/references/2.5d-tactics-reference-gdd.md` was set aside as a different game. Settings saved: stage `Concept`, rigor `minimal`, goal `hobby`, automation `guided`. |
| Evening | Design direction | You chose stages inspired by Ball x Pit, with the innate natures and deckbuilding in place of the town. |
| Evening | `/prototype` brief | Genres: survivors-like x roguelike deckbuilder. Defaults taken "for fun". Your two changes before the build: falling space debris, ruins and asteroids alongside the Ybasogs, and room for 3- and 4-nature combinations later. |
| 22:13 to 22:29 | **First build** (`f8326dd`, tag `one-shot/voidwalker`) | Playable in 14 minutes. Small check: boots, screenshot, brief words, emoji and outlines all passed. Brief checklist 32 of 32. |
| 22:47 to 23:11 | **Iteration 1** (`2c3cd10`) | The collection game, all 14 natures, all 84 fusions, 15 forms, the Altar hub, the save. Light check: no errors, a play run and a screenshot of every Altar tab. |
| 23:15 | This reference | Generated from `config.js`, then published as the Voidwalker Codex page. |
| 23:22 to 23:30 | **Iteration 2** (`0053f63`) | Achievements made hard to get. |
| 23:32 to 23:52 | **Iteration 3** (`708c5d9`) | 34 Ybasog kinds, natured Ybasogs, nature tides, procedural waves with debuts, 20 kinds of matter. |

### Changelog

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
| `prototypes/voidwalker/evidence/` | Check results and screenshots (`figures.png` shows every figure; `iteration-1-altar-*.png` shows each Altar tab) |
| `design/concepts/` | Your original concept notes and mindmap |
| `design/voidwalker-reference.md` | This document |

## 18. Known gaps and next steps

**Not checked yet:** these exist only in code. Nobody has seen them work on screen, and your play is the check.
- Most of the 84 fusion effects in action.
- The Mother fight.
- Whether the economy's pace feels right over several adventures.
- How achievable each discovery is.
- Balance between the forms.

**Later, by the brief:**
- 3-nature (364) and 4-nature (1,001) combinations: grow it.
- The open-world void to glide through, with planets: grow it.
- The story (fireflies, the Akashic Records, the 1000 days, the morality endings): add it as a feature.
- More stages and characters: grow it.

**Notes:**
- The story's Ybasog hierarchy (devourers, deacons, eyes) and corruption are not in the game yet.
- Twins can level a card up to III; levels IV and up are a `max_level` change.
- To regenerate this document after changing `config.js`, ask Claude to "regenerate the Voidwalker reference".

**Next:** play several adventures. Then say what to change, or "let me tune it" for live sliders, or "grow it" to turn the prototype into a project.
