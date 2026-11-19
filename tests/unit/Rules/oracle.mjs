// Golden values for the C# rules tests, computed by the browser build's own code (exact lines lifted from
// prototypes/voidwalker/index.html, run in Node). Run: node tests/unit/Rules/oracle.mjs
import { readFileSync } from 'node:fs';
import vm from 'node:vm';
const html = readFileSync('prototypes/voidwalker/index.html', 'utf8').split('\n');
const fn = (name) => { const i = html.findIndex(l => l.startsWith(`function ${name}(`)); let j = i, depth = 0;
  do { for (const ch of html[j]) { if (ch === '{') depth++; else if (ch === '}') depth--; } j++; } while (depth > 0); return html.slice(i, j).join('\n'); };
const line = (start) => html.find(l => l.startsWith(start));
const ctx = { window: {}, Math, Object, Array, JSON, Number, String, Set, console };
vm.createContext(ctx);
vm.runInContext(readFileSync('prototypes/voidwalker/config.js', 'utf8'), ctx);
vm.runInContext(`var CONFIG = window.CONFIG, NATURES = window.NATURES; const tune = new Proxy({}, { get: (_, k) => CONFIG[k].value });
let seed; ${fn('rng')}
${line('const BASE = NATURES.base')}
${line('const SIG = window.SIGILS')}
${line('const RELIC_IDS')}
${line('const NIDS')}
${line('const FUSION_KEYS')}
${line('const keyOf')}
${line('const stackKey')}
${fn('freshMeta')}
${fn('demoMeta')}
let meta = demoMeta();
${line('const owned =')}
${fn('addCards')}
${fn('bonuses')}
${fn('unlocked')}
${fn('rollRifts')}
const hub = {}; function hubSay() {} function saveMeta() {}
${fn('riteDraw')}
seed = 1000003; globalThis.firstRng = [rng(), rng(), rng()];
globalThis.rifts = rollRifts();
riteDraw(10); const focusedBankAfter = meta.bank;
meta.focus = null; riteDraw(10);
globalThis.out = { firstRng, focusedBankAfter, rifts, draw10: hub.reveal.map(r => [r.natures.join('+'), r.level, r.rar, r.isNew]), bank: meta.bank, pity: meta.pity, draws: meta.draws, fusions: meta.fusions };
`, ctx);
console.log(JSON.stringify(ctx.out, null, 1));
