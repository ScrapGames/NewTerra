using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PState_OrbitCam : PState_Move
{
    private InputAction action_ChangeMode;


    public override void CheckForNewState()
    {

    }

    public override void OnDisable()
    {
        base.OnDisable();
        action_ChangeMode.performed -= OnChangeMode;
    }

    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        allowCursor = true;
        allowPan = true;
        allowRotate = true;
        allowZoom = true;
        actionMapName = "Planet Orbit";

        base.OnEnable(owner, newStateMachine);

        cursorController.CanInteract = true;
        CursorController.selectionTag = "Building";

        action_ChangeMode = actionMap.FindAction("ChangeMode");

        // Set subscriptions
        action_ChangeMode.performed += OnChangeMode;
        Cursor.visible = false;
    }

    private void OnChangeMode(InputAction.CallbackContext context)
    {
        ownerStateMachine.CurrentState = new PState_ModeSelect(this);
    }


    public override void ThinkTick()
    {

    }
}
