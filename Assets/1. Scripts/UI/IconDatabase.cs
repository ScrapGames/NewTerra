using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Icon Database", menuName = "NewTerra/UI/IconDatabase")]
public class IconDatabase : ScriptableObject
{
    [Header("Notifications")]
    public Sprite terraformIcon;
    public Sprite buildIcon, alertIcon, orbiticon, resourceIcon, harvesterIcon, refineryIcon;

    public Sprite GetNotificationSprite(Notifications.Category category)
    {
        switch (category)
        {
            case Notifications.Category.Alert:
                return alertIcon;
            default:
            case Notifications.Category.Build:
                return buildIcon;
            case Notifications.Category.Terraform:
                return terraformIcon;
        }
    }
}
