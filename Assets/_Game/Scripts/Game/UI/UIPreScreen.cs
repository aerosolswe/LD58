using DG.Tweening;
using GCG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Upgrade
{
    public string Id;
    public int Cost;
    public int Max;
    public string ButtonText;
    public Button Button;
}

public class UIPreScreen : UIScreen
{
    public TextMeshProUGUI HighscoreText;
    public TextMeshProUGUI TotalEarnedText;
    public TextMeshProUGUI CashText;
    public Button PlayButton;

    public Truck truck;

    public Upgrade[] upgrades;

    public override void Show()
    {
        HighscoreText.text = "HIGHSCORE\n€ " + int.Parse(UserDataManager.GetSavedValue("highscore", "0")).FormatCurrency();
        TotalEarnedText.text = "TOTAL EARNED\n€ " + int.Parse(UserDataManager.GetSavedValue("total_earned", "0")).FormatCurrency();

        CashText.text = "€ " + int.Parse(UserDataManager.GetSavedValue("cash", "0")).FormatCurrency();

        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(OnStartClicked);

        InitializeUpgrades();

        base.Show();
    }

    private void OnStartClicked()
    {
        Hide();
        GameController.Instance.SetActiveState();
    }

    public void InitializeUpgrades()
    {
        int cash = int.Parse(UserDataManager.GetSavedValue("cash", "0"));

        foreach (var upgrade in upgrades)
        {
            upgrade.Button.GetComponentInChildren<TextMeshProUGUI>().text = upgrade.ButtonText + "\n" + "€ " + upgrade.Cost;

            int current = int.Parse(UserDataManager.GetSavedValue(upgrade.Id, "0"));

            if (upgrade.Cost > cash || current >= upgrade.Max)
            {
                upgrade.Button.interactable = false;
            }
        }
    }

    public void PressedUpgrade(string id)
    {
        var upgrade = GetUpgrade(id);

        if (upgrade == null)
            return;

        int cash = int.Parse(UserDataManager.GetSavedValue("cash", "0"));

        if (cash >= upgrade.Cost)
        {
            cash -= upgrade.Cost;
            UserDataManager.SetSavedValue("cash", cash);
            CashText.text = "€ " + int.Parse(UserDataManager.GetSavedValue("cash", "0")).FormatCurrency();

            int current = int.Parse(UserDataManager.GetSavedValue(id, "0"));
            UserDataManager.SetSavedValue(id, current + 1);

            truck.Initialize();
            InitializeUpgrades();
        }
    }

    public Upgrade GetUpgrade(string id)
    {
        foreach (var u in upgrades)
        {
            if (u.Id == id)
                return u;
        }

        return null;
    }
}
