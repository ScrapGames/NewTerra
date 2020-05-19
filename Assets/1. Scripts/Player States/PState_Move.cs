using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScrapGames;
using UnityEngine.InputSystem;

// Base state for any camera states needing movement
public class PState_Move : State<PlayerController>
{
    protected CameraController orbitCam;
    protected PlayerInput playerInput;
    protected InputAction action_Pan, action_Rotate, action_Zoom, action_MoveCursor, action_CursorSelect;
    protected Vector2 lastCursorPos;
    private float sensitivity;
    private Vector2 screenSize;
    protected CursorController cursorController;
    protected Camera cam;
    protected InputActionMap actionMap;
    protected bool allowPan, allowRotate, allowZoom, allowCursor;
    protected string actionMapName;
    public override void CheckForNewState()
    {

    }

    public override void OnDisable()
    {
        orbitCam.PanV2 = Vector2.zero;

        if (allowRotate)
        {
            action_Rotate.performed -= OnRotate;
            action_Rotate.canceled -= OnRotate;
            orbitCam.RotateDelta = 0;
        }

        if (allowZoom)
        {
            action_Zoom.performed -= OnZoom;
            action_Zoom.canceled -= OnZoom;
            orbitCam.ZoomDelta = 0;
        }

        if (allowCursor)
        {
            // Clear selection if any
            cursorController.OnDeselectObject();
            action_CursorSelect.performed -= OnCursorSelectTriggered;
        }
    }

    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        base.OnEnable(owner, newStateMachine);
        playerInput = ownerObject.playerInput;
        playerInput.SwitchCurrentActionMap(actionMapName);
        orbitCam = ownerObject.orbitCam;
        cam = Camera.main;
        actionMap = playerInput.currentActionMap;
        cursorController = UIManager.Instance.UI_Cursor;

        if (allowPan)
        {
            action_Pan = actionMap.FindAction("Move");
        }

        if (allowRotate)
        {
            action_Rotate = actionMap.FindAction("Rotate");
            action_Rotate.performed += OnRotate;
            action_Rotate.canceled += OnRotate;
        }

        if (allowZoom)
        {
            action_Zoom = actionMap.FindAction("Zoom");
            action_Zoom.performed += OnZoom;
            action_Zoom.canceled += OnZoom;
        }

        if (allowCursor)
        {
            action_CursorSelect = actionMap.FindAction("CursorSelect");
            action_MoveCursor = actionMap.FindAction("MoveCursor");
            lastCursorPos = new Vector2(Screen.width / 2, Screen.height / 2);
            action_CursorSelect.performed += OnCursorSelectTriggered;
        }

        sensitivity = Screen.width * ownerObject.cursorSensitivity;
        screenSize = new Vector2(Screen.width, Screen.height);
    }

    private void OnCursorSelectTriggered(InputAction.CallbackContext context)
    {
        cursorController.OnCursorSelectTriggered();
    }

    protected void OnRotate(InputAction.CallbackContext context)
    {
        orbitCam.RotateDelta = context.ReadValue<float>();
    }

    protected void OnZoom(InputAction.CallbackContext context)
    {
        orbitCam.ZoomDelta = context.ReadValue<float>();
    }

    protected void Pan()
    {
        orbitCam.PanV2 = action_Pan.ReadValue<Vector2>();
    }

    protected void MoveCursor()
    {
#if UNITY_EDITOR
        // Debug
        if (cursorController == null) return;
#endif

        cursorController.MoveCursor(action_MoveCursor.ReadValue<Vector2>());

    }

    public override void ThinkTick()
    {

    }

    public override void Update()
    {
        if (allowPan)
            Pan();
        if (allowCursor)
            MoveCursor();
    }
}
