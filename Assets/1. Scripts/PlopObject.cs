using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlopObject : MonoBehaviour
{
    #region Variables
    public event System.Action<PlopObject> PlopSet;

    // PLACEMENT
    private const float UPDATE_THROTTLE = 0.25f; // To stop flickering happening on precice placements;
    private const float NORMAL_DOT_THRESHOLD = 0.777f;
    public bool IsTouchingObstacle { get; private set; }
    public bool CanBePlaced
    {
        get { if (!BounceInDone) return false; return _CanBePlaced; }
        private set
        {
            // Throttle how quickly true values are set
            float t = Time.time;
            if (value && t > nextStatusUpdate)
            {
                _CanBePlaced = true;
                nextStatusUpdate = t + UPDATE_THROTTLE;
            }
            else if (!value)
            {
                _CanBePlaced = false;
                nextStatusUpdate = t + UPDATE_THROTTLE;
            }
        }
    }
    private bool _CanBePlaced;
    public string PlacementTag { get; set; } = "Land";
    private bool placementStatusLastFrame;
    private float skinHeight = 0.7f;

    // MATERIAL
    private MeshRenderer[] renderers;
    private Dictionary<Renderer, Material[]> originalMaterials;
    public bool ChangeMaterialAfterPlop { get; set; } = true;


    // COLLIDER/RAY
    private new BoxCollider collider;
    private Vector3[] rayPositions;
    private float rayLength = 2f;
    private LayerMask hitMask;
    private float nextStatusUpdate;
    private Rigidbody rb;
    public bool BounceInDone { get; private set; } = false;

    // BOUNDARY
    private PlopBoundary[] boundaries;
    private GameObject fence;
    private Mesh fenceMesh;
    private MeshFilter fenceMF;
    private Vector3[] verts;
    private int[] tris;
    private Vector2[] uvs;
    private bool isEndingPlop = false;

    #endregion

    private void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
        rayPositions = new Vector3[9];
        collider = GetComponent<BoxCollider>();
        hitMask = LayerMask.GetMask("Planet", "Water");
        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Get original materials
        originalMaterials = new Dictionary<Renderer, Material[]>();

        foreach (Renderer r in renderers)
        {
            originalMaterials.Add(r, r.materials);
            Material[] newMats = new Material[r.materials.Length];
            // Set materials to plop mat
            for (int i = 0; i < r.materials.Length; i++)
            {
                newMats[i] = GameManager.Instance.gameSettings.material_PlopTrue;
            }
            r.materials = newMats;
        }

        SetMaterialColor(false);

        // Get boundaries
        boundaries = new PlopBoundary[4];
        transform.localScale = Vector3.one;
        Vector3 size = collider.size * 0.5f;
        for (int i = 0; i < boundaries.Length; i++)
        {
            boundaries[i] = EPMInstance.SpawnObject("PlopBounds", transform).GetComponent<PlopBoundary>();
        }
        // Top Left
        SetSurfacePosition(transform.position - (transform.right * size.x) + (transform.forward * size.z), 0);
        // Top Right
        SetSurfacePosition(transform.position + (transform.right * size.x) + (transform.forward * size.z), 1);
        // Bottom Right
        SetSurfacePosition(transform.position + (transform.right * size.x) - (transform.forward * size.z), 2);
        // Bottom Left
        SetSurfacePosition(transform.position - (transform.right * size.x) - (transform.forward * size.z), 3);

        for (int i = 0; i < 4; i++)
        {
            PlopBoundary b = boundaries[i];
            b.transform.LookAt(b.transform.position + (b.transform.position - transform.position).normalized, transform.up);
        }
        CreateFence();

        transform.localScale = Vector3.one * 0.001f;
        transform.DOScale(Vector3.one, 1.25f).SetEase(Ease.OutBounce).OnComplete(() => { BounceInDone = true; });

    }

    private void CreateFence()
    {
        fence = new GameObject("Fence");
        fence.transform.SetParent(transform);
        fence.transform.position = transform.position;
        fenceMF = fence.AddComponent<MeshFilter>();
        MeshRenderer mr = fence.AddComponent<MeshRenderer>();
        mr.material = GameManager.Instance.gameSettings.material_PlopLineRenderer;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        fenceMesh = new Mesh();
        verts = new Vector3[10];
        tris = new int[24];
        uvs = new Vector2[10];


        UpdateFenceMesh();
    }

    private void UpdateFenceMesh()
    {
        // First Quad        
        verts[0] = fence.transform.InverseTransformPoint(boundaries[0].Position);
        verts[1] = fence.transform.InverseTransformPoint(boundaries[0].BeamPosition);
        verts[2] = fence.transform.InverseTransformPoint(boundaries[1].Position);
        verts[3] = fence.transform.InverseTransformPoint(boundaries[1].BeamPosition);
        // Second Quad
        verts[4] = fence.transform.InverseTransformPoint(boundaries[2].Position);
        verts[5] = fence.transform.InverseTransformPoint(boundaries[2].BeamPosition);
        // Third Quad
        verts[6] = fence.transform.InverseTransformPoint(boundaries[3].Position);
        verts[7] = fence.transform.InverseTransformPoint(boundaries[3].BeamPosition);
        verts[8] = fence.transform.InverseTransformPoint(boundaries[0].Position);
        verts[9] = fence.transform.InverseTransformPoint(boundaries[0].BeamPosition);

        // Tri's
        tris = new int[]
        {
            0, 1, 3, 0, 3, 2, // First Quad
            2, 3, 5, 2, 5, 4, // Second Quad
            4, 5, 7, 4, 7, 6, // Third Quad
            6, 7, 9, 6, 9, 8  // Forth Quad
        };

        // UVs
        float inc = 0.25f;
        float x = 0, y = 0;
        for (int i = 0; i < uvs.Length; i++)
        {
            if (i % 2 == 0)
            {
                x = (i / 2) * inc;
                y = 0;
            }
            else y = 1;
            uvs[i] = new Vector2(x, y);
        }

        // Set mesh data
        fenceMesh.vertices = verts;
        fenceMesh.triangles = tris;
        fenceMesh.uv = uvs;
        fenceMesh.name = "Fence Mesh";
        fenceMF.mesh = fenceMesh;
    }


    private void SetBoundaries()
    {
        //if (!BounceInDone) return;

        // Set corner top Left
        SetSurfacePosition(rayPositions[2], 0);

        // Set corner top Right
        SetSurfacePosition(rayPositions[0], 1);

        // Set corner bottom Right
        SetSurfacePosition(rayPositions[6], 2);

        // Set corner bottom Left
        SetSurfacePosition(rayPositions[8], 3);

    }

    private void SetSurfacePosition(Vector3 position, int boundaryID)
    {
        Vector3 planetPos = Moon.Target.transform.position;
        Vector3 rayDir = (planetPos - position).normalized;
        Vector3 point = position;
        PlopBoundary boundary = boundaries[boundaryID];
        Transform bTransform = boundary.transform;

        if (Physics.Raycast(position, rayDir, out RaycastHit hit, 10f, hitMask))
        {
            point = hit.point;
        }

        // Set position
        bTransform.position = point;

        // Set rotation
        Vector3 up = (point - planetPos).normalized;
        Vector3 forward = Vector3.Cross(bTransform.right, up);
        bTransform.rotation = Quaternion.LookRotation(forward, up);

        // Set fence verts
    }


    protected void SetMaterialColor(bool canPlop)
    {
        Material plopMat = canPlop
            ? GameManager.Instance.gameSettings.material_PlopTrue
            : GameManager.Instance.gameSettings.material_PlopFalse;

        foreach (Renderer r in renderers)
        {
            Material[] newMats = new Material[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                newMats[i] = plopMat;
            }
            r.materials = newMats;
        }
    }

    public void Plop()
    {
        isEndingPlop = true;
        if (ChangeMaterialAfterPlop)
        {
            foreach (Renderer r in renderers)
            {
                if (originalMaterials.TryGetValue(r, out Material[] mats))
                {
                    r.sharedMaterials = mats;
                }
            }
        }
        // Hide fence
        Destroy(fence);

        // Animate to show plop
        transform.DOPunchScale(-Vector3.one * 0.45f, 0.47f, 2, 0.8f)
        .OnComplete(() =>
        {
            // Hide boundary objects
            for (int i = 0; i < 4; i++)
            {
                GameObject go = boundaries[i].gameObject;
                float pitch = 0.5f + (0.5f * i);

                go.transform.DOScale(Vector3.zero, 0.95f)
                    .SetEase(Ease.OutExpo)
                    .SetDelay(i * 0.2f)
                    .OnComplete(() =>
                    {
                        go.GetComponent<EPMReycleObject>().Recycle();
                        go.transform.localScale = Vector3.one;
                    }
                ).OnStart(() =>
                {
                    AudioManager.Pop(pitch: pitch);
                });
            }
        });
        PlopSet?.Invoke(this);
        Destroy(rb);
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other is an obstacle
        if (other.CompareTag("Building"))
        {
            IsTouchingObstacle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if other is an obstacle
        if (other.CompareTag("Building"))
        {
            IsTouchingObstacle = false;
        }
    }

    private void Update()
    {
        if (isEndingPlop) return;

        CheckCanPlop();
        SetBoundaries();
        UpdateFenceMesh();
    }

    private void CheckCanPlop()
    {
        // Raycast each corner of bounds, if cast hits anything other than target (surface/water), unable to plop
        Vector3 size = collider.size * 0.5f;
        Vector3 skin = transform.up * skinHeight;

        // Top Right
        rayPositions[0] = transform.position + skin + (transform.right * size.x) + (transform.forward * size.z);

        // Top Center
        rayPositions[1] = transform.position + skin + (transform.forward * size.z);

        // Top Left
        rayPositions[2] = transform.position + skin - (transform.right * size.x) + (transform.forward * size.z);

        // Mid Right
        rayPositions[3] = transform.position + skin + (transform.right * size.x);

        // Mid Center
        rayPositions[4] = transform.position + skin;

        // Mid Left
        rayPositions[5] = transform.position + skin - (transform.right * size.x);

        // Bottom Right
        rayPositions[6] = transform.position + skin + (transform.right * size.x) - (transform.forward * size.z);

        // Bottom Center
        rayPositions[7] = transform.position + skin - (transform.forward * size.z);

        // Bottom Left
        rayPositions[8] = transform.position + skin - (transform.right * size.x) - (transform.forward * size.z);

        // Do raycast check
        bool placeCheck = true;
        // Skip if touching an obstacle
        if (IsTouchingObstacle)
            placeCheck = false;
        else
        {
            for (int i = 0; i < rayPositions.Length; i++)
            {
                Ray ray = new Ray(rayPositions[i], -transform.up);

                if (Physics.Raycast(ray, out RaycastHit hit, rayLength, hitMask))
                {
                    // Check if we hit something else first
                    if (hit.collider.tag != PlacementTag)
                    {
                        placeCheck = false;
                        break;
                    }

                    // Check the normal of the hit is within threshold
                    float dot = Vector3.Dot(hit.normal.normalized, transform.up);
                    if (dot < NORMAL_DOT_THRESHOLD)
                    {
                        // Angle of the surface was too steep
                        placeCheck = false;
                        break;
                    }
                }
                else
                    placeCheck = false;
            }
        }

        CanBePlaced = placeCheck;

        if (placementStatusLastFrame != CanBePlaced)
        {
            SetMaterialColor(CanBePlaced);
        }

        placementStatusLastFrame = CanBePlaced;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;


        for (int i = 0; i < rayPositions.Length; i++)
        {
            Ray ray = new Ray(rayPositions[i], -transform.up);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * rayLength);

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, hitMask))
            {
                if (hit.collider.tag != PlacementTag)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
            }

            Gizmos.DrawSphere(rayPositions[i], 0.1f);
        }
    }

}
