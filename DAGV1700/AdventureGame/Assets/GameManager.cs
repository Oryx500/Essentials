using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int winningScore = 10; // Points needed to win and change scene
    public string winSceneName = "WinScene"; // Scene to load when player wins
    public bool checkScoreAutomatically = true; // Automatically check score changes
    
    [Header("Scene Transition")]
    public float sceneChangeDelay = 2.0f; // Delay before changing scenes
    public bool showTransitionMessages = true; // Show win message before scene change
    public TextMeshProUGUI transitionMessage; // UI text for transition messages
    public string winMessage = "You Win! Moving to next area...";
    
    [Header("Audio")]
    public AudioClip winSound; // Sound when player wins
    [Range(0f, 1f)] public float winSoundVolume = 1f;
    
    [Header("Debug")]
    public bool enableDebugKeys = true; // Enable debug keys for testing
    
    // Static reference for easy access from other scripts
    public static GameManager Instance;
    
    private AudioSource audioSource;
    private bool gameEnded = false; // Prevent multiple scene changes
    
    void Awake()
    {
        // Singleton pattern - only one GameManager should exist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Hide transition message initially
        if (transitionMessage != null)
        {
            transitionMessage.gameObject.SetActive(false);
        }
        
        // Subscribe to scene loaded event to reset game state
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("GameManager initialized for win condition tracking");
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset game state when returning to main scene (or any non-win scene)
        if (scene.name != winSceneName)
        {
            gameEnded = false;
            Debug.Log("GameManager: Reset game state for scene: " + scene.name);
        }
    }
    
    void Update()
    {
        if (gameEnded) return;
        
        // Check score automatically if enabled
        if (checkScoreAutomatically)
        {
            CheckWinCondition();
        }
        
        // Debug keys for testing
        if (enableDebugKeys)
        {
            // F1 - Add 1 point
            if (Input.GetKeyDown(KeyCode.F1))
            {
                MelonPoints.totalScore += 1;
                MelonPoints.UpdateAllScoreDisplays();
                Debug.Log("Debug: Added 1 point. Total: " + MelonPoints.totalScore);
            }
            
            // F4 - Force win
            if (Input.GetKeyDown(KeyCode.F4))
            {
                TriggerWin();
            }
        }
    }
    
    void CheckWinCondition()
    {
        if (MelonPoints.totalScore >= winningScore)
        {
            TriggerWin();
        }
    }
    
    public void TriggerWin()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        // Capture the timer value when winning
        Timer gameTimer = FindObjectOfType<Timer>();
        if (gameTimer != null)
        {
            gameTimer.CaptureCompletionTime();
            gameTimer.StopTimer();
            Debug.Log("Game completed in: " + Timer.GetLastCompletionTimeFormatted());
        }
        else
        {
            Debug.LogWarning("No Timer found in scene! Cannot capture completion time.");
        }
        
        Debug.Log("Player wins with " + MelonPoints.totalScore + " points!");
        
        // Play win sound
        if (winSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winSound, winSoundVolume);
        }
        
        // Show win message and change scene
        StartCoroutine(HandleSceneTransition(winMessage, winSceneName));
    }
    
    IEnumerator HandleSceneTransition(string message, string sceneName)
    {
        // Show transition message
        if (showTransitionMessages && transitionMessage != null)
        {
            transitionMessage.text = message;
            transitionMessage.gameObject.SetActive(true);
        }
        
        Debug.Log("Scene transition: " + message);
        
        // Wait for delay
        yield return new WaitForSeconds(sceneChangeDelay);
        
        // Change scene
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("No scene name provided for transition!");
        }
    }
    
    // Public methods for other scripts to use
    public static void AddScore(int points)
    {
        MelonPoints.totalScore += points;
        MelonPoints.UpdateAllScoreDisplays();
        
        // Check win condition
        if (Instance != null && Instance.checkScoreAutomatically)
        {
            Instance.CheckWinCondition();
        }
    }
    
    public static bool HasGameEnded()
    {
        return Instance != null && Instance.gameEnded;
    }
    
    // Reset the game state (useful for restarting)
    public static void ResetGameState()
    {
        if (Instance != null)
        {
            Instance.gameEnded = false;
            Debug.Log("GameManager: Game state manually reset");
        }
    }
    
    // Context menu methods for testing in the inspector
    [ContextMenu("Test Win Condition")]
    void TestWin()
    {
        TriggerWin();
    }
    
    [ContextMenu("Add 5 Points")]
    void AddTestPoints()
    {
        AddScore(5);
    }
    
    [ContextMenu("Reset Game State")]
    void TestResetGameState()
    {
        ResetGameState();
    }
}
