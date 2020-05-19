using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UI_BoxedIcon : MonoBehaviour
{
#pragma warning disable CS0649

    public Image icon;
    public TextMeshProUGUI title;
    [SerializeField] private Ease ease;
    public float duration, delay;

    [SerializeField] bool tweenOnEnable = true;

    private void OnEnable()
    {
        if (tweenOnEnable) Tween();
    }

    public void Tween()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(ease).SetDelay(delay);
    }
}


