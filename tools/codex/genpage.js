// Builds the Voidwalker Codex, the game's reference as one web page, from the browser game's config.js and drawing
// code (the data sections: natures, fusions, effects, discoveries, omens, Ybasogs, tuning) and from
// design/voidwalker-reference.md (the written sections, the migration tracker, the history). Each section shows its
// source, Godot port or browser build, from the reference's "*Source: ..." line; section 18 tracks the migration.
// gendoc.js is read only for its effects glossary (FX).
// Run: node tools/codex/genpage.js [repo-root]   -> build/codex/voidwalker-codex.html (gitignored)
// Serve and open: bash tools/codex/serve.sh
const fs = require('fs'), path = require('path');
const here = __dirname, root = path.resolve(process.argv[2] || path.join(here, '..', '..'));
global.window = {};
require(path.join(root, 'prototypes/voidwalker/config.js'));
const C = window.CONFIG, N = window.NATURES, v = k => C[k].value;
const B = N.base, R = N.recipes, D = N.discover, O = N.omens, ids = Object.keys(B);
const pct = x => Math.round(x * 100) + '%', r2 = x => Math.round(x * 100) / 100;
// the effect glossary, shared with gendoc.js
const g = fs.readFileSync(path.join(here, 'gendoc.js'), 'utf8');
const FX = eval('(' + g.slice(g.indexOf('const FX = {') + 11, g.indexOf('\n};', g.indexOf('const FX = {')) + 2) + ')');
// colours as the game computes them
const hexMix = (a, b) => { const p = h => [1, 3, 5].map(i => parseInt(h.slice(i, i + 2), 16)); const x = p(a), y = p(b); return '#' + x.map((q, i) => Math.round((q + y[i]) / 2).toString(16).padStart(2, '0')).join(''); };
for (const [k, r] of Object.entries(R)) if (!r.color) { const ns = k.split('+'); r.color = ns.length === 1 ? B[ns[0]].color : hexMix(B[ns[0]].color, B[ns[1]].color); }
const esc = s => String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
const dmg = (fused, lvl, stone) => r2(v('bolt_damage') * (1 + v('level_bonus') * (lvl - 1)) * (fused ? 1 + v('fused_bonus') : 1) * (stone ? v('stone_mult') : 1));
const dmgLine = k => { const r = R[k], f = k.split('+').length === 2, s = r.fx.includes('stone'); return [1, 2, 3].map(l => dmg(f, l, s)).join(' · '); };
const fxList = r => r.fx.map(f => `<li><code>${f}</code> ${esc(FX[f] || f)}</li>`).join('') + (r.shardFx && r.shardFx.length ? `<li><code>shards</code> carry ${r.shardFx.join(', ')}</li>` : '');
const chip = n => `<span class="chip" style="--c:${B[n].color}">${B[n].name}</span>`;
const fusions = Object.keys(R).filter(k => k.split('+').length === 2);
const groupOf = k => { const [a, b] = k.split('+').map(x => B[x].kind); return a === b ? a : 'mixed'; };

// the game's drawing code, lifted as it is
const game = fs.readFileSync(path.join(root, 'prototypes/voidwalker/index.html'), 'utf8');
const cut = (from, to) => { const a = game.indexOf(from), b = game.indexOf(to, a); if (a < 0 || b < 0) throw new Error('missing ' + from); return game.slice(a, b); };
const drawCode = [cut('function glowStroke', '\nfunction drawPod'), cut('function drawPod', '\nfunction drawBolt'), cut('// ---- The hive, drawn', '\nfunction drawHazards'), cut('function figureMatter', '\n// ---- Nature sigils'),cut('function sigil(', '\nfunction roundRect')].join('\n');

