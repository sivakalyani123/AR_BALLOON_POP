# AR Balloon Pop – Testing

## Test Case Table

| ID | Test Case | Input | Expected Result | Actual Result | Status |
|----|-----------|-------|------------------|----------------|--------|
| TC01 | App launches | Open the app on Android | Camera feed appears, AR session starts | *(fill in after testing)* | Pending |
| TC02 | Camera starts | App running | Live camera passthrough visible behind UI | | Pending |
| TC03 | Plane detection | Move phone over a textured floor | A plane is detected and instruction updates to "Tap the surface to place balloons." | | Pending |
| TC04 | Plane selection | Tap on the detected plane | ARPlacementManager registers a valid hit position | | Pending |
| TC05 | Balloon spawning | After valid plane tap | 5–10 balloons appear above the tapped point, no overlap | | Pending |
| TC06 | Balloon floating | Observe balloons after spawn | Balloons gently bob up and down continuously | | Pending |
| TC07 | Balloon touch detection | Tap directly on a balloon | Physics.Raycast hits the balloon's collider | | Pending |
| TC08 | Balloon pop | Tap a balloon | Balloon.Pop() executes, balloon is destroyed | | Pending |
| TC09 | Particle effect | Pop a balloon | Particle burst plays at balloon's position | | Pending |
| TC10 | Audio effect | Pop a balloon | Pop sound plays once, no overlap/glitch | | Pending |
| TC11 | Score update | Pop a balloon | Score text increments by 1 (e.g. "1 / 8") | | Pending |
| TC12 | Game completion | Pop all balloons | Completion panel appears with message | | Pending |
| TC13 | Restart button | Tap Restart on completion panel | Scene reloads, AR session and score reset to 0 | | Pending |
| TC14 | Multiple balloons | Spawn round with 8 balloons | All 8 are individually tappable and poppable | | Pending |
| TC15 | Invalid/no plane tap | Tap an area with no detected plane before scanning completes | No balloons spawn, no crash, instruction remains visible | | Pending |

*(Fill in "Actual Result" and "Status" columns — Pass/Fail — after running the tests on a physical Android device.)*

## Unit Testing
Test individual scripts/functions in isolation:
- `Balloon.Pop()` — verify it doesn't run twice on rapid double-taps (isPopped guard).
- `BalloonSpawner.SpawnBalloons()` — verify it never places two balloons closer than `minSpacing`.
- `GameManager.BalloonPopped()` — verify score increments correctly and completion triggers only when `remainingBalloons` reaches exactly 0.
- `UIManager.UpdateScoreText()` — verify the text format matches "Popped: X / Y" for various inputs.

## Integration Testing
Test communication between modules:
- **AR placement → Balloon spawner**: confirm `ARPlacementManager.TryPlaceBalloons()` correctly passes a valid world position into `BalloonSpawner.SpawnBalloons()`.
- **Balloon → Game manager**: confirm each `Balloon.Pop()` call correctly reaches `GameManager.BalloonPopped()` exactly once.
- **Game manager → UI manager**: confirm score and completion state changes in `GameManager` are reflected immediately in `UIManager`'s on-screen text/panels.
- **Input arbiter**: confirm a single tap on a balloon never also triggers `ARPlacementManager.TryPlaceBalloons()` (this is the "same tap must not do both" requirement).

## System Testing
Run the complete game end-to-end on a physical Android device with ARCore support:
1. Fresh install → launch → scan → place → pop all balloons → see completion → restart → repeat.
2. Confirm frame rate stays smooth throughout (no noticeable stutter during balloon spawning or popping).
3. Confirm no crashes across at least 3 full play-throughs.
