// PROTOTYPE - NOT FOR PRODUCTION
// Question: Is building and fusing a nature deck fun when every card is a bolt you watch ricochet?
// Date: 2026-11-18
// Every tuning value lives here, each with value, min, max and step. The game reads them live
// through tune.<name> (index.html); the tweak panel, an add-on, writes them back here.
window.CONFIG = {
  seed: 1, // every gameplay random call reads rng(), seeded from this (or ?seed= in the address)
  volume: { value: 0.5, min: 0, max: 1, step: 0.05 },

  // ---- The economy: void energy is banked between adventures ----
  rite_cost: { value: 30, min: 0, max: 200, step: 5 },           // void energy for one Void Rite draw
  grand_rite_cost: { value: 270, min: 0, max: 2000, step: 10 },  // ten draws at once
  focus_cost: { value: 10, min: 0, max: 100, step: 5 },          // extra per draw to focus a nature
  focus_share: { value: 0.5, min: 0, max: 1, step: 0.05 },       // share of common and rare draws that land on the focus
  rare_chance: { value: 0.2, min: 0, max: 1, step: 0.01 },       // a level II card
  mythic_chance: { value: 0.07, min: 0, max: 1, step: 0.01 },    // an already-fused card
  mythic_pity: { value: 10, min: 1, max: 50, step: 1 },          // a mythic is certain by this many draws without one
  void_refund: { value: 8, min: 0, max: 100, step: 1 },          // void energy back when opposites annul at a Rift Rest
  rest_actions: { value: 2, min: 0, max: 6, step: 1 },           // rites (merge or temper) at each Rift Rest between stages
  relic_chance: { value: 1, min: 0, max: 5, step: 0.1 },         // multiplies every relic and log drop chance
  hitstop: { value: 0.06, min: 0, max: 0.3, step: 0.01 },        // s the action freezes on a heavy blow
  music_volume: { value: 0.35, min: 0, max: 1, step: 0.05 },     // the score, under the sound effects
  first_fusion_bonus: { value: 15, min: 0, max: 200, step: 5 },  // void energy for making a fusion for the first time
  discovery_bonus: { value: 40, min: 0, max: 300, step: 5 },     // void energy for discovering a nature
  discovery_copies: { value: 2, min: 1, max: 5, step: 1 },       // cards of a discovered nature added to the collection
  loss_keep: { value: 0.6, min: 0, max: 1, step: 0.05 },         // share of the run's void energy kept after a death
  win_bonus: { value: 100, min: 0, max: 1000, step: 10 },        // void energy for beating the Mother
  depth_void: { value: 0.2, min: 0, max: 1, step: 0.05 },        // extra void energy share per rift depth
  depth_hp: { value: 0.25, min: 0, max: 1, step: 0.05 },         // extra Ybasog hp share per rift depth
  deck_min: { value: 6, min: 1, max: 20, step: 1 },              // the smallest deck an adventure takes
  deck_max: { value: 12, min: 4, max: 30, step: 1 },             // the largest
  max_level: { value: 3, min: 1, max: 6, step: 1 },              // twin cards fuse up to this level
  stage_heal: { value: 5, min: 0, max: 20, step: 1 },            // energy back between stages

  // ---- The blob ----
  player_speed: { value: 470, min: 150, max: 600, step: 10 },   // px/s
  max_energy: { value: 20, min: 5, max: 60, step: 1 },          // energy is HP
  iframes: { value: 0.8, min: 0, max: 3, step: 0.1 },           // s of safety after a hit
  hit_damage: { value: 2, min: 0, max: 10, step: 1 },           // energy lost when a Ybasog touches you
  glob_damage: { value: 1, min: 0, max: 10, step: 1 },          // energy lost to a feeder glob
  bump_damage: { value: 1, min: 0, max: 10, step: 1 },          // energy lost when falling matter body-checks you
  magnet_radius: { value: 120, min: 20, max: 400, step: 10 },   // px: void-energy motes fly to you inside this
  mote_speed: { value: 560, min: 100, max: 1200, step: 20 },    // px/s

  // ---- Bolts ----
  fire_interval: { value: 0.17, min: 0.05, max: 0.6, step: 0.01 }, // s between bolts while firing
  bolt_speed: { value: 640, min: 200, max: 1200, step: 20 },    // px/s
  bolt_bounces: { value: 4, min: 0, max: 12, step: 1 },         // ricochets before a bolt fades
  bolt_damage: { value: 1, min: 0.2, max: 5, step: 0.1 },       // per bolt, before level and fusion
  fused_bonus: { value: 0.3, min: 0, max: 2, step: 0.05 },      // extra damage share for a fused card
  level_bonus: { value: 0.5, min: 0, max: 2, step: 0.05 },      // extra damage share per card level above 1
  // ---- Cards found in the adventure: the deck starts as one void card; the loadout is the pool they come from ----
  card_drop: { value: 1, min: 0, max: 5, step: 0.1 },           // multiplies every card drop chance below
  card_plain: { value: 0.005, min: 0, max: 0.2, step: 0.005 },  // chance a plain Ybasog drops a card
  card_natured: { value: 0.04, min: 0, max: 0.5, step: 0.01 },  // chance a natured Ybasog drops a card
  card_mini: { value: 0.6, min: 0, max: 1, step: 0.05 },        // chance a mini-boss drops a card
  card_boss: { value: 1, min: 0, max: 1, step: 0.05 },          // chance a boss drops a card
  card_wreck: { value: 0.1, min: 0, max: 1, step: 0.01 },       // chance a broken wreck holds a card
  card_ruin: { value: 0.15, min: 0, max: 1, step: 0.01 },       // chance a broken ruin piece holds a card
  trial_chance: { value: 0.02, min: 0, max: 0.5, step: 0.01 },  // chance a found card is a trial: not in your pool, kept for this adventure only
  pick_temper: { value: 0.3, min: 0, max: 1, step: 0.05 },      // chance a wave-clear pick offers "temper a card +1" in place of a card
  reshuffle_time: { value: 0.35, min: 0, max: 2, step: 0.05 },  // s with no fire while the discard is shuffled back

  // ---- Nature effects ----
  burn_dps: { value: 1.6, min: 0, max: 10, step: 0.1 },         // Wrath
  burn_time: { value: 2, min: 0, max: 6, step: 0.1 },
  chill_slow: { value: 0.55, min: 0, max: 0.95, step: 0.05 },   // Patience: share of speed taken away
  chill_time: { value: 2.2, min: 0, max: 6, step: 0.1 },
  pull_radius: { value: 110, min: 20, max: 300, step: 5 },      // Greed
  pull_strength: { value: 240, min: 0, max: 800, step: 10 },
  pull_time: { value: 0.6, min: 0, max: 3, step: 0.1 },
  split_count: { value: 2, min: 1, max: 6, step: 1 },           // Charity
  heal_on_kill: { value: 1, min: 0, max: 5, step: 0.5 },
  shard_damage: { value: 0.6, min: 0.1, max: 3, step: 0.1 },
  radiant_mult: { value: 2, min: 1, max: 5, step: 0.1 },        // Pride: damage on an unhurt target
  stone_mult: { value: 1.4, min: 1, max: 4, step: 0.1 },        // Humility: heavy bolt damage
  stone_matter: { value: 2, min: 1, max: 5, step: 0.1 },        // Humility: extra against falling matter
  venom_dps: { value: 0.6, min: 0, max: 5, step: 0.1 },         // Envy: per stack
  venom_stacks: { value: 5, min: 1, max: 12, step: 1 },
  venom_time: { value: 3, min: 0, max: 10, step: 0.5 },
  thorns_radius: { value: 55, min: 10, max: 200, step: 5 },     // Kindness: bramble patch
  thorns_dps: { value: 1.3, min: 0, max: 10, step: 0.1 },
  thorns_time: { value: 3, min: 0, max: 10, step: 0.5 },
  charm_time: { value: 2.5, min: 0, max: 10, step: 0.5 },       // Lust: a charmed Ybasog turns on its kin
  charm_dps: { value: 3, min: 0, max: 10, step: 0.5 },          // damage a charmed Ybasog does to those it touches
  pierce_count: { value: 3, min: 1, max: 10, step: 1 },         // Chastity: targets passed through
  devour_grow: { value: 0.3, min: 0, max: 2, step: 0.05 },      // Gluttony: damage gained per hit
  gust_push: { value: 70, min: 0, max: 300, step: 5 },          // Temperance: px knocked back
  sleep_time: { value: 1.2, min: 0, max: 5, step: 0.1 },        // Sloth
  chain_count: { value: 2, min: 1, max: 6, step: 1 },           // Diligence: jumps
  chain_range: { value: 170, min: 30, max: 400, step: 10 },
  chain_share: { value: 0.7, min: 0, max: 2, step: 0.05 },      // share of the bolt's damage per jump
  well_radius: { value: 95, min: 20, max: 300, step: 5 },
  well_time: { value: 1.4, min: 0, max: 5, step: 0.1 },
  well_dps: { value: 2.2, min: 0, max: 10, step: 0.1 },
  field_radius: { value: 105, min: 20, max: 300, step: 5 },
  field_time: { value: 1.8, min: 0, max: 5, step: 0.1 },
  field_slow: { value: 0.85, min: 0, max: 1, step: 0.05 },
  execute_line: { value: 0.35, min: 0, max: 1, step: 0.05 },    // share of hp under which an execute kills
  doom_time: { value: 1.5, min: 0.1, max: 5, step: 0.1 },
  doom_mult: { value: 3, min: 0, max: 10, step: 0.5 },
  storm_radius: { value: 95, min: 20, max: 300, step: 5 },
  storm_time: { value: 3, min: 0.5, max: 10, step: 0.5 },
  storm_zap: { value: 1.2, min: 0, max: 10, step: 0.1 },        // damage per strike, a strike every 0.35 s
  meteor_damage: { value: 4, min: 0, max: 20, step: 0.5 },
  meteor_radius: { value: 65, min: 10, max: 200, step: 5 },
  mine_damage: { value: 3, min: 0, max: 20, step: 0.5 },
  mine_radius: { value: 70, min: 10, max: 200, step: 5 },
  quake_damage: { value: 1.2, min: 0, max: 10, step: 0.1 },     // share of the bolt's damage to the whole row
  wall_time: { value: 4, min: 0.5, max: 12, step: 0.5 },
  timewarp_time: { value: 1.6, min: 0, max: 6, step: 0.1 },
  timewarp_slow: { value: 0.45, min: 0, max: 0.95, step: 0.05 },
  thrall_time: { value: 6, min: 0, max: 20, step: 0.5 },
  frenzy_time: { value: 3, min: 0, max: 10, step: 0.5 },
  frenzy_rate: { value: 0.6, min: 0.1, max: 1, step: 0.05 },    // fire interval multiplier while frenzied
  leech_share: { value: 0.15, min: 0, max: 1, step: 0.01 },
  decoy_time: { value: 4, min: 0, max: 12, step: 0.5 },
  ward_max: { value: 3, min: 1, max: 10, step: 1 },

  // ---- Ybasogs (each kind's life, speed and size live in window.YBASOGS below) ----
  pod_hatch: { value: 9, min: 1, max: 30, step: 0.5 },          // s until a pod-family Ybasog hatches
  grazer_dive_line: { value: 0.45, min: 0.1, max: 0.8, step: 0.05 }, // share of the screen height where grazers dive
  glob_speed: { value: 170, min: 40, max: 500, step: 10 },      // px/s
  mother_hp: { value: 320, min: 20, max: 2000, step: 10 },
  mother_spawn: { value: 3.2, min: 0.5, max: 10, step: 0.1 },
  mother_spread: { value: 3.6, min: 0.5, max: 10, step: 0.1 },
  mother_volley: { value: 7, min: 1, max: 20, step: 1 },
  telegraph_time: { value: 1, min: 0.2, max: 3, step: 0.1 },    // s of warning before a beam or a lash lands
  beam_damage: { value: 2, min: 0, max: 10, step: 1 },          // energy lost to a gaze beam or a lash
  pulse_speed: { value: 210, min: 60, max: 600, step: 10 },     // px/s a hymn or croak ring spreads
  pulse_damage: { value: 1, min: 0, max: 5, step: 1 },          // energy lost to a ring
  brine_slow: { value: 0.5, min: 0, max: 0.9, step: 0.05 },     // share of your speed a brine pool takes
  brine_time: { value: 5, min: 1, max: 12, step: 0.5 },         // s a brine pool lasts
  hunt_time: { value: 24, min: 8, max: 60, step: 1 },           // s a hunter chases you before it drifts off
  reach_time: { value: 13, min: 4, max: 30, step: 1 },          // s a reacher clings to the rift wall before it withdraws
  leech_drain: { value: 1.3, min: 0.3, max: 5, step: 0.1 },     // s between the energy a latched leech drains
  pull_speed: { value: 95, min: 0, max: 300, step: 5 },         // px/s a suckermouth drags you toward it
  shriek_push: { value: 520, min: 0, max: 1500, step: 20 },     // px/s a shriek throws you back

  // ---- Natured Ybasogs: some carry an innate nature ----
  nature_chance: { value: 0.2, min: 0, max: 1, step: 0.05 },    // share of stage 1 Ybasogs that carry a nature
  nature_chance_step: { value: 0.12, min: 0, max: 0.5, step: 0.01 }, // more per later stage
  prevailing_share: { value: 0.6, min: 0, max: 1, step: 0.05 }, // share of natured Ybasogs that carry the adventure's prevailing nature
  opposite_mult: { value: 1.6, min: 1, max: 4, step: 0.05 },    // damage of a bolt holding the opposite nature
  same_mult: { value: 0.5, min: 0, max: 1, step: 0.05 },        // damage of a bolt holding the same nature

  // ---- The run's ramp: waves are built from beats (patterns) by a director ----
  stage_hp_scale: { value: 0.35, min: 0, max: 2, step: 0.05 },  // extra hp share per stage after the first
  stage_speed_scale: { value: 0.1, min: 0, max: 1, step: 0.02 }, // extra speed share per stage after the first
  beats_base: { value: 3, min: 1, max: 12, step: 1 },           // beats in a stage's first wave
  beats_step: { value: 1, min: 0, max: 6, step: 1 },            // more beats per later wave and per later stage
  debuts_per_wave: { value: 1, min: 0, max: 3, step: 1 },       // new Ybasog kinds brought in each wave
};

