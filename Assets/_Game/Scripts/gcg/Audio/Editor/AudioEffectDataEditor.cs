using GCG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioEffectData))]
public class AudioEffectDataEditor : Editor
{
    private int previewIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var aed = (AudioEffectData)target;

        previewIndex = EditorGUILayout.IntField("Preview Index", previewIndex);

        if (GUILayout.Button("Preview sound"))
        {
            if (previewIndex < 0 || previewIndex >= aed.Amount())
            {
                GCGUtil.LogError("Preview index is out of range");
                return;
            }

            var ad = aed.GetAtIndex(previewIndex);
            var audioObjectPrefab = Resources.Load<AudioObject>("AudioObject");
            var ao = Instantiate(audioObjectPrefab);
            ao.Setup(ad);
            ao.DestroyOnComplete = true;
            ao.Play();
        }

        if (GUILayout.Button("Preview random sound"))
        {

            int index = aed.GetRandomIndex();
            GCGUtil.Log("Previewing index " + index);
            var ad = aed.GetAtIndex(index);
            var audioObjectPrefab = Resources.Load<AudioObject>("AudioObject");
            var ao = Instantiate(audioObjectPrefab);
            ao.Setup(ad);
            ao.DestroyOnComplete = true;
            ao.Play();
        }
    }
}
