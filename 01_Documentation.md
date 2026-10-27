# AR Balloon Pop – Project Documentation (IA3)

## 1. Introduction
AR Balloon Pop is a mobile Augmented Reality game built with Unity and AR Foundation. The player scans a real-world horizontal surface, taps it to place a cluster of balloons, and pops them by tapping the screen. The project demonstrates AR surface detection, touch-based interaction, physics-driven object behavior, particle effects, and audio feedback in a small, self-contained gameplay loop.

## 2. Problem Statement
Most beginner AR demos only place a static 3D object on a detected surface with no interaction, physics, or feedback loop. This project addresses that gap by combining AR plane detection with a simple, physically-motivated interaction (floating, tappable, poppable balloons) and a complete game loop (score, completion, restart), making it suitable for demonstrating both AR fundamentals and basic game physics in one project.

## 3. Objectives
- Detect a real-world horizontal surface using AR Foundation.
- Allow the user to place virtual balloons on that surface via touch.
- Give balloons controlled floating motion using Unity physics/transform logic.
- Let the user pop balloons via touch, with particle and audio feedback.
- Track and display score, and provide a restart flow.
- Keep the codebase modular and readable for a Software Engineering evaluation.

## 4. Game Concept
The player acts as a "balloon popper" inside their own room. After scanning the floor, a colorful cluster of balloons appears, gently bobbing in place. The player taps each balloon to pop it, seeing a particle burst and hearing a pop sound, until all balloons are cleared and a completion message appears.

## 5. Functional Requirements
1. The system shall start the device camera and AR session on launch.
2. The system shall detect horizontal planes in the real world.
3. The system shall display an on-screen instruction while no plane is detected.
4. The system shall allow the user to tap a detected plane to place balloons.
5. The system shall spawn between 5 and 10 balloons above the tapped position.
6. The system shall give each balloon a randomized position, scale, and color.
7. The system shall prevent balloons from spawning inside one another.
8. The system shall animate balloons with a continuous floating motion.
9. The system shall detect a tap on an individual balloon via raycasting.
10. The system shall play a particle effect and sound when a balloon is popped.
11. The system shall increase the score by 1 for each popped balloon.
12. The system shall display a completion message when all balloons are popped.
13. The system shall provide a Restart button that resets the game.

## 6. Non-Functional Requirements
1. **Usability** – Instructions must be visible and understandable without prior explanation.
2. **Performance** – The game shall maintain a playable frame rate (target 30+ FPS) on a mid-range Android device.
3. **Portability** – The project must build and run on Android devices supporting ARCore.
4. **Maintainability** – Code shall be split into single-responsibility scripts with inline comments.
5. **Reliability** – A single tap shall never trigger two conflicting actions (e.g., placing balloons AND popping one).

## 7. System Architecture
```
User
 ↓
Touch Input (BalloonInteraction.cs)
 ↓
 ├─ hits a balloon → Physics.Raycast → Balloon.Pop()
 │        ↓                                ↓
 │   PopEffect.cs                    GameManager.cs
 │   (particle + sound)              (score, state)
 │                                         ↓
 │                                    UIManager.cs
 │                                    (updates score/completion text)
 │
 └─ hits no balloon → ARPlacementManager.cs → AR Raycast
                              ↓
                       BalloonSpawner.cs
                              ↓
                        Balloon prefab instances
                              ↓
                       GameManager.RegisterBalloons()
```

## 8. Architecture Diagram (as specified)
```
User
↓
Touch Input
↓
AR Raycast / Physics Raycast
↓
AR Placement Manager
↓
Balloon Spawner
↓
Balloon
↓
Pop Effect
↓
Game Manager
↓
UI Manager
```

## 9. Game Flowchart
```
START
↓
SCAN ENVIRONMENT (ARPlaneController)
↓
PLANE DETECTED
↓
USER TAPS PLANE (BalloonInteraction → ARPlacementManager)
↓
SPAWN BALLOONS (BalloonSpawner)
↓
BALLOONS FLOAT (Balloon.Update)
↓
USER TAPS BALLOON (BalloonInteraction → Balloon.Pop)
↓
POP EFFECT + SOUND (PopEffect)
↓
SCORE +1 (GameManager.BalloonPopped)
↓
MORE BALLOONS? ── YES ──→ CONTINUE (loop back to "BALLOONS FLOAT")
   │
   NO
   ↓
GAME COMPLETE (UIManager.ShowCompletionUI)
↓
RESTART (GameManager.RestartGame)
```

