# Yellow-Hell (Backrooms game)

Unity 6000.4.3f1, URP, New Input System only. Repo: https://github.com/crafterten/Yellow-Hell

## Project facts

- Path: `C:/Users/craft/My project`
- Git: LFS enabled, main branch tracks origin
- Active scene: `Assets/Scenes/SampleScene.unity` (rename to Level0 in P1)
- `activeInputHandler: 1` in ProjectSettings → legacy `UnityEngine.Input` dead. Use `UnityEngine.InputSystem.Keyboard.current / Mouse.current`.

## Unity MCP

- Transport: stdio, single instance `My project@<hash>`
- Script create/edit auto-triggers compilation; poll `mcpforunity://editor/state` `is_compiling` before assuming types exist
- Instance IDs are negative for unsaved objects; save scene to stabilize before assigning object refs
- Use `batch_execute` for multi-op; max 25 per batch

## Scene hierarchy (current)

- Pill (capsule primitive, y=1.5) — Rigidbody (freezeRotation), CapsuleCollider, `FirstPersonController`
  - CameraPivot (localPos [0, 0.7, 0]) — empty, pitch target
    - Main Camera (tag=MainCamera) — Camera, AudioListener
      - Flashlight (scale 0.3, localPos [0.25, -0.25, 0.35], rot [10,-15,0]) — mesh only, out-of-view
      - Beam (reparented under Main Camera directly, local identity) — Light (spot, range 45, spotAngle 75, intensity 6), `FlashlightBeamAim`
- Ground (plane, scale 5×1×5) — M_Carpet material, tiled 10×10
- GlobalVolume_Backrooms — global URP Volume with Bloom/Vignette/FilmGrain/ColorAdjustments/ChromaticAberration (profile at `Assets/Settings/Volumes/VP_Backrooms.asset`)
- Directional Light + Main Camera (scene defaults)

## Scripts

- `Assets/Scripts/Core/FirstPersonController.cs` — WASD + shift sprint (hidden stamina 100, drain 25/s, regen 15/s, regen delay 1.2s, relock at 30%), mouselook (sens 0.15), jump (SphereCast ground check), Esc unlocks cursor
- `Assets/Scripts/Gameplay/FlashlightController.cs` — F toggles Light.enabled only (do NOT toggle GameObject active — disables the script)
- `Assets/Scripts/Gameplay/FlashlightBeamAim.cs` — LateUpdate slerps world rotation toward `aimTarget` (Main Camera) at `followSpeed=8`

## Materials (Assets/Art/Materials/)

- M_Carpet (BaseMap+Normal from `backrooms/carpet/`, tiling 10×10) → Ground
- M_Wallpaper (unused, for P1 walls)
- M_Ceiling (unused, for P1 ceiling)
- M_Flashlight (dark metal) → Flashlight mesh

## Textures available

- `Assets/Scenes/textures/backrooms/backrooms/{carpet,wallpaper,ceiling_tiles,ceiling_tiles_2,painted_wall,painted_wall_2,pool_tiles}` — each has `_color.png`, `_normal.png`, `_rough.png`
- `Assets/Scenes/textures/Carpet011_2K-JPG/` — alt carpet PBR
- `Assets/Scenes/sounds/Voicy_kirby=backrooms.mp3` — only audio so far

## Design decisions (user-approved)

- Scope: full-sandbox (levels + inventory + save/load)
- Level 0: medium ~100 rooms, ~45 min wander
- Noclip: random chance on wall collision, **increased chance while holding a hotkey** (for escape)
- Inventory: 6 slots, stackable
- Save: 3 slots
- Stamina: exists but hidden (no UI)
- Flashlight: toggle only (no battery)
- No AI in P0–P1
- Audio: user will drop files as needed

## Phase status

- [x] P0 — Foundation: git, scaffold, FPS controller, flashlight, URP volume, carpet material
- [ ] P1 — Level 0 environment: walls/ceiling, LevelGenerator (seeded grid), FluorescentFlicker, AudioManager (codex-parallel candidates)
- [ ] P2 — Interaction + inventory (3 codex workers)
- [ ] P3 — Save/Load
- [ ] P4 — Multi-level + noclip
- [ ] P5 — Polish

## Token strategy

- Main Claude session = Unity MCP orchestration, scene wiring, debug loops
- `/codex-parallel` for pure script authoring (LevelGenerator, Inventory, SaveSystem) — needs clean git working tree per phase
- `Explore` subagent for research (URP/NavMesh API questions)

## Known pitfalls (learned the hard way)

- Negative instance IDs drift after domain reload — re-find by name after any script compile
- `script_apply_edits` occasionally throws `WinError 10035` socket flap — retry; first call usually applied
- Legacy `Input.GetKey` silently dead with New Input System — always `Keyboard.current.xKey.isPressed`
- Toggling a GameObject's active state disables all scripts on it including the toggler itself
- URP Lit tiling prop is `_BaseMap_ST` as `[x, y, offsetX, offsetY]`, not `mainTextureScale`
- Plan file: `C:/Users/craft/.claude/plans/wobbly-sniffing-beacon.md`

## Pickup checklist for next session

1. Read this file
2. `git status` + `git log --oneline -5` in project dir
3. Read `mcpforunity://instances` to confirm Unity alive
4. Read `mcpforunity://editor/state` → `ready_for_tools`
5. Resume at next unchecked phase
