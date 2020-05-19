using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CraftingResources;

public class Moon : CelestialBody
{
#pragma warning disable CS0649

    public static Moon Target { get; private set; }
    [HideInInspector] public float radius;
    public Continent[] landContinents;
    public Continent waterContinent;



    public override void OnSetAsTarget()
    {
        Target = this;
        // Add material switcher to all objects
        Debug.Log(name + " set as target");
        waterContinent.OnPlanetSetAsTarget();
        for (int i = 0; i < landContinents.Length; i++)
        {
            landContinents[i].OnPlanetSetAsTarget();
        }

        CursorController.selectionMask = 1 << bodyObject.gameObject.layer;

        base.OnSetAsTarget();
    }

}
