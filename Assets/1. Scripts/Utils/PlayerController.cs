using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : Singleton<PlayerController>
{
    public enum StartMode { UI, ModeSelect, Build, Drop, Notification, OrbitCam, ResourceView }
    public StartMode startMode;



    public CameraController orbitCam;
    public PlayerInput playerInput;
    private InputAction action_Pause;
    [Range(0, 1)] public float cursorSensitivity = 1f;

    private StateMachine<PlayerController> stateMachine;
    private string currentActionMap;

    private void OnDestroy()
    {
        GameManager.Paused -= OnPause;
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Planet Orbit");
        action_Pause = playerInput.currentActionMap.FindAction("Pause");

        State<PlayerController> startState;
        switch (startMode)
        {
            case StartMode.Build:
                startState = new PState_BuildMenu();
                break;
            case StartMode.Drop:
                startState = new PState_DropProbe();
                break;
            case StartMode.Notification:
                startState = new PState_NotificationView();
                break;
            default:
            case StartMode.OrbitCam:
                startState = new PState_OrbitCam();
                break;
            case StartMode.ResourceView:
                startState = new PState_ResourceView();
                break;
            case StartMode.ModeSelect:
                startState = new PState_ModeSelect(null);
                break;
            case StartMode.UI:
                startState = new PState_UI();
                break;
        }

        action_Pause.performed += (ctx) => { if (ctx.performed) GameManager.Instance.OnPause(true); };

        stateMachine = new StateMachine<PlayerController>(startState, this);
        GameManager.Paused += OnPause;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnPause(bool pause)
    {
        if (pause)
        {
            playerInput.actions.FindActionMap("UI").Enable();
            playerInput.currentActionMap.Disable();
        }
        else
        {
            playerInput.actions.FindActionMap("UI").Disable();
            playerInput.currentActionMap.Enable();
        }

        InputSystem.settings.updateMode = pause
            ? InputSettings.UpdateMode.ProcessEventsInDynamicUpdate
            : InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    }

    public void SwitchToOrbitCam()
    {
        stateMachine.CurrentState = new PState_OrbitCam();
    }

    public void SwitchToDropProbe()
    {
        stateMachine.CurrentState = new PState_DropProbe();
    }

}
