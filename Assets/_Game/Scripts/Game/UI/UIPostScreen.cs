using GCG;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPostScreen : UIScreen
{
    public TextMeshProUGUI ScoreText;
    public Button BackButton;

    public override void Show()
    {
        var trashCollector = FindAnyObjectByType<TrashCollector>();

        int totalValue = trashCollector.TotalValue;
        ScoreText.text = "SCORE\n€ " + totalValue.FormatCurrency();

        int prevHighscore = int.Parse(UserDataManager.GetSavedValue("highscore", "0"));

        if (totalValue > prevHighscore)
        {
            UserDataManager.SetSavedValue("highscore", totalValue.ToString());
        }

        int totalEarned = int.Parse(UserDataManager.GetSavedValue("total_earned", "0"));
        UserDataManager.SetSavedValue("total_earned", (totalEarned + totalValue).ToString());

        BackButton.onClick.RemoveAllListeners();
        BackButton.onClick.AddListener(OnBackClicked);

        base.Show();
    }

    private void OnBackClicked()
    {
        Hide();
        SceneManager.LoadScene("Main");
    }
}
