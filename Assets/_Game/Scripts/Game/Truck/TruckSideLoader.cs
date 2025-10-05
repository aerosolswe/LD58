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

        if (trash == null || !trash.SideLoaderCapable || Busy)
            return;

        Busy = true;
        Animation.Play();
        trash.Renderer.enabled = false;
        trash.Body.bodyType = RigidbodyType2D.Kinematic;

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
