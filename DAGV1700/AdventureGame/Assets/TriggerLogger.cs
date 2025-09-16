using UnityEngine;

public class TriggerLogger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered!");
        GetComponent<Renderer>().material.color = Color.red;
    }
}