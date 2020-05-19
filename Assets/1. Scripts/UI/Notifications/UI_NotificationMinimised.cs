using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Notifications
{
    public class UI_NotificationMinimised : MonoBehaviour
    {
#pragma warning disable CS0649
        private UI_NotificationIndicator buildIndicator, terraformIndicator, alertIndicator;
        [SerializeField] private UI_NotificationIndicator indicatorPrefab;

        private void Awake()
        {
            terraformIndicator = Instantiate(indicatorPrefab, transform);
            terraformIndicator.icon.sprite = GameManager.Instance.gameSettings.iconDatabase.terraformIcon;
            buildIndicator = Instantiate(indicatorPrefab, transform);
            buildIndicator.icon.sprite = GameManager.Instance.gameSettings.iconDatabase.buildIcon;
            alertIndicator = Instantiate(indicatorPrefab, transform);
            alertIndicator.icon.sprite = GameManager.Instance.gameSettings.iconDatabase.alertIcon;
        }

        public void AddMessage(Category category)
        {
            UI_NotificationIndicator indicator = GetIndicator(category);
            indicator.ChangeCount(1);
        }

        public void RemoveMessage(Category category)
        {
            UI_NotificationIndicator indicator = GetIndicator(category);
            indicator.ChangeCount(-1);
        }

        public void SetMessageCount(Category category, int count)
        {
            UI_NotificationIndicator indicator = GetIndicator(category);
            indicator.ChangeCount(count);
        }

        private UI_NotificationIndicator GetIndicator(Category category)
        {
            switch (category)
            {
                default:
                case Category.Build: return buildIndicator;
                case Category.Terraform: return terraformIndicator;
                case Category.Alert: return alertIndicator;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}