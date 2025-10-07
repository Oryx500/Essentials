using UnityEngine;

public class AnimateOnTrigger : MonoBehaviour
{
    public Animator anim;
    public string triggerName = "Activate";
    public bool oneShot = false;
    bool played;

    void Reset() { if (!anim) anim = GetComponent<Animator>(); }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && played) return;
        anim.SetTrigger(triggerName);
        played = true;
    }
}

