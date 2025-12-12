using UnityEngine;
using TMPro;
using System.Collections;

public class ChestController : MonoBehaviour
{
    [Header("Chest Settings")]
    public string requiredKeyName = "MainKey"; // Which key is needed to open this chest
    public bool requiresPlayerTouch = true; // Whether player needs to touch chest to open it
    
    [Header("Chest Animation")]
    public Animator chestAnimator; // Animator for chest opening animation
    public string openAnimationTrigger = "OpenChest"; // Animation trigger name
    
    [Header("Chest Visual States")]
    public GameObject closedChestModel; // The closed chest visual (can be this GameObject itself)
    public GameObject openChestModel; // The opened chest visual to show after opening
    
    [Header("Chest Contents")]
    public GameObject[] itemsToSpawn; // Items that appear when chest opens
    public Transform spawnPoint; // Where items spawn (optional)
    public int pointsReward = 3; // Points given when chest is opened
    
    [Header("Audio")]
    public AudioClip openSound; // Sound when chest opens
    public AudioClip lockedSound; // Sound when trying to open locked chest
    [Range(0f, 1f)] public float openSoundVolume = 1f; // Volume for opening sound
    [Range(0f, 1f)] public float lockedSoundVolume = 0.8f; // Volume for locked sound
    
    [Header("Visual Effects")]
    public ParticleSystem openingParticles; // Particle effect when chest opens
    public GameObject openingEffectPrefab; // Alternative: instantiate a particle effect prefab
    
    [Header("UI Feedback")]
    public TextMeshProUGUI interactionText; // Text to show interaction prompts
    public string needKeyMessage = "You need a key to open this chest";
    public string openChestMessage = "Press E to open chest";
    
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool playerNearby = false;

    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Make sure we have a collider set as trigger for player detection
        Collider col = GetComponent<Collider>();
        if (col != null && requiresPlayerTouch)
        {
            col.isTrigger = true;
        }
        
