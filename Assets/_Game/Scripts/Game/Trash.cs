using GCG;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public Sprite[] sprites;
    public Collider2D[] Colliders;
    public SpriteRenderer Renderer;
    public Rigidbody2D Body;
    public ObjectPool<Trash> Pool;

    public bool SideLoaderCapable = false;

    public int Value = 5;

    private void OnEnable()
    {
        sprites.Shuffle();
        Renderer.sprite = sprites[0];
        GCGUtil.SetLayers(gameObject, "Trash");
        EnableColliders(true);
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
            Renderer.enabled = true;
            Body.bodyType = RigidbodyType2D.Dynamic;
        });
    }

    public void EnableColliders(bool enable)
    {
        foreach (Collider2D collider in Colliders)
        {
            collider.enabled = enable;
        }
    }
}
