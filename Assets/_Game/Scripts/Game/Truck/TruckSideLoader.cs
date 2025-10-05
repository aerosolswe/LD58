using GCG;
using System.Collections;
using UnityEngine;

public class TruckSideLoader : MonoBehaviour
{
    public Animation Animation;
    public TrashCollector TrashCollector;
    public Collider2D TriggerCollider;

    public bool Busy = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Trash trash = collision.GetComponent<Trash>();

        if (trash == null)
            trash = collision.GetComponentInParent<Trash>();

        if (trash == null || !trash.SideLoaderCapable || Busy)
            return;

        Busy = true;
        TriggerCollider.enabled = false;
        Animation.Stop();
        Animation.Play();
        trash.Renderer.enabled = false;
        trash.Body.bodyType = RigidbodyType2D.Kinematic;
        trash.EnableColliders(false);

        StartCoroutine(Routine(trash));
    }

    IEnumerator Routine(Trash trash)
    {
        yield return GCGUtil.Yield(1.3f);

        TrashCollector.Collect(trash.Body);

        yield return GCGUtil.Yield(1.3f);

        Busy = false;
        TriggerCollider.enabled = true;
    }
}
