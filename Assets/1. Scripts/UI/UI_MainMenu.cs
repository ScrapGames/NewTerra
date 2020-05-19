using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_MainMenu : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private Button button_Play, button_Settings, button_ChangeLog, button_Exit;
    [SerializeField] private float xMovement = -610f, showDuration;
    private ChangeLogMenu menu_ChangeLog;
    private bool isPlaying;
    private Tween tween;

    private void Awake()
    {
        menu_ChangeLog = Instantiate(GameManager.Instance.gameSettings.prefab_UI_ChangeLog, transform.parent);
    }

    private void OnDestroy()
    {
        GameManager.Paused -= Show;
    }

    private void Start()
    {
        // Assign button events
        button_ChangeLog.onClick.AddListener(menu_ChangeLog.Show);
        button_Play.onClick.AddListener(Play);
        button_Exit.onClick.AddListener(Application.Quit);
        GameManager.Paused += Show;
    }

    private void Play()
    {
        // Ensure change log is hidden
        if (menu_ChangeLog.Showing)
            menu_ChangeLog.Show();

        if (!isPlaying)
        {
            Show(false, () =>
            {
                FindObjectOfType<Notifications.DroneMessageRunner>().PlayMessage();
                isPlaying = true;
            });
            return;
        }
        // Hide this menu
        Show(false);
    }

    private void Show(bool show)
    {
        Show(show, null);
    }

    private void Show(bool show, System.Action callback)
    {
        if (show)
        {
            gameObject.SetActive(true);
            Cursor.visible = true;
        }

        tween?.Kill();
        tween = (transform as RectTransform).DOAnchorPosX(show ? 0 : xMovement, showDuration)
            .SetEase(show ? Ease.OutBack : Ease.InBack)
            .OnComplete(() =>
            {
                callback?.Invoke();
                if (!show)
                {
                    GameManager.Instance.OnPause(false);
                    gameObject.SetActive(false);
                }
            })
            .SetUpdate(true);
    }

}
