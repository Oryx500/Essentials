using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AddPointsOnTrigger : MonoBehaviour
{
    [SerializeField] private SimpleIntData scoreData;
    [SerializeField] private int amount = 10;
    [SerializeField] private bool destroyOnPickup = true;

    void Reset()
    {
        // ensure collider acts as trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (scoreData != null)
        {
            scoreData.Add(amount);
            Debug.Log($"Added {amount}, new total = {scoreData.value}");
        }

        if (destroyOnPickup) Destroy(gameObject);
    }
}

