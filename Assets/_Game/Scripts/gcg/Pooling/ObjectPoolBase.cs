using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCG
{
    [System.Serializable]
    public abstract class ObjectPoolBase
    {
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected Transform parent;

        protected List<GameObject> pool = new List<GameObject>();
        private bool initialized = false;

        /// <summary>
        /// Finds any pre-existing children in the parent that contain the same component type
        /// as the prefab and adds them to the pool. Runs once lazily.
        /// </summary>
        protected void EnsureInitialized<T>() where T : Component
        {
            if (initialized || prefab == null || parent == null)
                return;

            var prefabComponent = prefab.GetComponent<T>();
            if (prefabComponent == null)
            {
                Debug.LogWarning($"Prefab {prefab.name} does not have required component {typeof(T).Name}.");
                initialized = true;
                return;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                if (child.GetComponent<T>() != null)
                {
                    if (!pool.Contains(child))
                        pool.Add(child);
                }
            }

            initialized = true;
        }

        protected GameObject CreateNew<T>() where T : Component
        {
            EnsureInitialized<T>();
            var obj = Object.Instantiate(prefab, parent);
            pool.Add(obj);
            return obj;
        }

        public void DeactivateAll<T>() where T : Component
        {
            EnsureInitialized<T>();
            foreach (var obj in pool)
            {
                if (obj.activeSelf)
                    obj.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class ObjectPool<T> : ObjectPoolBase where T : Component
    {
        public T GetOne()
        {
            EnsureInitialized<T>();

            // Reuse inactive
            foreach (var obj in pool)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj.GetComponent<T>();
                }
            }

            // Or create new
            var newObj = CreateNew<T>();
            return newObj.GetComponent<T>();
        }

        public List<T> GetActive(int count)
        {
            EnsureInitialized<T>();
            List<T> result = new List<T>();

            for (int i = 0; i < count; i++)
            {
                GameObject obj;
                if (i < pool.Count)
                {
                    obj = pool[i];
                } else
                {
                    obj = CreateNew<T>();
                }

                if (!obj.activeSelf)
                    obj.SetActive(true);

                result.Add(obj.GetComponent<T>());
            }

            // Deactivate leftovers
            for (int i = count; i < pool.Count; i++)
            {
                if (pool[i].activeSelf)
                    pool[i].SetActive(false);
            }

            return result;
        }
    }

}