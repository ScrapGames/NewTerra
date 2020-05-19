using UnityEngine;
using UnityEngine.Audio;
namespace Buildings
{
    [RequireComponent(typeof(AudioSource), typeof(Animator))]
    public abstract class BuildingBase : MonoBehaviour
    {
        [SerializeField] protected AudioSource[] audioSources;
        [SerializeField] protected Animator animator;
        public BuildingData data;

        protected virtual void Start()
        {
            PlopObject plop = GetComponent<PlopObject>();
            if (plop != null)
                plop.PlopSet += OnPlop;
        }

        protected virtual void Update()
        {
            SetVolume();
        }


        private void SetVolume()
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].volume = CameraController.GetImpulseValue(1f, transform.up);
            }
        }

        protected virtual void OnPlop(PlopObject plop)
        {
            plop.PlopSet -= OnPlop;
            animator?.SetTrigger("Plopped");
        }
    }
}