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
  fuse_cost: { value: 20, min: 0, max: 200, step: 5 },           // void energy per fusion at the Forge
  void_refund: { value: 8, min: 0, max: 100, step: 1 },          // void energy back when opposites annul
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
  player_speed: { value: 330, min: 150, max: 600, step: 10 },   // px/s
  band_top: { value: 520, min: 400, max: 640, step: 10 },       // px: top of the band the blob may move in
  max_energy: { value: 20, min: 5, max: 60, step: 1 },          // energy is HP
  iframes: { value: 0.8, min: 0, max: 3, step: 0.1 },           // s of safety after a hit
  hit_damage: { value: 2, min: 0, max: 10, step: 1 },           // energy lost when a Ybasog touches you
  glob_damage: { value: 1, min: 0, max: 10, step: 1 },          // energy lost to a feeder glob
  bump_damage: { value: 1, min: 0, max: 10, step: 1 },          // energy lost when falling matter body-checks you
  leak_enemy: { value: 3, min: 0, max: 10, step: 1 },           // energy lost when a Ybasog reaches the bottom
  leak_matter: { value: 1, min: 0, max: 10, step: 1 },          // energy lost when matter reaches the bottom
  magnet_radius: { value: 120, min: 20, max: 400, step: 10 },   // px: void-energy motes fly to you inside this
  mote_speed: { value: 560, min: 100, max: 1200, step: 20 },    // px/s

  // ---- Bolts ----
  fire_interval: { value: 0.17, min: 0.05, max: 0.6, step: 0.01 }, // s between bolts while firing
  bolt_speed: { value: 640, min: 200, max: 1200, step: 20 },    // px/s
  bolt_bounces: { value: 4, min: 0, max: 12, step: 1 },         // ricochets before a bolt fades
  bolt_damage: { value: 1, min: 0.2, max: 5, step: 0.1 },       // per bolt, before level and fusion
  fused_bonus: { value: 0.3, min: 0, max: 2, step: 0.05 },      // extra damage share for a fused card
  level_bonus: { value: 0.5, min: 0, max: 2, step: 0.05 },      // extra damage share per card level above 1
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

  // ---- Ybasogs ----
  pod_hp: { value: 3, min: 1, max: 30, step: 1 },
  pod_speed: { value: 30, min: 5, max: 200, step: 1 },
  pod_hatch: { value: 9, min: 1, max: 30, step: 0.5 },
  grazer_hp: { value: 4, min: 1, max: 30, step: 1 },
  grazer_speed: { value: 44, min: 5, max: 200, step: 1 },
  grazer_dive_speed: { value: 230, min: 50, max: 600, step: 10 },
  grazer_dive_line: { value: 0.45, min: 0.1, max: 0.8, step: 0.05 },
  feeder_hp: { value: 7, min: 1, max: 40, step: 1 },
  feeder_speed: { value: 26, min: 5, max: 200, step: 1 },
  feeder_spit: { value: 2.4, min: 0.3, max: 8, step: 0.1 },
  glob_speed: { value: 170, min: 40, max: 500, step: 10 },
  mother_hp: { value: 320, min: 20, max: 2000, step: 10 },
  mother_spawn: { value: 3.2, min: 0.5, max: 10, step: 0.1 },
  mother_spread: { value: 3.6, min: 0.5, max: 10, step: 0.1 },
  mother_volley: { value: 7, min: 1, max: 20, step: 1 },

  // ---- Falling matter ----
  asteroid_hp: { value: 4, min: 1, max: 30, step: 1 },
  big_asteroid_hp: { value: 11, min: 1, max: 60, step: 1 },
  asteroid_speed: { value: 36, min: 5, max: 200, step: 1 },
  debris_hp: { value: 3, min: 1, max: 30, step: 1 },
  debris_speed: { value: 105, min: 10, max: 400, step: 5 },
  ruin_hp: { value: 26, min: 1, max: 120, step: 1 },
  ruin_speed: { value: 17, min: 5, max: 100, step: 1 },

  // ---- The run's ramp ----
  stage_hp_scale: { value: 0.35, min: 0, max: 2, step: 0.05 },
  stage_speed_scale: { value: 0.1, min: 0, max: 1, step: 0.02 },
  wave_base_count: { value: 10, min: 2, max: 40, step: 1 },
  wave_count_step: { value: 4, min: 0, max: 20, step: 1 },
  stage_count_step: { value: 5, min: 0, max: 20, step: 1 },
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
                  trait: { regen: 0.06, leak_guard: 1 }, traitText: 'slowly regrows energy, leaks hurt 1 less' },
    chastity:   { name: 'Chastity',   kind: 'virtue', element: 'purity',   color: '#ffffff', opposite: 'lust',       form: 'swan',     formName: 'Swan',
                  trait: { glob_dodge: 0.4 }, traitText: '40% of globs pass through you' },
    temperance: { name: 'Temperance', kind: 'virtue', element: 'wind',     color: '#9ad7ff', opposite: 'gluttony',   form: 'manta',    formName: 'Manta',
                  trait: { speed: 1.3, band: 60 }, traitText: '30% faster, a taller band to roam' },
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
    charity:    { stat: 'sealedwave',  n: 15,   text: 'Clear 15 waves with nothing slipping past' },
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
