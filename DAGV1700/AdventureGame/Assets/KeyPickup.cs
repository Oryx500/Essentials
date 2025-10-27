using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [Header("Choose ONE to use")]
    [SerializeField] private UnityEngine.Object keyObject; // assign your 4.56 object here
    [SerializeField] private float keyNumber = 0f;         // or use a numeric key if that's your system

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;      // drag your Audio Source here
    [SerializeField] private AudioClip pickupClip;         // leave empty to use audioSource.clip

    [Header("Behavior")]
    [SerializeField] private bool hideOnPickup = true;     // hide sprite + collider
    [SerializeField] private bool destroyAfterDelay = false;
    [SerializeField] private float cleanupDelay = 2f;

    private bool _picked;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_picked) return;
        _picked = true;

        // 1) Try to hand off the key to ANY component on the player hierarchy
        bool handedOff = TryGiveKeyTo(other);

        // 2) Play SFX (optional)
        float len = 0f;
        if (audioSource != null)
        {
            var clip = pickupClip != null ? pickupClip : audioSource.clip;
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
                len = clip.length;
            }
        }

        // 3) Hide the visual & stop further triggers (no Destroy here)
        if (hideOnPickup)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;

            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
        }

        // 4) Optional cleanup after the sound
        if (destroyAfterDelay)
        {
            Destroy(gameObject, Mathf.Max(len, cleanupDelay));
        }

        // Helpful logging
        if (handedOff)
        {
            if (keyObject != null)
                Debug.Log($"[KeyPickup] Gave key OBJECT: {keyObject.name}", this);
            else
                Debug.Log($"[KeyPickup] Gave key NUMBER: {keyNumber}", this);
        }
        else
        {
            Debug.LogWarning("[KeyPickup] Could not find a component on the player that accepts the key. " +
                             "If this was working before, ensure we only replaced Destroy() with hide lines.", this);
        }
    }

    // --- Helpers ---

    // Tries common method names and parameter types so we don't need to know your exact class name.
    private bool TryGiveKeyTo(Collider other)
    {
        var targets = other.GetComponentsInParent<MonoBehaviour>(true);
        if (targets == null || targets.Length == 0) return false;

        // Prefer object key if assigned, else numeric
        if (keyObject != null)
        {
            // AddKey(Object), CollectKey(Object), GiveKey(Object)
            var ok = TryInvokeOnTargets(targets, "AddKey", keyObject)
                     || TryInvokeOnTargets(targets, "CollectKey", keyObject)
                     || TryInvokeOnTargets(targets, "GiveKey", keyObject);
            if (ok) return true;
        }
        else
        {
            // AddKey(float/int), CollectKey(float/int), GiveKey(float/int)
            var ok = TryInvokeOnTargets(targets, "AddKey", keyNumber)
                     || TryInvokeOnTargets(targets, "CollectKey", keyNumber)
                     || TryInvokeOnTargets(targets, "GiveKey", keyNumber)
                     || TryInvokeOnTargets(targets, "AddKey", (int)keyNumber)
                     || TryInvokeOnTargets(targets, "CollectKey", (int)keyNumber)
                     || TryInvokeOnTargets(targets, "GiveKey", (int)keyNumber);
            if (ok) return true;
        }

        return false;
    }

    private bool TryInvokeOnTargets(MonoBehaviour[] targets, string methodName, UnityEngine.Object param)
    {
        foreach (var t in targets)
        {
            var m = t.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                          null, new Type[] { typeof(UnityEngine.Object) }, null);
            if (m != null)
            {
                m.Invoke(t, new object[] { param });
                return true;
            }
        }
        return false;
    }

    private bool TryInvokeOnTargets(MonoBehaviour[] targets, string methodName, float param)
    {
        foreach (var t in targets)
        {
            // float version
            var mFloat = t.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                               null, new Type[] { typeof(float) }, null);
            if (mFloat != null)
            {
                mFloat.Invoke(t, new object[] { param });
                return true;
            }
            // int version
            var mInt = t.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                             null, new Type[] { typeof(int) }, null);
            if (mInt != null)
            {
                mInt.Invoke(t, new object[] { (int)param });
                return true;
            }
        }
        return false;
    }
}



