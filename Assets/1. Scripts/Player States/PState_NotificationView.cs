using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Notifications;
using DG.Tweening;

public class PState_NotificationView : PState_UI
{
    private InputAction action_Delete, action_Submit, action_Back, action_Move;
    private NotificationHandler notificationHandler;
    private Notifications.UI_NotificationExpanded notificationExpanded;

    public override void CheckForNewState()
    {

    }

    public async override void OnDisable()
    {
        action_Delete.performed -= OnDelete;
        action_Delete.started -= OnDelete;
        action_Delete.canceled -= OnDelete;
        action_Back.performed -= OnBackPressed;
        action_Move.performed -= OnMove;
        notificationHandler.ShowExpandedView(false);

        CameraController orbitCam = PlayerController.Instance.orbitCam;
        orbitCam.DecoupleFromTarget(false);
        GameManager.Instance.cameraJuice.ClearBlur();
        await orbitCam.OffsetCameraAsync(false);

        base.OnDisable();
    }

    public async override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        Debug.Log("Entered Notification View State");
        showCursor = false;
        base.OnEnable(owner, newStateMachine);

        CameraController orbitCam = PlayerController.Instance.orbitCam;
        orbitCam.StopMovement();
        orbitCam.SetZoom(false);


        orbitCam.DecoupleFromTarget(true);
        await orbitCam.OffsetCameraAsync(true);
        GameManager.Instance.cameraJuice.TargetBlur(Moon.Target.transform);

        // Set actions
        action_Delete = map.FindAction("Option");
        action_Back = map.FindAction("Back");
        action_Move = map.FindAction("Move");

        action_Delete.started += OnDelete;
        action_Delete.performed += OnDelete;
        action_Delete.canceled += OnDelete;
        action_Back.performed += OnBackPressed;
        action_Move.performed += OnMove;

        notificationHandler = UIManager.Instance.notificationHandler;
        notificationExpanded = UIManager.Instance.UI_NotificationExpanded;
        notificationHandler.ShowExpandedView(true);
    }

    private void OnBackPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            ownerStateMachine.CurrentState = new PState_OrbitCam();
    }

    private void OnDelete(InputAction.CallbackContext context)
    {
        // Press Section
        if (context.interaction is TapInteraction)
        {
            if (context.phase == InputActionPhase.Performed)
                //notificationHandler.DeletePressed();
                notificationHandler.DeleteAllInCategory(notificationExpanded.CurrentCateogry);

            return;
        }

        // Hold Section
        if (context.interaction is HoldInteraction)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    notificationExpanded.deleteAllButton.StartHold();
                    break;
                case InputActionPhase.Canceled:
                    notificationExpanded.deleteAllButton.StopHold();
                    break;
                case InputActionPhase.Performed:
                    notificationExpanded.deleteAllButton.StopHold();
                    notificationHandler.DeleteAllInCategory(notificationExpanded.CurrentCateogry);
                    break;
                default: break;
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Switch between category panel and message panel
        Vector2 input = context.ReadValue<Vector2>();
        if (Mathf.Abs(input.y) > 0) return;

        GameObject selection = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (selection.GetComponent<UI_Message>())
        {
            // Current selection is a message, allow to move left to select the categories
            if (input == Vector2.left)
            {
                UIManager.Instance.UI_NotificationExpanded.CurrentCategoryButton.Select();
                return;
            }
        }
        else
        {
            // Current selection is a category button
            if (input == Vector2.right)
            {
                UIManager.Instance.UI_NotificationExpanded.CurrentMessageList.LastSelectedMessage?.Select();
            }
        }
    }
}
