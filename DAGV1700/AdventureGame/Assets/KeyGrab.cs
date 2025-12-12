using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyGrab : MonoBehaviour
{
    [Header("Key Settings")]
    public string keyName = "MainKey"; // Unique name for this key
    public bool destroyOnPickup = false; // Keep object alive but hide it
    
    [Header("Audio")]
    public AudioClip pickupSound; // Sound when key is collected
    
    [Header("Visual Effects")]
    public GameObject pickupEffect; // Optional particle effect on pickup
    
    // Static dictionary to track collected keys
    public static System.Collections.Generic.Dictionary<string, bool> collectedKeys = 
        new System.Collections.Generic.Dictionary<string, bool>();
    
    // Track if keys have been reset this scene load
    private static bool keysResetThisScene = false;
    
    private AudioSource audioSource;
    private Renderer keyRenderer;
    private Collider keyCollider;
    private bool isCollected = false;

    void Start()
    {
        // Reset all collected keys when scene loads (only once per scene)
        if (!keysResetThisScene)
        {
            ResetAllKeys();
            keysResetThisScene = true;
        }
        
        // Get components
        keyRenderer = GetComponent<Renderer>();
        keyCollider = GetComponent<Collider>();
        
        // Make sure the collider is set as trigger
        if (keyCollider != null)
        {
            keyCollider.isTrigger = true;
        }
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Initialize this key as not collected (after potential reset)
        if (!collectedKeys.ContainsKey(keyName))
        {
            collectedKeys[keyName] = false;
        }
        
        // Show key since it's been reset (or if never collected)
        if (!collectedKeys[keyName])
        {
            ShowKey();
        }
        else
        {
            HideKey(); // Hide if somehow still collected
        }
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
        keysResetThisScene = false; // Allow key reset for new scene
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player touched the key and it hasn't been collected yet
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectKey();
        }
    }
    
    void CollectKey()
    {
        isCollected = true;
        collectedKeys[keyName] = true;
        
        // Play pickup sound
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        
        // Show pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, transform.rotation);
        }
        
        // Hide the key
        HideKey();
        
        // Destroy after sound finishes (if there is one) or immediately
        if (destroyOnPickup)
        {
            if (pickupSound != null)
            {
                Destroy(gameObject, pickupSound.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    void HideKey()
    {
        // Hide visual and disable collision but keep GameObject alive
        if (keyRenderer != null)
            keyRenderer.enabled = false;
        if (keyCollider != null)
            keyCollider.enabled = false;
    }
    
    void ShowKey()
    {
        // Show visual and enable collision
        if (keyRenderer != null)
            keyRenderer.enabled = true;
        if (keyCollider != null)
            keyCollider.enabled = true;
    }
    
    // Static method to check if a specific key has been collected
    public static bool HasKey(string keyName)
    {
        return collectedKeys.ContainsKey(keyName) && collectedKeys[keyName];
    }
    
    // Static method to get all collected keys (useful for debugging)
    public static System.Collections.Generic.Dictionary<string, bool> GetAllKeys()
    {
        return collectedKeys;
    }
    
    // Method to reset all keys (useful for testing or new game)
    public static void ResetAllKeys()
    {
        collectedKeys.Clear();
        
        // Find all KeyGrab components in the scene and show them
        KeyGrab[] allKeys = FindObjectsOfType<KeyGrab>();
        foreach (KeyGrab key in allKeys)
        {
            if (key != null)
            {
                key.isCollected = false;
                key.ShowKey();
            }
        }
    }
}
