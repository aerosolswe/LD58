using DG.Tweening;
using GCG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    // only for when initiating from prefab
    [SerializeField] private AudioEffectData dataObject;
    [SerializeField] private GameObject owner;
    [SerializeField] private bool spatial;

    private Coroutine coroutine;

    private float volume = 0.0f;

    public AudioEffectDataObject AudioEffectDataObject
    {
        get; private set;
    }

    public float TimeRemaining
    {
        get => audioSource.time - audioSource.clip.length;
    }

    public float SpatialBlend
    {
        get => audioSource.spatialBlend;
        set => audioSource.spatialBlend = value;
    }

    public bool DestroyOnComplete
    {
        get;
        set;
    }

    public bool IsPlaying
    {
        get => audioSource.isPlaying;
    }

    public bool IsKilling
    {
        get; set;
    }

    public GameObject Owner
    {
        get; private set;
    }

    private void Awake()
    {
        if (dataObject != null)
        {
            Setup(dataObject.GetRandom());

            if (spatial)
            {
                SetSpatialPosition(transform.position, 1.0f);
                Owner = owner;
            }
        }
    }

    public void Setup(AudioEffectDataObject aedo, GameObject owner = null)
    {
        this.AudioEffectDataObject = aedo;
        this.Owner = owner;

        IsKilling = false;

        volume = aedo.volume + Random.Range(-aedo.randomVolumeOffset, aedo.randomVolumeOffset);

        audioSource.Stop();
        audioSource.loop = aedo.playUntilRemoval;
        audioSource.clip = aedo.clip;
        audioSource.volume = 0.0f;
        audioSource.pitch = aedo.pitch + Random.Range(-aedo.randomPitchOffset, aedo.randomPitchOffset);
        SpatialBlend = 0.0f;

        StopAllCoroutines();
    }

    public void SetSpatialPosition(Vector3 worldPosition, float spatialBlend = 1.0f)
    {
        transform.position = worldPosition;
        SpatialBlend = 1.0f;
    }

    public void SimplePlay()
    {
        audioSource.Play();
    }

    public void Play()
    {
        IsKilling = false;
        gameObject.SetActive(true);

        GCGUtil.Lerp(this, 0.0f, volume, AudioEffectDataObject.fadeInTime, (float val) =>
        {
            if (transform != null && audioSource != null)
            {
                audioSource.volume = val;
            }
        });

        if (AudioEffectDataObject.playUntilRemoval)
        {
            audioSource.Play();
            if (AudioEffectDataObject.playTime > 0.0f)
                GCGUtil.Lerp(this, 0.0f, 1.0f, AudioEffectDataObject.playTime, null, Kill);
        } else
        {
            coroutine = StartCoroutine(PlayLoops());
        }
    }

    private void Update()
    {
        if (Owner != null)
            transform.position = Owner.transform.position;
    }

    IEnumerator PlayLoops()
    {
        for (int i = 0; i < AudioEffectDataObject.loops; i++)
        {
            audioSource.Play();
            yield return GCGUtil.Yield(audioSource.clip.length);
        }

        Kill();
    }

    public void Stop(System.Action onComplete)
    {
        if (AudioEffectDataObject.fadeOutTime > 0.0f)
        {
            GCGUtil.Lerp(this, volume, 0.0f, AudioEffectDataObject.fadeOutTime, (float val) =>
            {
                if (transform != null && audioSource != null)
                {
                    audioSource.volume = val;
                }
            }, () => onComplete?.Invoke());
        } else
        {
            onComplete?.Invoke();
        }
    }

    public void Kill()
    {
        if (IsKilling)
            return;

        IsKilling = true;

        if (coroutine != null)
            StopCoroutine(coroutine);

        Stop(() =>
        {
            audioSource.Stop();
            if (DestroyOnComplete)
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(gameObject);
                } else
                {
                    Destroy(gameObject);
                }
            } else
            {
                gameObject.SetActive(false);
            }
        });
    }
}
