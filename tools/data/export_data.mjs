// Exports the browser build's data tables (prototypes/voidwalker/config.js) to src/data/game_data.json for the
// Godot port, so the tables have one source. Recipe colours are resolved the way index.html does: a base takes its
// nature's colour, a fusion with no colour of its own blends its natures' colours.
// Run from the project root: node tools/data/export_data.mjs
import { readFileSync, writeFileSync } from 'node:fs';
import vm from 'node:vm';

const window = {};
vm.runInNewContext(readFileSync('prototypes/voidwalker/config.js', 'utf8'), { window });

const hexMix = (a, b) => {
  const p = h => [1, 3, 5].map(i => parseInt(h.slice(i, i + 2), 16));
  const x = p(a), y = p(b);
  return '#' + x.map((v, i) => Math.round((v + y[i]) / 2).toString(16).padStart(2, '0')).join('');
};
const N = window.NATURES, BASE = N.base;
for (const [k, r] of Object.entries(N.recipes)) {
  if (r.color) continue;
  const ns = k.split('+');
  r.color = ns.length === 1 ? BASE[ns[0]].color : ns.slice(1).reduce((c, n) => hexMix(c, BASE[n].color), BASE[ns[0]].color);
}
const tune = {};
for (const [k, v] of Object.entries(window.CONFIG)) if (v && typeof v === 'object' && 'value' in v) tune[k] = v.value;

const out = {
  source: 'prototypes/voidwalker/config.js (exported by tools/data/export_data.mjs; do not edit by hand)',
  seed: window.CONFIG.seed,
  tune,
  natures: BASE,
  recipes: N.recipes,
  discover: N.discover,
  omens: N.omens,
  sigils: window.SIGILS,
  relics: window.RELICS,
  logs: window.LOGS,
  quests: window.QUESTS,
  unlocks: window.UNLOCKS,
  rifts: window.RIFTS,
  ybasogs: Object.fromEntries(Object.entries(window.YBASOGS).map(([k, y]) => [k, { name: y.name, says: y.says, role: y.role, fam: y.fam }])),
  matter: Object.fromEntries(Object.entries(window.MATTER).filter(([, m]) => !m.only).map(([k, m]) => [k, { name: m.name, says: m.says }])),
  palette: window.PALETTE,
  // the adventure's full stats (life, speed, radius, drops, behaviour numbers), matter pieces included
  adventure: { ybasogs: window.YBASOGS, matter: window.MATTER },
};
writeFileSync('src/data/game_data.json', JSON.stringify(out, null, 1) + '\n');
console.log(`natures ${Object.keys(BASE).length}, recipes ${Object.keys(N.recipes).length}, sigils ${Object.keys(out.sigils).length}, relics ${Object.keys(out.relics).length}, logs ${Object.keys(out.logs).length}, quests ${Object.keys(out.quests).length}, ybasogs ${Object.keys(out.ybasogs).length}, matter ${Object.keys(out.matter).length}, tune ${Object.keys(tune).length}`);
