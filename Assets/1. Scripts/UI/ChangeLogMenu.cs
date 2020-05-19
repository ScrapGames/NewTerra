using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeLogMenu : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private float duration;
    [SerializeField] private float yAmount;
    public bool Showing { get; private set; }
    private bool hasBeenSeen;
    Tween tween;

    public void Show()
    {
        Showing = !Showing;
        AudioManager.PlayUIShow(Showing);
        tween?.Kill();
        tween = (transform as RectTransform).DOAnchorPosY(Showing ? 0 : yAmount, duration)
        .SetEase(Showing ? Ease.OutBack : Ease.InBack)
        .SetUpdate(true);
        if (Showing && !hasBeenSeen)
        {
            hasBeenSeen = true;
            Notifications.Message msg = new Notifications.Message();
            msg.category = Notifications.Category.Alert;
            msg.title = "You Do Care!";
            msg.text = "Wow! Thankyou for checking out the change log, I really appreciate it :)";
            UIManager.Instance.notificationHandler.AddMessage(msg);
        }
    }
}