// markdown (history.md) to html: headings, tables, lists, paragraphs, inline code, bold, italics
function md(src) {
  const inl = s => esc(s).replace(/`([^`]+)`/g, '<code>$1</code>').replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>').replace(/\*([^*]+)\*/g, '<em>$1</em>');
  const out = [], L = src.split(/\r?\n/); let i = 0;
  while (i < L.length) {
    const l = L[i];
    if (/^#{2,4} /.test(l)) { const h = l.match(/^#+/)[0].length; const t = l.replace(/^#+ /, ''); if (h > 2) out.push(`<h${h} >${inl(t)}</h${h}>`); i++; continue; }
    if (l.startsWith('|')) { const rows = []; while (i < L.length && L[i].startsWith('|')) rows.push(L[i++]); const cells = r => r.slice(1, -1).split('|').map(c => c.trim());
      out.push('<div class="scroll"><table><thead><tr>' + cells(rows[0]).map(c => `<th>${inl(c)}</th>`).join('') + '</tr></thead><tbody>' + rows.slice(2).map(r => '<tr>' + cells(r).map(c => `<td>${inl(c)}</td>`).join('') + '</tr>').join('') + '</tbody></table></div>'); continue; }
    if (l.startsWith('- ')) { const items = []; while (i < L.length && L[i].startsWith('- ')) items.push(L[i++].slice(2)); out.push('<ul>' + items.map(x => `<li>${inl(x)}</li>`).join('') + '</ul>'); continue; }
    if (!l.trim()) { i++; continue; }
    out.push(`<p>${inl(l)}</p>`); i++;
  }
  return out.join('\n');
}
const ref = fs.readFileSync(path.join(root, 'design/voidwalker-reference.md'), 'utf8').replace(/\r\n/g, '\n');
// one numbered section of the reference, without its own heading
const sec = n => { const a = ref.indexOf(`\n## ${n}. `); if (a < 0) throw new Error('reference has no section ' + n); const b = ref.indexOf('\n## ', a + 1); const body = ref.slice(a + 1, b < 0 ? undefined : b); return body.slice(body.indexOf('\n') + 1); };
const histA = sec(17), histB = sec(19), portMd = sec(18);
// where each page section comes from: the reference section it follows, and whether that section is migrated
const PAGE_SECTIONS = { overview: 1, adventure: 3, altar: 4, natures: 6, forms: 7, fusions: 8, effects: 9, discoveries: 10, omens: 11, bestiary: 12, tuning: 16 };
const isGodot = n => /^\*Source: Godot port/m.test(sec(n));
const badge = id => id in PAGE_SECTIONS ? (isGodot(PAGE_SECTIONS[id]) ? '<span class="src godot">Source: Godot port</span>' : '<span class="src web">Source: browser build</span>') : '';
// the lobby as the port draws it now (the latest lobby evidence shot)
const shotPath = path.join(root, 'production/qa/evidence/lobby-islands/rocks-hub-640x360-x2.png');
const lobbyShot = fs.existsSync(shotPath) ? `<figure class="shot"><img src="data:image/png;base64,${fs.readFileSync(shotPath).toString('base64')}" alt="The Rift Altar lobby in the Godot port: the altar and six stations on floating islands in deep space" width="1280" height="720"><figcaption>The Rift Altar in the Godot port (640×360, shown at 2×).</figcaption></figure>` : '';
const { execSync } = require('child_process');
const commit = (() => { try { return execSync('git rev-parse --short HEAD', { cwd: root }).toString().trim(); } catch (e) { return 'unknown'; } })();
const today = new Date().toISOString().slice(0, 10);

