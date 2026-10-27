using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// ARPlacementManager.cs
/// ---------------------------------------------------------
/// ATTACH TO: The "XR Origin" GameObject (same object as ARRaycastManager
///            and ARPlaneController).
/// INSPECTOR FIELDS:
///   - arRaycastManager   : drag the ARRaycastManager component (same object).
///   - arPlaneController  : drag the ARPlaneController component (same object).
///   - balloonSpawner     : drag the BalloonSpawner component (from the
///                          BalloonSpawner empty GameObject in the scene).
///
/// RESPONSIBILITY:
/// Converts a 2D screen tap into a real-world 3D position on a detected
/// AR plane, using AR Foundation's raycasting against tracked planes
/// (NOT Unity's normal physics raycast — that is used only for tapping
/// balloons, in BalloonInteraction.cs). Once a valid position is found,
/// it hands that position to BalloonSpawner and locks placement so the
/// user can't spawn a second batch of balloons by tapping the floor again.
///
/// COMMUNICATION:
/// - Called by BalloonInteraction.cs ONLY when the tap did NOT hit a balloon.
/// - Calls BalloonSpawner.SpawnBalloons(position) once.
/// - Calls ARPlaneController.HidePlaneVisuals() after placement.
/// </summary>
public class ARPlacementManager : MonoBehaviour
{
    [Header("AR References")]
    public ARRaycastManager arRaycastManager;
    public ARPlaneController arPlaneController;

    [Header("Gameplay Reference")]
    public BalloonSpawner balloonSpawner;

    // Re-used every raycast call so we don't allocate a new List each frame.
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    /// <summary>True after balloons have already been spawned once.</summary>
    public bool HasPlaced { get; private set; } = false;

    /// <summary>
    /// Called by BalloonInteraction when a tap does not hit a balloon.
    /// screenPosition is the raw touch/mouse position in pixels.
    /// </summary>
    public void TryPlaceBalloons(Vector2 screenPosition)
    {
        // Don't allow placing twice, and don't allow placing before a plane exists.
        if (HasPlaced) return;
        if (arPlaneController != null && !arPlaneController.PlaneFound) return;

        // TrackableType.PlaneWithinPolygon = only hit the actual detected
        // plane surface (not infinite planes), which keeps balloons on real ground.
        if (arRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            HasPlaced = true;

            balloonSpawner.SpawnBalloons(hitPose.position);

            if (arPlaneController != null)
                arPlaneController.HidePlaneVisuals();
        }
    }
}
