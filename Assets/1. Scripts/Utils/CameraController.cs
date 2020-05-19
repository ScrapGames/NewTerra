using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Threading.Tasks;

public class CameraController : MonoBehaviour
{
    #region Variables
#pragma warning disable 0649

    // Movement properties
    public Vector2 PanV2 { get; set; }
    public float ZoomDelta { get; set; }
    public float RotateDelta { get; set; }
    // -------------------

    [Header("Links")] public CinemachineVirtualCamera orbitCam;


    [Header("Zoom"), SerializeField, Range(1, 200)] float minFOV = 50;
    [SerializeField, Range(1, 200)] float maxFOV = 100;
    [SerializeField, Range(1, 100)] float zoomSpeed;
    [SerializeField, Range(0, 1)] float zoomLag;
    float zoomVel, zoomTarget;
    float zoomLength;
    private Vector3 zoomPos;
    private static float currentFOV;
    private static float _maxFOV, _minFOV;
    private static Camera _cam;

    [Header("Pan"), SerializeField, Range(1, 120)] float hardPanSpeed;
    [SerializeField, Range(0, 1)] float hardPanLag;
    [SerializeField, Range(1, 120)] float panSpeedMax, panSpeedMin;
    [SerializeField, Range(0, 1)] float panLag;

    Vector2 panVel, panTarget, panDir;

    [Header("Rotate"), SerializeField, Range(1, 100)] float rotateSpeed;
    [SerializeField, Range(1, 120)] float rotateSpeedMax, rotateSpeedMin;
    [SerializeField, Range(0, 1)] float rotateLag;
    [SerializeField] float zoomRotation;
    float rotateVel, rotateCurrent;

    [Header("Menu Options"), SerializeField] private Vector3 cameraOffset;
    [SerializeField, Range(0, 1)] private float menuOffsetTime;
    private CinemachineCameraOffset cm_cameraOffset;
    private Vector3 hardPanVel, hardPanCurrent;



    #endregion

    void Start()
    {
        zoomTarget = orbitCam.m_Lens.FieldOfView;
        cm_cameraOffset = orbitCam.GetComponent<CinemachineCameraOffset>();
        _maxFOV = maxFOV;
        _minFOV = minFOV;
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!Moon.Target) return;
        Pan();
        Zoom();
        Rotate();
    }

    public void StopMovement()
    {
        panVel *= 0.1f;
        rotateVel *= 0.1f;
    }

    public void HardStop()
    {
        panVel = PanV2 = Vector2.zero;
        rotateVel = 0;
    }

    private void Pan()
    {
        zoomLength = Mathf.InverseLerp(minFOV, maxFOV, orbitCam.m_Lens.FieldOfView);
        float clampSpeed = Mathf.Lerp(panSpeedMin, panSpeedMax, zoomLength);
        panTarget = PanV2 * clampSpeed * Time.deltaTime;
        Vector3 rotAxisY = transform.TransformDirection(Vector3.down);
        Vector3 rotAxisX = transform.TransformDirection(Vector3.right);

        panDir = Vector2.SmoothDamp(panDir, panTarget, ref panVel, panLag);

        transform.RotateAround(Moon.Target.transform.position, rotAxisX, panDir.y);
        transform.RotateAround(Moon.Target.transform.position, rotAxisY, panDir.x);
    }

    private void Zoom()
    {
        float fov = orbitCam.m_Lens.FieldOfView;
        zoomTarget = Mathf.Clamp(zoomTarget - (ZoomDelta * zoomSpeed * Time.deltaTime), minFOV, maxFOV);
        orbitCam.m_Lens.FieldOfView = currentFOV = Mathf.SmoothDamp(fov, zoomTarget, ref zoomVel, zoomLag);
    }

    private void Rotate()
    {
        // Rotate along the vector between camera and planet
        float speed = Mathf.Lerp(rotateSpeedMin, rotateSpeedMax, zoomLength);
        Vector3 axis = (transform.position - Moon.Target.transform.position).normalized;
        float rotateTarget = RotateDelta * speed * Time.deltaTime;
        rotateCurrent = Mathf.SmoothDamp(rotateCurrent, rotateTarget, ref rotateVel, rotateLag);
        transform.Rotate(axis, rotateCurrent, Space.World);
    }

    ///<summary>Sets the zoom level to either extent</summary>    
    public void SetZoom(bool zoomIn)
    {
        zoomTarget = zoomIn ? minFOV : maxFOV;
    }

    public void SetZoom(float percentage)
    {
        zoomTarget = Mathf.Lerp(minFOV, maxFOV, percentage);
    }

    public void HardPan(Vector2 dir)
    {
        Vector3 rotAxisY = transform.TransformDirection(Vector3.down);
        Vector3 rotAxisX = transform.TransformDirection(Vector3.right);

        hardPanCurrent = Vector3.SmoothDamp(hardPanCurrent, dir * hardPanSpeed, ref hardPanVel, hardPanLag);

        transform.RotateAround(Moon.Target.transform.position, rotAxisX, hardPanCurrent.y);
        transform.RotateAround(Moon.Target.transform.position, rotAxisY, hardPanCurrent.x);
    }

    public async Task OffsetCameraAsync(bool start)
    {
        Vector3 value = start ? cameraOffset : Vector3.zero;
        bool isTweening = true;
        DOTween.To(() => cm_cameraOffset.m_Offset, (x) => cm_cameraOffset.m_Offset = x, value, menuOffsetTime)
            .SetUpdate(true)
            .OnComplete(() => isTweening = false);

        while (isTweening)
        {
            await Task.Yield();
        }
    }

    public void DecoupleFromTarget(bool decouple)
    {
        Transform newParent = decouple ? Moon.Target.transform : Moon.Target.bodyObject;
        transform.SetParent(newParent);
    }

    public static float GetImpulseValue(float value, Vector3 upDir)
    {
        float dot = Mathf.Clamp01(Vector3.Dot(upDir, -_cam.transform.forward).Remap(-1f, 1f, -4f, 1f));

        return value * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(_maxFOV, _minFOV, currentFOV)) * dot;
    }
}
