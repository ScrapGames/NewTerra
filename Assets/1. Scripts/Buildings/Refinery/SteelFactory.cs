using UnityEngine;
using UnityEngine.Audio;

namespace Buildings
{
    public class SteelFactory : BuildingBase
    {
        protected override void OnPlop(PlopObject plop)
        {
            audioSources[0].Play();
        }
    }
}