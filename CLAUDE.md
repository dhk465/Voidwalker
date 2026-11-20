# Claude Code Game Studios -- Game Studio Agent Architecture

Indie game development managed through 49 coordinated Claude Code subagents.
Each agent owns a specific domain, enforcing separation of concerns and quality.

## Technology Stack

- **Engine**: Godot 4.8 beta 4 (.NET build)
- **Language**: C# (.NET 10+, primary; Godot 4.8 requires .NET 10), C++ via GDExtension (native plugins only)
- **Version Control**: Git with trunk-based development
- **Build System**: .NET SDK + Godot Export Templates
- **Asset Pipeline**: Godot Import System + custom resource pipeline

> **Note**: Engine-specialist agents exist for Godot, Unity, and Unreal with
> dedicated sub-specialists. Use the set matching your engine.

## Project Structure

@.claude/docs/directory-structure.md

## Engine Version Reference

<!-- ENGINE-REFERENCE-IMPORT: the line under this comment is engine-specific,
     and there is none until an engine is set. /setup-engine, the grow's record
     step and /project-settings (engine.name) write
     @docs/engine-reference/<engine>/VERSION.md here for the chosen engine. Skills
     that need the pinned version read that file on demand either way. -->
@docs/engine-reference/godot/VERSION.md


## Technical Preferences

`project.yaml` at the repo root is the primary config store — engine, specialists,
naming, platform, performance, modes. Skills resolve it via `resolve_config`
(see `.claude/docs/config-resolution.md`).

`.claude/docs/technical-preferences.md` is the **legacy fallback**, read on demand
when a key is absent from `project.yaml`. It is not imported here: before
`/setup-engine` runs it is almost entirely `[TO BE CONFIGURED]` placeholders, and
after it runs `project.yaml` holds the real values.

## Coordination Rules

@.claude/docs/coordination-rules.md

## Collaboration Protocol

**User-driven collaboration, not autonomous execution.**
Every task follows: **Question -> Options -> Decision -> Draft -> Approval**

- Agents MUST ask "May I write this to [filepath]?" before using Write/Edit tools
  (exempt: `active.md` appends, agent memory, a subagent's new file at the path
  its orchestrating skill named; see `.claude/rules/agent-memory.md`)
- Agents MUST show drafts or summaries before requesting approval
- Multi-file changes require explicit approval for the full changeset
- No commits without user instruction
- A skipped or dismissed question is not an answer: act on nothing, re-ask in
  plain text and wait
- Started outside this file's folder? Warn first: hooks, deny rules and the
  ask-first mode load only when Claude Code starts here

See `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` for full protocol and examples.

> **First session?** Say "make me a game" (`/prototype`), or run `/start` to begin
> the guided onboarding flow.

## Coding Standards

@.claude/docs/coding-standards.md

## Context Management

Read `.claude/docs/context-management.md` on demand — it is a reference, not
session context. Two of its conventions are load-bearing and cited by name
elsewhere in the repo, so they are restated here rather than lost:

- **`production/session-state/active.md` is the session checkpoint.** The file is
  the memory, not the conversation. Read it first after any compaction, crash, or
  `/clear`.
- **Helpers in `.claude/scripts/` emit observations, never verdicts.** A script
  that scores or judges will eventually contradict a mode or override it cannot
  see. (Cited by `artifact-check.sh` and `adr-dep-graph.sh`.)

## Pixel art tools (standing rule from the user, 2026-11-20)

All pixel art for the game goes through the two toolkits the user provided, before
any other tool: **pixelfx** (`tools/pixelfx`, the `pixelfx` skill) and
**agent-sprite-forge** (`tools/agent-sprite-forge`, whose skills are
`generate2dsprite`, `generate2dmap`, `video2dsprite`, `generate2dmedia` and `codeart2d`).
- Hand-written generators and art drawn straight from engine code don't count.
- Follow each skill's own workflow: its capability check, route order, QC and
  review sheets.
- When a piece was made without these tools, redo it with them.

## Voidwalker Codex (standing rule from the user, 2026-11-19)

The Voidwalker Codex is the game's reference page, built by `tools/codex/genpage.js`
from `design/voidwalker-reference.md` and the browser game's data. Run it with
`bash tools/codex/serve.sh`.

- **Keep it current.** Any change to the game must update the reference doc
  in the same piece of work. Then rebuild the page and check that it loads
  without errors.
- **Migrate it with the port.** After each system moves from the browser
  build to Godot:
  1. Rewrite that system's reference section from the Godot sources, and
     mark it `*Source: Godot port (migrated <date>)*`.
  2. Update the tracker in section 18.
  3. Make the page's data sections read from the Godot data once the port
     owns them.
  Browser-only content is replaced bit by bit, never all at once. A section
  without a Godot source stays marked as the browser build.
