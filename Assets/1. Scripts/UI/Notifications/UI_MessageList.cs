using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Notifications
{
    public class UI_MessageList : MonoBehaviour
    {
#pragma warning disable CS0649
        public UI_Message LastSelectedMessage { get; private set; }
        public Category category;
        [SerializeField] private UI_NotificationExpanded notificationMenu;
        [SerializeField] private UI_Message uiMessagePrefab = null;
        [SerializeField] private GameObject uiEmptyPrefab = null;
        [SerializeField] private Transform messageContainer = null;
        [SerializeField] private ScrollRect scrollRect;
        private List<UI_Message> messages;
        private GameObject emptyMessage;

        int lastID;


        private void OnDisable()
        {
            LastSelectedMessage?.MarkReadOnShutdown();
        }

        public void Init()
        {
            messages = new List<UI_Message>();
            emptyMessage = Instantiate(uiEmptyPrefab, messageContainer);
            gameObject.SetActive(false);
            lastID = 0;
        }

        public void Show(bool show)
        {
            gameObject.SetActive(show);

            if (show)
            {
                if (messages.Count == 0)
                    emptyMessage.SetActive(true);
                else
                {
                    if (LastSelectedMessage == null)
                        LastSelectedMessage = messages[lastID];
                }
            }
            else
            {
                GameObject msgGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                if (msgGO != null)
                    lastID = msgGO.transform.GetSiblingIndex() - 1;
            }
        }

        public UI_Message AddUiMessage(Message message)
        {
            UI_Message uiMessage = GameObject.Instantiate(uiMessagePrefab, messageContainer);
            uiMessage.SetData(message, this);
            message.MessageDeleted += OnMessageDeleted;
            messages.Add(uiMessage);
            if (messages.Count == 1)
            {
                lastID = 0;
                emptyMessage.SetActive(false);
                if (gameObject.activeSelf)
                {
                    LastSelectedMessage = messages[0];
                }
            }
            uiMessage.MessageSelected += (msg) =>
            {
                LastSelectedMessage = msg;
                notificationMenu.footer.gameObject.SetActive(true);
            };
            return uiMessage;
        }

        public void Scroll(int childPos)
        {
            lastID = childPos - 1;
            scrollRect.verticalNormalizedPosition = 1 - ((float)childPos - 1) / (messageContainer.childCount - 2);
        }

        private void OnMessageDeleted(Message msg)
        {
            msg.MessageDeleted -= OnMessageDeleted;

            Dictionary<int, UI_Message> list = UIManager.Instance.notificationHandler.GetListByCategory(msg.category);
            if (list.TryGetValue(msg.id, out UI_Message uiMsg))
            {
                list.Remove(msg.id);
                uiMsg.MarkRead();
                int nextID = messages.IndexOf(uiMsg) - 1;

                if (nextID < 0)
                    nextID = 0;

                messages.Remove(uiMsg);
                Destroy(uiMsg.gameObject);

                if (messages.Count == 0)
                {
                    emptyMessage.SetActive(true);
                    // Select category button
                    notificationMenu.CurrentCategoryButton.Select();
                    LastSelectedMessage = null;
                }
                else
                {
                    messages[nextID].Select();
                    Scroll(nextID);
                }
            }
        }
    }
}