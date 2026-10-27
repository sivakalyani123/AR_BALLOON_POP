using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BalloonSpawner.cs
/// ---------------------------------------------------------
/// ATTACH TO: An empty GameObject named "BalloonSpawner" in the scene.
/// INSPECTOR FIELDS:
///   - balloonPrefab   : drag the Balloon prefab (Assets/Prefabs/Balloon.prefab).
///   - balloonCount    : how many balloons per round (default 8, spec asks 5-10).
///   - spawnRadius     : how far from the center point balloons can appear.
///   - minSpacing      : minimum distance between two balloons so they
///                       don't spawn inside each other.
///   - balloonColors   : array of Color values used to give balloons variety
///                       (assign 4-6 colors in the Inspector).
///
/// RESPONSIBILITY:
/// Given a single world-space position (the point the user tapped on the
/// floor), generate a small cluster of balloons above it with randomized
/// position, scale, and color — without overlapping each other. Once done,
/// it reports the total count to GameManager.
///
/// COMMUNICATION:
/// - Called once by ARPlacementManager.TryPlaceBalloons(position).
/// - Instantiates Balloon prefabs and calls Balloon.Init(color).
/// - Calls GameManager.Instance.RegisterBalloons(count) when finished.
/// </summary>
public class BalloonSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject balloonPrefab;

    [Header("Spawn Settings")]
    [Range(5, 10)] public int balloonCount = 8;
    public float spawnRadius = 0.35f;      // meters, keep small for a tabletop-size AR scene
    public float heightAboveSurface = 0.15f;
    public float minSpacing = 0.12f;       // meters between balloon centers

    [Header("Visual Variety")]
    public Color[] balloonColors;

    /// <summary>
    /// Spawns balloonCount balloons in a loose cluster above centerPosition.
    /// Uses simple rejection sampling to avoid balloons overlapping.
    /// </summary>
    public void SpawnBalloons(Vector3 centerPosition)
    {
        List<Vector3> placedPositions = new List<Vector3>();
        int spawned = 0;
        int safetyAttempts = 0; // prevents an infinite loop if space is too tight

        while (spawned < balloonCount && safetyAttempts < balloonCount * 20)
        {
            safetyAttempts++;

            // Random point inside a horizontal circle, then lift it up
            // above the surface so balloons appear to float above the floor.
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = centerPosition +
                new Vector3(randomCircle.x, heightAboveSurface + Random.Range(0f, 0.15f), randomCircle.y);

            // Reject if too close to an already-placed balloon.
            bool tooClose = false;
            foreach (Vector3 existing in placedPositions)
            {
                if (Vector3.Distance(existing, candidate) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            placedPositions.Add(candidate);
            SpawnSingleBalloon(candidate);
            spawned++;
        }

        // Tell GameManager how many balloons exist so the score UI (e.g. "0 / 8") is correct.
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterBalloons(spawned);
    }

    private void SpawnSingleBalloon(Vector3 position)
    {
        GameObject balloonObj = Instantiate(balloonPrefab, position, Quaternion.identity);

        // Slight random scale for visual variety (spec: "slightly random scale").
        float scale = Random.Range(0.85f, 1.15f);
        balloonObj.transform.localScale *= scale;

        Balloon balloon = balloonObj.GetComponent<Balloon>();
        if (balloon != null && balloonColors != null && balloonColors.Length > 0)
        {
            Color randomColor = balloonColors[Random.Range(0, balloonColors.Length)];
            balloon.Init(randomColor);
        }
    }
}
