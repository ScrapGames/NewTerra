using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BuildingPreviewCam : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private Transform spawnPos;
    private GameObject previewObj;
    [SerializeField] private Camera previewCam;
    [SerializeField] private float offset;
    private const float SHOW_TIME = .4f;
    private bool isLoaded = false;
    private List<int> objectLayers;
    private const int RENDER_LAYER = 11;

    private void Awake()
    {
        objectLayers = new List<int>();
    }

    public void SetPreview(string assetRef)
    {
        StartCoroutine(Load(assetRef));
    }

    private IEnumerator Load(string assetRef)
    {
        ReleaseBuilding();

        while (isLoaded)
        {
            yield return null;
        }

        // Get new model from pool and spawn in loc
        previewObj = EPMInstance.SpawnObject(assetRef, spawnPos);
        previewObj.transform.localPosition = Vector3.zero;

        // Set layer for all objects
        objectLayers.Clear();
        foreach (Renderer r in previewObj.GetComponentsInChildren<Renderer>())
        {
            objectLayers.Add(r.gameObject.layer);
            r.gameObject.layer = RENDER_LAYER;
        }

        // Get bounds and set camera
        float size;
        // Get max size to fit
        Bounds bounds = previewObj.GetComponent<Collider>().bounds;
        size = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.y);
        // Set camera size
        previewCam.orthographicSize = size + offset;

        // Animate
        previewObj.transform.localScale = Vector3.zero;
        previewObj.transform.DOScale(Vector3.one, SHOW_TIME).SetEase(Ease.OutBack);
        isLoaded = true;
    }

    public void ReleaseBuilding()
    {
        // Remove existing model if exists
        if (previewObj == null) return;

        // Hide 
        previewObj.transform.DOKill();
        previewObj.transform.localScale = Vector3.one;
        previewObj.transform.DOScale(Vector3.zero, SHOW_TIME).SetEase(Ease.InBack).OnComplete(() =>
        {
            // Reset layers
            int i = 0;
            foreach (Renderer r in previewObj.GetComponentsInChildren<Renderer>())
            {
                r.gameObject.layer = objectLayers[i];
                i++;
            }

            previewObj.transform.localScale = Vector3.one;
            previewObj.GetComponent<EPMReycleObject>().Recycle();
            isLoaded = false;
        });
    }

    private void OnEnable()
    {
        GameManager.Instance.BuildingPreview = this;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.BuildingPreview = null;
    }
}
