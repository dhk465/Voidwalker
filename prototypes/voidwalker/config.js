// PROTOTYPE - NOT FOR PRODUCTION
// Question: Is building and fusing a nature deck fun when every card is a bolt you watch ricochet?
// Date: 2026-11-18
// Every tuning value lives here, each with value, min, max and step. The game reads them live
// through tune.<name> (index.html); the tweak panel, an add-on, writes them back here.
window.CONFIG = {
  seed: 1, // every gameplay random call reads rng(), seeded from this (or ?seed= in the address)
  volume: { value: 0.5, min: 0, max: 1, step: 0.05 },

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

  // ---- Bolts and the deck ----
  fire_interval: { value: 0.17, min: 0.05, max: 0.6, step: 0.01 }, // s between bolts while firing
  bolt_speed: { value: 640, min: 200, max: 1200, step: 20 },    // px/s
  bolt_bounces: { value: 4, min: 0, max: 12, step: 1 },         // ricochets before a bolt fades
  bolt_damage: { value: 1, min: 0.2, max: 5, step: 0.1 },       // per bolt, before level and fusion
  fused_bonus: { value: 0.3, min: 0, max: 2, step: 0.05 },      // extra damage share for a fused card
  level_bonus: { value: 0.5, min: 0, max: 2, step: 0.05 },      // extra damage share per card level above 1
  reshuffle_time: { value: 0.35, min: 0, max: 2, step: 0.05 },  // s with no fire while the discard is shuffled back
  start_plain: { value: 4, min: 0, max: 10, step: 1 },          // plain void bolts in the first deck
  start_nature: { value: 2, min: 1, max: 6, step: 1 },          // copies of the first nature in the first deck
  min_deck: { value: 3, min: 1, max: 10, step: 1 },             // fusing never thins the deck below this

  // ---- Nature effects ----
  burn_dps: { value: 1.6, min: 0, max: 10, step: 0.1 },         // Wrath: damage per second
  burn_time: { value: 2, min: 0, max: 6, step: 0.1 },           // s
  chill_slow: { value: 0.55, min: 0, max: 0.95, step: 0.05 },   // Patience: share of speed taken away
  chill_time: { value: 2.2, min: 0, max: 6, step: 0.1 },        // s
  pull_radius: { value: 110, min: 20, max: 300, step: 5 },      // Greed: px
  pull_strength: { value: 240, min: 0, max: 800, step: 10 },    // px/s toward the hit
  pull_time: { value: 0.6, min: 0, max: 3, step: 0.1 },         // s
  split_count: { value: 2, min: 1, max: 6, step: 1 },           // Charity: shards on hit
  heal_on_kill: { value: 1, min: 0, max: 5, step: 0.5 },        // Charity: energy back on a kill
  shard_damage: { value: 0.6, min: 0.1, max: 3, step: 0.1 },    // per shard
  well_radius: { value: 95, min: 20, max: 300, step: 5 },       // Hunger: px
  well_time: { value: 1.4, min: 0, max: 5, step: 0.1 },         // s
  well_dps: { value: 2.2, min: 0, max: 10, step: 0.1 },         // Hunger: burn inside the well
  field_radius: { value: 105, min: 20, max: 300, step: 5 },     // Stasis: px
  field_time: { value: 1.8, min: 0, max: 5, step: 0.1 },        // s
  field_slow: { value: 0.85, min: 0, max: 1, step: 0.05 },      // Stasis: share of speed taken away

  // ---- Ybasogs ----
  pod_hp: { value: 3, min: 1, max: 30, step: 1 },
  pod_speed: { value: 30, min: 5, max: 200, step: 1 },          // px/s
  pod_hatch: { value: 9, min: 1, max: 30, step: 0.5 },          // s until a pod hatches a grazer
  grazer_hp: { value: 4, min: 1, max: 30, step: 1 },
  grazer_speed: { value: 44, min: 5, max: 200, step: 1 },       // px/s while drifting
  grazer_dive_speed: { value: 230, min: 50, max: 600, step: 10 }, // px/s once it dives
  grazer_dive_line: { value: 0.45, min: 0.1, max: 0.8, step: 0.05 }, // share of the screen height where it dives
  feeder_hp: { value: 7, min: 1, max: 40, step: 1 },
  feeder_speed: { value: 26, min: 5, max: 200, step: 1 },       // px/s
  feeder_spit: { value: 2.4, min: 0.3, max: 8, step: 0.1 },     // s between globs
  glob_speed: { value: 170, min: 40, max: 500, step: 10 },      // px/s
  mother_hp: { value: 320, min: 20, max: 2000, step: 10 },
  mother_spawn: { value: 3.2, min: 0.5, max: 10, step: 0.1 },   // s between pods
  mother_spread: { value: 3.6, min: 0.5, max: 10, step: 0.1 },  // s between volleys
  mother_volley: { value: 7, min: 1, max: 20, step: 1 },        // globs per volley

  // ---- Falling matter ----
  asteroid_hp: { value: 4, min: 1, max: 30, step: 1 },
  big_asteroid_hp: { value: 11, min: 1, max: 60, step: 1 },
  asteroid_speed: { value: 36, min: 5, max: 200, step: 1 },     // px/s
  debris_hp: { value: 3, min: 1, max: 30, step: 1 },
  debris_speed: { value: 105, min: 10, max: 400, step: 5 },     // px/s
  ruin_hp: { value: 26, min: 1, max: 120, step: 1 },
  ruin_speed: { value: 17, min: 5, max: 100, step: 1 },         // px/s

  // ---- The run's ramp ----
  stage_hp_scale: { value: 0.35, min: 0, max: 2, step: 0.05 },  // extra hp share per stage after the first
  stage_speed_scale: { value: 0.1, min: 0, max: 1, step: 0.02 }, // extra speed share per stage after the first
  wave_base_count: { value: 10, min: 2, max: 40, step: 1 },     // spawns in a stage's first generated wave
  wave_count_step: { value: 4, min: 0, max: 20, step: 1 },      // more spawns per later wave
  stage_count_step: { value: 5, min: 0, max: 20, step: 1 },     // more spawns per later stage

  // ---- Void energy and the Altar ----
  skip_reward: { value: 4, min: 0, max: 20, step: 1 },          // void energy for skipping a pick
  gacha_cost: { value: 6, min: 0, max: 50, step: 1 },           // void energy for the first gacha draw
  gacha_cost_step: { value: 4, min: 0, max: 20, step: 1 },      // each later draw costs this much more
  gacha_fused_chance: { value: 0.12, min: 0, max: 1, step: 0.01 }, // chance a gacha card comes already fused
  gacha_level_chance: { value: 0.18, min: 0, max: 1, step: 0.01 }, // chance a gacha card comes at level 2
  fuse_cost: { value: 3, min: 0, max: 30, step: 1 },            // void energy per fusion
  void_refund: { value: 2, min: 0, max: 20, step: 1 },          // void energy back when opposites annul
  max_level: { value: 3, min: 1, max: 6, step: 1 },             // a card fused with its twin levels up to this
};

