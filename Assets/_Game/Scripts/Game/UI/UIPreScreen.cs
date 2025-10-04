using DG.Tweening;
using GCG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPreScreen : UIScreen
{
    public TextMeshProUGUI HighscoreText;
    public TextMeshProUGUI TotalEarnedText;
    public Button PlayButton;

    public override void Show()
    {
        HighscoreText.text = "HIGHSCORE\n€ " + int.Parse(UserDataManager.GetSavedValue("highscore", "0")).FormatCurrency();
        TotalEarnedText.text = "TOTAL EARNED\n€ " + int.Parse(UserDataManager.GetSavedValue("total_earned", "0")).FormatCurrency();

        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(OnStartClicked);

        base.Show();
    }

    private void OnStartClicked()
    {
        Hide();
        GameController.Instance.SetActiveState();
    }
}
