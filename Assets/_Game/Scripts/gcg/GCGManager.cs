using GCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCGManager : Singleton<GCGManager>
{

    private void Awake()
    {
        SetPersistent(this);
    }

    public bool Initialized
    {
        get; private set;
    }

    public IEnumerator Init()
    {
        if (Initialized)
            yield break;


        UserDataManager.Init();
        GCGUtil.Log("UserData initialized");

        Initialized = true;
    }

}
