using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_ButtonJuice : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
#pragma warning disable CS0649
    public enum JuiceType
    {
        Scale,
        Bounce,
        Move
    }

    [SerializeField, Header("General Settings")] private JuiceType juiceType = JuiceType.Scale;
    [SerializeField] private Ease easeType = Ease.OutBack;
    [SerializeField, Range(0, 5)] private float duration = 0.24f;
    [SerializeField] private bool playOnMouseEnter = false, playOnMouseExit = false;
    [SerializeField] private bool playOnSelect = true, playOnDeselect = true;
    [SerializeField] private bool submitOnSelect, selectOnHighlight;
    [SerializeField] private bool disableOnComplete = false;

    [SerializeField, Header("Bounce Settings")] private Vector3 bounceAmount = Vector3.zero;
    [SerializeField] private int bounceVibrato = 3;
    [SerializeField, Range(0, 1)] private float bounceElasticity = 0.5f;
    [SerializeField, Range(0, 2), Header("Scale Settings")] private float scaleAmount = 1.1f;
    [SerializeField, Header("Move Settings")] private Vector2 moveDirection = Vector2.zero;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!playOnDeselect) return;
        DoJuice(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!playOnSelect) return;
        DoJuice(true);
        if (submitOnSelect)
            button.onClick.Invoke();
    }

    public void DoJuice(bool select)
    {
        switch (juiceType)
        {
            default:
            case JuiceType.Scale:
                Scale(select);
                break;
            case JuiceType.Bounce:
                Bounce();
                break;
            case JuiceType.Move:
                Slide();
                break;
        }
    }

    private void Bounce()
    {
        transform.DOKill(true);
        transform.localScale = Vector3.one;
        transform.DOPunchScale(bounceAmount, duration, bounceVibrato, bounceElasticity).SetEase(easeType).SetUpdate(true);
    }

    private void Slide()
    {
        transform.DOKill(true);
        RectTransform rt = transform as RectTransform;
        Tween tween = rt.DOAnchorPos(moveDirection, duration).SetEase(easeType).SetUpdate(true);
        if (disableOnComplete)
            tween.OnComplete(() => gameObject.SetActive(false));
    }

    private void Scale(bool select)
    {
        Vector3 scale = select ? Vector3.one * scaleAmount : Vector3.one;
        transform.DOKill(true);
        transform.DOScale(scale, duration).SetEase(easeType).SetUpdate(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectOnHighlight)
            button.Select();

        if (!playOnMouseEnter) return;
        DoJuice(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!playOnMouseExit) return;
        if (playOnDeselect)
            DoJuice(false);
    }

}
