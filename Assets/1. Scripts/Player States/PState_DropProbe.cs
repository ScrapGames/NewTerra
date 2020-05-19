using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PState_DropProbe : PState_Move
{
    private PlopObject probe;
    private Moon targetPlanet;
    private float rotateValue;
    private float rSpeed;


    public override void OnDisable()
    {
        base.OnDisable();
        action_Rotate.performed -= OnRotateAction;
        action_Rotate.canceled -= OnRotateAction;
        action_CursorSelect.performed -= OnCursorSelect;
    }


    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        allowZoom = true;
        allowCursor = true;
        allowPan = true;
        allowRotate = false;

        actionMapName = "Planet Orbit";
        base.OnEnable(owner, newStateMachine);

        // Disable cursor interaction
        cursorController.CanInteract = false;

        action_CursorSelect = actionMap.FindAction("CursorSelect");


        targetPlanet = null;
        if (Physics.Raycast(cam.ViewportPointToRay(Vector2.one * 0.5f), out RaycastHit hit, 50f, LayerMask.GetMask("Planet Shell")))
        {
            targetPlanet = hit.collider.GetComponent<Moon>();
        }

        probe = EPMInstance.SpawnObject("DROP-POD").AddComponent<PlopObject>();
        probe.transform.SetParent(targetPlanet.bodyObject);
        probe.transform.position = targetPlanet.transform.position;


        probe.PlacementTag = "Land";

        targetPlanet.OnSetAsTarget();
        cursorController.Init();

        action_Rotate = actionMap.FindAction("Rotate");

        action_Rotate.performed += OnRotateAction;
        action_Rotate.canceled += OnRotateAction;
        action_CursorSelect.performed += OnCursorSelect;

        cursorController.SetMoveObject(probe, targetPlanet.transform);
        rSpeed = GameManager.Instance.gameSettings.plopRotateSpeed;
        Cursor.visible = false;
    }

    public override void Update()
    {
        base.Update();

        if (rotateValue != 0)
            Rotate();
    }



    private void OnRotateAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            rotateValue = context.ReadValue<float>() * rSpeed;
        else if (context.canceled)
            rotateValue = 0;
    }

    private void OnCursorSelect(InputAction.CallbackContext context)
    {
        // Check cursor is over the selected object
        if (!cursorController.CheckPlopUnderCursor()) return;

        // Set building in place IF it is able
        if (probe.IsTouchingObstacle) return;

        // Check if able to plop
        if (!probe.CanBePlaced) return;

        // Set probe
        probe.Plop();

        // Change State
        ownerStateMachine.CurrentState = new PState_PlanetScan(probe.gameObject);
    }

    private void Rotate()
    {
        probe.transform.localRotation *= Quaternion.AngleAxis(rotateValue * Time.deltaTime, Vector3.up);
    }
}