// ---- Data, not tuning ----
// The fourteen innate natures. Opposites annul. Each shapes the blob into a form with a trait:
// trait keys are read by formTraits() in index.html.
window.NATURES = {
  base: {
    pride:      { name: 'Pride',      kind: 'sin',    element: 'radiance', color: '#fff0b0', opposite: 'humility',   form: 'lion',     formName: 'Lion',
                  trait: { damage: 1.2, max_energy: -3 }, traitText: '+20% bolt damage, -3 max energy' },
    greed:      { name: 'Greed',      kind: 'sin',    element: 'gravity',  color: '#f2c94c', opposite: 'charity',    form: 'rat',      formName: 'Rat',
                  trait: { magnet: 120, mote_bonus: 0.25 }, traitText: 'pulls void energy from afar, +25% void energy' },
    wrath:      { name: 'Wrath',      kind: 'sin',    element: 'fire',     color: '#ff4b3a', opposite: 'patience',   form: 'dragon',   formName: 'Dragon',
                  trait: { fire_rate: 1.15, bite: 1 }, traitText: '+15% fire rate, bites hurt 1 more' },
    envy:       { name: 'Envy',       kind: 'sin',    element: 'venom',    color: '#8be04e', opposite: 'kindness',   form: 'serpent',  formName: 'Serpent',
                  trait: { bounces: 1, status: 1.25 }, traitText: '+1 ricochet, effects last 25% longer' },
    lust:       { name: 'Lust',       kind: 'sin',    element: 'allure',   color: '#ff6fc8', opposite: 'chastity',   form: 'moth',     formName: 'Moth',
                  trait: { contact_charm: 0.5, speed: 1.1 }, traitText: 'half of biting Ybasogs are charmed instead' },
    gluttony:   { name: 'Gluttony',   kind: 'sin',    element: 'hunger',   color: '#ff8a2a', opposite: 'temperance', form: 'toad',     formName: 'Toad',
                  trait: { max_energy: 6, speed: 0.85 }, traitText: '+6 max energy, 15% slower' },
    sloth:      { name: 'Sloth',      kind: 'sin',    element: 'slumber',  color: '#9a8fc8', opposite: 'diligence',  form: 'snail',    formName: 'Snail',
                  trait: { aura: 0.3, speed: 0.75 }, traitText: 'Ybasogs near you slow by 30%, 25% slower' },
    humility:   { name: 'Humility',   kind: 'virtue', element: 'stone',    color: '#c8a77a', opposite: 'pride',      form: 'tortoise', formName: 'Tortoise',
                  trait: { armor: 1, max_energy: 2, speed: 0.85 }, traitText: 'hits hurt 1 less, +2 max energy, 15% slower' },
    charity:    { name: 'Charity',    kind: 'virtue', element: 'grace',    color: '#c8ffe8', opposite: 'greed',      form: 'bird',     formName: 'Dove',
                  trait: { wave_heal: 2, speed: 1.15 }, traitText: 'heal 2 after each wave, 15% faster' },
    patience:   { name: 'Patience',   kind: 'virtue', element: 'ice',      color: '#8fe3ff', opposite: 'wrath',      form: 'humanoid', formName: 'Wanderer',
                  trait: { iframes: 0.5, quick_shuffle: 1 }, traitText: 'longer safety after hits, instant reshuffle' },
    kindness:   { name: 'Kindness',   kind: 'virtue', element: 'growth',   color: '#3fd08f', opposite: 'envy',       form: 'stag',     formName: 'Stag',
                  trait: { regen: 0.1 }, traitText: 'regrows energy steadily' },
    chastity:   { name: 'Chastity',   kind: 'virtue', element: 'purity',   color: '#ffffff', opposite: 'lust',       form: 'swan',     formName: 'Swan',
                  trait: { glob_dodge: 0.4 }, traitText: '40% of globs pass through you' },
    temperance: { name: 'Temperance', kind: 'virtue', element: 'wind',     color: '#9ad7ff', opposite: 'gluttony',   form: 'manta',    formName: 'Manta',
                  trait: { speed: 1.3, iframes: 0.2 }, traitText: '30% faster, longer safety after hits' },
    diligence:  { name: 'Diligence',  kind: 'virtue', element: 'lightning', color: '#7d8bff', opposite: 'sloth',     form: 'bee',      formName: 'Bee',
                  trait: { fire_rate: 1.3, damage: 0.85 }, traitText: '+30% fire rate, -15% bolt damage' },
  },

  // Recipes: the key is the natures' ids sorted and joined by '+'. fx are the bolt's effects
  // (index.html, boltHits and fire). A fusion with no color blends its two natures' colours.
  // Three- and four-nature recipes go here later the same way; a set with no recipe is unstable.
  recipes: {
    '':           { name: 'Void Bolt', color: '#b9a6ff', fx: [], says: 'A plain bolt of void.' },
    pride:        { name: 'Pride',      fx: ['radiant'], says: 'Double damage to the unhurt.' },
    greed:        { name: 'Greed',      fx: ['pull'],    says: 'Pulls nearby Ybasogs to the hit.' },
    wrath:        { name: 'Wrath',      fx: ['burn'],    says: 'Burns what it hits.' },
    envy:         { name: 'Envy',       fx: ['venom'],   says: 'Venom that stacks.' },
    lust:         { name: 'Lust',       fx: ['charm'],   says: 'Charms: the Ybasog turns on its kin.' },
    gluttony:     { name: 'Gluttony',   fx: ['devour'],  says: 'Grows with every hit; kills drop more.' },
    sloth:        { name: 'Sloth',      fx: ['sleep'],   says: 'Puts what it hits to sleep.' },
    humility:     { name: 'Humility',   fx: ['stone'],   says: 'A heavy stone; cracks matter.' },
    charity:      { name: 'Charity',    fx: ['split', 'heal'], says: 'Splits on hit; a kill heals you.' },
    patience:     { name: 'Patience',   fx: ['chill'],   says: 'Freezes: slows what it hits.' },
    kindness:     { name: 'Kindness',   fx: ['thorns'],  says: 'Leaves a patch of brambles.' },
    chastity:     { name: 'Chastity',   fx: ['pierce'],  says: 'Pierces straight through.' },
    temperance:   { name: 'Temperance', fx: ['gust'],    says: 'A gust that knocks them back.' },
    diligence:    { name: 'Diligence',  fx: ['chain'],   says: 'Lightning leaps to two more.' },

    // sin with sin
    'greed+pride':       { name: 'Midas',         fx: ['gild'],                 says: 'The slain burst into golden shards.' },
    'pride+wrath':       { name: 'Supernova',     fx: ['nova', 'burn'], shardFx: ['burn'], says: 'A ring of burning shards on impact.' },
    'envy+pride':        { name: 'Vanity',        fx: ['mirror', 'radiant'],    says: 'A mirror-twin strikes from the far wall.' },
    'lust+pride':        { name: 'Adoration',     fx: ['charm', 'thrall'],      says: 'The charmed who die rise as your wisps.' },
    'gluttony+pride':    { name: 'Gilded Maw',    fx: ['execute', 'devour'],    says: 'Swallows any Ybasog below a third of its life.' },
    'pride+sloth':       { name: 'Throne',        fx: ['orbit', 'radiant'],     says: 'Circles you as a guard, then launches.' },
    'greed+wrath':       { name: 'Hunger',        fx: ['well', 'burn'],         says: 'A burning gravity well.' },
    'envy+greed':        { name: 'Covet',         fx: ['tether'],               says: 'Binds two Ybasogs: they share their wounds.' },
    'greed+lust':        { name: 'Siren',         fx: ['lure', 'charm'],        says: 'A wide pull, and the pulled are charmed.' },
    'gluttony+greed':    { name: 'Hoard',         fx: ['hoard', 'devour'],      says: 'Kills drop triple void energy.' },
    'greed+sloth':       { name: 'Event Horizon', fx: ['singularity'],          says: 'A slow orb that collapses into a black hole.' },
    'envy+wrath':        { name: 'Spite',         fx: ['doom'],                 says: 'Marks a Ybasog; it bursts for triple a moment later.' },
    'lust+wrath':        { name: 'Ardor',         fx: ['frenzy', 'burn'],       says: 'Each kill quickens your fire.' },
    'gluttony+wrath':    { name: 'Wildfire',      fx: ['killburst', 'burn'], shardFx: ['burn'], says: 'Kills erupt in a ring of fire.' },
    'sloth+wrath':       { name: 'Smolder',       fx: ['mine', 'burn'],         says: 'Clings, smoulders, then erupts.' },
    'envy+lust':         { name: 'Obsession',     fx: ['homing', 'venom'],      says: 'Hunts the nearest Ybasog and never lets go.' },
    'envy+gluttony':     { name: 'Parasite',      fx: ['leech', 'venom'],       says: 'Venom; the damage feeds you.' },
    'envy+sloth':        { name: 'Rot',           fx: ['spore', 'venom'],       says: 'Kills release a cloud of rot.' },
    'gluttony+lust':     { name: 'Indulgence',    fx: ['echo', 'devour'],       says: 'A second helping fires right behind it.' },
    'lust+sloth':        { name: 'Reverie',       fx: ['lullaby'],              says: 'Lulls every Ybasog nearby to sleep.' },
    'gluttony+sloth':    { name: 'Glut',          fx: ['pierce', 'devour', 'stone', 'big'], says: 'A slow boulder that pierces and grows.' },
    // virtue with virtue
    'charity+humility':  { name: 'Alms',          fx: ['aegis', 'heal'],        says: 'Kills grant a ward that blocks one hit.' },
    'humility+patience': { name: 'Glacier',       fx: ['quake', 'chill'],       says: 'A frost shockwave runs the whole row.' },
    'humility+kindness': { name: 'Hedgerow',      fx: ['thorns', 'split', 'wide'], shardFx: ['thorns'], says: 'Brambles burst wide and scatter seeds.' },
    'chastity+humility': { name: 'Penance',       fx: ['pierce', 'cleanse'],    says: 'Pierces and washes away nearby globs.' },
    'humility+temperance': { name: 'Sandstorm',   fx: ['storm', 'stone'],       says: 'Leaves a scouring storm of grit.' },
    'diligence+humility': { name: 'Bulwark',      fx: ['wall'],                 says: 'Raises a stone wall that stops globs and Ybasogs.' },
    'charity+patience':  { name: 'Mercy',         fx: ['shatter', 'chill', 'heal'], shardFx: ['chill'], says: 'Shatters into ice shards; kills heal.' },
    'charity+kindness':  { name: 'Blossom',       fx: ['bloom', 'heal'],        says: 'The slain bloom into healing petals.' },
    'charity+chastity':  { name: 'Halo',          fx: ['homing', 'heal', 'radiant'], says: 'A seeking halo; kills heal.' },
    'charity+temperance': { name: 'Zephyr',       fx: ['timewarp'],             says: 'A soft wind slows every Ybasog in the rift.' },
    'charity+diligence': { name: 'Providence',    fx: ['chain', 'heal'],        says: 'Chain lightning; kills heal.' },
    'kindness+patience': { name: 'Evergreen',     fx: ['thorns', 'chill'],      says: 'Frost brambles that slow and wound.' },
    'chastity+patience': { name: 'Snowdrop',      fx: ['pierce', 'chill'],      says: 'A frozen lance through the line.' },
    'patience+temperance': { name: 'Blizzard',    fx: ['storm', 'chill'],       says: 'A storm of frost that freezes as it strikes.' },
    'diligence+patience': { name: 'Hourglass',    fx: ['sleep', 'doom'],        says: 'Stops a Ybasog, then shatters it for triple.' },
    'chastity+kindness': { name: 'Lily',          fx: ['split3', 'heal'], shardFx: ['thorns'], says: 'Three seeds that root as brambles.' },
    'kindness+temperance': { name: 'Pollen',      fx: ['decoy', 'thorns'],      says: 'Plants a flower the grazers dive at.' },
    'diligence+kindness': { name: 'Hive',         fx: ['split', 'chain'], shardFx: ['homing'], says: 'Releases bees that hunt on their own.' },
    'chastity+temperance': { name: 'Clarity',     fx: ['beam'],                 says: 'A beam of pure light, instant and endless.' },
    'chastity+diligence': { name: 'Oath',         fx: ['snowball', 'pierce'],   says: 'Keeps going: every ricochet makes it stronger.' },
    'diligence+temperance': { name: 'Tempest',    fx: ['chain', 'gust'],        says: 'Lightning on the wind; flings them back.' },
    // sin with virtue
    'charity+pride':     { name: 'Largesse',      fx: ['prism'],                says: 'Splits into three bolts of natures you know.' },
    'patience+pride':    { name: 'Diamond',       fx: ['shatter', 'radiant'], shardFx: ['radiant'], says: 'A crystal that bursts into blinding shards.' },
    'kindness+pride':    { name: 'Laurel',        fx: ['execute', 'heal'],      says: 'Finishes the weakened; kills heal.' },
    'chastity+pride':    { name: 'Seraph',        fx: ['homing', 'pierce', 'radiant'], says: 'A seeking light that passes through.' },
    'pride+temperance':  { name: 'Comet',         fx: ['meteor'],               says: 'Calls a comet down on what it hits.' },
    'diligence+pride':   { name: 'Zenith',        fx: ['boomerang', 'radiant'], says: 'Flies out, then returns to strike again.' },
    'greed+humility':    { name: 'Lodestone',     fx: ['pull', 'stone', 'magnet'], says: 'A heavy magnet; calls all void energy to you.' },
    'greed+patience':    { name: 'Stasis',        fx: ['field', 'chill'],       says: 'A freezing field that holds them.' },
    'greed+kindness':    { name: 'Harvest',       fx: ['hoard', 'heal'],        says: 'Kills drop triple void energy and heal.' },
    'chastity+greed':    { name: 'Tithe',         fx: ['tithe', 'pierce'],      says: 'Takes a tenth of a Ybasog\'s life with each hit.' },
    'greed+temperance':  { name: 'Whirlpool',     fx: ['vortex', 'gust'],       says: 'A spinning vortex that grinds them down.' },
    'diligence+greed':   { name: 'Compound',      fx: ['snowball', 'hoard'],    says: 'Interest grows with every ricochet.' },
    'humility+wrath':    { name: 'Magma',         fx: ['stone', 'killburst', 'burn'], shardFx: ['burn'], says: 'Molten rock; kills erupt.' },
    'charity+wrath':     { name: 'Zeal',          fx: ['split3', 'burn', 'heal'], shardFx: ['burn'], says: 'Bursts into three burning shards.' },
    'kindness+wrath':    { name: 'Ember Bloom',   fx: ['thorns', 'burn'],       says: 'Burning brambles.' },
    'chastity+wrath':    { name: 'Crusade',       fx: ['pierce', 'burn'],       says: 'A burning lance through the line.' },
    'temperance+wrath':  { name: 'Firestorm',     fx: ['storm', 'burn'],        says: 'A storm that rains fire.' },
    'diligence+wrath':   { name: 'Arc Forge',     fx: ['chain', 'burn'],        says: 'Lightning that sets them alight.' },
    'envy+humility':     { name: 'Moss',          fx: ['venom', 'stone', 'wall'], says: 'A heavy venom stone that raises a wall.' },
    'charity+envy':      { name: 'Poisoned Gift', fx: ['split', 'venom'], shardFx: ['venom'], says: 'Splits into poisoned shards.' },
    'envy+patience':     { name: 'Frostbite',     fx: ['venom', 'chill'],       says: 'Cold venom: slows and rots.' },
    'chastity+envy':     { name: 'Truth',         fx: ['cleanse', 'venom', 'execute'], says: 'Washes globs away; finishes the weak.' },
    'envy+temperance':   { name: 'Miasma',        fx: ['storm', 'venom'],       says: 'A storm of rot.' },
    'diligence+envy':    { name: 'Rivalry',       fx: ['mirror', 'venom'],      says: 'A jealous twin strikes from the far wall.' },
    'humility+lust':     { name: 'Devotion',      fx: ['charm', 'aegis'],       says: 'Charms; kills grant a ward.' },
    'charity+lust':      { name: 'Embrace',       fx: ['charm', 'leech'],       says: 'Charms, and the damage feeds you.' },
    'lust+patience':     { name: 'Longing',       fx: ['charm', 'chill', 'long'], says: 'A long, cold charm.' },
    'kindness+lust':     { name: 'Rose',          fx: ['thorns', 'charm'],      says: 'Thorny brambles that charm.' },
    'lust+temperance':   { name: 'Allure',        fx: ['decoy', 'lure'],        says: 'A lure the Ybasogs cannot resist.' },
    'diligence+lust':    { name: 'Fervor',        fx: ['frenzy', 'chain'],      says: 'Lightning; each kill quickens your fire.' },
    'gluttony+humility': { name: 'Famine',        fx: ['quake', 'devour'],      says: 'A hungry shockwave across the row.' },
    'charity+gluttony':  { name: 'Feast',         fx: ['hoard', 'heal', 'devour'], says: 'Kills drop a feast of void energy and heal.' },
    'gluttony+patience': { name: 'Hibernate',     fx: ['sleep', 'chill', 'devour'], says: 'Cold sleep, and it grows as it feeds.' },
    'gluttony+kindness': { name: 'Orchard',       fx: ['bloom', 'devour'],      says: 'The slain fall as healing fruit.' },
    'chastity+gluttony': { name: 'Fast',          fx: ['pierce', 'execute'],    says: 'Pierces, and finishes the weak.' },
    'diligence+gluttony': { name: 'Grindstone',   fx: ['snowball', 'devour'],   says: 'Grinds harder with every ricochet.' },
    'humility+sloth':    { name: 'Mire',          fx: ['field', 'stone'],       says: 'A heavy mire that holds them fast.' },
    'charity+sloth':     { name: 'Respite',       fx: ['lullaby', 'heal'],      says: 'Lulls them to sleep; kills heal.' },
    'patience+sloth':    { name: 'Permafrost',    fx: ['timewarp', 'chill'],    says: 'The whole rift freezes for a moment.' },
    'kindness+sloth':    { name: 'Lichen',        fx: ['thorns', 'sleep'],      says: 'Sleepy brambles.' },
    'chastity+sloth':    { name: 'Daydream',      fx: ['orbit', 'sleep'],       says: 'Drifts around you, then puts them to sleep.' },
    'sloth+temperance':  { name: 'Doldrums',      fx: ['timewarp', 'sleep'],    says: 'The wind dies; the rift slows to a crawl.' },
  },

  // Discoveries: each nature but your innate one is found by an achievement. Progress is kept
  // between adventures. stat names are counted in index.html (track()).
  discover: {
    // Hard on purpose: most take several adventures. Rough pace of one full adventure: ~150 kills,
    // ~60 pods, ~15 feeders, ~25 asteroids, ~15 debris, ~6 ruins, ~400 void energy, 9 waves.
    wrath:      { stat: 'multikill',   n: 30,   text: 'Slay 3 Ybasogs within half a second, 30 times' },
    patience:   { stat: 'cleanwave',   n: 8,    text: 'Clear 8 waves without being hurt' },
    greed:      { stat: 'motes',       n: 2500, text: 'Gather 2,500 void energy' },
    charity:    { stat: 'sealedwave',  n: 15,   text: 'Clear 15 waves with nothing escaping' },
    pride:      { stat: 'kills',       n: 1500, text: 'Slay 1,500 Ybasogs' },
    humility:   { stat: 'asteroids',   n: 150,  text: 'Break 150 asteroids' },
    envy:       { stat: 'ricochets',   n: 200,  text: 'Slay 200 Ybasogs with bolts that ricocheted twice' },
    kindness:   { stat: 'lowwave',     n: 5,    text: 'Clear 5 waves on 5 energy or less' },
    lust:       { stat: 'closekills',  n: 150,  text: 'Slay 150 Ybasogs close to your body' },
    chastity:   { stat: 'ruins',       n: 40,   text: 'Shatter 40 ruins' },
    gluttony:   { stat: 'pods',        n: 400,  text: 'Slay 400 pods before they hatch' },
    temperance: { stat: 'debris',      n: 150,  text: 'Break 150 pieces of space debris' },
    sloth:      { stat: 'stillkills',  n: 300,  text: 'Slay 300 Ybasogs while standing still' },
    diligence:  { stat: 'feeders',     n: 120,  text: 'Slay 120 feeders' },
  },

  // Omens: each adventure is read before it starts, so the deck can be prepared for it.
  omens: {
    calm:      { name: 'A Quiet Drift', says: 'Nothing stirs but the Ybasogs.' },
    swarm:     { name: 'The Brood Wakes', says: 'Half again as many pods; void energy +25%.', pods: 1.5, void: 1.25 },
    rockfall:  { name: 'Rockfall', says: 'Twice the asteroids, and they drop twice the void energy.', rocks: 2, rockVoid: 2 },
    ruinwake:  { name: 'Ruinwake', says: 'Ruins drift from the first stage and drop triple.', ruinsEarly: 1, ruinVoid: 3 },
    hunger:    { name: 'The Hungry Dark', says: 'Feeders spit faster; every kill drops one more.', spit: 0.7, killVoid: 1 },
    eclipse:   { name: 'Eclipse', says: 'Bolts ricochet once more; Ybasogs have 20% more life.', bounces: 1, hp: 1.2 },
    resonance: { name: 'Resonance', says: 'Nature effects last half again as long.', status: 1.5 },
  },
};

