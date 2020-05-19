using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CraftingResources;

public class PState_ResourceView : PState_Move
{
    private InputAction action_ChangeMode;

    override public void OnDisable()
    {
        cursorController.CanInteract = false;
        cursorController.ClearSelection();
        base.OnDisable();
        action_ChangeMode.performed -= OnChangeMode;
        GameManager.Instance.OnExitResourceView();
    }

    override public void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        allowZoom = false;
        allowCursor = true;
        allowPan = true;
        allowRotate = true;
        actionMapName = "Planet Orbit";

        base.OnEnable(owner, newStateMachine);

        cursorController.CanInteract = true;
        CursorController.selectionTag = "Land";
        action_ChangeMode = actionMap.FindAction("ChangeMode");

        // Set full zoom out
        orbitCam.SetZoom(0.5f);

        action_ChangeMode.performed += OnChangeMode;
        GameManager.Instance.OnEnterResourceView();
        Cursor.visible = false;
    }

    private void OnChangeMode(InputAction.CallbackContext context)
    {
        ownerStateMachine.CurrentState = new PState_ModeSelect(this);
    }
}
