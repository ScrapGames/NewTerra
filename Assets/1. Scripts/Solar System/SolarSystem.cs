using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    private float rotateSpeed;
    private bool IsInAlternateOrbit { get; set; } = false;
    public CelestialBody sun;
    CelestialBody center;
    float skyboxRot = 0;
    float orbitTime;

    public void SetPlanet(CelestialBody planet)
    {
        Transform parent = transform;
        CelestialBody p = planet;
        p.IsCenterOfUniverse = true;


        while (p != null)
        {
            p.transform.SetParent(parent);
            p.alternateOrbitParent = parent.GetComponent<CelestialBody>();
            p.IsInAlternateOrbit = true;
            if (p.alternateOrbitParent != null)
                p.alternateOrbitTime = -p.alternateOrbitParent.orbitTime;
            parent = p.transform;
            p = p.parentObject;
        }

        planet.transform.SetParent(null);
        transform.SetParent(planet.transform, false);
        planet.parentObject.transform.SetParent(transform);
        planet.transform.position = Vector3.zero;
        rotateSpeed = -planet.rotationTime;
        orbitTime = -planet.orbitTime;
        IsInAlternateOrbit = true;
        center = planet;

        // Move moons to solar system
        for (int i = 0; i < center.satelliteObjects.Length; i++)
        {
            center.satelliteObjects[i].transform.SetParent(transform);
        }


        // Tilt alignment to be straight

        //Vector3 rotation = center.transform.rotation.eulerAngles;
        //rotation.x = 0; rotation.z = 0;
        //center.transform.rotation = Quaternion.Euler(rotation);

        Vector3 forward = Vector3.Cross(center.transform.right, Vector3.up);
        center.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }


    public void ResetSolarSystem()
    {
        IsInAlternateOrbit = false;
        // Clear parent from solar system
        transform.SetParent(null);

        // Loop through 'center' planet and reset parents back to original until null (Sun)
        Transform parent = center.parentObject.transform;
        CelestialBody planet = center;
        CelestialBody sun = null;
        planet.IsCenterOfUniverse = false;

        while (parent != null)
        {
            planet.IsInAlternateOrbit = false;
            planet.transform.SetParent(parent);
            CelestialBody newPlanet = parent.GetComponent<CelestialBody>();
            if (newPlanet != null)
            {
                if (newPlanet.parentObject == null)
                {
                    // Found the sun!
                    parent = null;
                    sun = newPlanet;
                    sun.transform.SetParent(transform);
                    sun.IsInAlternateOrbit = false;

                    // Reset previous planet parent to sun
                    planet.transform.SetParent(newPlanet.transform);
                }
                else
                    parent = newPlanet.parentObject.transform;
            }
            planet = newPlanet;
        }
        // Move moons back
        for (int i = 0; i < center.satelliteObjects.Length; i++)
        {
            center.satelliteObjects[i].transform.SetParent(center.transform);
        }

        // Reset position back to origin, reset sun position back to origin
        transform.position = Vector3.zero;
        sun.transform.position = Vector3.zero;

    }

    private void LocalRotate()
    {
        float r = rotateSpeed == 0 ? 0 : 360 / rotateSpeed;
        float o = orbitTime == 0 ? 0 : 360 / orbitTime;
        skyboxRot -= (o + r) * Time.deltaTime;
        if (skyboxRot < 0) skyboxRot += 360;
        if (skyboxRot > 360) skyboxRot -= 360;

        RenderSettings.skybox.SetFloat("_Rotation", skyboxRot);

        if (!IsInAlternateOrbit) return;
        if (rotateSpeed == 0) return;

        transform.Rotate(Vector3.up, (360 / rotateSpeed) * Time.deltaTime, Space.Self);

        // Rotate skybox
    }

    private void Update()
    {
        if (IsInAlternateOrbit)
            LocalRotate();
    }
}
