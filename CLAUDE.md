# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.
You are in the role of a senior software engineer. I am also a senior software engineer thus you can ask me questions specially about code architecture and general design. 

## Project Overview

**Farandole** is a Unity 2D mobile minigame collection (Android/iOS) built by Xenologos.Software. Players progress through sequences of minigames with increasing difficulty, rank-based advancement, and a mutation/artefact system.

## Unity Version Constraints

- Use `EditorApplication.isPlaying = true` — NOT `EditorApplication.EnterPlayMode()`
- `SessionState` has no `Erase*` methods — clear via `Set*(key, defaultValue)`
- IMGUI: Never use `GUILayout.FlexibleSpace()` inside `HorizontalScope` with conditional content (control count differs between Layout/Repaint events → crash). Use `GUILayoutUtility.GetRect` + `GUI.DrawTexture` instead.

## Build & Test

No CLI build pipeline. All building and testing is done through the Unity Editor:
- **Run game**: `Tools > Game Mode Launcher` (shortcut: `Ctrl+Shift+G`) — loads Game scene and enters PlayMode
- **Unit tests**: Unity Test Framework (`com.unity.test-framework`) — run via `Window > General > Test Runner`
- **New minigame**: `Tools > MiniGame Editor Tool`

## Key Paths

| Asset Type | Path |
|---|---|
| C# Scripts | `Assets/1.Scripts/` |
| Scenes | `Assets/0.Scenes/` (Title.unity, Game.unity) |
| Minigame Scripts | `Assets/1.Scripts/MiniGames/{Name}_MiniGame/` |
| Minigame SOs | `Assets/5.Presets/MiniGames/MG_{Name}.asset` |
| Minigame Prefabs | `Assets/2.Prefabs/_MiniGames/{Name}MiniGame.prefab` |
| MiniGame Bank | `Assets/5.Presets/MiniGameBank.asset` |
| Editor Tools | `Assets/1.Scripts/Editor/` |

## Architecture

### Initialization Flow

`GameManager.Start()` runs an `Init()` coroutine that sequentially initializes all managers:
1. `LayerManager2D` → `PlaygroundManager` → `MiniGameManager` (calls `LoadLoop()`)
2. `GameSceneManager` → `AnimationManager` → `AudioManager` → `UIGame`

`GameData` is a singleton `MonoBehaviour` with `DontDestroyOnLoad` (access via `GameData.Get`). It is configured *before* the Game scene loads — either by the title screen or the `GameModeLauncher` editor tool (which writes to `PlayerPrefs`, read in `GameData.Awake()` via `ApplyEditorLaunchConfig()`).

### Game Loop

`MiniGameManager` drives the loop:
- `LoadLoop()` → builds a `MiniGameLoop` from the bank
- `PlayCurrent()` → plays the active minigame; wires `OnHPLossCB`, `OnMiniGameComplete`
- On win/loss: `WinMiniGame()` → `MoveNext()` → rank/depth update → next minigame or `GameOver()`

`MiniGameLoop` tracks: depth (loop level), rank (E→S), combo, and snapshots history in `MiniGameLoopHistory`.

Each loop the depth is incremented and thus we modify the playground pattern accordingly.
Also if is met the condition, the player ranks up and games gets harder but also generate more point.
If loop is failed, the player is downranked.

Moreover, the global gamespeed increase at each depth.

### Minigame Structure

All minigames extend the abstract `MiniGame : MonoBehaviour, IMiniGame` base class:
- Override: `Init()`, `Reset()`, `Play()`, `Lose()`, and optionally `IntroAnim()`
- Each minigame has an associated `MiniGameSO` (metadata: ID, family, goal, thumbnail)
- Family interfaces: `IRegularFamily`, `IDogFamily`, `IChessFamily`, `IArcadeFamily`, `IPhysicsFamily`

### Mutation System

Minigames can have up to 2 `extensions` (Alpha/Beta mutations). Extensions implement both a family interface and `IMiniGameMod`:
```csharp
public interface IMiniGameMod {
    EMiniGameMods AssociatedTag();
    void Apply(MiniGameLoopSocket iMGSocket);
}
```
Mutations are applied via `MiniGameLoop.RefreshMutations()`. Artefacts (collected by player) are stored in `PlayerData.bag` and defined in `ArtefactCollectionSO`.

### Game Modes

`GAME_MODE` enum (in `GameModesSO.cs`): `DAILY_SEED`, `MUTATION`, `SINGLES`, `RANDOM`. Each maps to a `GameSettingsSO` with HP, loop size, timing, and rank thresholds.
They also enforce specific mechanics :
- DAILY_SEED propose a loop that is composed of mini game IDs extracted from the datetime and is free to play
- SINGLES propose a loop that is composed of only 1 minigame. Goal is to finish it in DIVINE (making 0 failure) and its equivalent to a practice mode. Its iin the paid version
- RANDOM makes a randomly generated loop. Its in the paid version.
- MUTATION is a gamemode where the player edits the loop by discarding//picking miini games and acquires artifacts that modifies the game and generates more score.

### UI Conventions

- Custom buttons use `clickCallback` event, NOT `onClick`
- Selectable UI elements extend `UISelectableImage`
- `UIMiniGamePresentationImage` has both `infoBubbleBtn` (tooltip) and `selectButton` — when enabling selection mode, redirect `infoBubbleBtn` to prevent interference

### ScriptableObjects

