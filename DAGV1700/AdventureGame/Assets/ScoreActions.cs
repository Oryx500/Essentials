using UnityEngine;

public class ScoreActions : MonoBehaviour
{
    [SerializeField] private SimpleIntData scoreData;

    // Call this from your trigger (no parameters version)
    public void Add10() {
        if (scoreData != null) scoreData.Add(10);
    }

    // Optional: if your trigger can pass an int parameter, use this instead
    public void AddPoints(int amount) {
        if (scoreData != null) scoreData.Add(amount);
    }

    // Optional: reset helper
    public void ResetScore() {
        if (scoreData != null) scoreData.Reset();
    }
}
