using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using Buildings;


public class DataManager : Singleton<DataManager>
{
#pragma warning disable CS0649
    [SerializeField, Header("Asset Groups")] private AssetLabelReference elementsRef;
    [SerializeField] private AssetLabelReference harvesterBlueprintRef, refineryBlueprintRef, dropPodBlueprintRef;

    // List of object databases
    public Dictionary<string, ScriptableObject> ElementDatabase { get; private set; }
    public Dictionary<string, ScriptableObject> HarvesterBlueprintDatabase { get; private set; }
    public Dictionary<string, ScriptableObject> RefineryBlueprintDatabase { get; private set; }
    public Dictionary<string, ScriptableObject> DropPodBlueprintDatabase { get; private set; }


    private void Awake()
    {
        DontDestroyOnLoad(this);
        ElementDatabase = new Dictionary<string, ScriptableObject>();
        RefineryBlueprintDatabase = new Dictionary<string, ScriptableObject>();
        HarvesterBlueprintDatabase = new Dictionary<string, ScriptableObject>();
        DropPodBlueprintDatabase = new Dictionary<string, ScriptableObject>();
    }

    public async Task LoadAllDatabasesAsync()
    {
        List<Task> tasks = new List<Task>();

        // Queue up databases to load
        tasks.Add(LoadDatabaseAsync(DropPodBlueprintDatabase, dropPodBlueprintRef));
        tasks.Add(LoadDatabaseAsync(HarvesterBlueprintDatabase, harvesterBlueprintRef));
        tasks.Add(LoadDatabaseAsync(RefineryBlueprintDatabase, refineryBlueprintRef));

        // Wait for load - but run other background tasks
        await Task.WhenAll(tasks);

        foreach (Task t in tasks)
        {
            t.Dispose();
        }

        // Pool objects
        tasks.Clear();
        tasks.Add(CreateBuildingPoolAsync(DropPodBlueprintDatabase));
        tasks.Add(CreateBuildingPoolAsync(HarvesterBlueprintDatabase));
        tasks.Add(CreateBuildingPoolAsync(RefineryBlueprintDatabase));
        await Task.WhenAll(tasks);
        foreach (Task t in tasks)
        {
            t.Dispose();
        }
    }

    private async Task CreateBuildingPoolAsync(Dictionary<string, ScriptableObject> database)
    {
        List<Task> tasks = new List<Task>();

        foreach (BuildingData building in database.Values)
        {
            tasks.Add(building.LoadBuildingPool());
        }

        await Task.WhenAll(tasks);

        foreach (Task t in tasks)
        {
            t.Dispose();
        }
    }

    // Load a database for scriptable objects
    private async Task LoadDatabaseAsync(Dictionary<string, ScriptableObject> database, AssetLabelReference assetGroup)
    {
        database.Clear();
        var op = Addressables.LoadAssetsAsync<ScriptableObject>(assetGroup, (obj) =>
        {
            database.Add(obj.name, obj);
        });
        await op.Task;
    }
}
