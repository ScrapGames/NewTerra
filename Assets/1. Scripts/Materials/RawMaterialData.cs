using UnityEngine;

namespace CraftingResources
{
    [CreateAssetMenu(menuName = "NewTerra/Materials/Raw Material")]
    public class RawMaterialData : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField] private string nameID;
        public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }
        public Sprite icon;
        public ProcessedOutput[] processedOutputs;


        [System.Serializable]
        public struct ProcessedOutput
        {
            public ElementData element;

            [Tooltip("Number of processed element produced from raw material")]
            public float amount;
        }
    }
}