// The natures and the fusion recipes: data, not tuning. A recipe's key is its natures' ids,
// sorted and joined with '+'. Three- and four-nature recipes are added here later, the same
// way ('charity+greed+wrath' would be one); a combination with no recipe reads "unstable".
// Any set holding an opposite pair is void, whatever its size.
window.NATURES = {
  base: {
    wrath:    { name: 'Wrath',    kind: 'sin',    element: 'fire',    color: '#ff5a36', opposite: 'patience', form: 'dragon' },
    patience: { name: 'Patience', kind: 'virtue', element: 'ice',     color: '#9fe8ff', opposite: 'wrath',    form: 'humanoid' },
    greed:    { name: 'Greed',    kind: 'sin',    element: 'gravity', color: '#e8b84a', opposite: 'charity',  form: 'rat' },
    charity:  { name: 'Charity',  kind: 'virtue', element: 'grace',   color: '#bfffd2', opposite: 'greed',    form: 'bird' },
  },
  recipes: {
    '':                 { name: 'Void Bolt', color: '#b9a6ff', effects: [],                         says: 'A plain bolt of void.' },
    'wrath':            { name: 'Wrath',     color: '#ff5a36', effects: ['burn'],                   says: 'Burns what it hits.' },
    'patience':         { name: 'Patience',  color: '#9fe8ff', effects: ['chill'],                  says: 'Freezes: slows what it hits.' },
    'greed':            { name: 'Greed',     color: '#e8b84a', effects: ['pull'],                   says: 'Pulls nearby Ybasogs to the hit.' },
    'charity':          { name: 'Charity',   color: '#bfffd2', effects: ['split', 'heal'],          says: 'Splits on hit; a kill heals you.' },
    'greed+wrath':      { name: 'Hunger',    color: '#ff9a2e', effects: ['well', 'burn'],           says: 'A burning gravity well.' },
    'charity+wrath':    { name: 'Zeal',      color: '#ffb59a', effects: ['split3', 'burn', 'heal'], says: 'Bursts into three burning shards.' },
    'greed+patience':   { name: 'Stasis',    color: '#d4e48a', effects: ['field', 'chill'],         says: 'A freezing field that holds them.' },
    'charity+patience': { name: 'Mercy',     color: '#c8fff4', effects: ['shatter', 'chill', 'heal'], says: 'Shatters into ice shards; kills heal.' },
  },
};
