using DG.Tweening;
using GCG;
using TMPro;
using UnityEngine;

public class WorldCash : MonoBehaviour
{
    public TextMeshPro Text;

    public void Initialize(int value)
    {
        Text.text = "$" + value.FormatCurrency();
        transform.DOLocalMoveY(2.0f, 1.0f);
        GCGUtil.Wait(this, 2.0f, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