// ---- The Ybasogs: the hive, in families of variants ----
// role: minion (in waves), mini (a mini-boss), boss. from: the first stage it appears in. w: weight in a wave's pool.
// group: how many come together. nature: a boss's fixed innate nature (others may roll one when they spawn).
window.YBASOGS = {
  // pods: drift down and hatch
  pod:      { fam: 'pod', name: 'Pod',        role: 'minion', hp: 3,  sp: 30, r: 15, drop: 2, from: 1, w: 3,   says: 'Drifts down and hatches a grazer.' },
  husk:     { fam: 'pod', name: 'Husk Pod',   role: 'minion', hp: 8,  sp: 24, r: 17, drop: 4, from: 1, w: 1,   says: 'An armoured shell halves damage until it cracks; hatches a horned grazer.' },
  clutch:   { fam: 'pod', name: 'Clutch Pod', role: 'minion', hp: 4,  sp: 28, r: 17, drop: 3, from: 2, w: 1.2, says: 'Bursts into three mites when it dies or hatches.' },
  bloat:    { fam: 'pod', name: 'Bloat Pod',  role: 'minion', hp: 3,  sp: 22, r: 19, drop: 3, from: 2, w: 1,   says: 'Swollen with bile: bursts into a ring of globs when killed.' },
  // grazers: drift, then dive
  grazer:   { fam: 'grazer', name: 'Grazer',        role: 'minion', hp: 4,  sp: 44, r: 15, drop: 3, from: 1, w: 2.5, dive: 230, says: 'Dives at you once it passes the middle.' },
  swift:    { fam: 'grazer', name: 'Swift Grazer',  role: 'minion', hp: 3,  sp: 70, r: 12, drop: 3, from: 1, w: 1.2, dive: 330, says: 'Small and quick; dives early.' },
  stalker:  { fam: 'grazer', name: 'Stalker',       role: 'minion', hp: 5,  sp: 40, r: 15, drop: 4, from: 2, w: 1,   dive: 260, says: 'Weaves as it falls and turns once mid-dive.' },
  horned:   { fam: 'grazer', name: 'Horned Grazer', role: 'minion', hp: 12, sp: 36, r: 18, drop: 5, from: 2, w: 0.8, dive: 300, says: 'Tough; plunges straight down.' },
  // feeders: hover and spit
  feeder:   { fam: 'feeder', name: 'Feeder',  role: 'minion', hp: 7, sp: 26, r: 22, drop: 5, from: 1, w: 1.6, spit: 2.4, says: 'Hovers and spits a glob at you.' },
  spitter:  { fam: 'feeder', name: 'Spitter', role: 'minion', hp: 8, sp: 24, r: 22, drop: 6, from: 2, w: 1,   spit: 3,   says: 'Three mouths: a spread of three globs.' },
  lobber:   { fam: 'feeder', name: 'Lobber',  role: 'minion', hp: 7, sp: 24, r: 22, drop: 6, from: 2, w: 0.8, spit: 3.4, says: 'Lobs a sac that bursts into droplets above you.' },
  sniper:   { fam: 'feeder', name: 'Sniper',  role: 'minion', hp: 6, sp: 30, r: 20, drop: 6, from: 3, w: 0.7, spit: 3.2, says: 'Aims a line, then fires a fast glob along it.' },
  // eyes: watch and strike from afar
  watcher:  { fam: 'eye', name: 'Watcher', role: 'minion', hp: 6, sp: 30, r: 16, drop: 5, from: 2, w: 0.9, says: 'Marks your column, then burns it with its gaze.' },
  blinker:  { fam: 'eye', name: 'Blinker', role: 'minion', hp: 5, sp: 0,  r: 14, drop: 5, from: 2, w: 0.8, says: 'Vanishes and reappears, spitting after each blink.' },
  weeper:   { fam: 'eye', name: 'Weeper',  role: 'minion', hp: 6, sp: 20, r: 16, drop: 4, from: 1, w: 0.9, says: 'Drips tears that fall straight down.' },
  // devourers: eat
  devourer: { fam: 'devourer', name: 'Devourer', role: 'minion', hp: 9,  sp: 40, r: 20, drop: 6, from: 2, w: 0.8, says: 'Chases and eats your void energy, growing as it feeds.' },
  gorger:   { fam: 'devourer', name: 'Gorger',   role: 'minion', hp: 14, sp: 20, r: 24, drop: 7, from: 2, w: 0.6, says: 'Eats falling matter it touches to heal and grow.' },
  maw:      { fam: 'devourer', name: 'Maw',      role: 'minion', hp: 10, sp: 26, r: 22, drop: 6, from: 3, w: 0.6, says: 'Its open mouth swallows bolts from below: strike it from the sides or bank shots off the top.' },
  // deacons: tend the hive
  warder:   { fam: 'deacon', name: 'Warder', role: 'minion', hp: 10, sp: 22, r: 18, drop: 8, from: 2, w: 0.5, says: 'Shields Ybasogs around it.' },
  mender:   { fam: 'deacon', name: 'Mender', role: 'minion', hp: 9,  sp: 22, r: 18, drop: 8, from: 2, w: 0.5, says: 'Heals Ybasogs around it.' },
  herald:   { fam: 'deacon', name: 'Herald', role: 'minion', hp: 11, sp: 20, r: 18, drop: 8, from: 3, w: 0.5, says: 'Calls pods out of the dark.' },
  // mites: tiny, in swarms
  mite:     { fam: 'mite', name: 'Mite',       role: 'minion', hp: 1,   sp: 80, r: 7, drop: 1, from: 1, w: 1.2, group: 5, says: 'A flickering swarm.' },
  lancer:   { fam: 'mite', name: 'Lancer',     role: 'minion', hp: 1.5, sp: 60, r: 8, drop: 1, from: 2, w: 0.8, group: 4, says: 'Hovers, then darts at you.' },
  thief:    { fam: 'mite', name: 'Thief Mite', role: 'minion', hp: 1,   sp: 90, r: 8, drop: 1, from: 2, w: 0.6, group: 3, says: 'Steals void energy on touch and flees.' },
  // wall crawlers
  creeper:  { fam: 'crawler', name: 'Creeper', role: 'minion', hp: 7,  sp: 35, r: 16, drop: 5, from: 2, w: 0.7, says: 'Crawls down a rift wall, spitting across.' },
  lasher:   { fam: 'crawler', name: 'Lasher',  role: 'minion', hp: 12, sp: 30, r: 18, drop: 7, from: 3, w: 0.5, says: 'Clings to a wall and sweeps a tendril across the rift.' },
  weaver:   { fam: 'crawler', name: 'Weaver',  role: 'minion', hp: 8,  sp: 30, r: 16, drop: 6, from: 2, w: 0.6, says: 'Spins webs across the rift that catch bolts.' },
  // mini-bosses
  warden:   { fam: 'mini', name: 'Brood Warden',     role: 'mini', hp: 90,  sp: 30, r: 40, drop: 25, from: 1, w: 1, says: 'A shelled brood-carrier that sows pods and mites.' },
  knight:   { fam: 'mini', name: 'Hive Knight',      role: 'mini', hp: 80,  sp: 30, r: 32, drop: 25, from: 1, w: 1, says: 'An armoured grazer that charges again and again.' },
  prime:    { fam: 'mini', name: 'Gazer Prime',      role: 'mini', hp: 70,  sp: 30, r: 34, drop: 25, from: 2, w: 1, says: 'Burns three columns at once; five when hurt.' },
  behemoth: { fam: 'mini', name: 'Gorging Behemoth', role: 'mini', hp: 110, sp: 26, r: 44, drop: 30, from: 2, w: 1, says: 'Eats falling matter and hurls it back at you.' },
  // bosses
  choir:    { fam: 'boss', name: 'The Choir',      role: 'boss', hp: 260, sp: 30, r: 56, drop: 40, from: 2, nature: 'pride', says: 'Three hooded heads behind orbiting wards; sings rings of globs and calls deacons.' },
  hollow:   { fam: 'boss', name: 'The Hollow Eye', role: 'boss', hp: 240, sp: 30, r: 60, drop: 40, from: 2, nature: 'envy', says: 'A vast eye that blinks across the rift and burns your column.' },
  mother:   { fam: 'boss', name: 'The Mother',     role: 'boss', hp: 320, sp: 40, r: 66, drop: 30, from: 3, nature: 'gluttony', says: 'The hive itself: pods, grazers, spreads of globs, hurled matter.' },
  // ---- iteration 6: every family has four variants, and four new families hunt you across the rift ----
  oracle:   { fam: 'eye', name: 'Oracle',            role: 'minion', hp: 7,  sp: 26, r: 17, drop: 5, from: 2, w: 0.7, says: 'Many eyes, one will: its globs seek you.' },
  leech:    { fam: 'devourer', name: 'Void Leech',   role: 'minion', hp: 5,  sp: 70, r: 12, drop: 4, from: 2, w: 0.7, says: 'Latches on and drains you until shot off.' },
  cantor:   { fam: 'deacon', name: 'Cantor',         role: 'minion', hp: 9,  sp: 22, r: 18, drop: 8, from: 2, w: 0.5, says: 'Sings a ring that spreads across the rift.' },
  gnat:     { fam: 'mite', name: 'Biter Gnat',       role: 'minion', hp: 1,  sp: 120, r: 7, drop: 1, from: 1, w: 0.7, group: 6, says: 'Circles you, then bites.' },
  skitter:  { fam: 'crawler', name: 'Skitterer',     role: 'minion', hp: 7,  sp: 90, r: 15, drop: 5, from: 1, w: 0.6, says: 'Runs along the rift floor, spitting up at you.' },
  grasper:  { fam: 'reacher', name: 'Grasping Limb', role: 'minion', hp: 6,  sp: 0, r: 16, drop: 4, from: 1, w: 0.7, says: 'Reaches from a wall at your height, then lunges.' },
  sucker:   { fam: 'reacher', name: 'Suckermouth',   role: 'minion', hp: 8,  sp: 0, r: 18, drop: 5, from: 2, w: 0.5, says: 'Clings to a wall and drags you toward its mouth.' },
  thrasher: { fam: 'reacher', name: 'Thrasher',      role: 'minion', hp: 10, sp: 0, r: 20, drop: 6, from: 2, w: 0.5, says: 'Hangs from the top and sweeps down at you.' },
  stalkeye: { fam: 'reacher', name: 'Stalk Eye',     role: 'minion', hp: 5,  sp: 0, r: 14, drop: 4, from: 1, w: 0.6, says: 'Peers from a wall on a stalk and fires at you.' },
  shambler: { fam: 'deep', name: 'Shambler',         role: 'minion', hp: 9,  sp: 42, r: 16, drop: 5, from: 1, w: 1,   says: 'A drowned thing that walks straight at you.' },
  croaker:  { fam: 'deep', name: 'Croaker',          role: 'minion', hp: 8,  sp: 50, r: 17, drop: 5, from: 2, w: 0.7, says: 'Keeps its distance and croaks out rings.' },
  harpooner:{ fam: 'deep', name: 'Harpooner',        role: 'minion', hp: 7,  sp: 60, r: 16, drop: 6, from: 2, w: 0.6, says: 'Aims a line at you, then throws.' },
  brine:    { fam: 'deep', name: 'Brine Priest',     role: 'minion', hp: 10, sp: 30, r: 17, drop: 7, from: 3, w: 0.5, says: 'Marks where you stand; the brine there slows you.' },
  polyp:    { fam: 'jelly', name: 'Polyp',           role: 'minion', hp: 6,  sp: 34, r: 16, drop: 4, from: 1, w: 0.8, says: 'Drifts after you and stings on touch, unharmed.' },
  siphon:   { fam: 'jelly', name: 'Siphon Jelly',    role: 'minion', hp: 7,  sp: 30, r: 18, drop: 6, from: 2, w: 0.5, says: 'Drains your void energy while you are near.' },
  lantern:  { fam: 'jelly', name: 'Lantern Jelly',   role: 'minion', hp: 9,  sp: 24, r: 18, drop: 6, from: 2, w: 0.5, says: 'Its light bends your bolts toward it.' },
  manowar:  { fam: 'jelly', name: 'Man-o-War',       role: 'minion', hp: 8,  sp: 28, r: 18, drop: 6, from: 3, w: 0.5, says: 'Trails stinging tendrils far below it.' },
  gaunt:    { fam: 'gaunt', name: 'Faceless Gaunt',  role: 'minion', hp: 6,  sp: 250, r: 16, drop: 5, from: 1, w: 0.8, says: 'Swoops at you in long arcs.' },
  harrier:  { fam: 'gaunt', name: 'Harrier',         role: 'minion', hp: 4,  sp: 520, r: 14, drop: 4, from: 2, w: 0.6, says: 'Marks your row, then strafes across it.' },
  shrieker: { fam: 'gaunt', name: 'Shrieker',        role: 'minion', hp: 7,  sp: 60, r: 16, drop: 5, from: 2, w: 0.5, says: 'Screams a ring that throws you back.' },
  carrion:  { fam: 'gaunt', name: 'Carrion Wing',    role: 'minion', hp: 6,  sp: 120, r: 17, drop: 5, from: 2, w: 0.6, says: 'Flies above you and drops bile bombs.' },
  matron:   { fam: 'mini', name: 'Tide Matron',      role: 'mini', hp: 95,  sp: 30, r: 38, drop: 25, from: 2, w: 1, says: 'Calls the drowned and sings rings of brine.' },
  gauntlord:{ fam: 'mini', name: 'Gaunt Lord',       role: 'mini', hp: 85,  sp: 220, r: 34, drop: 25, from: 2, w: 1, says: 'A great faceless swooper that calls harriers.' },
  drowned:  { fam: 'boss', name: 'The Drowned King', role: 'boss', hp: 250, sp: 30, r: 62, drop: 40, from: 2, nature: 'sloth', says: 'Dreaming in the deep: tidal walls, limbs from the rift walls, the drowned at his call.' },
};

