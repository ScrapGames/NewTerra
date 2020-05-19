using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Buildings
{
    public class UI_RefineryPreview : UI_PreviewBase
    {
#pragma warning disable CS0649
        [SerializeField] private Image inputIcon, outputIcon1, outputIcon2, outputIcon3;

        public override void ShowPreview(BuildingData data)
        {
            RefineryData rData = data as RefineryData;

            /*
            inputIcon.sprite = rData.inputMaterial.icon;
            outputIcon1.sprite = rData.outputMaterials[0].icon;

            if (rData.outputMaterials.Length > 1)
            {
                outputIcon2.sprite = rData.outputMaterials[1].icon;
                outputIcon2.gameObject.SetActive(true);
            }
            else
                outputIcon2.gameObject.SetActive(false);

            if (rData.outputMaterials.Length > 2)
            {
                outputIcon3.sprite = rData.outputMaterials[2].icon;
                outputIcon3.gameObject.SetActive(true);
            }
            else
                outputIcon3.gameObject.SetActive(false);
            */
            base.ShowPreview(data);
        }
    }
}