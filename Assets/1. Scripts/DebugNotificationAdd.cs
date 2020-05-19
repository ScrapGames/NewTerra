using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Notifications
{
    public class DebugNotificationAdd : MonoBehaviour
    {
        NotificationHandler handler;
        private void Start()
        {
            handler = UIManager.Instance.notificationHandler;
            Invoke("AddMessage1", 30f);
            Invoke("AddMessage2", 50f);
            Invoke("AddMessage3", 64f);
            Invoke("AddMessage4", 90f);
            Invoke("AddMessage5", 120f);
        }

        private void AddMessage1()
        {
            handler.AddMessage(new Message()
            {
                title = "Welcome!",
                text = "Welcome to NewTerra, thankyou for testing!",
                category = Category.Build
            }
            );
        }

        private void AddMessage2()
        {
            handler.AddMessage(new Message()
            {
                title = "Feedback!",
                text = "Be sure to send me feedback, it helps the game be better!",
                category = Category.Build
            }
            );
        }

        private void AddMessage3()
        {
            handler.AddMessage(new Message()
            {
                title = "Friends?",
                text = "Have friends who might be interested? Invite them to the discord!",
                category = Category.Terraform
            }
            );
        }
        private void AddMessage4()
        {
            handler.AddMessage(new Message()
            {
                title = "Thanks!",
                text = "Thanks for being a tester!",
                category = Category.Terraform
            }
            );
        }
        private void AddMessage5()
        {
            handler.AddMessage(new Message()
            {
                title = "Go You!",
                text = "You spent more than two minutes in here, well done!",
                category = Category.Alert
            }
            );
        }
    }
}