// ---- Falling matter: rocks, wrecks and ruins. shape: rock (jagged), rect or circle; only: never in a wave's pool ----
window.MATTER = {
  asteroid: { fam: 'rock', name: 'Asteroid',         shape: 'rock',   r: 26, hp: 4,   sp: 36,  drop: 3, from: 1, wt: 2,   says: 'Jagged rock; bolts ricochet off it.' },
  big:      { fam: 'rock', name: 'Big Asteroid',     shape: 'rock',   r: 42, hp: 11,  sp: 30,  drop: 5, from: 1, wt: 0.8, onBreak: 'split', says: 'Splits into two chunks.' },
  chunk:    { fam: 'rock', name: 'Chunk',            shape: 'rock',   r: 18, hp: 2.4, sp: 40,  drop: 1, only: 1, says: 'A broken piece of rock.' },
  iron:     { fam: 'rock', name: 'Iron Asteroid',    shape: 'rock',   r: 30, hp: 22,  sp: 24,  drop: 9, from: 2, wt: 0.5, says: 'Dense ore: tough, and rich in void energy.' },
  crystal:  { fam: 'rock', name: 'Crystal Asteroid', shape: 'rock',   r: 24, hp: 5,   sp: 34,  drop: 4, from: 2, wt: 0.6, onBreak: 'shards', says: 'Shatters into six shards that fly as your bolts.' },
  comet:    { fam: 'rock', name: 'Comet',            shape: 'rock',   r: 16, hp: 3,   sp: 230, drop: 6, from: 1, wt: 0.4, warn: 1, says: 'Dangerously fast, with rare materials; a mark at the top warns of it.' },
  cinder:   { fam: 'rock', name: 'Cinder Rock',      shape: 'rock',   r: 24, hp: 4,   sp: 34,  drop: 3, from: 2, wt: 0.6, onBreak: 'blast', says: 'Bursts in fire when broken, burning Ybasogs and you nearby.' },
  plate:    { fam: 'wreck', name: 'Hull Plate',      shape: 'rect', w: 78,  h: 16, hp: 3,   sp: 105, drop: 2,  from: 1, wt: 1.2, tilt: 1, spin: 1.6, drift: 60, says: 'A fast, spinning plate of hull.' },
  slab:     { fam: 'wreck', name: 'Debris Slab',     shape: 'rect', w: 150, h: 22, hp: 6,   sp: 40,  drop: 2,  from: 1, wt: 0.4, says: 'A wide, slow slab that shelters what falls behind it.' },
  hull:     { fam: 'wreck', name: 'Hull Section',    shape: 'rect', w: 190, h: 30, hp: 20,  sp: 22,  drop: 6,  from: 2, wt: 0.4, says: 'A long piece of a dead ship, dark windows in a row.' },
  panel:    { fam: 'wreck', name: 'Solar Panel',     shape: 'rect', w: 120, h: 10, hp: 1.5, sp: 70,  drop: 2,  from: 1, wt: 0.6, tilt: 1, spin: 0.6, onBreak: 'scatter', says: 'Fragile; breaks into a spray of scrap.' },
  scrap:    { fam: 'wreck', name: 'Scrap',           shape: 'rect', w: 26,  h: 8,  hp: 1,   sp: 120, drop: 0,  only: 1, tilt: 1, spin: 3, drift: 140, says: 'Shards of a panel.' },
  core:     { fam: 'wreck', name: 'Engine Core',     shape: 'circle', r: 20,       hp: 6,   sp: 30,  drop: 4,  from: 2, wt: 0.4, onBreak: 'coreblast', says: 'Explodes when broken, hurting only Ybasogs.' },
  cargo:    { fam: 'wreck', name: 'Cargo Pod',       shape: 'rect', w: 50,  h: 34, hp: 8,   sp: 30,  drop: 14, from: 1, wt: 0.3, says: 'Sealed cargo, full of void energy.' },
  spar:     { fam: 'wreck', name: 'Antenna Spar',    shape: 'rect', w: 140, h: 6,  hp: 3,   sp: 60,  drop: 2,  from: 2, wt: 0.5, tilt: 1, spin: 2.4, says: 'A long spinning mast with a dish.' },
  ruin:     { fam: 'ruin', name: 'Ruin Wall',        shape: 'rect', w: 150, h: 46, hp: 26,  sp: 17, drop: 8,  from: 2, wt: 0.9, brood: 1, glyphs: 1, says: 'A carved wall, with two pods sheltering behind it.' },
  pillar:   { fam: 'ruin', name: 'Fallen Pillar',    shape: 'rect', w: 34,  h: 120, hp: 18, sp: 20, drop: 6,  from: 2, wt: 0.6, says: 'A fluted column, falling upright.' },
  arch:     { fam: 'ruin', name: 'Broken Arch',      shape: 'arch', w: 150, h: 92, hp: 12,  sp: 18, drop: 4,  from: 3, wt: 0.4, says: 'Two legs and a lintel: bolts slip through the gap.' },
  idol:     { fam: 'ruin', name: 'Sunken Idol',      shape: 'circle', r: 34,       hp: 30,  sp: 16, drop: 12, from: 3, wt: 0.3, says: 'The carved head of a drowned god, its beard of tentacles.' },
  glyph:    { fam: 'ruin', name: 'Glyph Stone',      shape: 'rect', w: 56,  h: 56, hp: 10,  sp: 22, drop: 5,  from: 2, wt: 0.4, onBreak: 'ward', glyphs: 1, says: 'Breaking its glyph grants you a ward.' },
  shrine:   { fam: 'ruin', name: 'Shrine',           shape: 'rect', w: 70,  h: 50, hp: 12,  sp: 18, drop: 5,  from: 2, wt: 0.4, onBreak: 'heal', says: 'A small lit shrine; breaking it heals you 4.' },
  monolith: { fam: 'ruin', name: 'Monolith',         shape: 'rect', w: 40,  h: 150, hp: 40, sp: 14, drop: 15, from: 3, wt: 0.25, absorb: 1, says: 'A black slab that drinks light: bolts never ricochet off it.' },
};