        // Hide interaction text initially
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        
        // Setup visual states - make sure closed chest is visible and open chest is hidden
        if (closedChestModel != null)
        {
            closedChestModel.SetActive(true);
        }
        if (openChestModel != null)
        {
            openChestModel.SetActive(false);
        }
    }

    void Update()
    {
        // Check for player input to open chest
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed while near chest!");
            TryOpenChest();
        }
        
        // Debug info (remove this later)
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("=== CHEST DEBUG INFO ===");
            Debug.Log("Player nearby: " + playerNearby);
            Debug.Log("Chest is open: " + isOpen);
            Debug.Log("Required key: " + requiredKeyName);
            Debug.Log("Has key: " + KeyGrab.HasKey(requiredKeyName));
            Debug.Log("All collected keys: " + string.Join(", ", KeyGrab.GetAllKeys().Keys));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered chest trigger: " + other.name + " with tag: " + other.tag);
        
        if (other.CompareTag("Player") && requiresPlayerTouch && !isOpen)
        {
            playerNearby = true;
            ShowInteractionPrompt();
            Debug.Log("Player is now near chest, showing interaction prompt");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Something exited chest trigger: " + other.name);
        
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            HideInteractionPrompt();
            Debug.Log("Player left chest area, hiding interaction prompt");
        }
    }

    void TryOpenChest()
    {
        Debug.Log("TryOpenChest called - isOpen: " + isOpen + ", playerNearby: " + playerNearby);
        
        if (isOpen) return; // Already open
        
        // Check if player has the required key
        bool hasKey = KeyGrab.HasKey(requiredKeyName);
        Debug.Log("Checking for key '" + requiredKeyName + "': " + hasKey);
        
        if (hasKey)
        {
            Debug.Log("Key found! Opening chest...");
            OpenChest();
        }
        else
        {
            Debug.Log("No key found, chest remains locked");
            // Play locked sound and show message
            if (lockedSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(lockedSound, lockedSoundVolume);
            }
            
            Debug.Log("Chest is locked! Need key: " + requiredKeyName);
            ShowLockedMessage();
        }
    }

    void OpenChest()
    {
        isOpen = true;
        
        Debug.Log("Chest opened with key: " + requiredKeyName);
        
        // Give points to player (using MelonPoints scoring system)
        MelonPoints.totalScore += pointsReward;
        MelonPoints.UpdateAllScoreDisplays();
        Debug.Log("Chest opened! Gained " + pointsReward + " points. Total score: " + MelonPoints.totalScore);
        
        // Start opening sequence (handles sound, effects, and visuals)
        StartCoroutine(ChestOpeningSequence());
        
        // Hide interaction prompt immediately
        HideInteractionPrompt();
    }
    
    IEnumerator ChestOpeningSequence()
    {
        // Play opening sound with volume control
        if (openSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(openSound, openSoundVolume);
            Debug.Log("Playing chest opening sound at volume: " + openSoundVolume);
        }
        
        // Trigger particle effects
        TriggerOpeningEffects();
        
        // Small delay to let sound start before visual changes
        yield return new WaitForSeconds(0.1f);
        
        // Trigger opening animation
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger(openAnimationTrigger);
        }
        
        // Switch to open chest visual
        SwitchToOpenChest();
        
        // Spawn items
        SpawnChestContents();
    }
    
    void TriggerOpeningEffects()
    {
        // Play attached particle system with comprehensive debugging
        if (openingParticles != null)
        {
            // Force clear any previous particles
            openingParticles.Clear();
            
            // Debug particle system settings
            Debug.Log("=== PARTICLE SYSTEM DEBUG ===");
            Debug.Log("Particle System Name: " + openingParticles.name);
            Debug.Log("Is Playing: " + openingParticles.isPlaying);
            Debug.Log("Is Emitting: " + openingParticles.isEmitting);
            Debug.Log("Particle Count: " + openingParticles.particleCount);
            
            var main = openingParticles.main;
            Debug.Log("Max Particles: " + main.maxParticles);
            Debug.Log("Start Lifetime: " + main.startLifetime.constant);
            Debug.Log("Start Speed: " + main.startSpeed.constant);
            Debug.Log("Start Size: " + main.startSize.constant);
            Debug.Log("Start Color: " + main.startColor.color);
            Debug.Log("Simulation Space: " + main.simulationSpace);
            
            var emission = openingParticles.emission;
            Debug.Log("Emission Enabled: " + emission.enabled);
            Debug.Log("Emission Rate: " + emission.rateOverTime.constant);
            
            var shape = openingParticles.shape;
            Debug.Log("Shape Enabled: " + shape.enabled);
            Debug.Log("Shape Type: " + shape.shapeType);
            
            var renderer = openingParticles.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                Debug.Log("Renderer Material: " + (renderer.material != null ? renderer.material.name : "NULL"));
                Debug.Log("Renderer Enabled: " + renderer.enabled);
            }
            
            // Try different methods to ensure particles start
            openingParticles.gameObject.SetActive(true);
            openingParticles.Play(true); // Play with children
            
            // Force emit some particles immediately for testing
            openingParticles.Emit(50);
            
            Debug.Log("Triggered particle system play and manual emit");
            Debug.Log("Position: " + openingParticles.transform.position);
            Debug.Log("=== END PARTICLE DEBUG ===");
        }
        else
        {
            Debug.LogWarning("Opening particles ParticleSystem is NULL!");
        }
        
        // Instantiate particle effect prefab
        if (openingEffectPrefab != null)
        {
            Vector3 effectPosition = transform.position + Vector3.up * 0.5f; // Slightly above chest
            GameObject effect = Instantiate(openingEffectPrefab, effectPosition, transform.rotation);
            
            // Try to play any particle systems on the instantiated prefab
            ParticleSystem[] prefabParticles = effect.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in prefabParticles)
            {
                ps.Play();
                Debug.Log("Playing particle system on prefab: " + ps.name);
            }
            
            // Auto-destroy effect after a few seconds
            Destroy(effect, 5f);
            Debug.Log("Spawned chest opening effect prefab at position: " + effectPosition);
        }
        else
        {
            Debug.LogWarning("Opening effect prefab is NULL!");
        }
    }

    void SpawnChestContents()
    {
        if (itemsToSpawn == null || itemsToSpawn.Length == 0) return;
        
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.up;
        
        foreach (GameObject item in itemsToSpawn)
        {
            if (item != null)
            {
                // Spawn with slight random offset to prevent overlap
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0, 0.5f),
                    Random.Range(-0.5f, 0.5f)
                );
                
                Instantiate(item, spawnPosition + randomOffset, Quaternion.identity);
            }
        }
    }

    void SwitchToOpenChest()
    {
        // Hide closed chest model and show open chest model
        if (closedChestModel != null)
        {
            closedChestModel.SetActive(false);
        }
        
        if (openChestModel != null)
        {
            openChestModel.SetActive(true);
        }
        
        Debug.Log("Switched chest visual to open state");
    }

    void ShowInteractionPrompt()
    {
        Debug.Log("ShowInteractionPrompt called");
        
        if (interactionText == null) 
        {
            Debug.LogWarning("No interaction text assigned!");
            return;
        }
        
        bool hasKey = KeyGrab.HasKey(requiredKeyName);
        Debug.Log("Player has key '" + requiredKeyName + "': " + hasKey);
        
        if (hasKey)
        {
            interactionText.text = openChestMessage;
            Debug.Log("Showing message: " + openChestMessage);
        }
        else
        {
            interactionText.text = needKeyMessage;
            Debug.Log("Showing message: " + needKeyMessage);
        }
        
        interactionText.gameObject.SetActive(true);
    }

    void HideInteractionPrompt()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void ShowLockedMessage()
    {
        if (interactionText != null)
        {
            interactionText.text = needKeyMessage;
            // You could add a coroutine here to hide the message after a few seconds
        }
    }

    // Public method to force open chest (for testing or other scripts)
    public void ForceOpen()
    {
        if (!isOpen)
        {
            OpenChest();
        }
    }
    
    [ContextMenu("Test Particle System")]
    void TestParticleSystem()
    {
        Debug.Log("=== TESTING PARTICLE SYSTEM ===");
        TriggerOpeningEffects();
    }
    
    [ContextMenu("Reset And Configure Particle System")]
    void ResetAndConfigureParticleSystem()
    {
        if (openingParticles == null)
        {
            Debug.LogError("No particle system assigned to openingParticles!");
            return;
        }
        
        Debug.Log("Resetting and configuring particle system...");
        
        // Stop and clear
        openingParticles.Stop();
        openingParticles.Clear();
        
        // Configure main settings
        var main = openingParticles.main;
        main.startLifetime = 2.0f;
        main.startSpeed = 5.0f;
        main.startSize = 0.1f;
        main.startColor = Color.yellow;
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Configure emission
        var emission = openingParticles.emission;
        emission.enabled = true;
        emission.rateOverTime = 50;
        
        // Configure shape
        var shape = openingParticles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
        
        // Configure velocity over lifetime for upward movement
        var velocityOverLifetime = openingParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(2.0f);
        
        // Configure size over lifetime
        var sizeOverLifetime = openingParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0, 0.1f);
        sizeCurve.AddKey(0.5f, 1.0f);
        sizeCurve.AddKey(1, 0.1f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);
        
        // Configure color over lifetime
        var colorOverLifetime = openingParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        // Ensure renderer has a material
        var renderer = openingParticles.GetComponent<ParticleSystemRenderer>();
        if (renderer != null && renderer.material == null)
        {
            // Try to assign default particle material
            Material defaultMaterial = new Material(Shader.Find("Sprites/Default"));
            renderer.material = defaultMaterial;
            Debug.Log("Assigned default material to particle renderer");
        }
        
        Debug.Log("Particle system configured! Try testing it now.");
    }
}
