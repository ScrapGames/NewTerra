using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Buildings
{
    public class UIHarvesterStatus : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private UIHarvestDetailRow primaryDetailRow, secondaryDetailRow;
        [SerializeField] private TextMeshProUGUI text_Name;
        [SerializeField] private Image storageFillSlider;

        public void Shutdown(HarvesterBase harvester)
        {
            harvester.HarvestJobUpdate -= OnHarvestJobUpdate;
        }

        public void Init(HarvesterData.ExtractionInfo primaryExtractionInfo, 
            string harvesterName, HarvesterBase harvester)
        { 
            secondaryDetailRow.gameObject.SetActive(false);
            _Init(primaryExtractionInfo, harvesterName, harvester);
        }
        
        public void Init(HarvesterData.ExtractionInfo primaryExtractionInfo, 
            HarvesterData.ExtractionInfo secondaryExtractionInfo, string harvesterName, HarvesterBase harvester)
        {
            secondaryDetailRow.Init(secondaryExtractionInfo);
            secondaryDetailRow.gameObject.SetActive(true);
            _Init(primaryExtractionInfo, harvesterName, harvester);
        }

        private void _Init(HarvesterData.ExtractionInfo primaryExtractionInfo, 
            string harvesterName, HarvesterBase harvester)
        {
            primaryDetailRow.Init(primaryExtractionInfo);
            storageFillSlider.fillAmount = 0;
            text_Name.text = harvesterName;            
            harvester.HarvestJobUpdate += OnHarvestJobUpdate;
        }

        public void OnHarvestJobUpdate(HarvestJobData jobData)
        {
            if (!jobData.StackCreatedThisJob) return;

            // Set storage capacity
            storageFillSlider.fillAmount = (float)jobData.stackCount / jobData.stackCountMax;

            // Set row counts
            primaryDetailRow.UpdateCount(jobData.PrimaryResourceCount);
            secondaryDetailRow.UpdateCount(jobData.SecondaryResourceCount);
        }
    }
}