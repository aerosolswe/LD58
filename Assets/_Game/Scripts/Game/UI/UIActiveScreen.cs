using DG.Tweening;
using GCG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveScreen : UIScreen
{

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;

    public override void Show()
    {
        ScoreText.text = "€" + 0;
        TimeText.text = (int)0 + " sec";

        base.Show();
    }

}
