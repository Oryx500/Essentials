using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChestLock : MonoBehaviour
{
    [SerializeField] float requiredKey = 1.23f;  // Set per-chest in Inspector
    [SerializeField] bool consumeKey = false;    // Remove key after use?
    [SerializeField] bool openOnce = true;       // Ignore future triggers after opening

    bool isOpen;

    void Reset()
    {
        // Ensure this chest uses a 3D trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isOpen && openOnce) return;

        var keyring = other.GetComponentInParent<Keyring>();
        if (!keyring) return;

        if (keyring.HasKey(requiredKey))
        {
            if (consumeKey) keyring.RemoveKey(requiredKey);
            OpenChest();
        }
        else
        {
            Debug.Log($"{name}: LOCKED (needs key {requiredKey})");
        }
    }

    void OpenChest()
    {
        isOpen = true;
        Debug.Log($"{name}: OPEN! (key {requiredKey} matched)");

        // Visual feedback without animations: tint green
        var rend = GetComponentInChildren<Renderer>();
        if (rend) rend.material.color = Color.green;

        // (Optional) disable trigger so it stops spamming
        if (openOnce) GetComponent<Collider>().enabled = false;
    }
}
