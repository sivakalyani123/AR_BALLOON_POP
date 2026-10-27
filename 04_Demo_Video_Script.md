# AR Balloon Pop – Demo Video Script (2–3 minutes)

**Setup note:** Record with your phone in landscape or portrait (match how you built it), good lighting, and a textured floor/table (avoid plain white surfaces — ARCore needs visual detail to track).

---

**[0:00–0:15] Opening the application**
*Show: App icon, tap to launch, camera feed appears.*
Say: "This is AR Balloon Pop, an augmented reality game built in Unity using AR Foundation and ARCore. Let's start the app and scan the environment."

**[0:15–0:35] Scanning the environment / Detecting a plane**
*Show: Slowly pan the phone across the floor/table; plane visualization appears.*
Say: "The app is now scanning for a horizontal surface using AR Foundation's Plane Manager. You can see the on-screen instruction telling me to move my phone until a surface is detected."

**[0:35–0:50] Selecting the plane**
*Show: Instruction changes to "Tap the surface to place balloons"; tap the floor.*
Say: "Now that a plane has been detected, I'll tap the surface to place the balloons at this real-world position using an AR raycast."

**[0:50–1:05] Balloons appearing and floating**
*Show: Balloon cluster spawns, gently bobbing.*
Say: "A group of balloons has spawned above the tapped point, each with a random color and slight scale variation. Notice they float gently — this uses a kinematic Rigidbody with scripted motion instead of gravity."

**[1:05–2:00] Tapping balloons / pop animation / sound / score**
*Show: Tap several balloons one at a time; particle burst + sound each time; score UI updating.*
Say: "When I tap a balloon, a Physics Raycast detects the hit, triggering a particle pop effect, a sound, and incrementing my score, which you can see updating here at the top."

**[2:00–2:20] Popping the final balloon / completion message**
*Show: Pop the last balloon; completion panel appears.*
Say: "As I pop the final balloon, the game detects that all balloons are cleared and displays the completion message."

**[2:20–2:40] Restarting the game**
*Show: Tap Restart button; scene reloads back to scanning state.*
Say: "Tapping Restart reloads the scene, resetting the AR session, the score, and letting me place a new round of balloons. That completes the full gameplay loop for AR Balloon Pop."

**[2:40–2:50] Closing**
Say: "This project demonstrates AR plane detection, raycast-based placement and interaction, physics-driven floating motion, particle and audio feedback, and a complete score/restart game loop, all built with modular, reusable Unity scripts."

---

**Total runtime target: ~2:50, within the 2–5 minute demo requirement.**
