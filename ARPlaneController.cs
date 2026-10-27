using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

/// <summary>
/// ARPlaneController.cs
/// ---------------------------------------------------------
/// ATTACH TO: The "XR Origin" GameObject (the same object that has the
///            ARPlaneManager component on it).
/// INSPECTOR FIELDS:
///   - arPlaneManager : drag the ARPlaneManager component from the same object.
///     (You can also leave it empty; GetComponent will find it automatically.)
///
/// RESPONSIBILITY:
/// Watches ARPlaneManager's plane events. While no plane has been found yet,
/// it keeps showing the "move your phone" instruction (via UIManager). The
/// moment at least one plane is detected, it flips CurrentState so that
/// ARPlacementManager knows tapping is now allowed.
///
/// COMMUNICATION:
/// - Reads events from ARPlaneManager (AR Foundation package).
/// - Tells UIManager to show/hide the scanning instruction.
/// - Exposes a public bool "PlaneFound" that ARPlacementManager checks
///   before it accepts a tap-to-place.
/// </summary>
[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneController : MonoBehaviour
{
    private ARPlaneManager arPlaneManager;

    /// <summary>True as soon as AR Foundation has detected at least one plane.</summary>
    public bool PlaneFound { get; private set; } = false;

    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        // AR Foundation fires this event whenever planes are added, updated, or removed.
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // As soon as we have ANY tracked plane, tell the user they can tap.
        if (!PlaneFound && args.added != null && args.added.Count > 0)
        {
            PlaneFound = true;
            if (UIManager.Instance != null)
                UIManager.Instance.ShowInstruction("Tap the surface to place balloons.");
        }
    }

    /// <summary>
    /// Once balloons have been placed we no longer need plane meshes covering
    /// the floor with a grid/dots visual. Call this from ARPlacementManager
    /// after a successful placement to declutter the AR view (optional).
    /// </summary>
    public void HidePlaneVisuals()
    {
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        // Stop detecting/re-drawing new planes once we've placed our balloons.
        arPlaneManager.enabled = false;
    }
}
