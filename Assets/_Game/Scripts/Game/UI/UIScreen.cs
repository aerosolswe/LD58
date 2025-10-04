using DG.Tweening;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public CanvasGroup cg;

    public virtual void Show()
    {
        cg.DOFade(1.0f, 0.2f);
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        cg.DOFade(0.0f, 0.1f);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
