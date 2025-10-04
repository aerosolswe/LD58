using GCG;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Rigidbody2D Body;
    public ObjectPool<Trash> Pool;

    public int Value = 5;

    private void OnEnable()
    {
        GCGUtil.SetLayers(gameObject, "Trash");
    }

    public void Collect()
    {
        Renderer.sortingOrder = 0;
        GCGUtil.SetLayers(gameObject, "PickedUpTrash");

        if (ObjectPicker.Instance.HeldBody == Body)
        {
            ObjectPicker.Instance.Drop();
        }

        GCGUtil.Wait(this, 1, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
