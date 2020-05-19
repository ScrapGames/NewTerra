using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CelestialBody)), CanEditMultipleObjects]
public class Inspector_CelestialBody : Editor
{
    protected CelestialBody obj;

    protected void OnEnable()
    {
        obj = target as Moon;
    }

    protected void OnDisable()
    {
        FaceParent();
    }

    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Face Parent"))
        {
            // Face the parent body to ensure correct orbit
            FaceParent();
        }

        if (GUILayout.Button("Set Planet"))
        {
            FindObjectOfType<SolarSystem>().SetPlanet(target as CelestialBody);
        }

        if (GUILayout.Button("Reset Solar System"))
        {
            FindObjectOfType<SolarSystem>().ResetSolarSystem();
        }
    }

    protected void OnSceneGUI()
    {
        if (obj.showDebug)
        {
            FaceParent();
            if (obj.parentObject == null) return;
            float radius = (obj.transform.position - obj.parentObject.transform.position).magnitude;
            Handles.DrawWireArc(obj.parentObject.transform.position, obj.transform.up, obj.transform.forward, 360, radius);
        }
    }

    protected void FaceParent()
    {
        if (obj == null) return;
        if (obj.parentObject == null) return;
        obj.transform.LookAt(obj.parentObject.transform, obj.transform.up);
    }
}

[CustomEditor(typeof(Planet)), CanEditMultipleObjects]
public class Inspector_Planet : Inspector_CelestialBody
{

}

[CustomEditor(typeof(Moon)), CanEditMultipleObjects]
public class Inspector_Moon : Inspector_CelestialBody
{

}
