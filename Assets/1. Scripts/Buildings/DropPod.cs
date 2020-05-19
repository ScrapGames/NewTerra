using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DropPod : MonoBehaviour
{
#pragma warning disable CS0649
    public event System.Action DropPodLanded;

    private Animator animator;
    private const string TRIGGER_LAND = "Land";
    [SerializeField] private Transform transform_Sattelite;
    private Cinemachine.CinemachineImpulseSource impulseSource;
    [SerializeField] float holdsOpenImpulse, landingImpulse;
    [SerializeField] private AudioClip audio_Landing, audio_Satellite;
    [SerializeField] private AudioSource audioSource_Base, audioSource_Satellite;


    private void Start()
    {
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        PlopObject plop = GetComponent<PlopObject>();
        if (plop) plop.PlopSet += OnPlop;
    }

    public void OnDropLanded()
    {
        impulseSource.GenerateImpulse(Vector3.one * landingImpulse);
        GameManager.PManager.Play("LargeRingPuff", transform.position + transform.up, transform.rotation);
        audioSource_Satellite.Stop();
        Destroy(audioSource_Satellite);
    }

    public void OnHoldsOpened()
    {
        impulseSource.GenerateImpulse(Vector3.one * holdsOpenImpulse);
    }

    public void OnSateliteDescend()
    {
        audioSource_Satellite.clip = audio_Satellite;
        audioSource_Satellite.Play();
    }

    public void OnLandingSequenceFinished()
    {
        DropPodLanded?.Invoke();
    }

    private void OnPlop(PlopObject plop)
    {
        plop.PlopSet -= OnPlop;
        animator.SetTrigger(TRIGGER_LAND);
        audioSource_Base.clip = audio_Landing;
        audioSource_Base.Play();
    }
}
