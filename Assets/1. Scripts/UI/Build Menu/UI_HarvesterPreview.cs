using UnityEngine;
using Buildings;
using DG.Tweening;

public class UI_HarvesterPreview : UI_PreviewBase
{
#pragma warning disable CS0649

    [SerializeField] private UI_ExtractionRate primaryExtraction, secondaryExtraction;

    public override void ShowPreview(BuildingData data)
    {
        HarvesterData hData = data as HarvesterData;
        // Show Extraction Data
        /*
        primaryExtraction.SetData(hData.primaryExtraction);
        primaryExtraction.Show(true);
        if (hData.secondaryExtraction.rawMaterial != null)
        {
            secondaryExtraction.SetData(hData.secondaryExtraction);
            secondaryExtraction.Show(true);
        }
        else
            secondaryExtraction.Show(false);
        */
        base.ShowPreview(data);
    }
}