// ---- The look: a limited palette. Every frame is drawn at half resolution and snapped to these colours,
// each kept for one kind of thing: violet and bile for the hive, crimson rims for its projectiles, cold
// lavender for the player's void, a nature's colour only for that nature, greys and browns for matter.
window.PALETTE = [
  '#000000', '#07040d', '#0e0a18', '#1a1229', '#241a3a', '#3b3350', '#6d6488', '#9d92bb', '#cfc6e6', '#efe6ff', '#ffffff',
  '#b9a6ff', '#7b5bdf', '#d8c8ff', '#4a3a7a',
  '#9b55b0', '#5d2e6e', '#3a1746', '#1a0a20', '#b5c43a', '#6f7a1e', '#d6e25a', '#c06a8a', '#3a0a1c', '#e8e6c0', '#2a1636',
  '#c8e04a', '#8a1030', '#ff4466', '#ff8a8a', '#c94a4a',
  '#6e6156', '#2c2520', '#a89a8a', '#353a44', '#8b95a3', '#45404d', '#8a8296', '#5ac8b9', '#1f3a72', '#5a4a36',
  '#ffd27a', '#e0a040', '#fff0b0', '#d8f4ff',
  '#16302e', '#2e5a55', '#7fd6c8',   // iteration 6: the deep and its jellies
];

