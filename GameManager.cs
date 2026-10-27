using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager.cs
/// ---------------------------------------------------------
/// ATTACH TO: An empty GameObject named "GameManager" (create one in the scene).
/// INSPECTOR FIELDS: None required (UIManager finds this automatically),
///                    but you may drag a UIManager reference in if you prefer
///                    explicit wiring instead of FindObjectOfType.
///
/// RESPONSIBILITY (Software Engineering note):
/// This script owns all GAME STATE. It does not know anything about AR,
/// rendering, or UI widgets directly — it only stores numbers and fires
/// events / calls UIManager. This keeps "game rules" separate from
/// "how things look", which is the Single Responsibility Principle.
///
/// COMMUNICATION:
/// - BalloonSpawner calls GameManager.Instance.RegisterBalloons(count)
///   right after spawning, so GameManager knows the total.
/// - Balloon.cs calls GameManager.Instance.BalloonPopped() inside Pop().
/// - UIManager reads GameManager.Instance.score / totalBalloons to update text.
/// - The Restart button's OnClick() calls GameManager.Instance.RestartGame().
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton pattern: lets any script reach GameManager with
    // GameManager.Instance instead of doing FindObjectOfType every time.
    public static GameManager Instance { get; private set; }

    [Header("Game State (read-only at runtime, shown for debugging)")]
    public int score = 0;
    public int totalBalloons = 0;
    public int remainingBalloons = 0;

    // Simple state machine for the whole game.
    public enum GameState { Scanning, Placing, Playing, Completed }
    public GameState CurrentState { get; private set; } = GameState.Scanning;

    private void Awake()
    {
        // Enforce a single GameManager instance for the whole scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called once by BalloonSpawner right after it instantiates all balloons.
    /// This is how GameManager learns "how many balloons exist this round".
    /// </summary>
    public void RegisterBalloons(int count)
    {
        totalBalloons = count;
        remainingBalloons = count;
        score = 0;
        CurrentState = GameState.Playing;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(score, totalBalloons);
            UIManager.Instance.ShowGameplayUI();
        }
    }

    /// <summary>
    /// Called by Balloon.cs inside Pop(), AFTER the pop effect/sound has
    /// been triggered. Updates score and checks for game completion.
    /// </summary>
    public void BalloonPopped()
    {
        if (CurrentState != GameState.Playing) return;

        score++;
        remainingBalloons--;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateScoreText(score, totalBalloons);

        if (remainingBalloons <= 0)
        {
            CurrentState = GameState.Completed;
            if (UIManager.Instance != null)
                UIManager.Instance.ShowCompletionUI();
        }
    }

    /// <summary>
    /// Wired to the Restart button's OnClick() in the Inspector.
    /// Reloads the active scene, which resets AR session, balloons, and UI
    /// back to a clean state — simplest reliable restart for a student project.
    /// </summary>
    public void RestartGame()
    {
        CurrentState = GameState.Scanning;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
