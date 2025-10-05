using GCG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : Singleton<UI>
{
    public UIPreScreen PreScreen;
    public UIActiveScreen ActiveScreen;
    public UIPostScreen PostScreen;
    public Slider VolumeSlider;

    private void Start()
    {
        VolumeSlider.value = float.Parse(UserDataManager.GetSavedValue("volume", "0,5"));
    }

    public void SetVolume(float value)
    {
        UserDataManager.SetSavedValue("volume", value);
        AudioListener.volume = value;
    }
}
