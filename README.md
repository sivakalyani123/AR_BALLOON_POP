# AR Balloon Pop

**An Augmented Reality Physics-Based Interactive Game built with Unity, AR Foundation and ARCore.**

Skill-Based Evaluation project for **25AI2011 – Immersive Technologies and Extended Reality Systems**
Karunya Institute of Technology and Sciences, Division of Artificial Intelligence and Machine Learning.

---

## About the Game

AR Balloon Pop is an endless mobile AR game. The camera shows the real world, and when you tap **START**, coloured balloons rise one after another in front of you towards a red **thorn bar**.

- Tap a balloon to **pop** it and score a point.
- If a balloon touches the thorns, you **lose a life**.
- You have **3 lives**. When they run out, the game is over and your final score is shown.
- Balloons get faster the longer you play.

## Screenshots

> Add your screenshots to a `screenshots/` folder and link them here.

| Start screen | Balloons rising | Game Over |
|---|---|---|
| ![Start](screenshots/start.png) | ![Playing](screenshots/playing.png) | ![Game Over](screenshots/gameover.png) |

## Features

- Live AR camera view on Android using AR Foundation and ARCore
- Balloons spawn one by one, with random colour, size and position (never inside each other)
- Real Rigidbody physics: gravity, buoyancy force, air drag, wind, collisions
- Tap-to-pop touch input using `Physics.Raycast`
- Thorn bar built from a **trigger collider** that detects balloons touching it
- Pop effect with Particle System and sound (Audio Source)
- Score, lives, Game Over panel and Restart button
- Difficulty increases over time (faster balloons, shorter spawn gaps)

## Physics Used

| Concept | How it is used |
|---|---|
| Rigidbody | Dynamic body, mass 0.05 kg, moved by the physics engine |
| Gravity | Pulls each balloon down |
| Buoyancy (`AddForce`) | Upward lift cancels gravity, plus extra lift |
| Drag | Air resistance gives a steady rise speed (terminal velocity) |
| Wind | Small sideways force from Perlin noise |
| Collider + collision | Sphere colliders make balloons bump into each other |
| Trigger | The thorn bar trigger detects a balloon entering it |
| Explosion force | A pop pushes nearby balloons away |
| Destruction | `Destroy()` removes popped balloons and finished effects |

## Tech Stack

- Unity (URP) – 2022.3 LTS or Unity 6
- AR Foundation and ARCore XR Plugin
- C#
- Unity Particle System, Audio Source, Canvas UI

## Project Structure

```
Assets/
├── Scenes/        AR scene
├── Scripts/       C# game scripts
├── Prefabs/       Balloon and PopEffect prefabs
├── Materials/     Balloon material
├── Audio/         Pop sound
└── Settings/      URP render pipeline settings
```

### Scripts

| Script | Job |
|---|---|
| `GameManager.cs` | Game state, score, lives, start, game over, restart |
| `UIManager.cs` | Shows score, lives, instructions and the Game Over panel |
| `BalloonSpawner.cs` | Spawns balloons in front of the camera and builds the thorn bar |
| `Balloon.cs` | Balloon physics, popping and thorn hit |
| `ThornBar.cs` | Thorn bar visuals and trigger collision |
| `PopEffect.cs` | Plays the particle burst and sound, then destroys itself |
| `BalloonInteraction.cs` | Reads touch input and raycasts to find the tapped balloon |
| `StartButtonHelper.cs` | Connects the START button to the game |
| `LivesDisplay.cs` | Keeps the Lives text up to date |

## Architecture

```
Touch Input (BalloonInteraction)
        |
        v
Balloon  <---  BalloonSpawner  <---  GameManager  --->  UIManager
   |                                     ^
   +--- ThornBar (trigger) --------------+
   |
   +--- PopEffect (particles + sound)
```

## Game Flow

```
Open app and camera  ->  Tap START  ->  Balloons spawn and rise
        ->  Tap balloon: pop effect, sound, score +1
        ->  Balloon reaches thorns: life -1
        ->  Lives = 0: Game Over  ->  Restart
```

## Requirements

- Unity (2022.3 LTS or Unity 6) with **Android Build Support**
- An **ARCore-supported Android phone** ([supported devices](https://developers.google.com/ar/devices))
- USB cable and **USB debugging** turned on
- Google Play Services for AR (the phone asks to install it if needed)

## How to Run

1. Clone the repository:
   ```
   git clone https://github.com/<your-username>/<your-repo>.git
   ```
2. Open the folder in **Unity Hub** (Add project from disk).
3. Open the scene in `Assets/Scenes`.
4. **File > Build Settings > Android > Switch Platform**.
5. **Project Settings > XR Plug-in Management > Android**: tick **ARCore**.
6. **Project Settings > Player > Other Settings**:
   - Scripting Backend: **IL2CPP**, Target Architecture: **ARM64**
   - Remove **Vulkan** from the Graphics APIs list (keep OpenGLES3)
   - Active Input Handling: **Both**
7. Connect the phone, then **File > Build Settings > Build And Run**.

## How to Play

1. Open the app and allow the camera.
2. Point the phone ahead and tap **START**.
3. Tap the balloons before they reach the thorn bar.
4. When all 3 lives are gone, check your score and tap **Restart**.

## Troubleshooting

| Problem | Fix |
|---|---|
| Camera shows a solid colour | Add **AR Background Renderer Feature** to the URP Renderer Data |
| Build fails about Vulkan and API level | Remove Vulkan from the Graphics APIs list |
| START button does nothing | Check the EventSystem and that `GameManager` has the Balloon Spawner assigned |
| Lives or score text does not change | Check the text fields assigned on `UIManager` |
| Balloons cannot be popped | Check the Balloon prefab layer and the Balloon Layer field on `BalloonInteraction` |

## Limitations

- Needs an ARCore-supported Android phone.
- Balloons are placed in front of the camera, not on a detected plane.
- No saved high score; single player only.

## Future Enhancements

- AR plane detection and AR raycast placement
- Animator for balloon spawn and pop animations
- High score saved with PlayerPrefs
- Difficulty levels and bonus balloons
