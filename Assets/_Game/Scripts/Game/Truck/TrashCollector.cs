using GCG;
using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public ObjectPool<WorldCash> CashPool;
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

        UI.Instance.CashText.text = "€" + TotalValue.FormatCurrency();

        var cashObject = CashPool.GetOne();
        cashObject.transform.localPosition = Vector2.zero;
        cashObject.Initialize(trash.Value);

        trash.Collect();
    }
}
