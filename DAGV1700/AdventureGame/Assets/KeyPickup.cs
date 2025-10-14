using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] float keyCode = 4.56f;
    [SerializeField] bool destroyOnPickup = true;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[KeyPickup] Trigger ENTER with: {other.name}", this);

        var ring = other.GetComponentInParent<Keyring>();
        if (ring == null)
        {
            Debug.Log("[KeyPickup] No Keyring found on collider or parents.", this);
            return;
        }

        ring.AddKey(keyCode);
        Debug.Log($"[KeyPickup] ADDED key {keyCode} to {ring.name}", this);

        if (destroyOnPickup) Destroy(gameObject);
    }
}
