# Godot Engine — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Godot 4.8 beta 4 (.NET build) — a pre-release; move to 4.8 stable when it ships |
| **Installed at pin time** | 4.8.beta4.mono.official.b070e63b1 — probed 2026-11-19 at C:/Users/sorhg/Godot/Godot_v4.8-beta4_mono_win64/ (console build); .NET SDK 10.0.404 |
| **Release Date** | 4.8 beta 4: 18 November 2026 (https://godotengine.org/article/dev-snapshot-godot-4-8-beta-4/); 4.8 stable: NOT SOURCEABLE — not released at pin time |
| **Project Pinned** | 2026-11-19 (Voidwalker's move from the browser to Godot); earlier pin 4.7 on 2026-10-17 |
| **Last Docs Verified** | 2026-11-19 (4.7 → 4.8 beta 1-4, from the official beta articles and the draft upgrade guide); 2026-10-21 (Windows driver default and Compositor, against Godot source); 2026-10-17 (release timeline and 4.7 changes); 2026-02-12 (4.4-4.6 content) |
| **LLM Knowledge Cutoff** | May 2025 |

## Knowledge Gap Warning

May 2025 is a floor for every model the studio runs on: a newer model may know more, but check this folder first.

The LLM's training data likely covers Godot up to ~4.3. Versions 4.5, 4.6,
4.7 and 4.8 (beta) were released after the cutoff and introduced significant changes that
the model does NOT know about; 4.4 came out shortly before it and may be thinly
covered. Always cross-reference this directory before suggesting Godot API calls.

## Installed-Version Gap Warning

The warning above is one-directional — it covers the **model** knowing less than
this pin. The reverse gap is real and `/setup-engine` §3 creates it deliberately
("pin the newer one and upgrade later"): this reference can sit **ahead of the
installed editor**, and an agent citing it correctly then emits APIs that do not
compile locally. **Check `Installed at pin time` above before trusting a
version-qualified claim** — `NOT DETERMINED` means the gap is unknown, not absent.

## Post-Cutoff Version Timeline

| Version | Release | Risk Level | Key Theme |
|---------|---------|------------|-----------|
| 4.5 | Sep 2025 | HIGH | Accessibility (AccessKit), variadic args, @abstract, shader baker, SMAA |
| 4.6 | Jan 2026 | HIGH | Jolt default, glow rework, IK restored |
| 4.7 | Jun 2026 | HIGH | AreaLight3D, HDR output, Asset Store replaces the Asset Library, VirtualJoystick, Jolt soft-body and world-boundary changes, typed-return overrides |
| 4.8 (beta 4) | Nov 2026, beta | HIGH | .NET 10 minimum, in-editor Android/Java SDK setup, Compatibility decals, touch in TextEdit/TabBar/PopupMenu, runtime CanvasItem editing; stable not yet released |

## Before the Cutoff

4.4 (Mar 2025, Jolt option, FileAccess return types) predates the May 2025
cutoff; the Knowledge Gap Warning still applies to it.

Dates from https://github.com/godotengine/godot/releases, checked 2026-10-17:
4.4 Mar 3 2025, 4.5 Sep 15 2025, 4.6 Jan 26 2026, 4.7 Jun 18 2026, 4.7.2 Aug 18
2026. Migration guides and release notes: `breaking-changes.md` § Sources.
