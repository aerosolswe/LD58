using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioEffectDataObject
{
    public AudioClip clip;
    public float volume = 1.0f;
    public float randomVolumeOffset = 0.0f;
    public float pitch = 1.0f;
    public float randomPitchOffset = 0.0f;
    public float fadeInTime = 0.0f;
    public float fadeOutTime = 0.0f;
    public float playTime = 0.0f;
    public int loops = 1;
    public int priority = 1;
    public bool playUntilRemoval = false;
    public bool attachedToOwner = false;

    public AudioEffectDataObject()
    {
        this.volume = 1.0f;
        this.pitch = 1.0f;
        this.loops = 1;
        this.priority = 1;
    }
}

[CreateAssetMenu(fileName = "AED_audio", menuName = "Data/AudioObject")]
public class AudioEffectData : ScriptableObject
{
    [SerializeField] private AudioEffectDataObject[] audioEffectDataObjects;

    public AudioEffectDataObject GetRandom()
    {
        return GetAtIndex(GetRandomIndex());
    }

    public AudioEffectDataObject GetAtIndex(int index)
    {
        return audioEffectDataObjects[index];
    }

    public int GetRandomIndex()
    {
        return UnityEngine.Random.Range(0, audioEffectDataObjects.Length);
    }

    public int Amount()
    {
        return audioEffectDataObjects.Length;
    }
}
