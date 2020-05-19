using UnityEngine;

namespace CraftingResources
{
    [CreateAssetMenu(menuName = "NewTerra/Materials/Alloy")]
    public class AlloyData : MaterialData
    {
        public AlloyCompound[] compounds;


        [System.Serializable]
        public struct AlloyCompound
        {
            public ElementData element;
            public int contentPercentage;
        }
    }
}