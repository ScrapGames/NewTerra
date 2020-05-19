using UnityEngine;

public class EPMInstance : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] EPM.EpicPoolManager epm;
    static EPM.EpicPoolManager _epm;

    private void Awake()
    {
        if (epm == null)
        {
            Debug.LogError("EPM: No pool manager assigned!");
            return;
        }
        _epm = epm;
        epm.BuildPools();
    }

    /// <summary>
    /// Spawns an item from a specific pool, this item will be childed to the root of the scene
    /// </summary>
    /// <param name="id">ID of the pool to spawn from</param>
    /// <returns>GameObject of the item from the pool</returns>
    public static GameObject SpawnObject(string id)
    {
        return SpawnObject(id, null);
    }


    /// <summary>
    /// Spawns an item from a specific pool
    /// </summary>
    /// <param name="id">ID of the pool to spawn from</param>
    /// <param name="parent">Parent transform to assign the object</param>
    /// <returns>GameObject of the item from the pool</returns>
    public static GameObject SpawnObject(string id, Transform parent)
    {
        return _epm.SpawnObject(id, parent);
    }


    /// <summary>
    /// Spawns an item from a specific pool using a gameObject as the identifier. This item will be childed to the root of the scene
    /// </summary>
    /// <param name="prefab">Prefab to spawn</param>    
    /// <returns></returns>
    public static GameObject SpawnObject(GameObject prefab)
    {
        return _epm.SpawnObject(prefab, null);
    }

    /// <summary>
    /// Creates a runtime pool for a gameObject
    /// <param name="prefab">Prefab to pool.</param>    
    public static void AddNewPool(GameObject prefab, int poolSize)
    {
        _epm.AddNewPool(prefab, poolSize);
    }

    /// <summary>
    /// Spawns an item from a specific pool using a gameObject as the identifier
    /// </summary>
    /// <param name="prefab">Prefab to spawn</param>
    /// <param name="parent">Parent transform to assign the object</param>
    /// <returns></returns>
    public static GameObject SpawnObject(GameObject prefab, Transform parent)
    {
        return _epm.SpawnObject(prefab, parent);
    }

    /// <summary>
    /// Creates a new reference to an Epic Pool Manager asset 
    /// and initializes it
    /// </summary>
    /// <param name="epm">New EPM asset to reference</param>
    public static void CreateNewInstance(EPM.EpicPoolManager epm)
    {
        _epm = epm;
        epm.BuildPools();
    }

    public static void ClearInstance()
    {
        _epm.ClearPools();
        _epm = null;
    }

}
