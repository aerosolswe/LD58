using GCG;
using System.Collections;
using UnityEngine;

public class TruckPlow : MonoBehaviour
{
    public Animation Animation;
    public Collider2D Collider;
    public Collider2D Trigger;

    private bool collecting = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collecting)
            return;

        Collider.enabled = true;
        Trigger.enabled = false;

        StopAllCoroutines();
        Animation.Stop();
        Animation.Play();
        StartCoroutine(DelayedActivate());
    }
    
    IEnumerator DelayedActivate()
    {
        yield return GCGUtil.Yield(6.0f);
        Trigger.enabled = true;
        collecting = false;
        InactivateCollider();
    }

    public void InactivateCollider()
    {
        Collider.enabled = false;
    }
}
