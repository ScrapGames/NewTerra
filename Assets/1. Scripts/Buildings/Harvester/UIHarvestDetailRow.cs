using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Buildings;

public class UIHarvestDetailRow : MonoBehaviour
{
    public Image image_Icon, image_CountBG;    
    public TextMeshProUGUI text_Rate, text_Total;

    public void UpdateCount(int count)
    {
        text_Total.text = count.ToString();
        image_CountBG.rectTransform.DOPunchScale(Vector3.one * 1.1f, 0.3f, 4, 1f);
    }

    public void Init(HarvesterData.ExtractionInfo info)
    {
        text_Rate.text = string.Format("{0} / SEC", 
            HarvestJobManager.GetCountPerSec(info.outputPerTick).ToString());
        image_Icon.sprite = info.rawMaterial.icon;
        text_Total.text = "0";
    }
}
