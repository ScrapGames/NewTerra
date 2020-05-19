using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "NewTerra/Camera Setting")]
public class CameraEffectData : ScriptableObject
{
#pragma warning disable CS0649
    [SerializeField] private bool doFullBlur, doClearBlur, doVignette, doExposure;
    [SerializeField] private float vignetteAmount, exposureAmount;
    [SerializeField] private DG.Tweening.Ease ease = CameraJuice.DEFAULT_EASE;


    [SerializeField] private float tweenTime = CameraJuice.DEFAULT_TWEEN_TIME;


    public void DoEffect()
    {
        CameraJuice cj = GameManager.Instance.cameraJuice;

        if (doFullBlur)
            cj.Blur(tweenTime: tweenTime, ease: ease);
        if (doClearBlur)
            cj.ClearBlur(tweenTime: tweenTime, ease: ease);
        if (doVignette)
            cj.SetVignette(vignetteAmount == 0 ? CameraJuice.DEFAULT_VIGNETTE : vignetteAmount, tweenTime: tweenTime, ease: ease);
        if (doExposure)
            cj.SetExposure(exposureAmount, tweenTime, ease);
    }
}
