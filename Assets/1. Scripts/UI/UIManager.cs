using UnityEngine;
using Notifications;
using CraftingResources;
using Buildings;

public class UIManager : Singleton<UIManager>
{
    public CursorController UI_Cursor { get; private set; }
    public UI_NotificationMinimised UI_NotificationMinimised { get; private set; }
    public UI_NotificationExpanded UI_NotificationExpanded { get; private set; }
    public UI_ResourceList UI_ResourceList { get; private set; }
    public UI_ModeSelection UI_ModeSelection { get; private set; }
    public UI_MessagePanel UI_MessagePanel { get; private set; }
    public UI_MenuBuild UI_BuildMenu { get; private set; }
    public UI_MainMenu UI_Main { get; private set; }
    public NotificationHandler notificationHandler;
    private Vector2 messagePanelSize;
    private Transform screenCanvas, cameraCanvas;

    private void Awake()
    {
        SpawnUI();
    }

    public void SpawnUI()
    {
        GameSettings settings = GameManager.Instance.gameSettings;
        screenCanvas = Instantiate(settings.prefab_ScreenCanvas);
        cameraCanvas = Instantiate(settings.prefab_CameraCanvas);
        DontDestroyOnLoad(screenCanvas);
        DontDestroyOnLoad(cameraCanvas);
        cameraCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        // Load in UI
        UI_Cursor = Instantiate(settings.prefab_UI_Cursor, screenCanvas);
        UI_NotificationMinimised = Instantiate(settings.prefab_UI_NotificationMinimised, screenCanvas);
        UI_NotificationExpanded = Instantiate(settings.prefab_UI_NotificationExpanded, cameraCanvas);
        UI_ResourceList = Instantiate(settings.prefab_UI_ResourceList, screenCanvas);
        UI_ModeSelection = Instantiate(settings.prefab_UI_ModeSelect, screenCanvas);
        UI_MessagePanel = Instantiate(settings.prefab_UI_Panel, screenCanvas);
        UI_BuildMenu = Instantiate(settings.prefab_UI_BuildMenu, screenCanvas);
        UI_Main = Instantiate(settings.prefab_UI_Main, screenCanvas);

        // Toggle panels
        UI_MessagePanel.gameObject.SetActive(false);
        UI_ModeSelection.gameObject.SetActive(false);
        UI_BuildMenu.gameObject.SetActive(false);

        notificationHandler = new NotificationHandler(UI_NotificationMinimised, UI_NotificationExpanded);
    }
}