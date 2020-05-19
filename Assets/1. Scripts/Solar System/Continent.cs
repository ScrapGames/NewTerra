using UnityEngine;
using CraftingResources;
using DG.Tweening;

public class Continent : MonoBehaviour, ICursorInteractable
{
#pragma warning disable CS0649

    public enum Biome { Tundra, Rainforest, Desert, Grassland, Marine }
    public event System.Action CursorEnter;
    public event System.Action CursorExit;
    public RawMaterialData[] availableRawMaterials;
    public Biome biome;
    public CelestialBody parent;
    public bool isWater;

    [SerializeField] private GameObject[] subTerrainObjects;
    private UI_ResourceList uiResourceList;

    public void OnCursorEnter()
    {
        // Show UI resource list
        Debug.Log("Cursor Enter");
        uiResourceList.SetData(availableRawMaterials);
        GameSettings gs = GameManager.Instance.gameSettings;
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(gs.hoverPunchSize, gs.hoverPunchDuration, gs.hoverPunchVibrato, gs.hoverPunchElasticity);
        CursorEnter?.Invoke();
    }

    public void OnCursorExit()
    {
        Debug.Log("Cursor Exit");
        uiResourceList.ClearResources();
        GameSettings gs = GameManager.Instance.gameSettings;
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(gs.hoverPunchSize, gs.hoverPunchDuration, gs.hoverPunchVibrato, gs.hoverPunchElasticity);
        CursorExit?.Invoke();
    }

    public void OnDeselect()
    {

    }

    public void OnSelect()
    {

    }

    public void OnPlanetSetAsTarget()
    {
        // Add to continent
        AddMaterialSwitcher(gameObject);

        if (isWater) return;

        // Add to subterrain
        for (int i = 0; i < subTerrainObjects.Length; i++)
        {
            AddMaterialSwitcher(subTerrainObjects[i]);
        }

        uiResourceList = UIManager.Instance.UI_ResourceList;
    }

    private void AddMaterialSwitcher(GameObject target)
    {
        target.AddComponent<MaterialSwitcher>().SetParent(parent, this);
    }
}
