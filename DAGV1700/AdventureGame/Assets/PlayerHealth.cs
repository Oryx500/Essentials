using UnityEngine;
using UnityEngine.UI; // NEW

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth;

    [Header("UI")]
    [SerializeField] Slider healthBar; // NEW: drag your Slider here

    void Awake()
    {
        currentHealth = maxHealth; // start full
        InitHealthBar(); // NEW
    }

    void InitHealthBar() // NEW
    {
        if (healthBar == null) return;
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        healthBar.wholeNumbers = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - Mathf.Abs(amount));
        if (healthBar) healthBar.value = currentHealth; // NEW
        Debug.Log($"Took {amount} damage → HP: {currentHealth}/{maxHealth}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Abs(amount));
        if (healthBar) healthBar.value = currentHealth; // NEW
        Debug.Log($"Healed {amount} → HP: {currentHealth}/{maxHealth}");
    }

    [ContextMenu("Test: Damage 10")]
    void Test_Damage10() => TakeDamage(10);

    [ContextMenu("Test: Heal 10")]
    void Test_Heal10() => Heal(10);
}
