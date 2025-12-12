using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneOpener : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "NextScene"; // Name of the scene to load
    public bool useSceneIndex = false; // Use scene index instead of name
    public int sceneIndex = 1; // Scene index to load (if useSceneIndex is true)
    
    [Header("Button Setup")]
    public Button sceneButton; // The button that triggers scene change
    public bool autoFindButton = true; // Automatically find button component
    
    [Header("Transition Settings")]
    public float transitionDelay = 0f; // Delay before changing scene
    public bool showLoadingMessage = false; // Show a loading message
    public string loadingMessage = "Loading...";
    
    [Header("Audio")]
    public AudioClip clickSound; // Sound when button is clicked
    [Range(0f, 1f)] public float clickVolume = 1f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Auto-find button if enabled and no button assigned
        if (autoFindButton && sceneButton == null)
        {
            sceneButton = GetComponent<Button>();
            if (sceneButton == null)
            {
                sceneButton = GetComponentInChildren<Button>();
            }
        }
        
        // Setup button listener
        if (sceneButton != null)
        {
            sceneButton.onClick.AddListener(OnButtonClick);
            Debug.Log("SceneOpener: Button listener added successfully");
        }
        else
        {
            Debug.LogWarning("SceneOpener: No button found! Please assign a button in the inspector or attach this script to a GameObject with a Button component.");
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log("SceneOpener: Button clicked, preparing to load scene");
        
        // Play click sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, clickVolume);
        }
        
        // Show loading message if enabled
        if (showLoadingMessage)
        {
            Debug.Log(loadingMessage);
        }
        
        // Load scene with or without delay
        if (transitionDelay > 0)
        {
            StartCoroutine(LoadSceneWithDelay());
        }
        else
        {
            LoadScene();
        }
    }
    
    System.Collections.IEnumerator LoadSceneWithDelay()
    {
        Debug.Log("SceneOpener: Waiting " + transitionDelay + " seconds before scene change");
        yield return new WaitForSeconds(transitionDelay);
        LoadScene();
    }
    
    void LoadScene()
    {
        try
        {
            if (useSceneIndex)
            {
                Debug.Log("SceneOpener: Loading scene by index: " + sceneIndex);
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.Log("SceneOpener: Loading scene by name: " + sceneToLoad);
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("SceneOpener: Failed to load scene! Error: " + e.Message);
            Debug.LogError("Make sure the scene '" + (useSceneIndex ? "index " + sceneIndex : sceneToLoad) + "' exists in Build Settings");
        }
    }
    
    // Public method that can be called from other scripts
    public void LoadSpecificScene(string sceneName)
    {
        sceneToLoad = sceneName;
        useSceneIndex = false;
        LoadScene();
    }
    
    public void LoadSpecificSceneByIndex(int index)
    {
        sceneIndex = index;
        useSceneIndex = true;
        LoadScene();
    }
    
    // Context menu methods for testing
    [ContextMenu("Test Scene Load")]
    void TestSceneLoad()
    {
        Debug.Log("SceneOpener: Testing scene load...");
        OnButtonClick();
    }
    
    void OnDestroy()
    {
        // Clean up button listener when object is destroyed
        if (sceneButton != null)
        {
            sceneButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}
