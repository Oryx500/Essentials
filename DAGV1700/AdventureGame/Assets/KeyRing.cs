using System.Collections.Generic;
using UnityEngine;

public class Keyring : MonoBehaviour
{
    [SerializeField] List<float> heldKeys = new();   // your keys live here
    const float EPS = 0.0001f;                       // tolerance for float compare

    public bool HasKey(float keyCode)
    {
        foreach (var k in heldKeys)
            if (Mathf.Abs(k - keyCode) <= EPS) return true;
        return false;
    }

    public void AddKey(float keyCode)
    {
        if (!HasKey(keyCode)) heldKeys.Add(keyCode);
    }

    public void RemoveKey(float keyCode)
    {
        for (int i = 0; i < heldKeys.Count; i++)
            if (Mathf.Abs(heldKeys[i] - keyCode) <= EPS) { heldKeys.RemoveAt(i); return; }
    }
}
