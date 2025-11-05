using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScorePickup : MonoBehaviour
{
    [SerializeField] private SimpleIntData scoreData;
    [SerializeField] private int amount = 10;
    [SerializeField] private bool destroyOnPickup = true;

    void Reset()
    {
        // make sure collider works as trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (scoreData != null)
            scoreData.Add(amount);

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
