using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Notifications;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PState_BuildMenu : PState_UI
{

    private InputAction action_Submit, action_Back, action_Move;

    public override void CheckForNewState()
    {

    }

    public async override void OnDisable()
    {
        action_Back.performed -= OnBackPressed;
        action_Move.performed -= OnMove;
        UIManager.Instance.UI_BuildMenu.BuildStarted -= OnBuildStarted;

        CameraController orbitCam = PlayerController.Instance.orbitCam;
        orbitCam.DecoupleFromTarget(false);
        UIManager.Instance.UI_BuildMenu.Show(false);

        GameManager.Instance.cameraJuice.ClearBlur();
        await orbitCam.OffsetCameraAsync(false);


        // Load preview scene
        SceneManager.UnloadSceneAsync(GameManager.Instance.gameSettings.scene_BuildingPreview,
            UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        base.OnDisable();
    }

    public async override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        Debug.Log("Entered Build Menu State");
        showCursor = false;
        base.OnEnable(owner, newStateMachine);


        CameraController orbitCam = PlayerController.Instance.orbitCam;
        orbitCam.StopMovement();
        orbitCam.SetZoom(false);

        // Load preview scene
        SceneManager.LoadSceneAsync(GameManager.Instance.gameSettings.scene_BuildingPreview, LoadSceneMode.Additive);

        orbitCam.DecoupleFromTarget(true);
        GameManager.Instance.cameraJuice.TargetBlur(Moon.Target.transform);

        // Show build menu
        UIManager.Instance.UI_BuildMenu.Show(true);
        UIManager.Instance.UI_BuildMenu.BuildStarted += OnBuildStarted;
        await orbitCam.OffsetCameraAsync(true);

        // Set actions
        action_Back = map.FindAction("Back");
        action_Move = map.FindAction("Move");

        action_Back.performed += OnBackPressed;
        action_Move.performed += OnMove;
    }

    private void OnBackPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            ownerStateMachine.CurrentState = new PState_OrbitCam();
    }

    private void OnMove(InputAction.CallbackContext context)
    {

    }

    private void OnBuildStarted(Buildings.BuildingData data)
    {
        ownerStateMachine.CurrentState = new PState_Build(data);
    }
}
