# Technical Preferences

<!-- project.yaml at the repo root is the machine-readable source of truth for
     engine, specialists, naming, performance, platform, and testing.framework.
     This file is the human-readable LEGACY FALLBACK: agents and skills resolve
     each key from project.yaml first and fall back here only when the
     project.yaml key is absent. /setup-engine dual-writes both.
     Forbidden patterns and allowed libraries are NOT migrated — they live only
     in this file. Populated by /setup-engine; updated as decisions are made. -->

## Engine & Language

- **Engine**: Godot 4.8 beta 4 (.NET build)
- **Language**: C# (.NET 10+, primary; Godot 4.8 requires .NET 10), C++ via GDExtension (native plugins only)
- **Rendering**: Compatibility renderer (OpenGL ES 3 / WebGL 2) for the widest Android support; 2D, pixel-perfect: fixed 1280x720 base viewport, integer scaling, nearest filtering
- **Physics**: Godot 2D (GodotPhysics2D); the game's own collision for bolts and Ybasogs

## Input & Platform

<!-- Written by /setup-engine. Read by /ux-design, /ux-review, /test-setup, /team-ui, and /dev-story -->
<!-- to scope interaction specs, test helpers, and implementation to the correct input methods. -->

- **Target Platforms**: Android (now), iOS (later), PC (possible)
- **Input Methods**: Touch (primary), Keyboard/Mouse (PC), Gamepad (partial)
- **Primary Input**: Touch
- **Gamepad Support**: Partial
- **Touch Support**: Full
- **Platform Notes**: Mobile-first: auto-aim in adventures; touch moves the blob and drives every interaction; mobile-friendly UI (thumb-reach controls, large hit targets, no hover-only interactions, so the hub's hover highlight also needs a tap/selection state); landscape; safe areas on notched phones.

## Naming Conventions

- **Classes**: PascalCase, and `partial` (e.g., `PlayerController`)
- **Variables**: public fields/properties PascalCase (`MoveSpeed`); private fields `_camelCase` (`_currentHealth`); methods PascalCase (`TakeDamage()`)
- **Signals/Events**: PascalCase + `EventHandler` suffix (e.g., `HealthChangedEventHandler`)
- **Files**: PascalCase matching class (e.g., `PlayerController.cs`)
- **Scenes/Prefabs**: PascalCase matching root node (e.g., `PlayerController.tscn`)
- **Constants**: PascalCase (e.g., `MaxHealth`, `DefaultMoveSpeed`)

## Performance Budgets

- **Target Framerate**: 60 fps; 120 fps supported on high-refresh phones
- **Frame Budget**: 16.6 ms (8.3 ms at 120 fps)
- **Draw Calls**: 200 per frame (2D, mobile)
- **Memory Ceiling**: 512 MB (mobile)

## Testing

- **Framework**: gdUnit4 (C# via gdUnit4Net), added when the first tests are written
- **Minimum Coverage**: [TO BE CONFIGURED]
- **Required Tests**: Balance formulas, gameplay systems, networking (if applicable)

## Forbidden Patterns

<!-- Add patterns that should never appear in this project's codebase -->
- [None configured yet — add as architectural decisions are made]

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- [None configured yet — add as dependencies are approved]

## Architecture Decisions Log

<!-- Quick reference linking to full ADRs in docs/architecture/ -->
- [No ADRs yet — use /architecture-decision to create one]

## Engine Specialists

<!-- Written by /setup-engine when engine is configured. -->
<!-- Read by /code-review, /architecture-decision, /architecture-review, and team skills -->
<!-- to know which specialist to spawn for engine-specific validation. -->

- **Primary**: godot-specialist
- **Language/Code Specialist**: godot-csharp-specialist (all .cs files)
- **Shader Specialist**: godot-shader-specialist (.gdshader files, VisualShader resources)
- **UI Specialist**: godot-specialist (no dedicated UI specialist — primary covers all UI)
- **Additional Specialists**: godot-gdextension-specialist (GDExtension / native C++ bindings only)
- **Routing Notes**: Invoke primary for architecture decisions, ADR validation, and cross-cutting code review. Invoke C# specialist for code quality, [Signal] delegate patterns, [Export] attributes, .csproj management, and C#-specific Godot idioms. Invoke shader specialist for material design and shader code. Invoke GDExtension specialist only when native C++ plugins are involved.

### File Extension Routing

<!-- Skills use this table to select the right specialist per file type. -->
<!-- If a row says [TO BE CONFIGURED], fall back to Primary for that file type. -->

| File Extension / Type | Specialist to Spawn |
|-----------------------|---------------------|
| Game code (.cs files) | godot-csharp-specialist |
| Shader / material files (.gdshader, VisualShader) | godot-shader-specialist |
| UI / screen files (Control nodes, CanvasLayer) | godot-specialist |
| Scene / prefab / level files (.tscn, .tres) | godot-specialist |
| Project config (.csproj, NuGet) | godot-csharp-specialist |
| Native extension / plugin files (.gdextension, C++) | godot-gdextension-specialist |
| General architecture review | godot-specialist |
