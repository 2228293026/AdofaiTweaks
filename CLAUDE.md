# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Release Commands

```bash
# Build release version
dotnet build -c Release

# Build compat assemblies (required for release)
dotnet build -c SharpHook          # Builds AdofaiTweaks.Compat.Async with SharpHook
dotnet build -c SkyHook            # Builds AdofaiTweaks.Compat.Async with SkyHook

# Build and deploy directly to game's Mods folder
dotnet build -c Game

# Run the translation string generator
cd AdofaiTweaks.Generator/bin/Release && mono AdofaiTweaks.Generator.exe

# Full release packaging (see make_release.sh)
# Builds all configs, runs generator, assembles zip with all DLLs + assets
```

## Solution Structure

The solution (`AdofaiTweaks.sln`) has 4 projects:

**AdofaiTweaks** (main mod, .NET Framework 4.8, C# 12) — A UnityModManager mod for "A Dance of Fire and Ice". Entry point: `Startup.Load()` loads dependent assemblies, then calls `AdofaiTweaks.Setup()`. Uses Harmony for runtime IL patching.

**AdofaiTweaks.Translation** — Localization framework with `LanguageEnum` (10 languages) and `TweakString` translation lookup.

**AdofaiTweaks.Generator** — Build-time tool that generates `AdofaiTweaks.Strings.dll` and `TweakStrings.db` from an Excel sheet.

**AdofaiTweaks.Compat.Async** — Conditional async input compatibility built as 3 variants (SharpHook/SkyHook/Polyfill) selected at runtime based on available game assemblies.

## Architecture

### Tweak System (Plugin Framework)

Each feature is a **Tweak** registered via `[RegisterTweak]` attribute. The framework auto-discovers all tweak types via assembly reflection.

Tweak lifecycle (in order):
1. `OnEnable()` — before Harmony patches are applied
2. Harmony patches applied to `PatchesType`
3. `TweakPatch` instances applied (version-gated patches)
4. `OnPatch()` — after all patches are applied
5. `OnUpdate(deltaTime)` — per-frame
6. `OnDisable()` — before patches are removed
7. Harmony unpatched
8. `TweakPatch` instances unpatched
9. `OnUnpatch()` — after all patches removed

Each tweak lives in its own folder under `AdofaiTweaks/Tweaks/<TweakName>/` with:
- `*Tweak.cs` — class extending `Tweak`
- `*Settings.cs` — class extending `TweakSettings` (serialized as XML by UMM)
- `*Patches.cs` — static class with nested patch classes

### Patch Versioning System

The custom `[TweakPatch]` attribute wraps Harmony patches with version gating: `MinVersion`/`MaxVersion` check against `GCNS.releaseNumber`. If the game version is outside the range (or the target class/method doesn't exist), the patch is silently skipped — prevents crashes across game updates.

`TweakPatchAttribute` fields: `PatchId`, `ClassName` (string, not Type — avoids assembly resolution failures), `MethodName`, `MinVersion`, `MaxVersion`.

### Settings Synchronizer

`SettingsSynchronizer` auto-loads all `TweakSettings` subclasses from XML at startup, then injects them into any class with a `[SyncTweakSettings]` property. This means any class (tweak, patches, runner) can access settings without manual wiring:
```csharp
[SyncTweakSettings]
private static KeyLimiterSettings Settings { get; set; }
```

### Game Version State

`GameVersionState` provides boolean flags (`AsyncInputAvailable`, `OldAsyncInputAvailable`) set from the game's release number for runtime feature checks.

### Key Tweak Patterns

- **Settings classes** extend `TweakSettings` which extends `UnityModManager.ModSettings`. Properties auto-serialize to XML. Use `[XmlIgnore]` for runtime-only state.
- **Patches** use static nested classes with Harmony `Prefix`/`Postfix` methods. Each gets a unique `[TweakPatch]` ID and is version-gated.
- **Translations** use `TweakStrings.Get(TranslationKeys.Xxx.XXX)` for all user-facing strings.

## Dependencies

- **UnityModManager** — mod loader (entry: `AdofaiTweaks.Startup.Load`)
- **Harmony** (0Harmony) — IL patching
- **LiteDB** — bundled in release zip
- **IndexRange** — bundled in release zip
- **StyleCop.Analyzers** — code style enforcement via `StyleCop.ruleset`
