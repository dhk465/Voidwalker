// Generates design/voidwalker-reference.md from prototypes/voidwalker/config.js (the game's own data).
const fs = require('fs'), path = require('path');
const root = process.argv[2];
global.window = {};
require(path.join(root, 'prototypes/voidwalker/config.js'));
const C = window.CONFIG, N = window.NATURES, v = k => C[k].value;
const B = N.base, R = N.recipes, D = N.discover, O = N.omens;
const ids = Object.keys(B);
const pct = x => Math.round(x * 100) + '%';
const r2 = x => Math.round(x * 100) / 100;

const FX = {
  radiant: `x${v('radiant_mult')} damage to a target at full life`,
  pull: `pulls Ybasogs within ${v('pull_radius')} px toward the hit for ${v('pull_time')} s`,
  burn: `burns ${v('burn_dps')}/s for ${v('burn_time')} s`,
  venom: `venom: ${v('venom_dps')}/s per stack, up to ${v('venom_stacks')} stacks, ${v('venom_time')} s`,
  charm: `charm ${v('charm_time')} s: the Ybasog turns on its kin (${v('charm_dps')}/s on contact) and is not shot`,
  devour: `+${v('devour_grow')} damage and grows after every hit; its kills drop +1 void energy`,
  sleep: `sleep ${v('sleep_time')} s: no moving, no attacking, pods do not hatch`,
  stone: `heavy: x${v('stone_mult')} damage, x${v('stone_matter')} more against falling matter, 20% slower`,
  split: `splits into ${v('split_count')} shards (${v('shard_damage')} damage each)`,
  heal: `a kill heals ${v('heal_on_kill')} energy`,
  chill: `slows ${pct(v('chill_slow'))} for ${v('chill_time')} s`,
  thorns: `bramble patch (${v('thorns_radius')} px): ${v('thorns_dps')}/s for ${v('thorns_time')} s`,
  pierce: `passes through ${v('pierce_count')} targets`,
  gust: `knocks the target back ${v('gust_push')} px; a diving grazer stops diving`,
  chain: `lightning leaps to ${v('chain_count')} more within ${v('chain_range')} px at ${pct(v('chain_share'))} damage`,
  well: `burning gravity well (${v('well_radius')} px, ${v('well_time')} s, ${v('well_dps')}/s)`,
  field: `stasis field (${v('field_radius')} px, ${v('field_time')} s) slowing ${pct(v('field_slow'))}`,
  shatter: `bursts into 4 shards in a wide fan`,
  split3: `splits into 3 shards`,
  nova: `a ring of 6 shards`,
  mirror: `a twin bolt launches from the mirror point across the rift`,
  thrall: `charmed Ybasogs that die rise as your wisps for ${v('thrall_time')} s (ram for 3)`,
  execute: `kills any Ybasog (not the Mother) under ${pct(v('execute_line'))} life`,
  orbit: `circles the blob for 1.6 s as a shield, then launches along the aim`,
  hoard: `kills drop x3 void energy`,
  tether: `binds the target to its nearest kin for 4 s: 50% of damage is shared`,
  lure: `a wide pull (1.8x radius, 1.5x strength)`,
  singularity: `a slow orb (half speed) that collapses into a black hole (130 px, 1.6 s) after 1 s or on impact; its core swallows pods and grazers`,
  doom: `marks the target: after ${v('doom_time')} s it bursts for x${v('doom_mult')} the hit's damage`,
  frenzy: `a kill quickens your fire (interval x${v('frenzy_rate')}) for ${v('frenzy_time')} s`,
  killburst: `kills erupt in 8 shards`,
  mine: `clings to the target and erupts after 0.9 s: ${v('mine_damage')} damage in ${v('mine_radius')} px`,
  homing: `steers toward the nearest Ybasog`,
  leech: `heals ${pct(v('leech_share'))} of the damage it deals`,
  spore: `kills leave a venom cloud (70 px, 3 s)`,
  echo: `a second copy fires 0.12 s later`,
  lullaby: `sleeps every Ybasog within 150 px for ${r2(v('sleep_time') * 1.3)} s`,
  aegis: `a kill grants a ward that blocks one hit (up to ${v('ward_max')})`,
  quake: `a shockwave along the whole row (+/-38 px) for x${v('quake_damage')} the hit's damage`,
  wall: `raises a stone wall (140 px) for ${v('wall_time')} s that stops globs and Ybasogs`,
  storm: `a storm cloud (${v('storm_radius')} px, ${v('storm_time')} s) striking a Ybasog every 0.35 s for ${v('storm_zap')}`,
  bloom: `kills drop 2 petals that heal 1 each`,
  timewarp: `slows every Ybasog in the rift ${pct(v('timewarp_slow'))} for ${v('timewarp_time')} s`,
  decoy: `plants a flower decoy for ${v('decoy_time')} s: grazers dive at it, feeders spit at it, Ybasogs that touch it die`,
  beam: `an instant beam to the rift's edge, x1.5 damage to everything on the line (no bolt)`,
  snowball: `+4 ricochets; every wall ricochet adds +0.35 damage and size`,
  meteor: `a comet lands 0.55 s later: ${v('meteor_damage')} damage in ${v('meteor_radius')} px, and burns`,
  boomerang: `turns back after 0.55 s and returns to you, striking again`,
  prism: `splits into 3 bolts of random natures you have discovered (70% damage)`,
  magnet: `calls all void energy on screen to you for 2 s`,
  tithe: `+10% of the target's current life as extra damage`,
  vortex: `a spinning vortex (${v('well_radius')} px, ${v('well_time')} s) that pulls and grinds (${r2(v('well_dps') * 0.7)}/s)`,
  gild: `kills burst into 5 golden shards and drop +2 void energy`,
  cleanse: `destroys enemy globs within 140 px`,
  big: `huge and 40% slower`,
  long: `the charm lasts twice as long`,
  wide: `brambles 1.7x wider`,
};
const dmg = (fused, lvl, stone) => r2(v('bolt_damage') * (1 + v('level_bonus') * (lvl - 1)) * (fused ? 1 + v('fused_bonus') : 1) * (stone ? v('stone_mult') : 1));
const dmgLine = (k) => { const r = R[k], f = k.split('+').length === 2, s = r.fx.includes('stone'); return [1, 2, 3].map(l => dmg(f, l, s)).join(' / '); };
const fxText = r => r.fx.map(f => FX[f] || f).join('; ') + (r.shardFx && r.shardFx.length ? `. Shards carry: ${r.shardFx.join(', ')}` : '');

