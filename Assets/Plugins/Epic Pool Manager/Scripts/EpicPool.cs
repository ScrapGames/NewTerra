using System.Collections.Generic;
using UnityEngine;

namespace EPM
{
    public class EpicPool
    {
        PoolInfo info;
        EPMReycleObject[] pool;
        static GameObject parentPoolObj;
        Transform readyPoolObj;
        Stack<int> readySet;

        public EpicPool(PoolInfo poolInfo)
        {            
            const string name = "Epic Pool Manager - Ready Objects";
            info = poolInfo;
            pool = new EPMReycleObject[poolInfo.initialSize];
            readySet = new Stack<int>(poolInfo.initialSize);            

            if (parentPoolObj == null)
            {
                parentPoolObj = new GameObject(name);
                GameObject.DontDestroyOnLoad(parentPoolObj);
            }
            readyPoolObj = new GameObject(poolInfo.prefab.name + " Ready Pool").transform;
            readyPoolObj.SetParent(parentPoolObj.transform);

            for (int i = 0; i < poolInfo.initialSize; i++)
            {
                pool[i] = CreateNewInstance(i);
            }
        }

        EPMReycleObject CreateNewInstance(int id)
        {
            EPMReycleObject recycler = GameObject.Instantiate(info.prefab).AddComponent<EPMReycleObject>();
            recycler.gameObject.name = info.prefab.name + ": " + id.ToString();
            recycler.Init(id, readyPoolObj);
            recycler.Recycled += OnRecycle;
            readySet.Push(id);
            return recycler;
        }

        void GrowPool()
        {
            int newLen = pool.Length + info.growSize;
            EPMReycleObject[] newPool = new EPMReycleObject[newLen];            
            for (int i = 0; i < pool.Length; i++)
            {
                newPool[i] = pool[i];
            }
            for (int i = pool.Length; i < newLen; i++)
            {
                newPool[i] = CreateNewInstance(i);
            }            
            pool = newPool;
        }

        public void RemovePool()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                EPMReycleObject obj = pool[i];
                obj.Recycled -= OnRecycle;
                GameObject.Destroy(obj.gameObject);
            }
            readySet.Clear();
            readySet = null;
            GameObject.Destroy(readyPoolObj.gameObject);
        }

        public GameObject Spawn(Transform newParent)
        {
            if(readySet.Count == 0)
            {
                GrowPool();
            }

            int id = readySet.Pop();
            EPMReycleObject obj = pool[id];
            obj.transform.SetParent(newParent);            
            obj.gameObject.SetActive(true);            
            return obj.gameObject;
        }

        void OnRecycle(EPMReycleObject recycler)
        {
            if (info.sendReset)
                recycler.gameObject.SendMessage("EPMReset");
            recycler.transform.SetParent(readyPoolObj);            
            recycler.gameObject.SetActive(false);
            readySet.Push(recycler.ID);
        }

        public PoolStatus GetPoolStatus()
        {
            return new PoolStatus()
            {
                id = info.id,
                startSize = info.initialSize,
                growSize = info.growSize,
                currentSize = pool.Length
            };
        }
    }
      
    [System.Serializable]
    public struct PoolInfo
    {
        public GameObject prefab;        
        public int initialSize;        
        public int growSize;
        public string id;
        public bool sendReset;
    }

    public struct PoolStatus
    {
        public string id;
        public int startSize;
        public int growSize;
        public int currentSize;
        public int GrownCount { get { return (currentSize - startSize) / growSize; } }
    }
}