const S = [];
const P = s => S.push(s);
// ---------- head
P(`<title>Voidwalker Codex</title>
<meta name="claude:viewport-fit-sides" content="cover">
<link rel="preconnect" href="https://fonts.googleapis.com"><link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Cormorant+SC:wght@500;700&family=Karla:ital,wght@0,400;0,600;1,400&family=IBM+Plex+Mono:wght@400;500&display=swap">
<style>
/* Layout: a field manual from inside the rift. One dark reading column, a sticky section rail, data in mono, every nature in its own glow. Single dark look, chosen. */
:root {
  --void: #07050d; --well: #0f0b19; --rim: #241c38; --ink: #e9e2f7; --dim: #a69bc2; --faint: #6f6690;
  --accent: #b9a6ff; --gold: #ffd27a;
  --display: "Cormorant SC", "Cormorant Garamond", Georgia, serif;
  --body: "Karla", "Segoe UI", system-ui, sans-serif;
  --mono: "IBM Plex Mono", ui-monospace, Consolas, monospace;
  color-scheme: dark;
}
* { box-sizing: border-box }
html { scroll-behavior: smooth; scroll-padding-top: calc(64px + env(safe-area-inset-top, 0px)) }
@media (prefers-reduced-motion: reduce) { html { scroll-behavior: auto } }
body { background: var(--void); color: var(--ink); font: 16px/1.6 var(--body); margin: 0 }
body::before { content: ""; position: fixed; inset: 0 0 auto; height: env(safe-area-inset-top, 0px); z-index: 10; background: var(--void) }
.wrap { max-width: 1120px; margin: 0 auto; padding-left: max(16px, env(safe-area-inset-left, 0px)); padding-right: max(16px, env(safe-area-inset-right, 0px)); padding-block: 0 64px }
canvas.stars { position: fixed; inset: 0; width: 100%; height: 100%; z-index: -1; pointer-events: none }
header.hero { padding-block: 56px 28px; display: grid; grid-template-columns: auto 1fr; gap: 28px; align-items: center }
header.hero canvas { width: 120px; height: 120px }
h1 { font: 700 clamp(48px, 8vw, 84px)/0.95 var(--display); letter-spacing: 0.02em; margin: 0; text-wrap: balance }
.lede { color: var(--dim); max-width: 62ch; margin: 12px 0 0; font-size: 18px }
.counts { display: flex; flex-wrap: wrap; gap: 8px 22px; margin-top: 18px; font: 500 14px var(--mono); color: var(--dim) }
.counts b { color: var(--ink); font-weight: 500; font-size: 18px }
nav.rail { position: sticky; top: env(safe-area-inset-top, 0px); z-index: 5; background: color-mix(in srgb, var(--void) 92%, transparent); backdrop-filter: blur(6px); border-block: 1px solid var(--rim); overflow-x: auto; scrollbar-width: thin }
nav.rail ul { display: flex; gap: 2px; list-style: none; margin: 0 auto; max-width: 1120px; padding-block: 0; padding-left: max(16px, env(safe-area-inset-left, 0px)); padding-right: max(16px, env(safe-area-inset-right, 0px)) }
nav.rail a { display: block; padding: 12px 10px; color: var(--dim); text-decoration: none; font: 600 13px var(--body); letter-spacing: 0.06em; text-transform: uppercase; white-space: nowrap; border-bottom: 2px solid transparent }
nav.rail a:hover, nav.rail a:focus-visible { color: var(--ink); border-bottom-color: var(--accent); outline: none }
section { padding-top: 56px }
h2 { font: 700 clamp(30px, 4.4vw, 44px)/1.05 var(--display); margin: 0 0 6px; letter-spacing: 0.02em; text-wrap: balance }
h2 small { display: block; font: 600 12px var(--body); letter-spacing: 0.14em; text-transform: uppercase; color: var(--faint); margin-bottom: 8px }
h3 { font: 700 24px/1.2 var(--display); margin: 28px 0 8px; letter-spacing: 0.02em }
h4 { font: 600 15px var(--body); margin: 22px 0 6px; color: var(--accent) }
p, li { max-width: 72ch } p { margin: 8px 0 }
.note { color: var(--dim) }
a { color: var(--accent) }
code { font: 0.86em var(--mono); color: var(--gold); background: var(--well); padding: 1px 5px; border-radius: 4px }
.scroll { overflow-x: auto; margin: 12px 0; border: 1px solid var(--rim); border-radius: 8px }
table { border-collapse: collapse; width: 100%; font-size: 14.5px; font-variant-numeric: tabular-nums }
th, td { text-align: left; vertical-align: top; padding: 9px 12px; border-bottom: 1px solid var(--rim) }
th { font: 600 12px var(--body); letter-spacing: 0.08em; text-transform: uppercase; color: var(--faint); background: var(--well); position: sticky; top: 0 }
tbody tr:last-child td { border-bottom: 0 }
td.num, .mono { font-family: var(--mono); font-size: 13.5px; white-space: nowrap }
.chip { display: inline-block; font: 600 12.5px var(--body); color: var(--c); border: 1px solid color-mix(in srgb, var(--c) 55%, transparent); padding: 1px 8px; border-radius: 999px; white-space: nowrap; margin: 1px 2px 1px 0 }
.grid { display: grid; gap: 14px; grid-template-columns: repeat(auto-fill, minmax(min(100%, 250px), 1fr)) }
.nat { background: var(--well); border: 1px solid var(--rim); border-top: 2px solid var(--c); border-radius: 10px; padding: 16px; display: grid; gap: 6px; min-width: 0 }
.nat header { display: flex; gap: 12px; align-items: center }
.nat canvas { width: 44px; height: 44px; flex: none }
.nat h3 { margin: 0; color: var(--c); font-size: 26px; line-height: 1 }
.nat .meta { font: 500 12px var(--mono); color: var(--faint); letter-spacing: 0.04em; text-transform: uppercase }
.nat ul { margin: 0; padding-left: 18px; font-size: 14px; color: var(--dim) }
.nat .dmg { font: 13px var(--mono); color: var(--dim) }
.form { text-align: center; padding: 18px 12px 14px }
.form canvas { width: 96px; height: 96px; margin: 0 auto }
.form h3 { font-size: 24px; margin: 4px 0 0 }
.form .t { font-size: 14px; color: var(--ink) }
.form .v { font: 12px var(--mono); color: var(--faint) }
.matrix-wrap { display: grid; grid-template-columns: minmax(0, auto) minmax(0, 1fr); gap: 22px; align-items: start; margin-top: 16px }
@media (max-width: 860px) { .matrix-wrap { grid-template-columns: minmax(0, 1fr) } }
table.matrix { width: auto; border-collapse: separate; border-spacing: 3px; font-size: 11px }
table.matrix th { position: static; background: none; padding: 0 2px; text-align: center; font: 600 10.5px var(--mono); letter-spacing: 0; text-transform: none }
table.matrix th.row { text-align: right; padding-right: 6px; white-space: nowrap }
table.matrix td { padding: 0; border: 0; width: 30px; height: 30px }
.cell { all: unset; box-sizing: border-box; display: grid; place-items: center; width: 30px; height: 30px; border-radius: 5px; cursor: pointer; background: color-mix(in srgb, var(--c) 22%, var(--well)); border: 1px solid color-mix(in srgb, var(--c) 60%, transparent); font: 600 10px var(--mono); color: var(--c) }
.cell:hover, .cell:focus-visible, .cell[aria-pressed="true"] { background: var(--c); color: var(--void); outline: none; box-shadow: 0 0 12px var(--c) }
.cell.void { background: #000; border-color: var(--rim); color: var(--faint); cursor: default; box-shadow: none }
.cell.same { background: none; border: 1px dashed var(--rim); color: var(--faint); cursor: default }
.detail { background: var(--well); border: 1px solid var(--rim); border-left: 3px solid var(--c, var(--accent)); border-radius: 10px; padding: 18px; position: sticky; top: calc(64px + env(safe-area-inset-top, 0px)); min-width: 0 }
.detail h3 { margin: 0; color: var(--c, var(--ink)); font-size: 32px }
.detail ul { padding-left: 18px; color: var(--dim); font-size: 14.5px }
.tools { display: flex; flex-wrap: wrap; gap: 10px; align-items: center; margin: 22px 0 6px }
.tools input, .tools select { font: 15px var(--body); color: var(--ink); background: var(--well); border: 1px solid var(--rim); border-radius: 8px; padding: 8px 12px; min-width: 0 }
.tools input { flex: 1 1 240px } .tools input:focus, .tools select:focus { outline: 2px solid var(--accent); outline-offset: 1px }
.tools output { font: 13px var(--mono); color: var(--faint) }
td.fname { font: 700 19px/1.1 var(--display); color: var(--c); white-space: nowrap }
td.fx ul { margin: 0; padding-left: 16px; color: var(--dim); font-size: 13.5px }
details { margin-top: 14px; border: 1px solid var(--rim); border-radius: 10px; background: var(--well) }
details summary { cursor: pointer; padding: 12px 16px; font-weight: 600 }
details .scroll { margin: 0; border: 0; border-top: 1px solid var(--rim); border-radius: 0 }
.two { display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 420px), 1fr)); gap: 0 28px }
.two > * { min-width: 0 }
.src { display: inline-block; font: 600 11.5px var(--body); letter-spacing: 0.08em; text-transform: uppercase; padding: 2px 10px; border-radius: 999px; margin: 2px 0 10px; border: 1px solid }
.src.godot { color: #7fd6c8; border-color: color-mix(in srgb, #7fd6c8 55%, transparent) }
.src.web { color: var(--faint); border-color: var(--rim) }
figure.shot { margin: 16px 0 20px } figure.shot img { width: 100%; height: auto; image-rendering: pixelated; border: 1px solid var(--rim); border-radius: 10px; display: block }
figure.shot figcaption { color: var(--faint); font-size: 13px; margin-top: 6px }
footer { margin-top: 64px; color: var(--faint); font-size: 13px; border-top: 1px solid var(--rim); padding-top: 16px }
</style>`);
// ---------- body
P(`<canvas class="stars" id="stars" aria-hidden="true"></canvas>`);
P(`<div class="wrap"><header class="hero"><canvas id="heroBlob" width="240" height="240" aria-hidden="true"></canvas><div>
<h1>Voidwalker Codex</h1>
<p class="lede">The complete reference for Voidwalker: every nature, fusion, form and foe, the Rift Altar's economy, and how the game was made. Numbers come straight from the browser game's <code>config.js</code>; the Godot port has its own section.</p>
<div class="counts"><span><b>${ids.length}</b> natures</span><span><b>${fusions.length}</b> fusions</span><span><b>${ids.length + 1}</b> forms</span><span><b>${Object.keys(window.YBASOGS).length}</b> Ybasogs</span><span><b>${Object.keys(window.MATTER).length}</b> kinds of matter</span><span><b>${Object.keys(O).length}</b> omens</span></div>
</div></header></div>`);
const NAV = [['overview', 'Overview'], ['adventure', 'Adventure'], ['altar', 'Altar'], ['natures', 'Natures'], ['forms', 'Forms'], ['fusions', 'Fusions'], ['effects', 'Effects'], ['discoveries', 'Discoveries'], ['omens', 'Omens'], ['bestiary', 'Bestiary'], ['tuning', 'Tuning'], ['port', 'Migration'], ['history', 'History'], ['next', 'Next']];
P(`<nav class="rail" aria-label="Sections"><ul>${NAV.map(([h, t]) => `<li><a href="#${h}">${t}</a></li>`).join('')}</ul></nav><div class="wrap">`);

