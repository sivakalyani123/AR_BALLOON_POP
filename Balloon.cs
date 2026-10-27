using UnityEngine;

/// <summary>
/// Balloon.cs
/// ---------------------------------------------------------
/// ATTACH TO: The Balloon prefab root object (Assets/Prefabs/Balloon.prefab).
/// REQUIRED COMPONENTS ON THE SAME OBJECT:
///   - Rigidbody      (see physics settings explained below)
///   - Sphere Collider (Is Trigger = false, see explanation below)
///   - MeshRenderer / MeshFilter (a Sphere primitive works fine)
/// INSPECTOR FIELDS:
///   - popEffectPrefab : drag the PopEffect prefab.
///   - popSound        : (optional) an AudioClip; only needed if you don't
///                        want to rely on the AudioSource on PopEffect.
///   - floatSpeed / floatHeight : control the up-and-down bobbing motion.
///
/// PHYSICS DESIGN DECISIONS (explained per the project's requirement that
/// every Rigidbody/Collider choice be justified in comments):
///
/// - Rigidbody.isKinematic = true:
///   The balloon's movement (floating up and down) is done by directly
///   scripting the Transform's position in Update(), NOT by applying
///   physics forces. If the Rigidbody were non-kinematic, Unity's physics
///   engine would fight our manual position changes and gravity would pull
///   the balloon down uncontrollably, which the spec explicitly says to avoid
///   ("does not fall uncontrollably"). Kinematic still lets the Rigidbody take
///   part in collision detection, which we need for the Collider to register
///   Physics.Raycast hits reliably and to look "physically present" in the scene.
///
/// - Rigidbody.useGravity = false:
///   Since the balloon is kinematic and we move it manually, gravity is
///   switched off, otherwise it would have no visible effect anyway but is
///   turned off explicitly to document the design intent: this object is
///   NOT meant to fall like a normal physics object, it floats like a real
///   helium balloon.
///
/// - Sphere Collider (non-trigger):
///   A normal (non-trigger) collider is used so that Physics.Raycast fired
///   from BalloonInteraction.cs can detect it directly, which is the
///   simplest and most reliable way to implement "tap to pop" for a
///   beginner project. (A trigger collider would only fire OnTriggerEnter
///   when another collider physically enters it — that is useful for
///   things like balloons bumping each other, not for tap-detection, so we
///   do not use Is Trigger here.)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Balloon : MonoBehaviour
{
    [Header("Effects")]
    public GameObject popEffectPrefab;
    public AudioClip popSound;

    [Header("Floating Motion")]
    public float floatSpeed = 1.2f;
    public float floatHeight = 0.05f;

    private Vector3 startPosition;
    private float randomOffset; // makes each balloon bob out of sync with the others
    private bool isPopped = false;

    private Rigidbody rb;
    private Renderer balloonRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        balloonRenderer = GetComponentInChildren<Renderer>();

        // See class-level comment block above for why these are set this way.
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, Mathf.PI * 2f); // random phase so balloons don't bob in sync
    }

    /// <summary>Called once by BalloonSpawner right after Instantiate().</summary>
    public void Init(Color color)
    {
        if (balloonRenderer != null)
        {
            // Instantiate a fresh material instance so each balloon can have
            // its own color without affecting other balloons that share a prefab material.
            balloonRenderer.material = new Material(balloonRenderer.material);
            balloonRenderer.material.color = color;
        }
    }

    private void Update()
    {
        if (isPopped) return;

        // Simple sine-wave floating motion — gentle up/down bob.
        // This is the "controlled floating motion" mentioned in the spec,
        // used INSTEAD OF gravity/physics forces (see Awake() comment).
        float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    /// <summary>
    /// Called by BalloonInteraction.cs when Physics.Raycast hits this balloon.
    /// </summary>
    public void Pop()
    {
        if (isPopped) return; // guard against double-pop from the same frame
        isPopped = true;

        // 1. Spawn the pop particle + sound effect at this balloon's position.
        if (popEffectPrefab != null)
        {
            GameObject effect = Instantiate(popEffectPrefab, transform.position, Quaternion.identity);
            PopEffect popEffect = effect.GetComponent<PopEffect>();
            if (popEffect != null && popSound != null)
                popEffect.SetSound(popSound);
        }

        // 2. Tell GameManager the score changed.
        if (GameManager.Instance != null)
            GameManager.Instance.BalloonPopped();

        // 3. Remove the balloon itself immediately (the pop effect is a
        //    separate object so it can finish playing after this is destroyed).
        Destroy(gameObject);
    }
}
