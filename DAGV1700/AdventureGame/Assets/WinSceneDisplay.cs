using UnityEngine;
using TMPro;

public class WinSceneDisplay : MonoBehaviour
{
    [Header("Display Elements")]
    public TextMeshProUGUI completionTimeText; // UI text to show completion time
    public TextMeshProUGUI scoreText; // UI text to show final score
    public TextMeshProUGUI congratulationsText; // Main congratulations message
    
    [Header("Display Settings")]
    public string timePrefix = "Time: "; // Text before the time
    public string scorePrefix = "Final Score: "; // Text before the score
    public string congratsMessage = "Congratulations!"; // Main message
    
    void Start()
    {
        DisplayResults();
    }
    
    void DisplayResults()
    {
        // Display congratulations message
        if (congratulationsText != null)
        {
            congratulationsText.text = congratsMessage;
        }
        
        // Display completion time
        if (completionTimeText != null)
        {
            string timeText = timePrefix + Timer.GetLastCompletionTimeFormatted();
            completionTimeText.text = timeText;
            Debug.Log("Displaying completion time: " + timeText);
        }
        else
        {
            Debug.LogWarning("WinSceneDisplay: No completion time TextMeshPro assigned!");
        }
        
        // Display final score
        if (scoreText != null)
        {
            string scoreString = scorePrefix + MelonPoints.GetScore();
            scoreText.text = scoreString;
            Debug.Log("Displaying final score: " + scoreString);
        }
        else
        {
            Debug.LogWarning("WinSceneDisplay: No score TextMeshPro assigned!");
        }
    }
    
    // Public method to refresh display (useful if called from other scripts)
    public void RefreshDisplay()
    {
        DisplayResults();
    }
    
    // Context menu method for testing in the inspector
    [ContextMenu("Test Display")]
    void TestDisplay()
    {
        DisplayResults();
    }
}
