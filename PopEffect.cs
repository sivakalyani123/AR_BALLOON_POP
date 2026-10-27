using UnityEngine;

/// <summary>
/// PopEffect.cs
/// ---------------------------------------------------------
/// ATTACH TO: The PopEffect prefab root object (Assets/Prefabs/PopEffect.prefab).
/// REQUIRED COMPONENTS ON THE SAME OBJECT:
///   - Particle System (set "Play On Awake" = true, "Stop Action" = Destroy
///     is optional since this script also handles destruction)
///   - Audio Source    (set "Play On Awake" = false; this script triggers it manually)
/// INSPECTOR FIELDS:
///   - lifetime : how many seconds before this object self-destroys
///                (should be >= the particle system's duration and the clip length).
///
/// RESPONSIBILITY:
/// A short-lived, self-contained "play effect and clean yourself up" object.
/// Balloon.cs instantiates this and forgets about it — PopEffect is
/// responsible for destroying itself so the scene doesn't accumulate
/// leftover empty GameObjects after many balloons have been popped.
/// </summary>
public class PopEffect : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 1.5f;

    private ParticleSystem popParticles;
    private AudioSource audioSource;

    private void Awake()
    {
        popParticles = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (popParticles != null)
            popParticles.Play();

        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();

        // Auto-destroy after "lifetime" seconds so the particle/sound has
        // time to finish before the object disappears.
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Optional: called by Balloon.cs if you want to override which sound
    /// clip plays (useful if you want variety between balloon colors, etc).
    /// </summary>
    public void SetSound(AudioClip clip)
    {
        if (audioSource != null)
            audioSource.clip = clip;
    }
}
