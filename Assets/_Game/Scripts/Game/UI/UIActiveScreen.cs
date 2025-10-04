using DG.Tweening;
using GCG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveScreen : UIScreen
{

    public TextMeshProUGUI TimeText;

    public override void Show()
    {
        UI.Instance.ActiveScreen.TimeText.text = (int)0 + " sec";

        base.Show();
    }

}
