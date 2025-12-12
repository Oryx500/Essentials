using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MelonPoints : MonoBehaviour
{
    [Header("Score Settings")]
    public int pointsToGive = 1; // Points given when player touches this melon
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Reference to the UI text that displays score
    
    // Static score so all melons share the same score counter
    public static int totalScore = 0;
    
    // Track if score has been reset this scene load
    private static bool scoreResetThisScene = false;
    
    [Header("Optional Settings")]
    public bool destroyOnPickup = true; // Whether the melon disappears when collected
    public AudioClip pickupSound; // Optional sound when collected
    
    private AudioSource audioSource;

    void Start()
    {
        // Reset score when scene loads (only once per scene)
        if (!scoreResetThisScene)
        {
            ResetScore();
            scoreResetThisScene = true;
        }
        
        // Get or create AudioSource for pickup sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Update score display when game starts
        UpdateScoreDisplay();
    }
    
    void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreResetThisScene = false; // Allow score reset for new scene
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player touched this melon
        if (other.CompareTag("Player"))
        {
            // Give points to player
            totalScore += pointsToGive;
            
            // Update the UI using static method (works even after this melon is destroyed)
            UpdateAllScoreDisplays();
            
            // Play pickup sound if available
            bool soundPlayed = false;
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
                soundPlayed = true;
            }
            
            // Destroy the melon if set to do so
            if (destroyOnPickup)
            {
                if (soundPlayed)
                {
                    // Delay destruction to let sound play
                    StartCoroutine(DestroyAfterSound(pickupSound.length));
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore;
        }
        else
        {
            Debug.LogWarning("MelonPoints: No scoreText assigned! Please drag a TextMeshPro element to the 'Score Text' field in the inspector.");
        }
    }
    
    // Static method to update all score displays - works even when melons are destroyed
    // Now uses a registry system instead of searching all TextMeshPro elements
    public static void UpdateAllScoreDisplays()
    {
        // Find all MelonPoints components and update their specific score displays
        MelonPoints[] melonComponents = FindObjectsOfType<MelonPoints>();
        bool updatedAny = false;
        
        foreach (MelonPoints melon in melonComponents)
        {
            if (melon.scoreText != null)
            {
                melon.scoreText.text = "Score: " + totalScore;
                updatedAny = true;
            }
        }
        
        // If no MelonPoints components have score text assigned, check for a global score manager
        if (!updatedAny)
        {
            // Look for a dedicated ScoreManager or GameManager with score display
            GameObject scoreManager = GameObject.Find("ScoreManager");
            if (scoreManager == null)
                scoreManager = GameObject.Find("GameManager");
                
            if (scoreManager != null)
            {
                TextMeshProUGUI scoreDisplay = scoreManager.GetComponent<TextMeshProUGUI>();
                if (scoreDisplay == null)
                    scoreDisplay = scoreManager.GetComponentInChildren<TextMeshProUGUI>();
                    
                if (scoreDisplay != null)
                {
                    scoreDisplay.text = "Score: " + totalScore;
                    return;
                }
            }
            
            Debug.LogWarning("MelonPoints: No score displays found! Make sure at least one MelonPoints component has a scoreText assigned, or create a ScoreManager GameObject with a TextMeshPro component.");
        }
    }
    
    // Static method to reset the score (can be called from other scripts)
    public static void ResetScore()
    {
        totalScore = 0;
        UpdateAllScoreDisplays();
    }
    
    // Static method to get current score (for other scripts)
    public static int GetScore()
    {
        return totalScore;
    }
    
    // Coroutine to delay destruction until sound finishes playing
    System.Collections.IEnumerator DestroyAfterSound(float delay)
    {
        // Hide the visual components but keep the GameObject alive for audio
        Renderer melonRenderer = GetComponent<Renderer>();
        Collider melonCollider = GetComponent<Collider>();
        
        if (melonRenderer != null)
            melonRenderer.enabled = false;
        if (melonCollider != null)
            melonCollider.enabled = false;
            
        // Wait for sound to finish
        yield return new WaitForSeconds(delay);
        
        // Now destroy the GameObject
        Destroy(gameObject);
    }
}
