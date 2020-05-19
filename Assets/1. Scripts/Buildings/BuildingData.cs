using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace Buildings
{
    public abstract class BuildingData : ScriptableObject
    {
#pragma warning disable CS0649
        public Sprite icon;
        public string nameID;
        public AssetReferenceGameObject prefabRef;
        public int poolSize;
        public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }
        public string LocalizedDescription { get { return LocalizationManager.GetText(nameID + "-D"); } }

        public BuildingMaterial[] requiredMaterialsToBuild;
        public BuildingComponent[] requiredComponentsToBuild;


        public async Task LoadBuildingPool()
        {
            var loadOp = Addressables.LoadAssetAsync<GameObject>(prefabRef);

            await loadOp.Task;

            // Send into pool
            GameObject go = loadOp.Result;

            // Set Data
            BuildingBase buildingBase = go.GetComponent<BuildingBase>();
            if(buildingBase != null) buildingBase.data = this;

            Debug.Log("Loaded Asset: " + go.name);
            go.name = nameID;
            EPMInstance.AddNewPool(go, poolSize);
            //Addressables.Release(loadOp);
        }

        public GameObject GetBuilding()
        {
            return EPMInstance.SpawnObject(nameID);
        }
    }
}