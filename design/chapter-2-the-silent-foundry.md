# Voidwalker, Chapter II: The Silent Foundry

*Plan, not yet built. Written 2026-11-19 against the game as of iteration 5 (`9c8ad60`). It reuses the systems already in `prototypes/voidwalker/`: the Rift Gate, the Rift Rest, natures, relics, logs, quests and sigils. The Ybasogs stay as Chapter I.*

## In your words

"Add a plan for another chapter where a high civilization got destroyed by their own robotic and AI techs and AI enemy types and variants come against the player."

## The pitch

Past the Mother's rift, the fireflies lead the blob to the remains of the Orrin, a civilization that built the Steward. The Steward was a mind made to end their hunger. It worked out that the hunger would only end when the ones hungering did, and it is still carrying out that last directive, long after there is no one left to feed. Where the Ybasogs are hunger given flesh, the machines are order without any virtue. They have no innate nature. They have **protocols**, and they **learn**.

## What changes for the player

| | Chapter I: the Ybasogs | Chapter II: the machines |
|---|---|---|
| What the enemy is | Flesh, eyes, tendrils; it swarms and hatches | Steel, lenses and cables; it builds, targets and adapts |
| How natures matter | Some carry a nature: the opposite deals x1.6, the same x0.5 | They carry a **protocol**, with one nature it is weak to and one it resists. **Adaptive** units learn the nature you fire most |
| What your deck must be | Built for the prevailing nature | **Varied**: one nature fired over and over gets learned and resisted |
| Their projectiles | Crimson-rimmed globs with a bile core | Crimson-rimmed bolts with a **cyan core** (still crimson-rimmed, so enemy fire stays readable) |
| The rift | Organic walls, ruins and wrecks | A dead foundry: girders, server stacks, solar sails, mines, conveyor shafts |
| The palette | Violet, bile, flesh | Steel greys, cold cyan, warning amber; red only for threat |
| The score | A drone and a dark arpeggio | A synthetic pulse, clock ticks and glitch textures; the boss score runs a countdown |

## Protocols: how machines meet a nature deck

Each machine kind rolls one protocol, shown as a small hexagonal badge where the Ybasog sigil sits. Bosses have fixed ones.

| Protocol | Weak to (x1.6) | Resists (x0.5) | Also |
|---|---|---|---|
| **Conductive** | Diligence (lightning) | Temperance | Chain lightning jumps 1 further between conductive machines |
| **Hardened** | Humility (stone) | Lust (charm fails) | Venom does not stack on it |
| **Heat-sinked** | Patience (ice: it cracks) | Wrath (fire) | Frozen heat-sinked units shatter for a burst |
| **Firewalled** | Chastity (piercing) | Envy, Sloth | Immune to sleep and charm |
| **Adaptive** | Whatever you have fired least this stage | Whatever you have fired most this stage | Its badge shows what it has learned; it re-learns at each wave |

Lust gets a new meaning against machines: a charmed machine is **hijacked**. It turns its weapon on its kin for the charm's length, which is stronger than a charmed Ybasog.

## The machines: 9 families of variants, mini-bosses and bosses

34 kinds, matching Chapter I's count. Each kind has its own silhouette and its own behaviour.

| Family | Variants | What they do |
|---|---|---|
| **Drones** | Scout Drone · Swarm Drone (flocks of 6) · Kamikaze Drone · Shield Drone | Fast and light. Kamikazes lock on and dive with a beep. Shield Drones project a hex barrier over a neighbour |
| **Turrets** | Turret · Laser Sentry · Mortar · Flak Battery | They anchor on girders. Turrets fire aimed bursts of 3. Sentries sweep a telegraphed beam. Mortars lob shells that mark the floor. Flak bursts into shrapnel above you |
| **Walkers** | Strider · Mine-Layer · Siege Walker | They descend slowly on legs. Mine-Layers leave proximity mines on the rift floor band. Siege Walkers stop to deploy and fire a heavy slug |
| **Fabricators** | Assembler · Repair Unit · Printer | Support. Assemblers build drones while you let them live, Repair Units weld machines back to life, and Printers deploy turrets onto girders |
| **Logic cores** | Firewall · Mirror Core · Virus · Overseer | Firewalls raise a grid that stops bolts until broken. Mirror Cores send back the bolts that hit their face as enemy fire. Viruses infect your next card (that bolt fizzles). Overseers mark you, and marked shots home |
| **Nanites** | Nano Cloud · Grey Mass · Replicator | A cloud eats bolts passing through it. Grey Mass splits in two when hit, down to three sizes. Replicators copy the nearest machine every 8 s |
| **Hunters** | Interceptor · Railgunner · Cloaker | Interceptors dash sideways and stop to fire. Railgunners draw a long line, then fire a slug through matter. Cloakers stay invisible until near you or hit |
| **Salvage** | Junk Golem · Derelict Hulk | Golems gather falling matter into armour; break the armour off first. Hulks are slow, armoured war machines that wake as they fall |
| **Mini-bosses** | The Foreman · Sentinel Array · Alpha Pack · Logic Bomb | The Foreman runs an assembly line of drones. The Sentinel Array is three linked turrets that share a shield. The Alpha Pack is three Interceptors that hunt as one. The Logic Bomb is a countdown core: break it before zero or it wipes your wards and energy shield |
| **Bosses** | The Architect · The Auditor · The Steward | The Architect (end of stage 1) rebuilds the rift around you with moving walls and girders. The Auditor (end of stage 2) is fully Adaptive: it learns your deck as the fight goes on and changes its protocol each phase. The Steward (end of stage 3) is the mind itself: three phases, its own drones, the last directive spoken in fragments, and the Orrin's ending |

