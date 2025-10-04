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
        GCGUtil.Wait(this, 1, () => {
            gameObject.SetActive(false);
        });
        //gameObject.SetActive(false);
    }
}
