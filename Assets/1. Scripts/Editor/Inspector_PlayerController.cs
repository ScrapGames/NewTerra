using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class Inspector_PlayerController : Editor
{
    private PlayerController controller;
    private void OnEnable()
    {
        controller = target as PlayerController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Orbit Cam Mode"))
        {
            controller.SwitchToOrbitCam();
        }
    }
}
