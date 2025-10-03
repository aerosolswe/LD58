using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GCG
{
    public class AudioManager : Singleton<AudioManager>
    {

        [SerializeField] private AudioObject audioObjectPrefab;
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private float baseMasterVolume = 1.0f;
        [SerializeField] private float baseMusicVolume = 1.0f;
        [SerializeField] private float baseEffectsVolume = 1.0f;

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip[] musicClips;
        [SerializeField][Range(1.0f, 60.0f)] private float musicMinDelay = 5.0f;
        [SerializeField][Range(1.0f, 60.0f)] private float musicMaxDelay = 30.0f;

        [Header("Effects")]
        [SerializeField] private int ConcurrentSFX = 64;
        [SerializeField] private AudioEffectData defaultClickAudio;
        [SerializeField] private AudioEffectData defaultPopupOpenAudio;
        [SerializeField] private AudioEffectData defaultPopupHideAudio;

        private List<AudioObject> audioObjects = new List<AudioObject>();

        public bool Initialized
        {
            get; private set;
        }

        private void Awake()
        {
            SetPersistent(this);
        }

        public IEnumerator Init()
        {
            if (Initialized)
                yield break;

            StartMusic();

            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
            EffectsVolume = EffectsVolume;

            yield return null;

            for (int i = 0; i < ConcurrentSFX; i++)
            {
                var ao = Instantiate(audioObjectPrefab, transform);
                ao.gameObject.SetActive(false);
                audioObjects.Add(ao);
            }

            Initialized = true;
        }


        public void StartMusic()
        {
            bool firstTimePlaying = UserDataManager.GetSavedValue("FirstTimePlaying", "1").ToBool();
            musicSource.clip = firstTimePlaying ? musicClips[0] : musicClips[Random.Range(0, musicClips.Length)];

            musicSource.Play();
            StartCoroutine(MusicLoop());

            IEnumerator MusicLoop()
            {
                bool playingMusic = true;
                int lastIndex = 0;

                while (playingMusic)
                {
                    yield return GCGUtil.Yield(musicSource.clip.length);
                    float delay = Random.Range(musicMinDelay, musicMinDelay + musicMaxDelay);
                    yield return GCGUtil.Yield(delay);

                    List<AudioClip> clips = new List<AudioClip>();

                    for (int i = 0; i < musicClips.Length; i++)
                    {
                        if (i == lastIndex)
                            continue;
                        clips.Add(musicClips[i]);
                    }

                    int randomIndex = Random.Range(0, clips.Count);
                    lastIndex = randomIndex;
                    var clip = clips[randomIndex];
                    musicSource.clip = clip;
                    musicSource.Play();
                }

            }
        }

        public AudioObject GetAvailableAudioObject(int priority = 1)
        {
            List<AudioObject> activeAudioObjects = new List<AudioObject>();
            foreach (AudioObject ao in audioObjects)
            {
                if (!ao.gameObject.activeInHierarchy)
                    return ao;
                else
                    activeAudioObjects.Add(ao);
            }

            activeAudioObjects.Sort((p1, p2) => p1.TimeRemaining.CompareTo(p2.TimeRemaining));
            activeAudioObjects.Reverse();
            foreach (AudioObject ao in activeAudioObjects)
            {
                if (ao.AudioEffectDataObject.priority < priority)
                {
                    ao.Kill();
                    return ao;
                }
            }

            var newAO = Instantiate(audioObjectPrefab, transform);
            newAO.gameObject.SetActive(false);
            audioObjects.Add(newAO);

            return newAO;
        }

        public void PlayClip(AudioEffectData aed)
        {
            PlayClip(aed.GetRandom());
        }

        public void PlayClip(AudioEffectDataObject aedo)
        {
            var ao = GetAvailableAudioObject();
            if (ao == null)
                return;

            ao.Setup(aedo);
            ao.Play();
        }

        public void PlayClick(float volume = 1.0f) => PlayClip(defaultClickAudio.GetRandom());

        public void PlayOpenPopup(float volume = 1.0f) => PlayClip(defaultPopupOpenAudio.GetRandom());
        public void PlayHidePopup(float volume = 1.0f) => PlayClip(defaultPopupHideAudio.GetRandom());

        public static AudioClip GetRandomClip(AudioClip[] clipArray)
        {
            return clipArray[Random.Range(0, clipArray.Length)];
        }

        public static AudioClip GetRandomClip(List<AudioClip> clipList)
        {
            return clipList[Random.Range(0, clipList.Count)];
        }

        public static float MasterVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MasterVolume", Instance.baseMasterVolume);
            }
            set
            {
                value = Mathf.Clamp(value, 0.001f, 1.0f);
                PlayerPrefs.SetFloat("MasterVolume", value);
                PlayerPrefs.Save();
                Instance.audioMixer.SetFloat("MasterVolume", Mathf.Log(value) * 20);
            }
        }

        public static float MusicVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MusicVolume", Instance.baseMusicVolume);
            }
            set
            {
                value = Mathf.Clamp(value, 0.001f, 1.0f);
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
                Instance.audioMixer.SetFloat("MusicVolume", Mathf.Log(value) * 20);
            }
        }

        public static float EffectsVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("EffectsVolume", Instance.baseEffectsVolume);
            }
            set
            {
                value = Mathf.Clamp(value, 0.001f, 1.0f);
                PlayerPrefs.SetFloat("EffectsVolume", value);
                PlayerPrefs.Save();
                Instance.audioMixer.SetFloat("EffectsVolume", Mathf.Log(value) * 20);
            }
        }

    }
}