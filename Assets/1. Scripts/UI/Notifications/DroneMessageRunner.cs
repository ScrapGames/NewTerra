using UnityEngine;
using DG.Tweening;

namespace Notifications
{
    public class DroneMessageRunner : MonoBehaviour
    {
#pragma warning disable CS0649

        [SerializeField] private DroneMessageData[] messages;
        private int currentMessageID = 0;
        private Vector2 lastPanelSize;
        UI_MessagePanel messagePanel;

        private void Awake()
        {
            // Play messages in order
            for (int i = 0; i < messages.Length - 1; i++)
            {
                messages[i].onCompleteEvent.AddListener(() =>
                {
                    currentMessageID++;
                    PlayMessage(currentMessageID);
                });
            }
        }

        private void Start()
        {
            messagePanel = UIManager.Instance.UI_MessagePanel;
            lastPanelSize = messages[0].panelSize;
            if (lastPanelSize == Vector2.zero) lastPanelSize = (messagePanel.transform as RectTransform).sizeDelta;
        }

        public void PlayMessage(int id = 0)
        {
            DroneMessageData message = messages[id];
            bool changePanelSize = message.panelSize != Vector2.zero;

            if (changePanelSize)
            {
                if (message.panelSize != lastPanelSize)
                    (messagePanel.transform as RectTransform).DOSizeDelta(message.panelSize, 0.75f).SetEase(Ease.OutBack);

                lastPanelSize = message.panelSize;
            }
            messagePanel.ShowMessage(message);
        }

    }
}