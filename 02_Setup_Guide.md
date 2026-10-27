# AR Balloon Pop – Setup Guide

## PHASE 1 – Folder Structure
Create this structure under `Assets/`:
```
Assets/
├── Scenes/       → the main AR scene (e.g. "ARBalloonPop.unity")
├── Scripts/      → all 8 .cs files (Scripts/ folder in this package)
├── Prefabs/      → Balloon.prefab, PopEffect.prefab
├── Materials/    → balloon material(s) used for tinting colors
├── Audio/        → pop sound effect clip(s)
├── Particles/    → optional: particle system presets, if you build one outside the prefab
├── UI/           → any UI sprites/icons (optional, can use built-in UI)
└── Models/       → not required if using primitive Sphere for balloons
```

## PHASE 2 – Step-by-Step Unity Setup

1. **Create the Unity project**: Unity Hub → New Project → 3D (Core) template. Name it "AR Balloon Pop".
2. **Install AR Foundation**: Window → Package Manager → search "AR Foundation" → Install (latest version compatible with your Unity LTS).
3. **Install ARCore XR Plugin**: In Package Manager, search "ARCore XR Plugin" → Install.
4. **Configure Android platform**:
   - File → Build Settings → select **Android** → Switch Platform.
   - Edit → Project Settings → XR Plug-in Management → check **Android** tab → enable **ARCore**.
   - Player Settings → Other Settings → set **Minimum API Level** to Android 7.0 (API 24) or higher, and **Target API Level** to the highest installed.
   - Player Settings → set **Scripting Backend** to IL2CPP, **Target Architectures** → check ARM64.