P(`<section id="overview"><h2><small>The game in one page</small>Overview</h2>${md(sec(1))}<h3>Controls</h3>${md(sec(2))}</section>`);
P(`<section id="adventure"><h2><small>Inside the rift</small>The adventure</h2>${md(sec(3))}</section>`);
P(`<section id="altar"><h2><small>Between adventures</small>The Rift Altar</h2>${lobbyShot}${md(sec(4))}<h3>The economy</h3>${md(sec(5))}<h3>The save</h3>${md(sec(15))}</section>`);

const pairs = [...new Set(ids.map(i => [i, B[i].opposite].sort().join('|')))].map(p => p.split('|'));
P(`<section id="natures"><h2><small>Seven sins, seven virtues</small>The fourteen natures</h2>
<p>Opposites annul when fused: ${pairs.map(([a, b]) => `${chip(a)}${chip(b)}`).join(' · ')}.</p><div class="grid">
${ids.map(i => `<article class="nat" style="--c:${B[i].color}"><header><canvas data-sigil="${i}" width="88" height="88" aria-hidden="true"></canvas><div><h3>${B[i].name}</h3><div class="meta">${B[i].kind} · ${B[i].element} · opposes ${B[B[i].opposite].name}</div></div></header>
<p>${esc(R[i].says)}</p><ul>${fxList(R[i])}</ul><div class="dmg">Damage I · II · III: ${dmgLine(i)}</div></article>`).join('\n')}
<article class="nat" style="--c:${R[''].color}"><header><canvas data-sigil="" width="88" height="88" aria-hidden="true"></canvas><div><h3>Void Bolt</h3><div class="meta">plain · void · no opposite</div></div></header><p>${R[''].says}</p><div class="dmg">Damage I · II · III: ${[1, 2, 3].map(l => dmg(false, l, false)).join(' · ')}</div></article>
</div></section>`);

