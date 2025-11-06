using UnityEngine;

public class PlayAttachedVFX : MonoBehaviour
{
    public ParticleSystem vfx;

    public void PlayVFX()
    {
        if (vfx == null) return;

        // Detach so it can finish after the apple despawns
        vfx.transform.parent = null;
        vfx.Play();

        // Clean up if Stop Action isn't Destroy
        // Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
    }
}
