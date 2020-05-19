using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScrapGames.UI;

namespace Notifications
{
    public class UI_NotificationExpanded : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private UI_MessageList terraformList, buildList, alertList;
        [SerializeField] private UI_Button terraformButton, buildButton, alertButton;
        public UI_HoldButton deleteAllButton;
        public Transform footer;
        public UI_MessageList CurrentMessageList { get; set; }
        public UI_Button CurrentCategoryButton { get; set; }
        public Category CurrentCateogry { get { return CurrentMessageList.category; } }


        private void Awake()
        {
            footer.gameObject.SetActive(false);
            InitCategory(Category.Alert);
            InitCategory(Category.Build);
            InitCategory(Category.Terraform);

            CurrentCategoryButton = terraformButton;
            CurrentMessageList = terraformList;
            Show(false);
        }

        private void InitCategory(Category category)
        {
            UI_Button button = GetButton(category);
            UI_MessageList list = GetList(category);
            list.Init();
            button.onClick.AddListener(() =>
            {
                ButtonAction(button, list);
            });
            button.image.sprite = GameManager.Instance.gameSettings.iconDatabase.GetNotificationSprite(category);
        }

        private void ButtonAction(UI_Button button, UI_MessageList list)
        {
            bool isDifferentCategory = button != CurrentCategoryButton;
            // Hide footer
            footer.gameObject.SetActive(false);

            // Check if selecting itself
            if (isDifferentCategory)
            {
                // Hide previous panel
                CurrentMessageList.Show(false);
            }

            if (CurrentMessageList != list)
            {
                // Show new list & set as current
                CurrentMessageList = list;
                CurrentMessageList.Show(true);
            }

            // Animate button & set colors - reset previous button
            Color col_Selected = GameManager.Instance.colorThemeData.colorSet.accentColor;
            Color col_Normal = GameManager.Instance.colorThemeData.colorSet.primaryColor;

            UI_IconPanel iconPanel = button.GetComponent<UI_IconPanel>();
            iconPanel.icon.color = col_Selected;
            iconPanel.image_Corner.color = col_Selected;
            iconPanel.Fade(true);

            if (isDifferentCategory)
            {
                iconPanel = CurrentCategoryButton.GetComponent<UI_IconPanel>();
                iconPanel.icon.color = col_Normal;
                iconPanel.image_Corner.color = col_Normal;
                iconPanel.Fade(false);
                // Set new button            
                CurrentCategoryButton = button;
            }
        }

        public void Show(bool show)
        {
            AudioManager.PlayUIShow(show);
            gameObject.SetActive(show);
            if (show)
            {
                CurrentCategoryButton.Select();
                CurrentMessageList.Show(true);
            }
        }

        public UI_Message AddUiMessage(Message message)
        {
            UI_MessageList list = GetList(message.category);
            return list.AddUiMessage(message);
        }


        private UI_MessageList GetList(Category category)
        {
            switch (category)
            {
                default:
                case Category.Build: return buildList;
                case Category.Terraform: return terraformList;
                case Category.Alert: return alertList;
            }
        }

        private UI_Button GetButton(Category category)
        {
            switch (category)
            {
                default:
                case Category.Alert: return alertButton;
                case Category.Build: return buildButton;
                case Category.Terraform: return terraformButton;
            }
        }
    }
}