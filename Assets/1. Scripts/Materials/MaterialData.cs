using UnityEngine;

namespace CraftingResources
{
    public abstract class MaterialData : ScriptableObject
    {
        public Sprite icon;
        [SerializeField] protected string nameID;
        public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }
    }
}