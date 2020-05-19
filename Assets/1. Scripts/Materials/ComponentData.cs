using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CraftingResources
{
    [CreateAssetMenu(menuName = "NewTerra/Materials/ComponentData")]
    public class ComponentData : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField] private string nameID;
        public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }
        public Sprite icon;
        public Buildings.BuildingMaterial[] materials;
        public Buildings.BuildingComponent[] components;

    }
}