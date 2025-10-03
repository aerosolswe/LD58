using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IStateAnimation
{
    public abstract Tween Apply();
    public abstract void OnReset(GameObject gameObject);
}


[Serializable]
public abstract class StateAnimation<T> : IStateAnimation where T : UnityEngine.Object
{
    public float duration = 0.25f;
    public Ease ease = Ease.InOutSine;
    [SerializeReference] public T target;

    public virtual void OnReset(GameObject gameObject)
    {
        if (target == null)
        {
            target = gameObject.GetComponent<T>();
        }
    }

    public virtual Tween Apply()
    {
        if (target != null)
        {
            DOTween.Kill(target);
        }

        return null;
    }

    public bool IsValid()
    {
        return target != null;
    }
}

[Serializable]
public class VisualState
{
    [SerializeReference, SubclassSelector]
    public List<IStateAnimation> animations = new();

    public void Apply()
    {
        foreach (var anim in animations)
        {
            var tween = anim.Apply();

            if (tween != null)
            {
                tween.Play();
            }
        }
    }

    public void OnReset(GameObject gameObject)
    {
        foreach (var anim in animations)
        {
            anim.OnReset(gameObject);
        }
    }
}

[Serializable]
public class LocalPositionAnimation : StateAnimation<Transform>
{
    public Vector3 targetPosition = Vector3.zero;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.DOLocalMove(targetPosition, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class GameObjectActivateAnimation : StateAnimation<GameObject>
{
    public bool activate = false;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            target.SetActive(activate);
            return DOTween.To(null, null, 1.0f, 0.01f);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class ScaleAnimation : StateAnimation<Transform>
{
    public Vector3 targetScale = Vector3.one;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.transform.DOScale(targetScale, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class ScaleSpriteAnimation : StateAnimation<SpriteRenderer>
{
    public Vector3 targetScale = Vector3.one;

    public override Tween Apply()
    {
        base.Apply();

        if (!IsValid())
        {
            return null;
        }

        Bounds bounds = target.bounds;
        Vector3 center = bounds.center;

        // Get offset from current position to bounds center in world space
        Vector3 offsetToCenter = center - target.transform.position;

        // Store initial scale and position
        Vector3 originalScale = target.transform.localScale;
        Vector3 originalPos = target.transform.position;

        // Calculate the expected position change when scaled
        Vector3 scaleFactor = new Vector3(
            targetScale.x / originalScale.x,
            targetScale.y / originalScale.y,
            targetScale.z / originalScale.z
        );

        Vector3 scaledOffset = Vector3.Scale(offsetToCenter, scaleFactor);
        Vector3 newOffset = scaledOffset - offsetToCenter;
        Vector3 adjustedPosition = originalPos - newOffset;

        // Animate both scale and position to maintain visual center
        Tween scaleTween = target.transform.DOScale(targetScale, duration).SetEase(ease);
        Tween moveTween = target.transform.DOMove(adjustedPosition, duration).SetEase(ease);

        return DOTween.Sequence().Join(scaleTween).Join(moveTween);
    }
}

[Serializable]
public class CanvasGroupAnimation : StateAnimation<CanvasGroup>
{
    [Range(0f, 1f)] public float alpha = 1f;
    public bool setInteractable = false;
    [ConditionalHide("setInteractable")] public bool interactable = true;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            if (setInteractable)
            {
                target.interactable = interactable;
                target.blocksRaycasts = interactable;
            }

            return target.DOFade(alpha, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class SpriteAlphaAnimation : StateAnimation<SpriteRenderer>
{
    [Range(0f, 1f)] public float alpha = 1f;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.DOFade(alpha, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class SpriteColorAnimation : StateAnimation<SpriteRenderer>
{
    public Color color = Color.white;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.DOColor(color, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class LightColorAnimation : StateAnimation<Light2D>
{
    public Color color = Color.white;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.DOColor(color, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class ImageColorAnimation : StateAnimation<UnityEngine.UI.Image>
{
    public Color color = Color.white;

    public override Tween Apply()
    {
        base.Apply();

        if (IsValid())
        {
            return target.DOColor(color, duration).SetEase(ease);
        } else
        {
            return null;
        }
    }
}

[ExecuteAlways]
public class StateSwitcher : MonoBehaviour
{
    [SerializeField]
    private List<VisualState> states = new List<VisualState>();

    public List<VisualState> States => states;

    public int SelectedState = 0;
    public bool ExecuteOnAwake = false;
    public int ExecuteState = 0;

    private void Start()
    {
        if (Application.isPlaying && ExecuteOnAwake)
        {
            ApplyState(ExecuteState);
        }
    }

    public void ApplyState(int index)
    {
        if (index < 0 || index >= states.Count)
            return;

        SelectedState = index;

        states[index].Apply();
    }

    private void Reset()
    {
        foreach (var state in states)
        {
            state.OnReset(gameObject);
        }
    }

    private void OnValidate()
    {
        foreach (var state in states)
        {
            state.OnReset(gameObject);
        }
    }

}
