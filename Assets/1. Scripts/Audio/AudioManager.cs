using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : Singleton<AudioManager>
{
#pragma warning disable CS0649
    public static AudioMixer Mixer { get { return Instance._Mixer; } }
    [SerializeField] private AudioMixer _Mixer;

    private static AudioMixerGroup mixer_BgmGroup, mixer_UIGroup;
    [SerializeField] private AudioClip bgmClip; // TODO: Make this into asset ref in data manager

    private static AudioSource audioSource_BGM;
    private static List<AudioSource> audioSource_FX;
    private const float BGM_FADE_TIME = 2f;
    private int fxVoiceCount = 4;

    [Header("Common UI Audio")]
    public AudioClip clip_Pop;
    public AudioClip clip_ButtonOK, clip_UiIn, clip_UiOut;



    private void Start()
    {
        // Get mixer groups
        mixer_BgmGroup = GameManager.Instance.gameSettings.audioMixerGroup_BGM;
        mixer_UIGroup = GameManager.Instance.gameSettings.audioMixerGroup_UI;
        // Create static audio sources
        audioSource_BGM = gameObject.AddComponent<AudioSource>();
        audioSource_BGM.outputAudioMixerGroup = mixer_BgmGroup;

        audioSource_FX = new List<AudioSource>();

        for (int i = 0; i < fxVoiceCount; i++)
        {
            audioSource_FX.Add(AddNewFxVoice(i));
        }

        PlayBGM(bgmClip);
    }

    public static void Pop(float pitch = 1f, float volume = 1f)
        => PlayUISound(Instance.clip_Pop, pitch, volume);


    public static void PlayUIShow(bool show, float pitch = 1f, float volume = 1f)
        => PlayUISound(show ? Instance.clip_UiIn : Instance.clip_UiOut, pitch, volume);


    public static AudioSource PlayUISound(AudioClip clip, float pitch = 1f, float volume = 1f, bool loop = false)
    {
        // Find a free audio source        
        int max = audioSource_FX.Count;
        AudioSource aSource = null;

        for (int i = 0; i < max; i++)
        {
            if (audioSource_FX[i].isPlaying)
            {
                // Check if last source
                if (i == max - 1)
                {
                    // Get new fx voice and add
                    aSource = AddNewFxVoice(max);
                    audioSource_FX.Add(aSource);
                    break;
                }
            }
            else
            {
                // We can use this source
                aSource = audioSource_FX[i];
                break;
            }
        }

        aSource.clip = clip;
        aSource.volume = volume;
        aSource.pitch = pitch;
        aSource.loop = loop;
        aSource.Play();
        return aSource;
    }

    private static AudioSource AddNewFxVoice(int id)
    {
        AudioSource aSource = Instance.gameObject.AddComponent<AudioSource>();
        aSource.playOnAwake = false;
        aSource.outputAudioMixerGroup = mixer_UIGroup;
        return aSource;
    }

    public static void PlaySound(AudioSource audioSource, AudioClip audioClip, bool loop = false)
    {
        audioSource.loop = loop;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public static void PlayBGM(AudioClip clip) // TODO: Use asset reference
    {
        audioSource_BGM.volume = 0;
        PlaySound(audioSource_BGM, clip, true);
        audioSource_BGM.DOFade(1f, BGM_FADE_TIME);
    }
}
