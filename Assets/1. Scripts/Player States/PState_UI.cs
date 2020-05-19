using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ScrapGames;

public class PState_UI : State<PlayerController>
{
    protected PlayerInput playerInput;
    protected CursorController cursorController;
    protected PlayerController playerController;
    protected string actionMapName = "UI";

    protected InputActionMap map;
    protected bool showCursor = true;

    public override void CheckForNewState()
    {

    }

    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        base.OnEnable(owner, newStateMachine);
        playerInput = ownerObject.playerInput;
        playerController = PlayerController.Instance;
        playerController.playerInput.SwitchCurrentActionMap(actionMapName);
        map = playerController.playerInput.currentActionMap;

        if (showCursor)
        {
            cursorController = UIManager.Instance.UI_Cursor;
            if (cursorController == null) return;
            cursorController.CanInteract = false;

        }

        Cursor.visible = true;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ThinkTick()
    {

    }
}
