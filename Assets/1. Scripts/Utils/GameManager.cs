using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

public class GameManager : Singleton<GameManager>
{
#pragma warning disable CS0649
    public static event Action EnterResourceView;
    public static event Action ExitResourceView;
    public static event Action EnterOrbitView;
    public static event Action ExitOrbitView;
    public static event Action EnterPlanetScan;
    public static event Action ExitPlanetScan;
    public static event Action EnterNotificationView;
    public static event Action ExitNotificationView;
    public static event Action<bool> Paused;

    public static ParticleManager PManager { get { return Instance._PManager; } }
    [HideInInspector] public HarvestJobManager harvestJobManager;

    [Header("DEBUG")]

    [Space()]
    [SerializeField] private ParticleManager _PManager;
    public BuildingPreviewCam BuildingPreview { get; set; }


    public bool IsUsingController
    {
        get
        {
            return PlayerController.Instance.playerInput.currentControlScheme == "Controller";
        }
    }

    public GameSettings gameSettings;
    public ScrapGames.UI.UI_ColorThemeData colorThemeData;
    public CameraJuice cameraJuice;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _PManager.Init();
    }

    private void Start()
    {
        harvestJobManager = new HarvestJobManager();
    }

    public void OnEnterResourceView()
    {
        EnterResourceView?.Invoke();
    }

    public void OnExitResourceView()
    {
        UIManager.Instance.UI_ResourceList.Show(false);
        ExitResourceView?.Invoke();
    }

    public void OnEnterOrbitView()
    {
        EnterOrbitView?.Invoke();
    }

    public void OnEnterPlanetScan()
    {
        EnterPlanetScan?.Invoke();
    }

    public void OnExitPlanetScan()
    {
        ExitPlanetScan?.Invoke();
    }

    public void OnEnterNotificationView()
    {
        EnterNotificationView?.Invoke();
    }
    public void OnExitNotificationView()
    {
        ExitNotificationView?.Invoke();
    }

    public void OnPause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        Paused?.Invoke(pause);
    }

    private void FixedUpdate()
    {
        harvestJobManager.UpdateJobs();
    }
}