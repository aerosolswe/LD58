using DG.Tweening;
using GCG;
using TMPro;
using UnityEngine;

public class WorldCash : MonoBehaviour
{
    public TextMeshPro Text;
    public AudioSource cashingSFX;

    public void Initialize(int value)
    {
        if (cashingSFX != null)
        {
            cashingSFX.Play();
        }

        Text.text = "€" + value.FormatCurrency();
        transform.DOLocalMoveY(2.0f, 0.5f).SetEase(Ease.OutBack);
        GCGUtil.Wait(this, 2.0f, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
