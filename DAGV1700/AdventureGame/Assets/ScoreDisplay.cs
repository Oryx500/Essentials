using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private SimpleIntData scoreData;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string prefix = "Score: ";

    void Awake()
    {
        if (!label) label = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        if (scoreData != null) scoreData.Reset(); // optional reset
        UpdateLabel();
    }

    void Update()
    {
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (label != null && scoreData != null)
            label.text = prefix + scoreData.value.ToString();
    }
}

