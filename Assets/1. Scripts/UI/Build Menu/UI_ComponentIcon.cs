using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Buildings;
using DG.Tweening;

public class UI_ComponentIcon : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private Image image_Background, imageIcon;
    [SerializeField] private TextMeshProUGUI text_Count;
    [SerializeField] private float animationTime = 0.24f;

    public void SetData(BuildingComponent component)
    {
        imageIcon.sprite = component.component.icon;
        text_Count.text = component.count.ToString();
    }

    public void SetData(BuildingMaterial material)
    {
        imageIcon.sprite = material.material.icon;
        text_Count.text = material.count.ToString();
    }

    public void Animate(float animationDelay)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, animationTime)
            .SetEase(Ease.OutBack)
            .SetDelay(animationDelay);
    }
}
