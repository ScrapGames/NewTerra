using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Notifications
{
    public class NotificationHandler
    {
        private UI_NotificationMinimised ui_Minimised;
        private UI_NotificationExpanded ui_Expanded;

        private Dictionary<int, UI_Message> buildMessages, terraformMessages, alertMessages;
        private Dictionary<int, Message> messageDatabase;
        private int nextMessageID;


        public NotificationHandler(UI_NotificationMinimised minimised, UI_NotificationExpanded expanded)
        {
            buildMessages = new Dictionary<int, UI_Message>();
            terraformMessages = new Dictionary<int, UI_Message>();
            alertMessages = new Dictionary<int, UI_Message>();
            messageDatabase = new Dictionary<int, Message>();

            ui_Minimised = minimised;
            ui_Expanded = expanded;

            ui_Minimised.SetMessageCount(Category.Build, 0);
            ui_Minimised.SetMessageCount(Category.Terraform, 0);
            ui_Minimised.SetMessageCount(Category.Alert, 0);
        }
        public int AddMessage(Message message)
        {
            message.id = nextMessageID;

            // Determine list to add it to
            Dictionary<int, UI_Message> messageList = GetListByCategory(message.category);

            // Add message into list
            messageList.Add(message.id, ui_Expanded.AddUiMessage(message));
            messageDatabase.Add(message.id, message);

            nextMessageID++;

            // Notify UI
            ui_Minimised.AddMessage(message.category);
            return message.id;
        }

        public void DeleteAllInCategory(Category category)
        {
            // Remove all UI messages
            Dictionary<int, UI_Message> list = GetListByCategory(category);

            int[] ids = new int[list.Count];

            int i = 0;
            foreach (int id in list.Keys)
            {
                ids[i] = id;
                i++;
            }
            i--;
            while (i >= 0)
            {
                RemoveMessage(ids[i]);
                i--;
            }
        }

        public void RemoveMessage(int messageID)
        {
            if (messageDatabase.TryGetValue(messageID, out Message message))
            {
                // Call remove event
                message.OnMessageDeleted();

                // Get category list
                Dictionary<int, UI_Message> list = GetListByCategory(message.category);

                // Remove Message from lists
                list.Remove(messageID);
                messageDatabase.Remove(messageID);
            }
            else
                Debug.LogErrorFormat("Attempted to remove messageID #{0} - message not found!", messageID);
        }

        public Dictionary<int, UI_Message> GetListByCategory(Category category)
        {
            switch (category)
            {
                default:
                case Category.Build: return buildMessages;
                case Category.Terraform: return terraformMessages;
                case Category.Alert: return alertMessages;
            }
        }

        public void ShowExpandedView(bool show)
        {
            ui_Expanded.Show(show);
        }

        public void DeletePressed()
        {
            //UI_Message selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UI_Message>();
            UI_Message selected = ui_Expanded.CurrentMessageList.LastSelectedMessage;
            if (selected == null) return;
            RemoveMessage(selected.ID);
        }

    }
}