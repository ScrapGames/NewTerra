using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PState_Build : PState_Move
{
    private PlopObject building;
    private Moon targetPlanet;
    private float rotateValue;
    private Buildings.BuildingData data;
    private float rSpeed;


    public PState_Build(Buildings.BuildingData data)
    {
        this.data = data;
    }

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

        targetPlanet = Moon.Target;

        building = data.GetBuilding().AddComponent<PlopObject>();
        building.transform.SetParent(targetPlanet.bodyObject);
        building.transform.position = targetPlanet.transform.position;
        orbitCam.SetZoom(0.5f);
        action_Rotate = actionMap.FindAction("Rotate");

        action_Rotate.performed += OnRotateAction;
        action_Rotate.canceled += OnRotateAction;
        action_CursorSelect.performed += OnCursorSelect;

        cursorController.SetMoveObject(building, targetPlanet.transform);

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

        // Check if is able to plop
        if (!building.CanBePlaced) return;

        // Set building 
        building.Plop();

        // Go back to orbit cam        
        ownerStateMachine.CurrentState = new PState_OrbitCam();
    }

    private void Rotate()
    {
        building.transform.localRotation *= Quaternion.AngleAxis(rotateValue * Time.deltaTime, Vector3.up);
    }
}
