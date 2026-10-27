using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager.cs
/// ---------------------------------------------------------
/// ATTACH TO: An empty GameObject named "UIManager" (can be a child of the Canvas).
/// INSPECTOR FIELDS (drag these from your Canvas hierarchy):
///   - instructionText   : Text  (e.g. "Move your phone to detect a surface")
///   - scoreText         : Text  (e.g. "Popped: 0 / 10")
///   - completionPanel   : GameObject (panel shown when all balloons are popped)
///   - restartButton     : Button (child of completionPanel, or always visible)
///   - gameplayPanel     : GameObject (optional, holds scoreText, shown once playing)
///
/// RESPONSIBILITY:
/// UIManager ONLY touches UI elements. It never spawns balloons or does AR
/// logic itself — other scripts call its public methods to tell it what to show.
/// This is why ARPlaneController, BalloonSpawner, and GameManager all just
/// call "UIManager.Instance.SomeMethod()" instead of finding Text objects themselves.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Text Elements")]
    public Text instructionText;
    public Text scoreText;

    [Header("Panels")]
    public GameObject completionPanel; // contains "Great Job!" message
    public GameObject gameplayPanel;   // optional, holds score UI during play

    [Header("Buttons")]
    public Button restartButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Initial UI state: scanning for a plane, no score yet, no completion panel.
        if (completionPanel != null) completionPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        ShowInstruction("Move your phone to detect a surface.");

        // Hook the Restart button here in code so you don't forget to wire it
        // in the Inspector. GameManager.Instance may not exist yet at Awake,
        // so we hook it in Start instead.
        if (restartButton != null && GameManager.Instance != null)
        {
            restartButton.onClick.AddListener(GameManager.Instance.RestartGame);
        }
    }

    /// <summary>Called by ARPlaneController while the user is still scanning.</summary>
    public void ShowInstruction(string message)
    {
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = message;
        }
    }

    /// <summary>Called by ARPlacementManager once the user taps a valid plane.</summary>
    public void HideInstruction()
    {
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);
    }

    /// <summary>Called by GameManager.RegisterBalloons() when balloons spawn.</summary>
    public void ShowGameplayUI()
    {
        HideInstruction();
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }

    /// <summary>Called by GameManager.BalloonPopped() every time the score changes.</summary>
    public void UpdateScoreText(int score, int total)
    {
        if (scoreText != null)
            scoreText.text = $"Popped: {score} / {total}";
    }

    /// <summary>Called by GameManager when remainingBalloons reaches 0.</summary>
    public void ShowCompletionUI()
    {
        if (completionPanel != null) completionPanel.SetActive(true);
    }
}
