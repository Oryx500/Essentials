using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Display")]
    public TextMeshProUGUI timerText; // UI text to display the timer
    
    [Header("Timer Settings")]
    public bool startAutomatically = true; // Start timer when script starts
    public string timeFormat = "mm:ss.ff"; // Format: mm = minutes, ss = seconds, ff = milliseconds
    
    [Header("Timer Control")]
    public bool isRunning = false; // Whether timer is currently running
    
    private float elapsedTime = 0f; // Total elapsed time in seconds
    
    // Static variable to store completion time for display in other scenes
    public static float lastCompletionTime = 0f;
    
    void Start()
    {
        // Reset timer to zero
        elapsedTime = 0f;
        
        // Start timer automatically if enabled
        if (startAutomatically)
        {
            StartTimer();
        }
        
        // Update display immediately
        UpdateTimerDisplay();
        
        // Warn if no timer text assigned
        if (timerText == null)
        {
            Debug.LogWarning("Timer: No TextMeshPro assigned! Please assign a TextMeshPro element to display the timer.");
        }
    }

    void Update()
    {
        // Only count time if timer is running
        if (isRunning)
        {
            // Add time since last frame
            elapsedTime += Time.deltaTime;
            
            // Update the display
            UpdateTimerDisplay();
        }
    }
    
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            // Convert elapsed time to readable format
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            
            // Format based on timeFormat setting
            string displayText = "";
            
            if (timeFormat == "mm:ss.ff")
            {
                // Minutes:Seconds.Milliseconds (00:15.75)
                displayText = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            }
            else if (timeFormat == "ss.ff")
            {
                // Seconds.Milliseconds only (75.50)
                displayText = string.Format("{0:00}.{1:00}", (int)elapsedTime, milliseconds);
            }
            else if (timeFormat == "mm:ss.fff")
            {
                // Minutes:Seconds.Milliseconds with 3 digits (00:15.750)
                int milliseconds3 = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f);
                displayText = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds3);
            }
            else
            {
                // Default format
                displayText = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            }
            
            timerText.text = displayText;
        }
    }
    
    // Public methods to control the timer
    public void StartTimer()
    {
        isRunning = true;
        Debug.Log("Timer started");
    }
    
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer stopped at: " + GetFormattedTime());
    }
    
    public void PauseTimer()
    {
        isRunning = false;
        Debug.Log("Timer paused at: " + GetFormattedTime());
    }
    
    public void ResumeTimer()
    {
        isRunning = true;
        Debug.Log("Timer resumed");
    }
    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        UpdateTimerDisplay();
        Debug.Log("Timer reset");
    }
    
    public void RestartTimer()
    {
        ResetTimer();
        StartTimer();
        Debug.Log("Timer restarted");
    }
    
    // Get current time values
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
    
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }
    
    // Capture completion time when game is completed
    public float CaptureCompletionTime()
    {
        lastCompletionTime = elapsedTime;
        Debug.Log("Completion time captured: " + GetFormattedTime());
        return lastCompletionTime;
    }
    
    // Get the last captured completion time as formatted string
    public static string GetLastCompletionTimeFormatted()
    {
        int minutes = Mathf.FloorToInt(lastCompletionTime / 60f);
        int seconds = Mathf.FloorToInt(lastCompletionTime % 60f);
        int milliseconds = Mathf.FloorToInt((lastCompletionTime * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
    
    // Get the raw completion time value
    public static float GetLastCompletionTime()
    {
        return lastCompletionTime;
    }
    
    // Context menu methods for testing in the inspector
    [ContextMenu("Start Timer")]
    void TestStart()
    {
        StartTimer();
    }
    
    [ContextMenu("Stop Timer")]
    void TestStop()
    {
        StopTimer();
    }
    
    [ContextMenu("Reset Timer")]
    void TestReset()
    {
        ResetTimer();
    }
    
    [ContextMenu("Restart Timer")]
    void TestRestart()
    {
        RestartTimer();
    }
}