// ---- Sigils: permanent upgrades bought with banked void energy at the hub ----
window.SIGILS = {
  vessel:    { name: 'Vessel',    says: '+3 max energy',               costs: [60, 140, 300],  give: { max_energy: 3 } },
  reach:     { name: 'Reach',     says: '+50 px void energy pull',     costs: [50, 120, 250],  give: { magnet: 50 } },
  swiftness: { name: 'Swiftness', says: '+8% speed',                   costs: [80, 180],       give: { speed: 0.08 } },
  ricochet:  { name: 'Ricochet',  says: '+1 ricochet',                 costs: [200, 450],      give: { bounces: 1 } },
  resonance: { name: 'Resonance', says: '+0.1 damage to opposites', costs: [120, 260, 420], give: { opposite_bonus: 0.1 } },
  warding:   { name: 'Warding',   says: 'begin with a ward', costs: [120, 280], give: { ward_start: 1 } },
  recovery:  { name: 'Recovery',  says: '+3 energy between stages',    costs: [90, 220],       give: { stage_heal: 3 } },
  rites:     { name: 'Rites',     says: '+1 rite at each Rift Rest',   costs: [250, 500],      give: { rest_actions: 1 } },
  breadth:   { name: 'Breadth',   says: '+2 cards in the pool',          costs: [100, 250],      give: { deck_max: 2 } },
  favor:     { name: 'Favor',     says: '+2% mythic chance at the Rite', costs: [150, 300, 500], give: { mythic: 0.02 } },
  keeping:   { name: 'Keeping',   says: 'keep +10% void after a fall', costs: [70, 160, 320],  give: { loss_keep: 0.1 } },
  shrine:    { name: 'Shrine',    says: '+1 relic slot',               costs: [400],           give: { relic_slots: 1 } },
  inborn:    { name: 'Inborn',    says: "begin with your form's nature card", costs: [150], give: { inborn: 1 } },
  chosen:    { name: 'Chosen',    says: 'begin with a card chosen at the Gate', costs: [300], give: { chosen: 1 }, need: 'chosen' },
};

