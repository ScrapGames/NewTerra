using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingResources;

public abstract class CelestialBody : MonoBehaviour
{
    public event System.Action SetAsTarget;
    public event System.Action ClearTarget;

    [SerializeField, Header("Body Info")] protected string nameID;
    public string LocalizedName { get { return LocalizationManager.GetText(nameID); } }


    [Header("Solar System Links")]
    public Transform bodyObject;
    public CelestialBody parentObject, alternateOrbitParent;
    public CelestialBody[] satelliteObjects;

    [Header("Orbit Info")]
    public float orbitTime;
    public float rotationTime;
    public float alternateOrbitTime;
    public bool IsInAlternateOrbit; //{ get; set; }
    public bool IsCenterOfUniverse;//{ get; set; } = false;

#if UNITY_EDITOR
    public bool showDebug;
#endif


    public virtual void OnSetAsTarget()
    {
        SetAsTarget?.Invoke();
    }

    protected virtual void OnClearTarget()
    {
        if (PlayerController.Instance != null)
        {
            ClearTarget?.Invoke();
        }
    }

    protected virtual void Update()
    {
        if (IsCenterOfUniverse) return;

        if (IsInAlternateOrbit)
            AlternateOrbit();
        else
            Orbit();

        LocalRotate();
    }

    private void Orbit()
    {
        // If no parent, early out
        if (parentObject == null)
            return;

        // Rotate around parent via axis
        if (orbitTime == 0) return;
        transform.RotateAround(parentObject.transform.position, transform.up, (360 / orbitTime) * Time.deltaTime);
    }

    private void AlternateOrbit()
    {
        if (alternateOrbitTime == 0) return;
        if (alternateOrbitParent == null) return;
        transform.Rotate(alternateOrbitParent.transform.up, (360 / rotationTime) * Time.deltaTime, Space.World);
    }

    private void LocalRotate()
    {
        // Rotate self
        if (rotationTime == 0) return;
        bodyObject.transform.Rotate(transform.up, (360 / rotationTime) * Time.deltaTime, Space.World);
    }
}
