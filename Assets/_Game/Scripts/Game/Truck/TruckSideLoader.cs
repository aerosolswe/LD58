using GCG;
using System.Collections;
using UnityEngine;

public class TruckSideLoader : MonoBehaviour
{
    public Animation Animation;
    public TrashCollector TrashCollector;

    public bool Busy = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trash trash = collision.GetComponent<Trash>();

        if (trash == null)
            trash = collision.GetComponentInParent<Trash>();

        if (trash == null || !trash.SideLoaderCapable || Busy)
            return;

        Busy = true;
        Animation.Play();
        trash.Renderer.enabled = false;
        trash.Body.bodyType = RigidbodyType2D.Kinematic;

        Collider2D[] collider2Ds = new Collider2D[0];
        int colliderCount = trash.Body.GetAttachedColliders(collider2Ds);

        for (int i = 0; i < colliderCount; i++)
        {
            collider2Ds[i].enabled = false;
        }

        StartCoroutine(Routine(trash));
    }

    IEnumerator Routine(Trash trash)
    {
        yield return GCGUtil.Yield(1.3f);

        TrashCollector.Collect(trash.Body);

        yield return GCGUtil.Yield(1.3f);

        Busy = false;
    }
}
