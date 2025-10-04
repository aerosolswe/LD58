using UnityEngine;

public class TrashCollector : MonoBehaviour
{

    public int TotalValue = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            Vector2 relativeVel = rb.linearVelocity;

            if (relativeVel.y < 0f)
            {
                Collect(rb);
            }
        }
    }

    public void Collect(Rigidbody2D rb)
    {
        Trash trash = rb.GetComponent<Trash>();
        if (trash == null)
            return;

        TotalValue += trash.Value;

        trash.Collect();
    }
}
