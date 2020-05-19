using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public abstract class HarvesterBase : BuildingBase
    {
#pragma warning disable CS0649
        public event System.Action<HarvestJobData> HarvestJobUpdate;
        
        public int StackStorageCapacity { get { return (data as HarvesterData).baseStackStorage; } } // TODO: Add upgrade storage here
        
        [SerializeField] private UIHarvesterStatus harvesterStatusPanel;

        protected override void OnPlop(PlopObject plop)
        {
            base.OnPlop(plop);

            // Register to harvester job manager
            Debug.LogFormat("Registering {0} to the harvest job manager", name);
            GameManager.Instance.harvestJobManager.RegisterHarvester(this);

            // Init info panel
            HarvesterData hd = data as HarvesterData;
            if (hd.SingleExtractionHarvester)
                harvesterStatusPanel.Init(hd.primaryExtraction, hd.LocalizedName, this);
            else
                harvesterStatusPanel.Init(hd.primaryExtraction, hd.secondaryExtraction, hd.LocalizedName, this);
        }

        public virtual void OnHarvestJobUpdate(HarvestJobData jobData)
        {
            HarvestJobUpdate?.Invoke(jobData);
        }

    }
}