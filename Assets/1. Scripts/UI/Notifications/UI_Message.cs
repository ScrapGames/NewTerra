using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Notifications
{
    public class UI_Message : Selectable, ISelectHandler, IDeselectHandler
    {
#pragma warning disable CS0649
        public event System.Action<UI_Message> MessageSelected;
        public static int LastSelectedID { get; private set; } = -1;
        [Header("UI Links"), SerializeField] private TextMeshProUGUI titleText = null;
        [SerializeField] private TextMeshProUGUI messageText = null;
        [SerializeField] private Image readNotification = null;
        [SerializeField] private Image background = null;
        [SerializeField] private Color readColor, unreadColor, selectedColor;
        private UI_MessageList parent;
        private Message message;
        public int ID { get { return message.id; } }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            MarkRead();
            background.color = selectedColor;
            LastSelectedID = message.id;
            parent.Scroll(transform.GetSiblingIndex());
            MessageSelected?.Invoke(this);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            background.color = readColor;
            LastSelectedID = -1;
        }

        public void MarkReadOnShutdown()
        {
            background.color = readColor;
        }

        public void SetData(Message message, UI_MessageList parent)
        {
            this.message = message;
            titleText.text = message.title;
            messageText.text = message.text;
            readNotification.enabled = !message.isRead;
            background.color = unreadColor;
            this.parent = parent;
        }

        public void MarkRead()
        {
            if (!message.isRead)
            {
                readNotification.color = readColor;
                message.isRead = true;
                UIManager.Instance.UI_NotificationMinimised.RemoveMessage(message.category);
            }
        }
    }
}