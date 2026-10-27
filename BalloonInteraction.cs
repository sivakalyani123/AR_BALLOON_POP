using UnityEngine;

/// <summary>
/// BalloonInteraction.cs
/// ---------------------------------------------------------
/// ATTACH TO: The "XR Origin" GameObject (same object as the other AR scripts),
///            OR a separate empty "InputManager" GameObject — either works,
///            just make sure only ONE object in the scene has this script.
/// INSPECTOR FIELDS:
///   - placementManager : drag the ARPlacementManager component.
///   - balloonLayer     : set this to the Layer you create for balloons
///                        (e.g. create a Layer called "Balloon" in
///                        Project Settings > Tags and Layers, assign it to
///                        the Balloon prefab, and select it here).
///
/// RESPONSIBILITY:
/// This is the SINGLE place in the whole project that reads touch input.
/// It decides, for every tap: "did the user tap an existing balloon, or
/// did they tap the floor to place new ones?" — and only one of those two
/// things happens per tap, which is exactly what the spec requires
/// ("make sure the same tap does not accidentally both place balloons and
/// pop a balloon").
///
/// HOW THE DECISION WORKS:
/// 1. Physics.Raycast (normal 3D physics, not AR raycast) is fired from the
///    camera through the tap position, filtered to the "Balloon" layer only.
/// 2. If it hits a balloon's Collider -> call that balloon's Pop() and STOP.
/// 3. If it hits nothing on the Balloon layer -> forward the tap to
///    ARPlacementManager, which tries an AR plane raycast instead.
///
/// COMMUNICATION:
/// - Calls Balloon.Pop() directly on whatever balloon was hit.
/// - Calls ARPlacementManager.TryPlaceBalloons() when no balloon was hit.
/// </summary>
public class BalloonInteraction : MonoBehaviour
{
    [Header("References")]
    public ARPlacementManager placementManager;
    public Camera arCamera; // usually the AR Camera under XR Origin

    [Header("Balloon Layer")]
    public LayerMask balloonLayer;

    private void Update()
    {
        Vector2 tapPosition;

        // Support both a phone touch and a mouse click (mouse click is handy
        // for quickly testing in the Unity Editor before building to Android).
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began) return; // only react once per tap
            tapPosition = touch.position;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            tapPosition = Input.mousePosition;
        }
        else
        {
            return; // no input this frame
        }

        HandleTap(tapPosition);
    }

    private void HandleTap(Vector2 screenPosition)
    {
        if (arCamera == null) arCamera = Camera.main;

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        // Step 1: did we tap a balloon? Only test against the Balloon layer.
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, balloonLayer))
        {
            Balloon balloon = hit.collider.GetComponent<Balloon>();
            if (balloon != null)
            {
                balloon.Pop();
                return; // stop here — do NOT also try to place balloons
            }
        }

        // Step 2: not a balloon, so try placing balloons on a detected plane.
        if (placementManager != null)
            placementManager.TryPlaceBalloons(screenPosition);
    }
}
