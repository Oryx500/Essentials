using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SimplePlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10; // Starting health
    public int currentHealth;
    
    [Header("Damage Settings")]
    public int enemyDamage = 5; // Damage taken from enemies
    public int healAmount = 1; // Health gained from heal objects
    
    [Header("Scene Settings")]
    public string gameOverScene = "NewStart"; // Scene to load when health reaches zero
    public float deathDelay = 1.0f; // Delay before loading game over scene
    
    [Header("UI Display")]
    public TextMeshProUGUI healthDisplay; // UI text to show current health
    public string healthTextPrefix = "Health: "; // Text before health number
    public string healthTextSuffix = ""; // Text after health number (e.g., " HP")
    
    [Header("Audio")]
    public AudioClip damageSound; // Sound when taking damage
    public AudioClip healSound; // Sound when healing
    public AudioClip deathSound; // Sound when player dies
    [Range(0f, 1f)] public float damageSoundVolume = 0.8f;
    [Range(0f, 1f)] public float healSoundVolume = 0.6f;
    [Range(0f, 1f)] public float deathSoundVolume = 1.0f;
    
    [Header("Visual Effects")]
    public ParticleSystem damageEffect; // Particle effect when taking damage
    public ParticleSystem healEffect; // Particle effect when healing
    public GameObject damageEffectPrefab; // Alternative: instantiate damage effect
    public GameObject healEffectPrefab; // Alternative: instantiate heal effect
    
    [Header("Damage Protection")]
    public float invulnerabilityTime = 0.5f; // Time player is invulnerable after taking damage
    public bool showInvulnerabilityFeedback = true; // Flash player during invulnerability
    
    private AudioSource audioSource;
    private bool isInvulnerable = false;
    private bool isDead = false;
    private Renderer playerRenderer;
    private Color originalColor;
    
    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Get player renderer for visual feedback
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        
        // Make sure player has a collider for trigger detection
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("SimplePlayerHealth: No collider found on player! Adding BoxCollider...");
            gameObject.AddComponent<BoxCollider>();
        }
        
        // Update health display
        UpdateHealthDisplay();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return; // Don't process if player is dead
        
        // Check for enemy contact
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
        }
        // Check for heal object contact
        else if (other.CompareTag("Heal"))
        {
            Heal(healAmount);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Don't go below 0
        
        // Play damage sound
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound, damageSoundVolume);
        }
        
        // Play damage effects
        PlayDamageEffect();
        
        // Update UI
        UpdateHealthDisplay();
        
        // Start invulnerability period
        if (invulnerabilityTime > 0 && currentHealth > 0)
        {
            StartCoroutine(InvulnerabilityPeriod());
        }
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        // Don't heal if already at max health
        if (currentHealth >= maxHealth)
        {
            return;
        }
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // Don't exceed max health
        
        // Play heal sound
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound, healSoundVolume);
        }
        
        // Play heal effects
        PlayHealEffect();
        
        // Update UI
        UpdateHealthDisplay();
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolume);
        }
        
        // Update UI to show 0 health
        UpdateHealthDisplay();
        
        // Load game over scene after delay
        StartCoroutine(LoadGameOverScene());
    }
    
    IEnumerator LoadGameOverScene()
    {
        Debug.Log("SimplePlayerHealth: Waiting " + deathDelay + " seconds before loading game over scene");
        yield return new WaitForSeconds(deathDelay);
        
        try
        {
            Debug.Log("SimplePlayerHealth: Loading game over scene: " + gameOverScene);
            SceneManager.LoadScene(gameOverScene);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SimplePlayerHealth: Failed to load game over scene! Error: " + e.Message);
            Debug.LogError("Make sure scene '" + gameOverScene + "' exists in Build Settings");
        }
    }
    
    IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;
        Debug.Log("SimplePlayerHealth: Invulnerability period started for " + invulnerabilityTime + " seconds");
        
        // Visual feedback during invulnerability
        if (showInvulnerabilityFeedback && playerRenderer != null)
        {
            StartCoroutine(FlashPlayer());
        }
        
        yield return new WaitForSeconds(invulnerabilityTime);
        
        isInvulnerable = false;
        Debug.Log("SimplePlayerHealth: Invulnerability period ended");
        
        // Reset player color
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
    }
    
    IEnumerator FlashPlayer()
    {
        float flashInterval = 0.1f;
        float elapsed = 0f;
        
        while (elapsed < invulnerabilityTime && playerRenderer != null)
        {
            // Toggle between original color and red
            playerRenderer.material.color = (elapsed % (flashInterval * 2) < flashInterval) ? Color.red : originalColor;
            
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }
        
        // Ensure we end on original color
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
        }
    }
    
    void PlayDamageEffect()
    {
        // Play attached particle system
        if (damageEffect != null)
        {
            damageEffect.Play();
        }
        
        // Instantiate damage effect prefab
        if (damageEffectPrefab != null)
        {
            GameObject effect = Instantiate(damageEffectPrefab, transform.position, transform.rotation);
            Destroy(effect, 3f); // Clean up after 3 seconds
        }
    }
    
    void PlayHealEffect()
    {
        // Play attached particle system
        if (healEffect != null)
        {
            healEffect.Play();
        }
        
        // Instantiate heal effect prefab
        if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, transform.position, transform.rotation);
            Destroy(effect, 3f); // Clean up after 3 seconds
        }
    }
    
    void UpdateHealthDisplay()
    {
        if (healthDisplay != null)
        {
            healthDisplay.text = healthTextPrefix + currentHealth + "/" + maxHealth + healthTextSuffix;
            
            // Change color based on health level
            if (currentHealth <= 0)
            {
                healthDisplay.color = Color.red;
            }
            else if (currentHealth <= maxHealth * 0.3f) // 30% or less
            {
                healthDisplay.color = Color.yellow;
            }
            else
            {
                healthDisplay.color = Color.white;
            }
        }
    }
    
    // Public methods for other scripts to use
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public bool IsAlive()
    {
        return !isDead;
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    public void SetHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthDisplay();
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthDisplay();
    }
    
    // Context menu methods for testing
    [ContextMenu("Take 5 Damage")]
    void TestDamage()
    {
        TakeDamage(5);
    }
    
    [ContextMenu("Heal 1 Health")]
    void TestHeal()
    {
        Heal(1);
    }
    
    [ContextMenu("Kill Player")]
    void TestDeath()
    {
        TakeDamage(currentHealth);
    }
    
    [ContextMenu("Heal to Full")]
    void TestFullHeal()
    {
        Heal(maxHealth);
    }
}