// ---- Relics: artifacts of lost civilizations, found in ruins, wrecks and on fallen bosses; equip at the hub ----
window.RELICS = {
  bell:    { name: 'Drowned Bell',       give: { ward_stage: 1 },       says: 'A ward at the start of each stage.',     lore: 'It still rings under the void, for a city no one remembers.' },
  shard:   { name: 'Star-Map Shard',     give: { void_mul: 0.15 },      says: '+15% void energy.',                      lore: 'A sliver of a chart of stars that have since gone out.' },
  lens:    { name: 'Hollow Lens',        give: { telegraph: 0.4 },      says: 'Warnings last 0.4 s longer.',            lore: 'Ground from the eye of something that watched too long.' },
  charm:   { name: 'Chitin Charm',       give: { shell_break: 1 },      says: 'Shells no longer halve your damage.',    lore: 'Carved from a husk; the brood fear their own skins.' },
  ember:   { name: 'First Ember',        give: { status: 0.2 },         says: 'Nature effects last 20% longer.',        lore: 'Taken from the first fire, before Wrath had a name.' },
  tide:    { name: 'Tide Stone',         give: { opposite_bonus: 0.2 }, says: '+0.2 to opposite-nature damage.',        lore: 'Two halves that will not touch, however long you hold them.' },
  lamp:    { name: "Pilgrim's Lamp",     give: { magnet: 80 },          says: '+80 px void energy pull.',               lore: 'Its light falls toward whoever carries it.' },
  crown:   { name: 'Broken Crown',       give: { damage: 0.1 },         says: '+10% bolt damage.',                      lore: 'Three points remain. The rest were eaten.' },
  tooth:   { name: 'Hive Tooth',         give: { fire_rate: 0.1 },      says: '+10% fire rate.',                        lore: 'It hums when the Mother is near.' },
  salt:    { name: 'Vial of Black Salt', give: { max_energy: 3 },       says: '+3 max energy.',                         lore: 'Salt of a sea that dried into the void.' },
  scale:   { name: 'Mirror Scale',       give: { glob_dodge: 0.15 },    says: '15% of globs pass through you.',         lore: 'What strikes it sees itself and hesitates.' },
  nail:    { name: 'Keel Nail',          give: { bounces: 1 },          says: '+1 ricochet.',                           lore: 'From a ship that sailed between the stars and struck one.' },
  orrery:  { name: 'Brass Orrery',       give: { stage_heal: 3 },       says: '+3 energy between stages.',              lore: 'The planets on it no longer exist. It still keeps their time.' },
  idoleye: { name: "Idol's Eye",         give: { relic_find: 0.5 },     says: 'Relics and logs turn up more often.',    lore: 'A green stone that was set in a drowned face. It looks for its kin.' },
  psalm:   { name: 'Sealed Psalm',       give: { rest_actions: 1 },     says: 'One more rite at each Rift Rest.',       lore: 'The Choir sang it once. No one has opened it since.' },
  pearl:   { name: 'Void Pearl',         give: { loss_keep: 0.15 },     says: 'Keep 15% more void energy after a fall.', lore: 'It formed around nothing at all.' },
};

