using UnityEngine;

public class Disappear : MonoBehaviour
{
    [Header("Disappear Settings")]
    public bool requiresPlayerTag = true; // Only disappear when touched by objects with "Player" tag
    public bool hideRenderer = true; // Hide the visual appearance
    public bool disableCollider = true; // Disable collision detection after disappearing
    
    private Renderer objectRenderer;
    private Collider objectCollider;
    private bool hasDisappeared = false;

    void Start()
    {
        // Get components for hiding/disabling
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        
        // Make sure the collider is set as trigger for collision detection
        if (objectCollider != null)
        {
            objectCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if already disappeared
        if (hasDisappeared) return;
        
        // Check if this should disappear when touched by this object
        if (requiresPlayerTag && !other.CompareTag("Player")) return;
        
        // Make the object disappear
        DisappearObject();
    }
    
    void DisappearObject()
    {
        hasDisappeared = true;
        
        // Hide the visual appearance
        if (hideRenderer && objectRenderer != null)
        {
            objectRenderer.enabled = false;
        }
        
        // Disable collision detection
        if (disableCollider && objectCollider != null)
        {
            objectCollider.enabled = false;
        }
        
        Debug.Log("Object disappeared: " + gameObject.name);
    }
    
    // Public method to make the object reappear (useful for resets or other scripts)
    public void ReappearObject()
    {
        hasDisappeared = false;
        
        // Show the visual appearance
        if (objectRenderer != null)
        {
            objectRenderer.enabled = true;
        }
        
        // Re-enable collision detection
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
        
        Debug.Log("Object reappeared: " + gameObject.name);
    }
    
    // Check if object has disappeared (useful for other scripts)
    public bool HasDisappeared()
    {
        return hasDisappeared;
    }
}
