using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that adds a final offset to the camera
/// </summary>
[AddComponentMenu("")] // Hide in menu
public class CinemachineRotationOffset : CinemachineExtension
{
    [Tooltip("Offset the camera's rotation by this much (camera space)")]
    public Vector3 m_Offset = Vector3.zero;

    [Tooltip("When to apply the offset")]
    public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Aim;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == m_ApplyAfter)
        {
            Vector3 offset = state.FinalOrientation * m_Offset;
            state.OrientationCorrection = Quaternion.LookRotation(((VirtualCamera.LookAt.position - transform.position) + offset));
            //state.PositionCorrection += offset;
        }
    }
}
