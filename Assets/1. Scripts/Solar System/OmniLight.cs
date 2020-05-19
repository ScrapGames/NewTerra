using UnityEngine;

[ExecuteInEditMode]
public class OmniLight : MonoBehaviour
{
#pragma warning disable CS0649
    private const string SUN_POSITION = "_SunPos";
    private const string SUN_COLOR = "_SunColor";
    [SerializeField] private Color sunColor;


    private void Update()
    {
        Shader.SetGlobalVector(SUN_POSITION, transform.position);
        Shader.SetGlobalVector(SUN_COLOR, sunColor);
    }
}