// ---- Logs: what is left of lost civilizations, read in the Journal ----
window.LOGS = {
  l1:  { src: 'wreck', title: 'Flight recorder, the Cassiel',  text: 'Day 212. The stars ahead are going out one by one. Navigation says they are not dying. Something is eating the light.' },
  l2:  { src: 'wreck', title: 'Cargo manifest',                text: 'Hold 3: seed vaults, sealed. Hold 4: void-glass, do not open in darkness. Hold 5: unlisted. Hold 5 is warm.' },
  l3:  { src: 'wreck', title: "Engineer's note",               text: 'The pods on the hull hatched. We vented the deck. The grazers came back through the vents.' },
  l4:  { src: 'wreck', title: 'Last broadcast',                text: 'To anyone: do not follow the fireflies. They show you the way, then they leave you there.' },
  l5:  { src: 'wreck', title: 'Medical log',                   text: 'Crew report the same dream: a choir of three, singing from behind their own faces.' },
  l6:  { src: 'wreck', title: 'Captain, unsent',               text: 'If the hive takes the ship, burn the cargo. If the cargo wakes, burn the ship.' },
  l7:  { src: 'wreck', title: 'Survey drone',                  text: 'Rift wall composition: unknown. Rift wall temperature: rising. Rift wall: breathing.' },
  l8:  { src: 'wreck', title: "Child's drawing",               text: 'A blob of black crayon with two small eyes, holding a light. Written under it: our friend.' },
  l9:  { src: 'ruin',  title: 'Glyph band, translated',        text: 'Seven we cast down, seven we raised up. Where two opposites meet, nothing remains, and nothing is holy.' },
  l10: { src: 'ruin',  title: 'Idol inscription',              text: 'He sleeps in the drowned deep and dreams the rift. When he wakes, the void will be awake too.' },
  l11: { src: 'ruin',  title: 'Shrine tablet',                 text: 'Bring light to the hollow, and the hollow will take its shape from you.' },
  l12: { src: 'ruin',  title: 'Monolith, the black face',      text: 'There is no writing. Only your reflection, and behind it something that is not reflecting you.' },
  l13: { src: 'ruin',  title: 'Wall of names',                 text: 'A thousand names, each scratched out. The last name is unfinished, and the stone is wet.' },
  l14: { src: 'ruin',  title: 'The record of records',         text: 'Those who gather all fourteen will read the Akashic Records, and learn why the void dreamed them.' },
  l15: { src: 'ruin',  title: 'Hymn fragment',                 text: 'The Mother does not hunger. She is hunger, given a body so it could be killed.' },
  l16: { src: 'ruin',  title: 'Pillar graffiti',               text: 'We were here. We were many. We were food.' },
};

// ---- Quests: long goals with rewards, claimed in the Journal ----
window.QUESTS = {
  mother:   { name: 'The Brood Ends',     text: 'Fell the Mother',                 stat: 'fell_mother', n: 1,  reward: { void: 150 } },
  choir:    { name: 'Silence the Hymn',   text: 'Fell the Choir',                  stat: 'fell_choir',  n: 1,  reward: { void: 100 } },
  hollow:   { name: 'Close the Eye',      text: 'Fell the Hollow Eye',             stat: 'fell_hollow', n: 1,  reward: { void: 100 } },
  minis:    { name: 'Wardens and Knights', text: 'Fell 6 mini-bosses',             stat: 'minis',       n: 6,  reward: { void: 120 } },
  natures:  { name: 'Seven and Seven',    text: 'Discover 6 natures',              stat: 'natures',     n: 6,  reward: { void: 200 } },
  logs:     { name: 'Archivist',          text: 'Find 6 logs',                     stat: 'logs',        n: 6,  reward: { void: 100 } },
  relics:   { name: 'Reliquary',          text: 'Find 4 relics',                   stat: 'relics',      n: 4,  reward: { relic: 1 } },
  deep:     { name: 'Deeper',             text: 'Win at depth 1+',        stat: 'deepwin',     n: 1,  reward: { void: 250 } },
  bestiary: { name: 'Naturalist',         text: 'Meet 20 Ybasog kinds',         stat: 'bestiary',    n: 20, reward: { void: 150 } },
  weak:     { name: 'Opposites',          text: 'Slay 100 with opposites', stat: 'weakkills', n: 100, reward: { void: 150 } },
  clean:    { name: 'Untouched',          text: 'Clear a stage unhurt', stat: 'cleanstage', n: 1, reward: { relic: 1 } },
  codex:    { name: 'Codex Keeper',       text: 'Record 12 fusions',  stat: 'codex',       n: 12, reward: { void: 250 } },
};

// ---- Unlocks: what opens over time ----
window.UNLOCKS = {
  rift3:   { name: 'A third rift at the Gate',            stat: 'wins',   n: 1 },
  omens:   { name: 'Darker omens', stat: 'runs', n: 4 },
  slot2:   { name: 'A second relic slot',                 stat: 'relics', n: 3 },
  depth:   { name: 'Deeper rifts',   stat: 'wins',   n: 1 },
  chosen:  { name: 'The Chosen sigil', stat: 'wins',   n: 1 },
};

// ---- Rifts: the adventure is chosen at the Gate. Each rift has an omen, a prevailing nature and a boon ----
window.RIFTS = {
  adj:  ['Drowned', 'Whispering', 'Hollow', 'Weeping', 'Starless', 'Coiled', 'Ashen', 'Gilded', 'Sunken', 'Screaming'],
  noun: ['Trench', 'Choir', 'Maw', 'Reach', 'Deep', 'Vault', 'Spiral', 'Expanse', 'Throat', 'Cradle'],
  boons: {
    relics: { name: 'Relic-rich', says: 'Relics and logs turn up twice as often.', relic: 2 },
    void:   { name: 'Rich veins', says: '+30% void energy.',                        void: 1.3 },
    hive:   { name: 'Teeming',    says: 'One more new kind each wave; more natured Ybasogs.', debut: 1, natured: 0.15 },
    calm:   { name: 'Still',      says: 'No boon, and no added danger.' },
  },
};

// ---- Paths: chosen at the Rift Rest after stages 1 and 2; they shape the next stage ----
window.PATHS = {
  ruins: { name: 'Sunken Path', says: 'More ruins, relics and logs.',           ruins: 2, relic: 1.5 },
  hive:  { name: 'Hive Path',   says: 'More Ybasogs, +25% void energy.',        hive: 1.4, void: 1.25 },
  storm: { name: 'Storm Path',  says: 'More rocks and comets, +1 ricochet.',    rocks: 2, bounces: 1 },
  tide:  { name: 'Tide Path',   says: 'Nature surges every wave; natured drop double.', surge: 1 },
};