5. **Create AR Session**: In your scene Hierarchy, right-click → XR → AR Session. This object manages the AR session lifecycle (start/pause).
6. **Create XR Origin / AR Camera**: Right-click Hierarchy → XR → XR Origin (Mobile AR). This replaces your default Main Camera with an AR-tracked camera.
7. **Add AR Plane Manager**: Select the XR Origin object → Add Component → "AR Plane Manager". Assign a plane prefab (Unity's AR Foundation samples include a simple `ARFeatheredPlane` or you can use a basic plane visual — a simple flat quad with a semi-transparent material works fine for a student project) to its "Plane Prefab" field.
8. **Add AR Raycast Manager**: On the same XR Origin object → Add Component → "AR Raycast Manager".
9. **Attach AR scripts to XR Origin**: Add `ARPlaneController.cs`, `ARPlacementManager.cs`, and `BalloonInteraction.cs` to the XR Origin object (see each script's header comment for exact Inspector wiring).
10. **Create the Balloon prefab**:
    - Hierarchy → 3D Object → Sphere. Rename it "Balloon".
    - Scale it down to a believable size, e.g. (0.1, 0.13, 0.1).
11. **Add Rigidbody** to the Balloon object (Add Component → Rigidbody). Leave default values — `Balloon.cs` sets `isKinematic` and `useGravity` in code at runtime (see script comments for why).
12. **Add Collider**: The Sphere already has a Sphere Collider by default — keep "Is Trigger" **unchecked** (see `Balloon.cs` comments for why a non-trigger collider is used).
13. **Set the Balloon's Layer**: In Project Settings → Tags and Layers, add a new Layer called "Balloon". Set the Balloon GameObject's Layer to "Balloon" (this is what `BalloonInteraction.cs`'s `balloonLayer` field filters against).
14. **Create balloon material/colors**: Create a simple Standard/URP Lit material in `Materials/`, drag it onto the Balloon's Mesh Renderer. `Balloon.cs` will instance and tint this material per-balloon at runtime.
15. **Attach `Balloon.cs`** to the Balloon object. Drag your `PopEffect` prefab (created next) into its `popEffectPrefab` field, and an `AudioClip` into `popSound` if desired. Drag the Balloon object into `Assets/Prefabs/` to make it a prefab, then delete the scene instance.
16. **Create the particle effect**:
    - Hierarchy → Effects → Particle System. Rename it "PopEffect".
    - Set a short burst-style effect (Duration ~0.5s, Emission → Bursts, small Start Lifetime, bright colors).
    - Set "Play On Awake" = true (or leave it, since `PopEffect.cs` calls `Play()` manually — either is fine).
17. **Add audio**: Add Component → Audio Source to the PopEffect object. Set "Play On Awake" = false (the script controls playback). Assign a short pop sound clip. If you don't have one, Unity's built-in `Assets/Audio` samples or any short royalty-free "pop"/"click" sound (a few hundred milliseconds) works — search "free pop sound effect" for a CC0 source, or record a quick mouth-pop sound as a placeholder.
18. **Attach `PopEffect.cs`** to the PopEffect object. Drag it into `Assets/Prefabs/` as a prefab, then delete the scene instance.
19. **Create the BalloonSpawner object**: Hierarchy → Create Empty → rename "BalloonSpawner". Attach `BalloonSpawner.cs`. Drag the Balloon prefab into its `balloonPrefab` field, and set 4-6 `balloonColors` in the Inspector array.
20. **Create the GameManager object**: Hierarchy → Create Empty → rename "GameManager". Attach `GameManager.cs`. No fields required.
21. **Create the UIManager object and Canvas UI** (see Phase 5 below), attach `UIManager.cs`, and wire its fields.
22. **Cross-wire the manager references**:
    - On XR Origin's `ARPlacementManager` component: drag `ARRaycastManager` (same object), `ARPlaneController` (same object), and `BalloonSpawner` (from the scene) into their fields.
    - On XR Origin's `BalloonInteraction` component: drag `ARPlacementManager` (same object) into its field, set `balloonLayer` to the "Balloon" layer, and drag the AR Camera (child of XR Origin) into `arCamera`.
23. **Building and running on Android**:
    - Connect your Android device via USB with Developer Mode + USB Debugging enabled.
    - File → Build Settings → Add Open Scenes → select Android → Build And Run.
    - Ensure your device supports ARCore (check Google's ARCore supported devices list).

## PHASE 4 – Prefab and Inspector Configuration Summary

| Script | Attached To | Key Inspector Fields |
|---|---|---|
| `ARPlaneController.cs` | XR Origin | (auto-finds ARPlaneManager on same object) |
| `ARPlacementManager.cs` | XR Origin | arRaycastManager, arPlaneController, balloonSpawner |
| `BalloonInteraction.cs` | XR Origin | placementManager, arCamera, balloonLayer |
| `BalloonSpawner.cs` | BalloonSpawner (empty GO) | balloonPrefab, balloonCount, spawnRadius, balloonColors |
| `Balloon.cs` | Balloon prefab | popEffectPrefab, popSound, floatSpeed, floatHeight |
| `PopEffect.cs` | PopEffect prefab | lifetime |
| `GameManager.cs` | GameManager (empty GO) | — |
| `UIManager.cs` | UIManager (empty GO, under Canvas) | instructionText, scoreText, completionPanel, gameplayPanel, restartButton |

## PHASE 5 – UI Setup

1. Hierarchy → UI → Canvas (set Render Mode = Screen Space - Overlay).
2. Add a **Text** element at the top: content "AR BALLOON POP" (title, static, no script needed).
3. Add a **Text** element for score: leave text empty at design time, `UIManager` will set it to "Popped: 0 / 8" at runtime. Assign to `UIManager.scoreText`.
4. Add a **Text** element for instructions: default text "Move your phone to detect a surface." Assign to `UIManager.instructionText`.
5. Add a **Panel** named "CompletionPanel" containing a Text ("Great Job! All Balloons Popped!") and a **Button** named "RestartButton" (text: "Restart"). Set the panel inactive by default in the Inspector (its `SetActive(false)` is also handled in code at Start). Assign panel to `UIManager.completionPanel` and button to `UIManager.restartButton`.
6. Optionally add a "GameplayPanel" wrapping the score text, inactive until balloons spawn — assign to `UIManager.gameplayPanel`.
7. Add the `UIManager.cs` script to an empty child GameObject of the Canvas (or the Canvas itself) and drag all the above references into its fields.
8. For mobile readability: set Canvas Scaler → UI Scale Mode = "Scale With Screen Size", Reference Resolution 1080x1920, and use font sizes ≥ 36 for titles, ≥ 28 for body text.

## Common Errors & Fixes
- **"NullReferenceException" on UIManager.Instance** — Make sure the UIManager GameObject exists in the scene and its script is enabled before any other script's `Start()` runs; Unity's default execution order usually handles this fine, but if not, set UIManager's Script Execution Order earlier in Project Settings.
- **Balloons don't respond to taps** — Check that the Balloon prefab's Layer is set to "Balloon" AND that `BalloonInteraction.balloonLayer` is set to that same layer in the Inspector.
- **Plane never detected** — Make sure you're testing on a real ARCore-supported Android device (the Unity Editor alone cannot simulate real-world plane detection), in a well-lit room with enough surface texture (avoid plain white floors/tables).
- **Balloons spawn inside each other** — Increase `minSpacing` or `spawnRadius` on `BalloonSpawner`.
- **Build fails on Android** — Double check Player Settings → Other Settings → Package Name is set (e.g. com.yourname.arballoonpop) and Minimum API Level meets ARCore's requirement.
