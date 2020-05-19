using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    private Material scanMatRef, resourceMatRef, resourceSelectedMat, defaultMaterial;
    private new Renderer renderer;
    private CelestialBody parentPlanet;
    private Continent continent;


    public void SetParent(CelestialBody parent, Continent continent)
    {
        parentPlanet = parent;
        renderer = GetComponent<Renderer>();
        scanMatRef = GameManager.Instance.gameSettings.material_PlanetScan;
        resourceMatRef = GameManager.Instance.gameSettings.material_PlanetResourceView;
        resourceSelectedMat = GameManager.Instance.gameSettings.material_PlanetResourceViewSelected;
        defaultMaterial = renderer.material;

        // Subscribe to planet events
        parentPlanet.ClearTarget += OnClearPlanet;
        GameManager.EnterPlanetScan += OnEnterPlanetScan;
        GameManager.ExitPlanetScan += OnExitPlanetScan;
        GameManager.EnterResourceView += OnEnterResourceView;
        GameManager.ExitResourceView += OnExitResourceView;

        this.continent = continent;
        continent.CursorEnter += OnCursorEnter;
        continent.CursorExit += OnCursorExit;
    }

    private void OnClearPlanet()
    {
        parentPlanet.ClearTarget -= OnClearPlanet;

        // Ensure material is back to default
        renderer.material = defaultMaterial;
        GameManager.EnterPlanetScan -= OnEnterPlanetScan;
        GameManager.ExitPlanetScan -= OnExitPlanetScan;
        GameManager.EnterResourceView -= OnEnterResourceView;
        GameManager.ExitResourceView -= OnExitResourceView;
        continent.CursorEnter -= OnCursorEnter;
        continent.CursorExit -= OnCursorExit;

        Destroy(this);
    }
    private void OnEnterPlanetScan()
    {
        renderer.material = scanMatRef;
    }

    private void OnExitPlanetScan()
    {
        renderer.material = defaultMaterial;
    }

    private void OnEnterResourceView()
    {
        renderer.material = resourceMatRef;
    }

    private void OnExitResourceView()
    {
        renderer.material = defaultMaterial;
    }

    private void OnCursorEnter()
    {
        renderer.material = resourceSelectedMat;
    }

    private void OnCursorExit()
    {
        renderer.material = resourceMatRef;
    }
}
