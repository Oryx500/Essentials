using UnityEngine;

[RequireComponent(typeof(Collider))] // 3D collider required
public class HealthChanger : MonoBehaviour
{
    [SerializeField] int amount = -10;       // negative = damage, positive = heal
    [SerializeField] bool destroyAfterTouch; // pickups = true

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;                // make this a TRIGGER in 3D
    }

    void OnTriggerEnter(Collider other)      // 3D trigger callback
    {
        // find PlayerHealth on the object or its parents
        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp == null) return;

        if (amount < 0) hp.TakeDamage(-amount);
        else            hp.Heal(amount);

        Debug.Log($"HurtZone applied {amount} to {hp.name}");
        if (destroyAfterTouch) Destroy(gameObject);
    }
}
