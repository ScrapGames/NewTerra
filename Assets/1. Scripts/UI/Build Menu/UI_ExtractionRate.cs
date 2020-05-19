using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ExtractionRate : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private Image icon;
    [SerializeField] private Widgets.Speedometer speedometer;
    private float rate;
    public void SetData(Buildings.HarvesterData.ExtractionInfo extractionInfo)
    {
        icon.sprite = extractionInfo.rawMaterial.icon;
        rate = extractionInfo.outputPerTick;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
        {
            speedometer.SetValue(rate);
        }
    }

}