P(`<section id="forms"><h2><small>What the deck makes of you</small>The fifteen forms</h2>
<p>The nature most present in the deck (card levels count; ties go to your innate nature) shapes the blob for the whole adventure. A deck of Void Bolts stays shapeless.</p><div class="grid">
<article class="nat form" style="--c:#b9a6ff"><canvas data-form="blob" data-color="#b9a6ff" width="192" height="192" aria-hidden="true"></canvas><h3>Shapeless</h3><div class="t">No trait</div><div class="v">a deck of Void Bolts</div></article>
${ids.map(i => `<article class="nat form" style="--c:${B[i].color}"><canvas data-form="${B[i].form}" data-color="${B[i].color}" width="192" height="192" aria-hidden="true"></canvas><h3>${B[i].formName}</h3><div class="meta">shaped by ${B[i].name}</div><div class="t">${esc(B[i].traitText)}</div><div class="v">${Object.entries(B[i].trait).map(([k, x]) => `${k} ${x}`).join(' · ')}</div></article>`).join('\n')}
</div>
<p class="note">Trait keys: <code>damage</code> and <code>fire_rate</code> multiply; <code>max_energy</code> adds; <code>magnet</code> adds px of pull; <code>mote_bonus</code> is the chance of an extra void energy per mote; <code>bite</code> adds to bites; <code>bounces</code> adds ricochets; <code>status</code> stretches effect durations; <code>contact_charm</code> is the chance a biting Ybasog is charmed instead; <code>speed</code> multiplies movement; <code>aura</code> slows Ybasogs within 160 px; <code>armor</code> reduces each hit (never below 1); <code>wave_heal</code> heals after each wave; <code>iframes</code> adds safety; <code>quick_shuffle</code> removes the reshuffle pause; <code>regen</code> is energy per second; <code>leak_guard</code> softens leaks; <code>glob_dodge</code> lets globs pass through; <code>band</code> raises the roaming band.</p></section>`);

// fusions: matrix + table
const ab = n => B[n].name.slice(0, 3);
const cellLabel = k => R[k].name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase() === R[k].name.slice(0, 2).toUpperCase() ? R[k].name.slice(0, 2) : R[k].name.slice(0, 2);
P(`<section id="fusions"><h2><small>Two natures become one</small>The 84 fusions</h2>
<p>Every fused card deals ×${1 + v('fused_bonus')} damage and carries rings on its bolt. Pick a cell to read a fusion; black cells are opposites, which annul. 21 sin + sin, 21 virtue + virtue, 42 sin + virtue.</p>
<div class="matrix-wrap"><div class="scroll" style="border:0;margin:0"><table class="matrix" aria-label="Fusion grid"><thead><tr><th></th>${ids.map(n => `<th style="color:${B[n].color}" title="${B[n].name}">${ab(n)}</th>`).join('')}</tr></thead><tbody>
${ids.map(a => `<tr><th class="row" style="color:${B[a].color}">${B[a].name}</th>${ids.map(b => { if (a === b) return `<td><span class="cell same" title="Twins: a level up">+1</span></td>`; if (B[a].opposite === b) return `<td><span class="cell void" title="${B[a].name} and ${B[b].name} annul">·</span></td>`; const k = [a, b].sort().join('+'); return `<td><button class="cell" style="--c:${R[k].color}" data-k="${k}" aria-pressed="false" title="${R[k].name}">${R[k].name.slice(0, 2)}</button></td>`; }).join('')}</tr>`).join('\n')}
</tbody></table></div>
<aside class="detail" id="detail" aria-live="polite"></aside></div>
<div class="tools"><input id="q" type="search" placeholder="Search fusions, natures or effects (e.g. black hole, Wrath, heal)" aria-label="Search fusions"><select id="grp" aria-label="Group"><option value="">All 84</option><option value="sin">Sin + sin</option><option value="virtue">Virtue + virtue</option><option value="mixed">Sin + virtue</option></select><output id="cnt">84 shown</output></div>
<div class="scroll"><table id="ftable"><thead><tr><th>Fusion</th><th>Natures</th><th>What it does</th><th>Exact effects</th><th>Damage I · II · III</th></tr></thead><tbody>
${fusions.sort((a, b) => R[a].name.localeCompare(R[b].name)).map(k => `<tr data-g="${groupOf(k)}" data-s="${esc((R[k].name + ' ' + k.split('+').map(n => B[n].name).join(' ') + ' ' + R[k].says + ' ' + R[k].fx.map(f => f + ' ' + (FX[f] || '')).join(' ')).toLowerCase())}"><td class="fname" style="--c:${R[k].color}">${R[k].name}</td><td>${k.split('+').map(chip).join('')}</td><td>${esc(R[k].says)}</td><td class="fx"><ul>${fxList(R[k])}</ul></td><td class="num">${dmgLine(k)}</td></tr>`).join('\n')}
</tbody></table></div>
<p class="note">Rules: the union of both cards' natures is looked up in the recipe table (natures sorted and joined with "+"). An opposite pair anywhere in the set is void. Twins rise a level. A set with no recipe is "unstable, not yet known" and nothing is spent. Three-nature (364) and four-nature (1,001) recipes are planned as more rows in the same table.</p></section>`);

