using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ParticleManager", menuName = "NewTerra/ParticleManager")]
public class ParticleManager : ScriptableObject
{
#pragma warning disable CS0649

    [SerializeField] private InstancedParticleSystem[] instancedParticles;
    private Dictionary<string, ParticleSystem> instancedPool;

    private Transform particleParent;

    public void Init()
    {
        // Create parent for particle pool
        particleParent = new GameObject("Particle Pool").transform;
        DontDestroyOnLoad(particleParent);



        instancedPool = new Dictionary<string, ParticleSystem>();
        for (int i = 0; i < instancedParticles.Length; i++)
        {
            InstancedParticleSystem ip = instancedParticles[i];
            ParticleSystem ps = GameObject.Instantiate(ip.prefab);
            ps.Stop();

            // Ensure play on awake & looping is disabled
            var psMain = ps.main;
            psMain.playOnAwake = false;
            psMain.loop = false;


            // Set world parent
            ps.transform.SetParent(particleParent);

            // Add to dictionary
            instancedPool.Add(ip.id, ps);
        }
    }

    public ParticleSystem Play(string id, Vector3 position)
    {
        return Play(id, position, Quaternion.identity);
    }

    public ParticleSystem Play(string id, Vector3 position, Quaternion rotation)
    {
        if (instancedPool.TryGetValue(id, out ParticleSystem ps))
        {
            ps.transform.position = position;
            ps.transform.rotation = rotation;
            ps.Play();
            return ps;
        }
        return null;
    }

    [System.Serializable]
    public struct InstancedParticleSystem
    {
        public string id;
        public ParticleSystem prefab;
    }
}
