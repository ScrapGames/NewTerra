using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class CameraJuice : MonoBehaviour
{
#pragma warning disable CS0649

    [SerializeField] Volume globalVolume;
    private DepthOfField fx_dof;
    private Vignette fx_vignette;
    private ColorAdjustments fx_colorAdjustments;
    public Transform target;
    private Camera cam;

    public const float DEFAULT_TWEEN_TIME = 2.5f;
    public const float DEFAULT_VIGNETTE = 0.42f;
    private const float MAX_FOCUS_DISTANCE = 20f;
    private const float DEFAULT_EXPOSURE = 0f;
    public const Ease DEFAULT_EASE = Ease.OutExpo;

    private bool isBluring;
    private Tween tween_Aperture, tween_Focus, tween_Vignette, tween_Exposure;

    private void Start()
    {
        GameManager.Instance.cameraJuice = this;
        cam = Camera.main;

        // Depth of Field
        if (!globalVolume.profile.TryGet<DepthOfField>(out fx_dof))
        {
            fx_dof = globalVolume.profile.Add<DepthOfField>(true);
        }

        // Vignette
        if (!globalVolume.profile.TryGet<Vignette>(out fx_vignette))
        {
            fx_vignette = globalVolume.profile.Add<Vignette>(true);
        }

        // Color Adjustments
        if (!globalVolume.profile.TryGet<ColorAdjustments>(out fx_colorAdjustments))
        {
            fx_colorAdjustments = globalVolume.profile.Add<ColorAdjustments>(true);
        }

        // *** DOF Settings *** //
        fx_dof.mode.value = DepthOfFieldMode.Bokeh;
        fx_dof.bladeCount.value = 5;
        fx_dof.bladeCount.overrideState = true;
        fx_dof.bladeCurvature.value = 0;
        fx_dof.bladeCurvature.overrideState = true;
        fx_dof.focalLength.value = 200f;
        fx_dof.focalLength.overrideState = true;
        fx_dof.aperture.value = 32f;
        fx_dof.aperture.overrideState = true;
        fx_dof.active = false;
        // ----------------------- //

        // *** Vignette Settings *** //
        fx_vignette.intensity.value = DEFAULT_VIGNETTE;
        fx_vignette.active = true;
        // ----------------------- //        
    }

    public void TargetBlur(Transform target, float aperture = 2.8f, float tweenTime = DEFAULT_TWEEN_TIME, Ease ease = DEFAULT_EASE)
    {
        float focusValue = Vector3.Dot(target.position - cam.transform.position, cam.transform.forward);
        Blur(focusValue: focusValue, aperture: aperture, tweenTime: tweenTime, ease: ease);
    }

    public void Blur(float tweenTime = DEFAULT_TWEEN_TIME, float focusValue = 0, float aperture = 1f, Ease ease = DEFAULT_EASE)
    {
        fx_dof.active = true;
        tween_Focus?.Kill();
        tween_Focus = DOTween.To(() => fx_dof.focusDistance.value, x => fx_dof.focusDistance.value = x, focusValue, DEFAULT_TWEEN_TIME).SetEase(ease);
        tween_Aperture?.Kill();
        tween_Aperture = DOTween.To(() => fx_dof.aperture.value, x => fx_dof.aperture.value = x, aperture, tweenTime).SetEase(ease);
    }

    public void ClearBlur(float tweenTime = DEFAULT_TWEEN_TIME, Ease ease = DEFAULT_EASE)
    {
        tween_Focus?.Kill();
        tween_Focus = DOTween.To(() => fx_dof.focusDistance.value, x => fx_dof.focusDistance.value = x, MAX_FOCUS_DISTANCE, tweenTime).SetEase(ease);
        tween_Aperture?.Kill();
        tween_Aperture = DOTween.To(() => fx_dof.aperture.value, x => fx_dof.aperture.value = x, 32f, DEFAULT_TWEEN_TIME).SetEase(ease)
            .OnComplete(() => fx_dof.active = false);
    }

    public void SetVignette(float vignetteValue = DEFAULT_VIGNETTE, float tweenTime = DEFAULT_TWEEN_TIME, Ease ease = DEFAULT_EASE)
    {
        tween_Vignette?.Kill();
        tween_Vignette = DOTween.To(() => fx_vignette.intensity.value, x => fx_vignette.intensity.value = x, vignetteValue, tweenTime).SetEase(ease);
    }

    public void SetExposure(float exposureValue = DEFAULT_EXPOSURE, float tweenTime = DEFAULT_TWEEN_TIME, Ease ease = DEFAULT_EASE)
    {
        tween_Exposure?.Kill();
        tween_Exposure = DOTween.To(() => fx_colorAdjustments.postExposure.value,
            x => fx_colorAdjustments.postExposure.value = x, exposureValue, tweenTime).SetEase(ease);
    }
}