P(`<section id="effects"><h2><small>Every effect id</small>Effects glossary</h2>
<p>Fusions and natures are built from these effects. Values are the current tuning.</p>
<div class="scroll"><table><thead><tr><th>Effect</th><th>Meaning</th><th>Used by</th></tr></thead><tbody>
${Object.entries(FX).map(([k, t]) => { const users = Object.keys(R).filter(r => r && (R[r].fx.includes(k) || (R[r].shardFx || []).includes(k))).map(r => R[r].name); return `<tr><td class="mono">${k}</td><td>${esc(t)}</td><td>${users.length} · <span class="note">${users.slice(0, 6).join(', ')}${users.length > 6 ? '…' : ''}</span></td></tr>`; }).join('\n')}
</tbody></table></div>
<p class="note">Status marks on targets: burn is a red arc, chill a blue arc, venom green drops (one per stack), sleep a "z", charm a pink heart, doom a filling pink ring, tether a dashed gold line.</p></section>`);

P(`<section id="discoveries"><h2><small>Achievements</small>Discoveries</h2>
<p>Your innate nature is known from the start. Every other nature is discovered when its counter reaches the goal; counters add up across all adventures and are saved. A discovery gives ${v('discovery_copies')} cards and ${v('discovery_bonus')} void energy.</p>
<div class="scroll"><table><thead><tr><th>Nature</th><th>Achievement</th><th>Counter</th></tr></thead><tbody>
${ids.map(i => `<tr><td>${chip(i)}</td><td>${esc(D[i].text)}</td><td class="mono">${D[i].stat} ≥ ${D[i].n}</td></tr>`).join('')}
</tbody></table></div></section>`);

P(`<section id="omens"><h2><small>Reading the rift ahead</small>Omens and depth</h2>
<p>A new omen is rolled after every adventure and shows at the Altar, so the deck can be prepared for it.</p>
<div class="scroll"><table><thead><tr><th>Omen</th><th>Effect</th><th>Data</th></tr></thead><tbody>
${Object.values(O).map(x => `<tr><td><strong>${x.name}</strong></td><td>${esc(x.says)}</td><td class="mono">${Object.entries(x).filter(([a]) => a !== 'name' && a !== 'says').map(([a, b]) => `${a} ${b}`).join(', ') || 'none'}</td></tr>`).join('')}
</tbody></table></div>
<p><strong>Depth:</strong> each win opens the next depth. Each depth adds ${pct(v('depth_hp'))} Ybasog life and ${pct(v('depth_void'))} void energy; choose any depth up to the deepest opened.</p></section>`);

const YBD = window.YBASOGS, MTD = window.MATTER;
const famName = { pod: 'Pods', grazer: 'Grazers', feeder: 'Feeders', eye: 'Eyes', devourer: 'Devourers', deacon: 'Deacons', mite: 'Mites', crawler: 'Wall crawlers', reacher: 'Reachers', deep: 'The deep', jelly: 'Jellies', gaunt: 'Gaunts', mini: 'Mini-bosses', boss: 'Bosses' };
const fams = [...new Set(Object.values(YBD).map(d => d.fam))];
P(`<section id="bestiary"><h2><small>What falls into the rift</small>Bestiary</h2>
<p>The hive draws on Lovecraftian horror, in original creatures: ${Object.keys(YBD).length} Ybasog kinds in families of variants, with mini-bosses and bosses. Each card is drawn with the game's own code.</p>
${fams.map(f => `<h3>${famName[f] || f}</h3><div class="grid">${Object.entries(YBD).filter(([, d]) => d.fam === f).map(([k, d]) => `<article class="nat form" style="--c:${d.nature ? B[d.nature].color : (d.role === 'minion' ? '#b5c43a' : '#e0a040')}"><canvas data-y="${k}" width="192" height="160" aria-hidden="true"></canvas><h3>${d.name}</h3><div class="meta">${d.role === 'minion' ? 'from stage ' + d.from : d.role === 'mini' ? 'mini-boss' : 'boss'}${d.group ? ' · swarms of ' + d.group : ''}${d.nature ? ' · ' + B[d.nature].name : ''}</div><div class="t">${esc(d.says)}</div><div class="v">life ${k === 'mother' ? v('mother_hp') : d.hp} · ${d.sp} px/s · ${d.drop} void</div></article>`).join('')}</div>`).join('\n')}
<h3>Natured Ybasogs</h3>
<p>Some Ybasogs carry an innate nature: an aura in its colour and its sigil above. They have 30% more life and drop 2 more void energy. A bolt holding the <strong>opposite</strong> nature deals <strong>×${v('opposite_mult')}</strong> ("WEAK"); a bolt holding the <strong>same</strong> nature deals <strong>×${v('same_mult')}</strong> ("RESISTS"). Each adventure has a <strong>prevailing nature</strong>, shown at the Altar, carried by ${pct(v('prevailing_share'))} of natured Ybasogs.</p>
<div class="scroll"><table><thead><tr><th>Tide</th><th>Natured share</th><th>Stage 1 · 2 · 3 odds</th></tr></thead><tbody>
<tr><td><strong>Void tide</strong></td><td>5%</td><td class="mono">40% · 25% · 15%</td></tr>
<tr><td><strong>Mixed tide</strong></td><td>${pct(v('nature_chance'))}, +${pct(v('nature_chance_step'))} per later stage</td><td class="mono">50% · 50% · 45%</td></tr>
<tr><td><strong>Nature surge</strong></td><td>70%, mostly the prevailing nature</td><td class="mono">10% · 25% · 40%</td></tr></tbody></table></div>
<p class="note">Bosses carry fixed natures: The Choir is Pride, The Hollow Eye is Envy, The Mother is Gluttony. Mini-bosses always carry one. Mini-bosses and bosses resist slows, charms, sleep, pulls and executes.</p>
<h3>Falling matter</h3><div class="grid">${Object.entries(MTD).map(([k, d]) => `<article class="nat form" style="--c:${d.fam === 'rock' ? '#a89a8a' : d.fam === 'wreck' ? '#8b95a3' : '#8a8296'}"><canvas data-m="${k}" width="192" height="140" aria-hidden="true"></canvas><h3>${d.name}</h3><div class="meta">${d.fam}${d.only ? ' · from breaking' : ' · from stage ' + d.from}</div><div class="t">${esc(d.says)}</div><div class="v">life ${d.hp} · ${d.sp} px/s · ${d.drop} void</div></article>`).join('')}</div>
<h3>How waves are built</h3><ul>
<li>Stage 1, wave 1 is fixed and teaches pods, grazers, asteroids and the debris slab.</li>
<li>Every other wave opens with a <strong>debut</strong>: ${v('debuts_per_wave')} Ybasog kind not yet met this adventure (never-met first), alone, plus one new kind of matter. The first meeting ever shows a name card.</li>
<li>Then ${v('beats_base')} + ${v('beats_step')} per later wave and stage <strong>beats</strong> from kinds already met: trickle, line, V, column, flank, swarm, rockfall, wreck field, ruin wall, elite pair, lull.</li>
<li>Mini-bosses close stage 1 wave 3 and wave 2 of stages 2 and 3; The Choir, The Hollow Eye or The Drowned King end stage 2; The Mother ends stage 3.</li></ul></section>`);
P(`<section id="tuning"><h2><small>config.js (browser build)</small>Every tuning value</h2><p>All ${Object.keys(C).length} values the game reads live. Change one in <code>prototypes/voidwalker/config.js</code>, or ask for the tweak panel ("let me tune it").</p>
<details><summary>Show all values</summary><div class="scroll"><table><thead><tr><th>Key</th><th>Value</th><th>Range</th><th>Step</th></tr></thead><tbody>
${Object.entries(C).map(([k, x]) => typeof x !== 'object' ? `<tr><td class="mono">${k}</td><td class="num">${x}</td><td></td><td></td></tr>` : `<tr><td class="mono">${k}</td><td class="num">${x.value}</td><td class="num">${x.min} to ${x.max}</td><td class="num">${x.step}</td></tr>`).join('')}
</tbody></table></div></details></section>`);

