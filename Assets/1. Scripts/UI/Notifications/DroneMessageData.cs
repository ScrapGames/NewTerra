using UnityEngine;

namespace Notifications
{
    [System.Serializable]
    public class DroneMessageData
    {
        public string messageID;
        public Vector2 panelSize;

        // Options
        public bool hideOnComplete;
        public UnityEngine.Events.UnityEvent onBeginEvent, onCompleteEvent;
    }
}