## 10. Technologies Used
- Unity (2022 LTS or newer recommended)
- AR Foundation package
- ARCore XR Plugin (Android)
- C# scripting
- Unity Physics (Rigidbody, Collider, Physics.Raycast)
- Unity Particle System
- Unity Audio Source
- Unity Canvas UI (uGUI)

## 11. Physics Implementation
See inline comments in `Balloon.cs`. Summary:
- **Rigidbody** on each balloon is set `isKinematic = true` so the object still participates in collision/raycast detection but is not affected by physics forces.
- **Gravity is disabled** (`useGravity = false`) because the balloon should float, not fall — controlled floating is implemented manually with a sine-wave offset on the Transform in `Update()`, not by manipulating physics forces.
- **Sphere Collider** (non-trigger) enables `Physics.Raycast` from the input script to detect exactly which balloon was tapped.
- **Object destruction**: `Destroy(gameObject)` removes a popped balloon; `PopEffect.cs` uses a timed `Destroy(gameObject, lifetime)` to clean up the particle/sound object after it finishes playing.

## 12. AR Implementation
- `ARPlaneManager` (AR Foundation) detects horizontal planes and raises a `planesChanged` event, handled in `ARPlaneController.cs`.
- `ARRaycastManager` performs a raycast against `TrackableType.PlaneWithinPolygon` in `ARPlacementManager.cs`, converting a 2D tap into a real-world 3D position for balloon placement — the user never enters coordinates manually.

## 13. Touch Interaction
All touch input is centralized in `BalloonInteraction.cs`. Every tap first does a **normal Unity physics raycast** filtered to a "Balloon" layer; if that hits a balloon, `Pop()` is called and the function returns immediately. Only if no balloon was hit does the tap get forwarded to `ARPlacementManager` for plane placement — guaranteeing a single tap can never do both actions.

## 14. Implementation
See the `Scripts/` folder for all 8 C# scripts, each documented individually in `02_Setup_Guide.md` with attachment target, Inspector fields, and cross-script communication notes.

## 15–18. Testing
See `03_Testing.md` for the full test case table and an explanation of unit, integration, and system testing performed.

## 19. Results
(Fill in after running on your device — e.g. "Plane detection took ~2 seconds on average; all 8 balloons could be popped in under 20 seconds; particle and audio feedback fired correctly on every pop during 3 test runs.")

## 20. Limitations
- Only horizontal plane detection is supported (no vertical surfaces).
- Balloon placement is a single fixed round per session (no re-placement without restart).
- No persistence of high scores between sessions.
- Designed and tested for a single-player, single-device experience only.

## 21. Future Enhancements
- Add difficulty levels (more balloons, faster floating).
- Add a timer-based scoring mode.
- Support vertical plane placement (e.g., balloons on a wall).
- Save high scores locally using PlayerPrefs.
- Add balloon variety (different sizes/point values).

## 22. Conclusion
AR Balloon Pop demonstrates a complete, modular AR mobile game loop — from surface detection and placement through physics-driven interaction to score tracking and restart — using only Unity's built-in AR Foundation and physics systems, without unnecessary complexity such as networking or backend services.

---

## Evaluation Rubric Mapping
- **Game Concept & Design (10):** Sections 2–4, 9.
- **Physics & Interaction (10):** Section 11, `Balloon.cs`, `PopEffect.cs`.
- **Immersive Input & AR Functionality (10):** Sections 12–13, `ARPlaneController.cs`, `ARPlacementManager.cs`, `BalloonInteraction.cs`.
- **Demonstration, Documentation & Presentation (5):** This document + `04_Demo_Video_Script.md`.
- **Teamwork & Certification (5):** Not implemented in code. For documentation, include: individual contribution statements (even for solo work, describe role/phases owned), any relevant online course certificates (e.g., Unity Learn, AR Foundation tutorials) you completed while preparing this project, and a short reflection on what you learned. If this was a team project, include a signed contribution table and meeting/version-control (Git) history as evidence.