Configuration is SO-driven. Key types: `GameModesSO`, `GameSettingsSO`, `MiniGameBankSO`, `MiniGameSO`, `UIThemeSO`, `LoopRankSO`, `ArtefactCollectionSO`, `GlobalSettingsSO`.

### Key Dependencies

- **UniTask** (`com.cysharp.unitask`) — all async/await patterns use UniTask
- **FMOD** — audio (not Unity's built-in AudioSource)
- **Unity Input System** — new input system; do not use legacy `Input.*`
- **URP** — rendering pipeline
- **Unity Gaming Services** — cloud save, leaderboards, authentication, IAP

---

## Title Scene & Services Architecture

### Title Scene Flow

`UITitle : UINavigator` is the root. On `Start()` it wires all button callbacks then synchronously checks `UGSAuthenticationManager.IsSignedInUGS()` — since UGS likely hasn't finished initialising at that point, this will return `false` and always show the sign-in popup on first run. The popup flow is handled by `UIPopupPanel`.

Scene launch sequence:
1. Player taps a game mode button → `GameData.PickGameMode()` / `SetToDailySeed()` / `AddGameSeed()` called
2. `UITitle.DelayedLaunch()` — plays FMOD exit parameter + fade, then `SceneManager.LoadScene("Game")`
3. `GameData` (DontDestroyOnLoad singleton) carries the configured mode into the Game scene

### IAP / Premium System

`IAPManager` is a `DontDestroyOnLoad` singleton. Its async `Start()` calls `UnityServices.InitializeAsync()` then `InitializeIAP()`. The premium gate is `SessionData.IsPremium`, which safely null-checks `IAPManager.Get` using Unity's MonoBehaviour bool operator.

Premium unlock is cached to `PlayerPrefs` key `"PremiumFeatures"` so it survives sessions without re-querying the store. In the editor, `PlayerPrefs.SetInt(kLocalPremiumUnlocked, 0)` resets this on every play — intentional for dev testing.

**WIP**: `OpenStore()` / `CloseStore()` are stubbed (commented out). `UIStorePage` is an empty class. The store UI flow is not yet connected.

### UGS Authentication

`UGSAuthenticationManager` (static class) wraps `AuthenticationService.Instance`:
- `IsSignedInUGS()` — safe, has `catch (ServicesInitializationException)`
- `PlayerName` / `PlayerId` properties — **not guarded**, will throw if called before UGS init
- `SessionData.UserName` is safe (uses `IsSignedInUGS()` guard); `SessionData.UserID` is **not** safe
- On Android: `InitializeAndSignIn()` calls `LinkGoogleAsync()` → `GPGSAuthentication.SignInAndGetIdToken()`

`IAPManager.AsyncStart()` and `UGSAuthenticationManager.InitializeAndSignIn()` both call `UnityServices.InitializeAsync()` — Unity Services handles double-init gracefully.

### Cloud Save & Leaderboards (`UGSCloudSaveManager`)

All methods are `static async`. Key leaderboard ID: `"LEADERBOARD_DAILY_SEED"` (currently hardcoded — the per-date key scheme is commented out in `GetDailySeedLeaderboardKey()`).

Submit flow for Daily Seed: `SubmitDailySeedScore()` → loads current best from leaderboard → if new score is higher, saves to Cloud Save then posts to leaderboard via `LeaderboardsService.Instance.AddPlayerScoreAsync()`.

`UIHighScores.OnNavEnter()` calls `InitScores()` fire-and-forget (not awaited) — leaderboard entries populate asynchronously after the panel is visible.

### Singles Mode Progress

Persisted locally via `PlayerPrefs` (JSON, key per minigame ID). `SinglesModeProgress.SaveLocalProgress()` writes on game-over; `LoadProgress()` reads on panel open. Data class: `SinglesData` (score, completed flag, rank, date string).

---

## Creating a New Minigame

1. Open `Tools > MiniGame Editor Tool`
2. Fill in Name, Goal, Family, Thumbnail, Compatible Mods
3. **Phase 1**: Generates `{Name}MiniGame.cs` + `MG_{Name}.asset`
4. Wait for Unity to recompile
5. **Phase 2**: Generates `{Name}MiniGame.prefab` and links everything
6. Implement game logic in the generated script (override `Play()`, `Lose()`, etc.)
7. Test via `Tools > Game Mode Launcher` in SINGLES mode

# Known bugs

`bugs.txt` stores the current known bugs.

Confirmed bugs found during code audit (not yet fixed):

| File | Location | Issue |
|---|---|---|
| `GPGSAuthentication.cs:23` | Android auth | `GetUserId()` returns player ID, not an auth token — `LinkWithGoogleAsync()` will reject it; should use `GetServerAuthCode()` |
| `SessionData.cs:13` | `UserID` property | Accesses `AuthenticationService.Instance.PlayerId` with no `ServicesInitializationException` guard — throws before UGS init |
| `IAPManager.cs:176` | `CheckPremiumPurchased()` | Grants premium for ANY confirmed order, doesn't filter by `kPremiumAccStoreKey` — will break if a second product is added |
| `UGSCloudSaveManager.cs:86` | `LoadTodayBestScore()` | Ignores its `iKey` parameter and hardcodes `"LEADERBOARD_DAILY_SEED"` — won't follow if per-date scheme is ever activated |
| `UITitle.cs:68` | Singles launch button | Uses `onClick` instead of `clickCallback`, inconsistent with every other button in the codebase |