P(`<section id="port"><h2><small>Godot 4.8 .NET, mobile-first</small>Migration to Godot</h2>${md(portMd)}</section>`);
P(`<section id="history"><h2><small>How it was made</small>History and changelog</h2>${md(histA)}</section>`);
P(`<section id="next"><h2><small>What is open</small>Known gaps and next steps</h2>${md(histB)}</section>`);
P(`<footer>Voidwalker Codex · generated ${today} from <code>prototypes/voidwalker/config.js</code>, the game's drawing code and <code>design/voidwalker-reference.md</code> at commit <code>${commit}</code>.</footer></div>`);

// ---------- script: the game's own drawing, the fusion panel, search
const RJ = {}; for (const [k, r] of Object.entries(R)) RJ[k] = { name: r.name, color: r.color, says: r.says, fx: r.fx.map(f => [f, FX[f] || f]), shard: r.shardFx || [], dmg: k ? dmgLine(k) : '' };
const BJ = {}; for (const i of ids) BJ[i] = { name: B[i].name, color: B[i].color, kind: B[i].kind };
P(`<script>
'use strict';
const BASE = ${JSON.stringify(BJ)}, REC = ${JSON.stringify(RJ)};
const CHEAP = false, INK = '#07040d', YB = ${JSON.stringify(window.YBASOGS)}, MT = ${JSON.stringify(window.MATTER)}, tune = { pod_hatch: 9, meteor_radius: 65 };
function choirOrbs(e) { return [0, 1, 2].map(i => ({ x: e.x + Math.cos(e.orbA + i / 3 * 6.283) * 92, y: e.y + Math.sin(e.orbA + i / 3 * 6.283) * 60, r: 16 })); }
let ctx;
const RECIPES = REC; const keyOf = n => [...n].sort().join('+'); const recipeOf = c => RECIPES[keyOf(c.natures)]; const cardColor = c => (recipeOf(c) || { color: '#fff' }).color;
${drawCode}
function paint(cv, fn) { const c = cv.getContext('2d'); const k = cv.width / cv.clientWidth || 2; ctx = c; c.setTransform(1, 0, 0, 1, 0, 0); c.clearRect(0, 0, cv.width, cv.height); c.save(); fn(cv.width, cv.height); c.restore(); }
const sigils = [...document.querySelectorAll('canvas[data-sigil]')], forms = [...document.querySelectorAll('canvas[data-form]')], ys = [...document.querySelectorAll('canvas[data-y]')], ms = [...document.querySelectorAll('canvas[data-m]')];
function drawStatic(t) {
  sigils.forEach(cv => paint(cv, (w, h) => { const n = cv.dataset.sigil; sigil(n, w / 2, h / 2, w * 0.3, n ? BASE[n].color : '#b9a6ff'); }));
  forms.forEach(cv => paint(cv, (w, h) => drawForm(cv.dataset.form, w / 2, h / 2 + 6, cv.dataset.color, t, w / 80)));
  ys.forEach(cv => paint(cv, (w, h) => { const k = cv.dataset.y, d = YB[k], sc = Math.min(1.6, 46 / (d.r + 30)) * 1.6; ctx.translate(w / 2, h / 2); ctx.scale(sc, sc); const e = { k, fam: d.fam, role: d.role, x: 0, y: 0, r: d.r, ph: 1, age: 2, vx: 0, vy: 1, hp: 1, maxhp: 1, side: -1, gi: 0, orbA: t }; drawY(e, t, 0, 300); if (k === 'choir') drawChoirOrbs(e, t); }));
  ms.forEach(cv => paint(cv, (w, h) => { const m = figureMatter(cv.dataset.m), sc = Math.min(1.4, 100 / Math.max(m.h || 0, (m.r || 0) * 2, 1), 160 / Math.max(m.w || 0, 1)); ctx.translate(w / 2, h / 2); ctx.scale(sc, sc); drawMatter(m, t); }));
  const hb = document.getElementById('heroBlob'); paint(hb, (w, h) => drawForm('blob', w / 2, h / 2, '#b9a6ff', t, w / 50));
}
const still = matchMedia('(prefers-reduced-motion: reduce)').matches;
let t0 = performance.now();
function loop(now) { drawStatic((now - t0) / 1000); requestAnimationFrame(loop); }
drawStatic(0); if (!still) requestAnimationFrame(loop);
// stars
(function () { const cv = document.getElementById('stars'), c = cv.getContext('2d'); let s = 7; const r = () => (s = s * 16807 % 2147483647) / 2147483647;
  const pts = Array.from({ length: 220 }, () => [r(), r(), 0.3 + r() * 0.9]);
  function draw() { cv.width = innerWidth; cv.height = innerHeight; c.clearRect(0, 0, cv.width, cv.height); for (const [x, y, z] of pts) { c.fillStyle = 'rgba(210,200,255,' + (0.12 + z * 0.3) + ')'; c.fillRect(x * cv.width, y * cv.height, z * 1.6, z * 1.6); } }
  addEventListener('resize', draw); draw(); })();
// fusion detail
const detail = document.getElementById('detail');
function show(k) {
  const r = REC[k], ns = k.split('+');
  document.querySelectorAll('.cell[aria-pressed="true"]').forEach(b => b.setAttribute('aria-pressed', 'false'));
  document.querySelectorAll('.cell[data-k="' + k + '"]').forEach(b => b.setAttribute('aria-pressed', 'true'));
  detail.style.setProperty('--c', r.color);
  detail.innerHTML = '<div style="display:flex;gap:14px;align-items:center"><canvas width="112" height="112" style="width:56px;height:56px" aria-hidden="true"></canvas><div><h3>' + r.name + '</h3><div>' + ns.map(n => '<span class="chip" style="--c:' + BASE[n].color + '">' + BASE[n].name + '</span>').join('') + '</div></div></div>'
    + '<p>' + r.says + '</p><ul>' + r.fx.map(([f, t]) => '<li><code>' + f + '</code> ' + t + '</li>').join('') + (r.shard.length ? '<li><code>shards</code> carry ' + r.shard.join(', ') + '</li>' : '') + '</ul>'
    + '<p class="mono" style="color:var(--dim)">Damage I · II · III: ' + r.dmg + '</p>';
  const cv = detail.querySelector('canvas'); paint(cv, (w, h) => cardSigil({ natures: ns, level: 1 }, w / 2, h / 2, w * 0.32));
}
document.querySelectorAll('.cell[data-k]').forEach(b => b.addEventListener('click', () => show(b.dataset.k)));
show('greed+sloth');
// search and filter
const q = document.getElementById('q'), grp = document.getElementById('grp'), cnt = document.getElementById('cnt'), rows = [...document.querySelectorAll('#ftable tbody tr')];
function filter() { const s = q.value.trim().toLowerCase(), g = grp.value; let n = 0; rows.forEach(tr => { const ok = (!g || tr.dataset.g === g) && (!s || tr.dataset.s.includes(s)); tr.hidden = !ok; if (ok) n++; }); cnt.textContent = n + ' shown'; }
q.addEventListener('input', filter); grp.addEventListener('change', filter);
</script>`);
let html = S.join('\n').replace(/<section id="([a-z]+)"><h2>([\s\S]*?)<\/h2>/g, (m, id) => m + badge(id));
html = html.replace(/<p><em>Source: [^<]*<\/em><\/p>\n?/g, '');  // the badge carries it
const outDir = path.join(root, 'build/codex'); fs.mkdirSync(outDir, { recursive: true });
fs.writeFileSync(path.join(outDir, 'voidwalker-codex.html'), html);
console.log('ok', path.join(outDir, 'voidwalker-codex.html'), html.length);
