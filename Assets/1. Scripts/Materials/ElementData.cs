using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingResources
{
    [CreateAssetMenu(menuName = "NewTerra/Materials/Element")]
    public class ElementData : ScriptableObject
    {
#pragma warning disable CS0649
        [SerializeField] private string nameID;
        public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }
        public string elementSymbol;
        public int atomicNumber;
    }
}