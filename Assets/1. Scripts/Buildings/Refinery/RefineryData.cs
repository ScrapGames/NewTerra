using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingResources;

namespace Buildings
{
    [CreateAssetMenu(fileName = "Refinery", menuName = "NewTerra/Buildings/Refinery")]
    public class RefineryData : BuildingData
    {
        public RawMaterialData inputMaterial;
        public ProcessedMaterialData[] outputMaterials;

        public int powerRequirement;
    }
}