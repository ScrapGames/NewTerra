using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EPM
{
    [CreateAssetMenu(fileName = "Pool Manager", menuName = "Epic Pool Manager/Manager")]
    public class EpicPoolManager : ScriptableObject
    {
        public int defaultPoolSize = 50, defaultGrowSize = 5;
        public string scriptPath = "";
        public string scriptName = "EPMPoolIDs01";
        public List<PoolInfo> poolableObjects;
        List<string> ids;
        public bool IsActive { get; private set; }
        public Dictionary<string, EpicPool>.ValueCollection Pools { get { return _Pools.Values; } }
        Dictionary<string, EpicPool> _Pools;
        Dictionary<GameObject, string> prefabLookup;

        List<PoolInfo> runtimeGeneratedPools = new List<PoolInfo>(); // Pools created at runtime which do not match the database


        public void AddPoolData(PoolInfo info)
        {
            poolableObjects.Add(info);
            ids.Add(info.id);
        }

        /// <summary>
        /// Initializes the data object - ONLY TO BE USED IN EDITOR
        /// </summary>
        public void EditorInit()
        {
            if (Application.isPlaying) return;
            if (poolableObjects == null)
                poolableObjects = new List<PoolInfo>();
            //if (ids == null)
            BuildIDs();
        }

        public void RemoveObject(PoolInfo obj)
        {
            ids.Remove(obj.id);
            poolableObjects.Remove(obj);
        }

        public void UpdateInfo(PoolInfo update)
        {
            poolableObjects[ids.IndexOf(update.id)] = update;
        }

        public bool CheckIfEntryExists(string id)
        {
            return ids.Contains(id);
        }

        public void BuildIDs()
        {
            ids = new List<string>();

            for (int i = 0; i < poolableObjects.Count; i++)
            {
                ids.Add(poolableObjects[i].id);
            }
        }

        public void BuildPools()
        {
            if (poolableObjects.Count == 0)
            {
                Debug.LogError("EPM: No pools to build! Check the pool manager!");
                return;
            }
            BuildIDs();

            _Pools = new Dictionary<string, EpicPool>();
            prefabLookup = new Dictionary<GameObject, string>();
            for (int i = 0; i < poolableObjects.Count; i++)
            {
                _Pools.Add(poolableObjects[i].id, new EpicPool(poolableObjects[i]));
                prefabLookup.Add(poolableObjects[i].prefab, poolableObjects[i].id);
            }
            IsActive = true;
            Application.quitting += ClearRuntimePools;
        }

        public GameObject SpawnObject(GameObject prefab, Transform parent)
        {
            AddNewPool(prefab);
            return _Pools[SanitizeName(prefab.name)].Spawn(parent);
        }

        public void AddNewPool(GameObject prefab)
        {
            AddNewPool(prefab, defaultPoolSize);
        }

        public void AddNewPool(GameObject prefab, int poolSize)
        {
            if (prefab == null)
            {
                Debug.LogError("Attempted to create a pull with a null prefab - aborting");
                return;
            }

            string id = SanitizeName(prefab.name);
            if (ids.Contains(id))
            { Debug.LogWarningFormat("{0} pool already exists!", prefab.name); return; }

            // This prefab has not been added to the EPM database!
            // Create new entry to the database with default values

            Debug.LogWarningFormat("Prefab {0} is not managed by EPM! It is recommended to add it to the EPM database before spawning it. " +
                "Creating pool for {1}...", prefab.name, id);

            PoolInfo info = new PoolInfo()
            {
                prefab = prefab,
                id = id,
                growSize = defaultGrowSize,
                initialSize = defaultPoolSize,
                sendReset = false
            };
            AddPoolData(info);
            _Pools.Add(id, new EpicPool(info));
            prefabLookup.Add(prefab, id);
            runtimeGeneratedPools.Add(info);
        }

        public GameObject SpawnObject(string id, Transform parent)
        {
            id = SanitizeName(id);
            if (!ids.Contains(id))
            {
                Debug.LogError("EPM: No pool exists with ID: " + id);
                return null;
            }
            return _Pools[id].Spawn(parent);
        }

        public void ClearPools()
        {
            foreach (EpicPool pool in _Pools.Values)
            {
                pool.RemovePool();
            }
            _Pools.Clear();
            _Pools = null;
            prefabLookup.Clear();
            prefabLookup = null;
            ClearRuntimePools();
            IsActive = false;
        }

        private void OnDestroy()
        {
            ClearPools();
        }

        void ClearRuntimePools()
        {
            Application.quitting -= ClearRuntimePools;
            for (int i = runtimeGeneratedPools.Count - 1; i >= 0; i--)
            {
                PoolInfo pool = runtimeGeneratedPools[i];
                poolableObjects.Remove(pool);
                ids.Remove(pool.id);
                runtimeGeneratedPools.RemoveAt(i);
            }
            runtimeGeneratedPools.Clear();
            runtimeGeneratedPools = null;
        }

        public static string SanitizeName(string name)
        {
            Regex r = new Regex("[A-Za-z0-9_]");
            MatchCollection match = r.Matches(name);
            string ret = "";
            foreach (var s in match)
            {
                ret += s.ToString();
            }
            return ret;
        }
    }
}
