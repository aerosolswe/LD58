using GCG;
using UnityEngine;

public class TrashPool : MonoBehaviour
{
    public ObjectPool<Trash> BagPool;
    public SpriteRenderer SpawnBounds;

    [Header("Spawn Settings")]
    public int trashCount = 25;
    public float jitter = 0.3f; // how much randomness inside each cell

    private void Start()
    {
        SpawnTrash();
    }

    public void SpawnTrash()
    {
        Bounds bounds = SpawnBounds.bounds;

        // Figure out how many grid cells we need (square-ish grid)
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(trashCount));
        float cellWidth = bounds.size.x / gridSize;
        float cellHeight = bounds.size.y / gridSize;

        int spawned = 0;

        for (int gx = 0; gx < gridSize; gx++)
        {
            for (int gy = 0; gy < gridSize; gy++)
            {
                if (spawned >= trashCount)
                    return;

                var trash = BagPool.GetOne();

                // Base position in the cell center
                float x = bounds.min.x + gx * cellWidth + cellWidth / 2f;
                float y = bounds.min.y + gy * cellHeight + cellHeight / 2f;

                // Add jitter to make it look natural
                x += Random.Range(-cellWidth * jitter, cellWidth * jitter);
                y += Random.Range(-cellHeight * jitter, cellHeight * jitter);

                trash.transform.position = new Vector3(x, y, 0f);

                spawned++;
            }
        }
    }
}
