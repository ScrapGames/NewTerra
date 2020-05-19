using UnityEngine;
using Notifications;
using CraftingResources;
using Buildings;
using UnityEngine.Audio;

#pragma warning disable CS0649


[CreateAssetMenu(fileName = "GameSettings", menuName = "NewTerra/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("World")]
    public string scene_BuildingPreview;
    public float timer_PlanetScan = 6f;
    public float plopRotateSpeed = 100f;

    [Header("UI")]
    public IconDatabase iconDatabase;
    public UI_ModeSelection prefab_UI_ModeSelect;
    public CursorController prefab_UI_Cursor;
    public UI_NotificationMinimised prefab_UI_NotificationMinimised;
    public UI_NotificationExpanded prefab_UI_NotificationExpanded;
    public UI_ResourceList prefab_UI_ResourceList;
    public UI_ComponentIcon prefab_UI_ComponentIcon;
    public UI_MessagePanel prefab_UI_Panel;
    public UI_MenuBuild prefab_UI_BuildMenu;
    public UI_MainMenu prefab_UI_Main;
    public ChangeLogMenu prefab_UI_ChangeLog;
    public float ui_TextSpeed;
    public float ui_ControllerCursorSensitivity;
    public Transform prefab_ScreenCanvas, prefab_CameraCanvas;


    [Header("Materials")]
    public Material material_PlanetScan;
    public Material material_PlanetResourceView;
    public Material material_PlanetResourceViewSelected;
    public Material material_PlopTrue, material_PlopFalse;
    public Material material_PlopLineRenderer;


    [Header("Continent Settings")]
    public Vector3 hoverPunchSize;
    public float hoverPunchDuration;
    [Range(0, 1)] public float hoverPunchElasticity;
    public int hoverPunchVibrato;

    [Header("Audio")]
    public AudioMixerGroup audioMixerGroup_SFX,
        audioMixerGroup_BGM, audioMixerGroup_UI, audioMixerGroup_SFXBG;

}