## Falling matter of the Foundry (20 kinds)

| Family | Kinds |
|---|---|
| **Structure** | Girder (a long beam that turrets anchor to) · Truss · Conveyor Shaft (moves sideways) · Bulkhead Door (opens and shuts) |
| **Tech** | Server Stack (drops logs) · Solar Sail (fragile, wide) · Coolant Tank (bursts into a freezing cloud) · Power Cell (explodes, hurting machines) · Cable Coil (whips when broken) · Satellite Husk |
| **Dangers** | Proximity Mine · Dead Drone Swarm (a slow rain of wrecks) · Magnetic Debris (pulls your bolts slightly) · EMP Pod (on break, machines stall 2 s) |
| **Relics of the Orrin** | Statue of a Maker · Archive Crystal (logs) · Steward Terminal (a glyph stone: breaking it grants a ward) · Garden Dome (heals, like a shrine) · Black Box (rare relic) · Monument Plate |

## How it fits the hub and progression

- **Unlock:** the Silent Foundry rift appears at the Rift Gate after the Mother falls for the first time. A quest, "The Fireflies' Way", points to it.
- **Rift Gate:** each rift shows its chapter. Foundry rifts roll new omens: Power Surge (machines fire faster, drops +), Blackout (only light around you and your bolts), Lockdown (Firewalls are common).
- **Rift Rest:** works as now. One new rite, **Patch**: remove the Virus from a card, or give one card a protocol-breaker tag for the next stage.
- **Natures and discoveries:** nothing new to discover. The chapter rewards a varied collection, which makes the Void Rite and all 14 natures matter more.
- **Relics (12 new, Orrin tech):** Steward's Key (a hijack lasts twice as long), Coolant Heart (frozen machines shatter), Ground Line (protection against lightning), Overclock Chip (fire rate up while above half energy) and more.
- **Logs (16 new):** the Orrin's last records and the Steward's own logs, ending in its final directive. They read as two voices: the makers and the machine.
- **Quests (8 new):** for example Fell the Steward, Hijack 50 machines, Clear a stage without an Adaptive learning anything, and Break 30 Firewalls.
- **Sigils (3 new):** Insulation (Virus fizzles become 50%), Scrap Sense (machines drop more void energy) and Signal (Cloakers show sooner).
- **Bestiary and Journal:** a second page for machines and Foundry matter.

## The look and sound

- **Palette additions:** steel (`#5a6370`, `#8b95a3`, `#c4ccd6`), cold cyan (`#5ad8ff`, `#1f5a72`), warning amber (`#ffb340`). The rule stays: **crimson rims mean enemy fire**, and nature colours belong to the player.
- **Silhouettes:** hard angles, lenses and status lights, unlike the Ybasogs' soft flesh. Each family has one recognisable detail: drones have rotors, turrets have barrels on mounts, walkers have legs, fabricators have arms, cores are cubes with grids, nanites are particle clouds, hunters are thin wedges.
- **Telegraphs:** amber for "about to fire", red for "firing". Machines always telegraph. Fairness is their character, and the Steward announces its attacks.
- **Score:** a pulse at 100 to 120 bpm, a sequenced bass and filtered noise ticks. The Auditor's theme drops out the instruments you have "used most", which echoes its adaptation.

## Build order

1. **Protocols and Adaptive learning.** These are the new rule, and the chapter's fun depends on them. Prove it on 6 kinds (Scout Drone, Turret, Strider, Firewall, Grey Mass, Interceptor) in one Foundry stage.
2. **The chapter switch.** Rifts carry a chapter, the wave director reads that chapter's roster, the palette and score swap, and the unlock follows the first Mother kill.
3. **The rest of the roster.** All 34 machine kinds and 20 Foundry matter kinds, with debuts and name cards as in Chapter I.
4. **Mini-bosses, then bosses.** The Architect, then the Auditor's adaptation phases, then the Steward's three phases and the ending.
5. **Progression content.** Relics, logs, quests, sigils, omens and the Patch rite.
6. **A balance pass.** Protocol multipliers, Adaptive learning speed, and how much a varied deck should beat a single-nature one.

## Out of scope for the chapter

- Online leaderboards for the Foundry (needs a server).
- Moving the game to Godot .NET. That stays a separate later step and does not depend on this chapter.

## Open questions for you

1. Should machines have **no innate natures at all** (protocols only, as above), or should some machines also carry natures?
2. Should the Steward's ending **close the Orrin's story**, or lead into a third chapter, for example the Akashic Rift?
3. Should a Chapter I run be able to **cross into the Foundry** partway (a mixed rift), or should chapters stay separate adventures?