let o = [];
const P = s => o.push(s);
P('# Voidwalker: Game Reference');
P('');
P('*Generated 2026-11-18 from `prototypes/voidwalker/config.js` (game data and tuning) and the project history. Numbers are the current `config.js` values; change them there and regenerate.*');
P('');
P('**Build:** `prototypes/voidwalker/index.html` (open in a browser) · **Brief:** `design/game-brief.md` · **Checkpoints:** tag `one-shot/voidwalker`, commit `708c5d9` (iteration 3)');
P('');
P('## Contents');
['1. The game in one page', '2. Controls', '3. The adventure', '4. The Rift Altar', '5. The economy', '6. The fourteen natures', '7. The forms and their traits', '8. The 84 fusions', '9. Effects glossary', '10. Discoveries (achievements)', '11. Omens and depth', '12. Ybasogs and natured Ybasogs', '13. Falling matter', '14. Stages and waves', '15. The save', '16. Every tuning value', '17. Development history and changelog', '18. Known gaps and next steps'].forEach(x => P(`- ${x}`));
P('');
P('## 1. The game in one page');
P('');
P('A blob born of the void holds the bottom of a vertical rift while Ybasogs (a hive of organic creatures) and falling matter (asteroids, space debris, ruins) pour down. Its **deck of nature cards is its ammunition**: each bolt fired is the next card in the deck, and bolts ricochet off the rift walls, enemies and matter. The fourteen **innate natures** are the seven sins and seven virtues, each an element. Two natures fuse into one of **84 fusions**; two **opposites annul** each other into void.');
P('');
P('The loop: **prepare** a deck at the **Rift Altar** → **adventure** through 3 stages of waves ending on the **Mother** → **bank** the void energy gathered → spend it on the **Void Rite** (gacha) and the **Forge** (fusion) → prepare again. **Natures are discovered by achievements** during adventures and join the collection at the Altar, never mid-adventure. The deck\'s most common nature shapes the blob into one of 15 **forms**, each with a trait. **Omens** and **depth** change each adventure, so the deck is prepared for what waits.');
P('');
P('Genres: survivors-like (the horde, movement) x roguelike deckbuilder (the collection, deck, fusion). Stage shape inspired by Ball x Pit; the town is replaced by the Altar.');
P('');
P('## 2. Controls');
P('');
P('| Where | Input | Does |');
P('|---|---|---|');
P('| Title | Enter, Space or click | Begin (first time) or go to the Altar |');
P('| Adventure | WASD or arrow keys | Move inside the bottom band |');
P('| Adventure | Mouse | Aim (always kept upward) |');
P('| Adventure | Hold left mouse button | Fire |');
P('| Adventure | Q | Auto-fire on or off |');
P('| Adventure | Space, P or Esc | Pause |');
P('| Anywhere | M | Mute |');
P('| Altar | Mouse | Everything; 1-5 switch tabs; Enter begins the adventure; Esc to title |');
P('| End screen | Enter, R or click | Return to the Altar |');
P('');
P('## 3. The adventure');
P('');
P(`- **Energy is life.** Start at ${v('max_energy')} (plus the form's bonus). A bite costs ${v('hit_damage')}, a glob ${v('glob_damage')}, a body-check from matter ${v('bump_damage')}. A Ybasog slipping past the bottom costs ${v('leak_enemy')}, matter ${v('leak_matter')}. After a hit you are safe for ${v('iframes')} s. Clearing a stage restores ${v('stage_heal')}.`);
P(`- **Bolts:** one every ${v('fire_interval')} s at ${v('bolt_speed')} px/s, ${v('bolt_bounces')} ricochets before fading. Base damage ${v('bolt_damage')}; +${pct(v('level_bonus'))} per card level above I; fused cards +${pct(v('fused_bonus'))}.`);
P(`- **The deck is the magazine.** Shuffled at the start of each wave. Each shot draws the next card; an empty draw pile reshuffles the discard (${v('reshuffle_time')} s with no fire). The left panel shows the next bolts in order.`);
P(`- **Void energy** motes drop from kills and broken matter and fly to you within ${v('magnet_radius')} px (all of them at a wave's end).`);
P('- **Structure:** 3 stages x 3 waves. Stage 3 wave 3 is the Mother. Win: the Mother falls. Loss: energy reaches 0.');
P('- **The first adventure** begins alone in the dark: a firefly whispers "take it" beside a crack of light; touching it gives a random innate nature of the 14, 4 Void Bolts and 2 cards of that nature.');
P('');
P('## 4. The Rift Altar');
P('');
P('The hub between adventures, in five tabs. The collection (every card owned, stacked by kind and level) is always on the left.');
P('');
P('| Tab | What you do |');
P('|---|---|');
P(`| 1 Prepare | Click collection cards to add them to the deck (${v('deck_min')} to ${v('deck_max')} cards); click a deck row to remove one. See the form the deck gives and its trait, the omen of the next adventure, and choose the depth. Begin. |`);
P(`| 2 Void Rite | The gacha. Draw from the natures you have discovered: one rite ${v('rite_cost')}, ten rites ${v('grand_rite_cost')}. Optional focus on one nature (+${v('focus_cost')} per draw): ${pct(v('focus_share'))} of draws land on it. Odds: rare (level II) ${pct(v('rare_chance'))}, mythic (an already-fused card, written into the Codex) ${pct(v('mythic_chance'))}, and a mythic is certain within ${v('mythic_pity')} draws (pity). New kinds are marked NEW. |`);
P(`| 3 Forge | Fuse two cards for ${v('fuse_cost')}. Two natures make their fusion (unknown ones show "?" until forged); a first-time fusion pays ${v('first_fusion_bonus')} and fills its Codex cell. Twins rise a level (up to ${['', 'I', 'II', 'III', 'IV'][v('max_level')] || v('max_level')}). Opposites annul: both cards are gone, ${v('void_refund')} back. A Void Bolt melts into any nature card (a way to trim plain bolts). |`);
P('| 4 Codex | The 84 fusions as a grid; point at a cell to read it. A brighter "?" can already be made from natures you know. |');
P('| 5 Discoveries | The 14 natures, each one\'s achievement and progress, and the form each gives. "Forget everything" (click twice) wipes the save. |');
P('');
P('## 5. The economy');
P('');
P('| Source or sink | Void energy |');
P('|---|---|');
P('| Pod / grazer / feeder kill | 2 / 3 / 5 motes |');
P('| Mother | 30 motes |');
P('| Asteroid / big asteroid / chunk | 3 / 5 / 1 |');
P('| Debris / ruin | 2 / 8 |');
P(`| Banked after a win | everything gathered x omen x depth, + ${v('win_bonus')} |`);
P(`| Banked after a fall | ${pct(v('loss_keep'))} of it |`);
P(`| Discovering a nature | +${v('discovery_bonus')}, and ${v('discovery_copies')} cards |`);
P(`| First time forging a fusion | +${v('first_fusion_bonus')} |`);
P(`| Annulling opposites | +${v('void_refund')} (after paying ${v('fuse_cost')}) |`);
P(`| One rite / ten rites | -${v('rite_cost')} / -${v('grand_rite_cost')} (+${v('focus_cost')} each when focused) |`);
P(`| Fusion | -${v('fuse_cost')} |`);
P('');
P('## 6. The fourteen natures');
P('');
P('Opposite pairs annul when fused: ' + [...new Set(ids.map(i => [i, B[i].opposite].sort().join('|')))].map(p => p.split('|').map(x => B[x].name).join(' and ')).join(', ') + '.');
P('');
P('| Nature | Kind | Element | Opposite | Bolt effect | Damage I / II / III |');
P('|---|---|---|---|---|---|');
for (const i of ids) P(`| **${B[i].name}** | ${B[i].kind} | ${B[i].element} | ${B[B[i].opposite].name} | ${R[i].says} ${fxText(R[i])} | ${dmgLine(i)} |`);
P(`| Void Bolt | plain | void | none | ${R[''].says} | ${[1, 2, 3].map(l => dmg(false, l, false)).join(' / ')} |`);
P('');
P('## 7. The forms and their traits');
P('');
P('The nature most present in the deck (card levels count; ties go to your innate nature) shapes the blob for the whole adventure. A deck of only Void Bolts stays shapeless.');
P('');
P('| Form | Nature | Trait | Exact values |');
P('|---|---|---|---|');
P('| Shapeless blob | none | none | none |');
for (const i of ids) P(`| ${B[i].formName} | ${B[i].name} | ${B[i].traitText} | ${Object.entries(B[i].trait).map(([k, x]) => `${k} ${x}`).join(', ')} |`);
P('');
P('Trait keys: damage and fire_rate multiply; max_energy adds; magnet adds px; mote_bonus is the chance of an extra void energy per mote; bite adds to bite damage; bounces adds ricochets; status multiplies effect durations; contact_charm is the chance a biting Ybasog is charmed instead; speed multiplies movement; aura slows Ybasogs within 160 px; armor reduces each hit (never below 1); wave_heal heals after every wave; iframes adds safety time; quick_shuffle removes the reshuffle pause; regen is energy per second; leak_guard reduces leak damage; glob_dodge is the chance a glob passes through; band raises the top of the band you can roam (px).');
P('');
P('## 8. The 84 fusions');
P('');
P(`Every fused card: x${1 + v('fused_bonus')} damage, drawn with rings around its bolt. Damage shown for levels I / II / III. Groups: sin with sin (21), virtue with virtue (21), sin with virtue (42; the 7 opposite pairs never fuse).`);
const fusions = Object.keys(R).filter(k => k.split('+').length === 2);
const group = k => { const [a, b] = k.split('+').map(x => B[x].kind); return a === b ? (a === 'sin' ? 'Sin with sin' : 'Virtue with virtue') : 'Sin with virtue'; };
for (const g of ['Sin with sin', 'Virtue with virtue', 'Sin with virtue']) {
  P(''); P(`### ${g}`); P('');
  P('| Fusion | Natures | What it does | Exact effects | Damage I / II / III |'); P('|---|---|---|---|---|');
  fusions.filter(k => group(k) === g).sort((a, b) => R[a].name.localeCompare(R[b].name)).forEach(k => {
    P(`| **${R[k].name}** | ${k.split('+').map(x => B[x].name).join(' + ')} | ${R[k].says} | ${fxText(R[k])} | ${dmgLine(k)} |`);
  });
}
P('');
P('### Fusion grid');
P('');
P('Row nature + column nature. "void" marks opposites.');
P('');
const short = ids.map(i => B[i].name.slice(0, 4));
P('| | ' + short.join(' | ') + ' |'); P('|---' + '|---'.repeat(ids.length) + '|');
for (const a of ids) P(`| **${B[a].name}** | ` + ids.map(b => a === b ? 'level up' : B[a].opposite === b ? 'void' : R[[a, b].sort().join('+')].name).join(' | ') + ' |');
P('');
P('**Fusion rules:** the union of both cards\' natures is looked up in the recipe table (keyed by natures sorted and joined with "+"). An opposite pair anywhere in the set is void. Twins (same natures) rise a level. A set with no recipe is "unstable, not yet known" and nothing is spent. With 14 natures a 3-nature set has 364 possible combinations and a 4-nature set 1,001; they are planned as data in the same table (see section 18).');
P('');
P('## 9. Effects glossary');
P('');
P('| Effect id | Meaning with current values |'); P('|---|---|');
for (const [k, t] of Object.entries(FX)) P(`| \`${k}\` | ${t} |`);
P('');
P('Status marks drawn on targets: burn (red arc), chill (blue arc), venom (green drops, one per stack), sleep (z), charm (pink heart), doom (pink ring filling), tether (dashed gold line).');
P('');
P('## 10. Discoveries (achievements)');
P('');
P('Your innate nature is known from the start; each other nature is discovered when its counter reaches the goal. Counters add up across all adventures and are saved.');
P('');
P('| Nature | Achievement | Counter |'); P('|---|---|---|');
for (const i of ids) P(`| ${B[i].name} | ${D[i].text} | \`${D[i].stat}\` >= ${D[i].n} |`);
P('');
P('## 11. Omens and depth');
P('');
P('After every adventure a new omen is rolled; it shows at the Altar so the deck can be prepared for it.');
P('');
P('| Omen | Effect | Data |'); P('|---|---|---|');
for (const [k, x] of Object.entries(O)) P(`| ${x.name} | ${x.says} | ${Object.entries(x).filter(([a]) => a !== 'name' && a !== 'says').map(([a, b]) => `${a} ${b}`).join(', ') || 'none'} |`);
P('');
P(`**Depth:** each win opens the next depth. Each depth adds ${pct(v('depth_hp'))} Ybasog life and ${pct(v('depth_void'))} void energy. Choose any depth up to the deepest opened.`);
P('');
P('## 12. Ybasogs');
P('');
P('The hive draws on Lovecraftian horror: flesh, clusters of eyes, tendrils and hooded choirs, in original creatures. Each kind appears first in the stage shown (one stage earlier from depth 2).');
P('');
const Y = window.YBASOGS, M = window.MATTER;
const famName = { pod: 'Pods', grazer: 'Grazers', feeder: 'Feeders', eye: 'Eyes', devourer: 'Devourers', deacon: 'Deacons', mite: 'Mites', crawler: 'Wall crawlers', mini: 'Mini-bosses', boss: 'Bosses' };
P('| Family | Ybasog | Life | Speed | From stage | Behaviour | Void |'); P('|---|---|---|---|---|---|---|');
for (const [k, d] of Object.entries(Y)) P(`| ${famName[d.fam]} | **${d.name}**${d.group ? ` (x${d.group})` : ''}${d.nature ? ` · ${B[d.nature].name}` : ''} | ${k === 'mother' ? v('mother_hp') : d.hp} | ${d.sp} px/s | ${d.from} | ${d.says} | ${d.drop} |`);
P('');
P(`Life x (1 + ${v('stage_hp_scale')} per later stage) x omen x depth; speed x (1 + ${v('stage_speed_scale')} per later stage). Mini-bosses and bosses are not slowed, charmed, slept, pulled or executed; touching one costs ${v('hit_damage') + 1} energy. Gaze beams and lashes are telegraphed for ${v('telegraph_time')} s and cost ${v('beam_damage')}.`);
P('');
P('### Natured Ybasogs');
P('');
P(`Some Ybasogs carry an innate nature, shown by an aura in its colour and its sigil. A natured Ybasog has 30% more life and drops 2 more void energy. **A bolt holding the opposite nature deals x${v('opposite_mult')}** ("WEAK"); **a bolt holding the same nature deals x${v('same_mult')}** ("RESISTS"); burns and venom follow the same multiplier. A fused bolt counts every nature it holds.`);
P('');
P(`- **How many:** ${pct(v('nature_chance'))} of stage 1 Ybasogs, +${pct(v('nature_chance_step'))} per later stage, in a mixed tide.`);
P(`- **Prevailing nature:** each adventure has one, shown at the Altar's Prepare tab; ${pct(v('prevailing_share'))} of natured Ybasogs carry it (85% in a surge).`);
P('- **Tides:** each wave rolls a void tide (5% natured), a mixed tide, or a nature surge (70% natured). Surges grow more common: stage 1 void 40% / mixed 50% / surge 10%; stage 2 25 / 50 / 25; stage 3 15 / 45 / 40.');
P('- **Bosses** have fixed natures: The Choir is Pride, The Hollow Eye is Envy, The Mother is Gluttony. Mini-bosses always carry a nature (60% the prevailing one).');
P('');
P('## 13. Falling matter');
P('');
P('| Family | Matter | Life | Speed | From stage | Behaviour | Void |'); P('|---|---|---|---|---|---|---|');
const mf = { rock: 'Rocks', wreck: 'Wrecks', ruin: 'Ruins' };
for (const [k, d] of Object.entries(M)) P(`| ${mf[d.fam]} | **${d.name}**${d.only ? ' (from breaking)' : ''} | ${d.hp} | ${d.sp} px/s | ${d.only ? '' : d.from} | ${d.says} | ${d.drop} |`);
P('');
P('Matter body-checks the blob and takes burn, chill and venom; it ignores sleep, charm and pulls. Matter that slips past costs 1 energy (scrap costs nothing).');
P('');
P('## 14. Stages and waves');
P('');
P('- **Stage 1, wave 1** is fixed: an asteroid and pods, a debris slab with three grazers behind it. It introduces pods, grazers, asteroids and the slab.');
P(`- **Every other wave** opens with a **debut**: ${v('debuts_per_wave')} Ybasog kind not yet met this adventure (never-met kinds first, earliest stage first), shown alone, and one new kind of matter. The first meeting ever shows a name card. The wave then plays **${v('beats_base')} + ${v('beats_step')} per later wave and stage beats**, drawn from kinds already met: trickle, line, V formation, column, flank, swarm, rockfall, wreck field, ruin wall, elite pair, lull.`);
P('- **Mini-bosses** close stage 1 wave 3 and wave 2 of stages 2 and 3. **Bosses:** The Choir or The Hollow Eye end stage 2; The Mother ends stage 3.');
P('- About 9 new kinds are met in one adventure, so the bestiary (counted at the Altar) fills over several adventures.');
P('');
P('## 15. The save');
P('');
P('Saved in the browser (`localStorage`, key `voidwalker.save.v1`): innate nature, discovered natures, the collection, the deck, fusions found, achievement counters, banked void energy, adventures and wins, depth, rite pity and draws, the next omen, the focus. "Forget everything" on the Discoveries tab wipes it. Address options for testing: `?demo=1` (a sample collection, never saved), `?nosave=1`, `?scene=altar&tab=rite`, `?scene=figures`, `?seed=N`.');
P('');
P('## 16. Every tuning value');
P('');
P('| Key | Value | Range | Step |'); P('|---|---|---|---|');
for (const [k, x] of Object.entries(C)) { if (typeof x !== 'object') { P(`| \`${k}\` | ${x} | | |`); continue; } P(`| \`${k}\` | ${x.value} | ${x.min} to ${x.max} | ${x.step} |`); }
P('');
fs.writeFileSync(path.join(__dirname, 'tables.md'), o.join('\n'));
console.log('lines', o.length, 'fusions', fusions.length);
