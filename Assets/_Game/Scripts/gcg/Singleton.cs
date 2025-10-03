using GCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public void SetPersistent(T inst)
    {
        if (Exists())
        {
            Destroy(this.gameObject);
        } else
        {
            instance = inst;
            DontDestroyOnLoad(gameObject);
        }

    }

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                {
                    GCGUtil.LogError("Singleton doesn't exist.");
                }
            }

            return instance;
        }
    }

    public static bool Exists()
    {
        return instance != null;
    }

}
