using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_HoldButton : MonoBehaviour
{

    public Image backgroundImage, foregroundImage;
    private float targetHoldTime;
    private float currentValue;
    private bool isHolding;

    private void Start()
    {
        targetHoldTime = InputSystem.settings.defaultHoldTime;
    }

    private void OnDisable()
    {
        Stop();
    }

    public void StopHold()
    {
        // Only trigger if is active
        if (gameObject.activeSelf)
            Stop();
    }

    private void Stop()
    {
        currentValue = 0;
        foregroundImage.fillAmount = 0;
        isHolding = false;
    }

    public void StartHold()
    {
        // Only trigger if is active
        if (gameObject.activeSelf)
            isHolding = true;
    }

    private void Update()
    {
        if (isHolding)
        {
            currentValue += Time.deltaTime / targetHoldTime;
            foregroundImage.fillAmount = Mathf.Clamp01(currentValue);
        }
    }
}
