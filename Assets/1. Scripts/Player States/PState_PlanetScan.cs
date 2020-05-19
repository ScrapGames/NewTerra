using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PState_PlanetScan : PState_Move
{
    private float endTime;
    private DropPod probe;
    private Vector2 screenMid, screenLandPos;
    private Vector3 probeOffset;
    private bool inPosition;
    private Moon planet;

    public PState_PlanetScan(GameObject probe)
    {
        this.probe = probe.GetComponent<DropPod>();
    }


    public override void CheckForNewState()
    {
        if (Time.time > endTime)
        {
            // Time to end scan
            GameManager.Instance.OnExitPlanetScan();
            ownerStateMachine.CurrentState = new PState_ResourceView();
        }
    }

    public override void OnEnable(PlayerController owner, StateMachine<PlayerController> newStateMachine)
    {
        allowCursor = false;
        allowPan = false;
        allowRotate = false;
        allowZoom = false;
        actionMapName = "Planet Orbit";
        endTime = Mathf.Infinity;
        base.OnEnable(owner, newStateMachine);
        cursorController.SetCursorActivity(false);
        orbitCam.SetZoom(.4f);
        screenMid = new Vector2(Screen.width, Screen.height) * 0.5f;
        screenLandPos = screenMid + (Vector2.up * Screen.height * 0.005f);
        planet = Moon.Target;
        probeOffset = (probe.transform.position - planet.transform.position).normalized * probeOffset.magnitude;
        orbitCam.StopMovement();
        probe.DropPodLanded += OnDropHasLanded;
        Cursor.visible = false;
    }

    private void OnDropHasLanded()
    {
        probe.DropPodLanded -= OnDropHasLanded;
        GameManager.Instance.OnEnterPlanetScan();
        endTime = Time.time + GameManager.Instance.gameSettings.timer_PlanetScan;
    }

    public override void Update()
    {
        base.Update();
        if (inPosition) return;

        Vector3 planetPos = planet.transform.position;
        Vector3 probePos = probe.transform.position;



        // Determine which way to pan        
        Vector2 probeScreenPos = cam.WorldToScreenPoint(probePos);
        Vector2 dir = probeScreenPos - screenLandPos;
        dir /= Screen.width; // Normalize direction across different resolutions
        float dot = Vector3.Dot(probe.transform.up, -cam.transform.forward);

        //if (dir.sqrMagnitude <= 0.0001f) inPosition = true;
        if (dot > 0.9999) inPosition = true;
        orbitCam.HardPan(dir);

    }
}
