using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Buildings
{
    public class IronMine : HarvesterBase
    {
#pragma warning disable CS0649

        [SerializeField] private Transform bucket;
        private CinemachineImpulseSource impulseSource;
        [SerializeField] private float cameraShakeValue;

        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            animator = GetComponent<Animator>();
        }



        public void OnBucketLanded()
        {
            float impulse = CameraController.GetImpulseValue(cameraShakeValue, transform.up);
            impulseSource.GenerateImpulse(Vector3.one * impulse);
            GameManager.PManager.Play("SmallPolyPuff", bucket.position, bucket.rotation);
            audioSources[0].Play();
        }

        public void OnCraneMove() => audioSources[1].Play();
        public void OnCraneStop() => audioSources[1].Stop();
    }
}