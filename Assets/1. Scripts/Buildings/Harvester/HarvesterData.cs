using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingResources;

namespace Buildings
{
    [CreateAssetMenu(fileName = "HarvesterData", menuName = "NewTerra/Buildings/HarvesterData")]
    public class HarvesterData : BuildingData
    {
        public bool SingleExtractionHarvester { get { return secondaryExtraction.outputPerTick == 0; } }
        public ExtractionInfo primaryExtraction;
        public ExtractionInfo secondaryExtraction;
        public int turnsPerStack;
        public int baseStackStorage;


        [System.Serializable]
        public struct ExtractionInfo
        {
            public RawMaterialData rawMaterial;
            [Range(0, 10)] public int outputPerTick;
        }
    }


}