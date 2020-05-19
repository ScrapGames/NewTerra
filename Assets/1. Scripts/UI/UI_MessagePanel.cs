using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Notifications
{
    public class UI_MessagePanel : MonoBehaviour
    {
#pragma warning disable CS0649

        [SerializeField] private TextMeshProUGUI text_Title, text_Message;
        private string[] lines;
        public bool IsReady { get; set; }
        private Coroutine coroutine;
        [SerializeField] private Button okButton;
        private DroneMessageData currentMessageData;

        [SerializeField] private AudioClip clip_typing;


        public void ShowMessage(DroneMessageData messageData)
        {
            currentMessageData = messageData;
            EventSystem.current.SetSelectedGameObject(null);
            okButton.gameObject.SetActive(false);
            lines = LocalizationManager.GetText(currentMessageData.messageID).Split("|"[0]);
            text_Title.text = lines[0];


            text_Message.text = ""; // Reset text

            // Removing white spaces
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimStart();
                lines[i] = lines[i].TrimEnd();
            }


            if (!gameObject.activeSelf)
            {

                // Play Tween
                transform.localScale = Vector3.zero;
                gameObject.SetActive(true);
                transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack)
                    .OnComplete(() => StartMessage());

                // Play Audio
                AudioManager.PlayUISound(AudioManager.Instance.clip_UiIn);
            }
            else
                StartMessage();

        }

        private void StartMessage()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(TypeMessage());
        }

        private IEnumerator TypeMessage()
        {
            WaitForSeconds wait = new WaitForSeconds(GameManager.Instance.gameSettings.ui_TextSpeed);
            IsReady = IsReady = false;
            int currentLine = 1;
            int currentChar = 0;
            bool isTyping = true;

            // Play Typing Sound
            AudioSource typingSource = AudioManager.PlayUISound(clip_typing, loop: true);


            // Execute onBegin event
            currentMessageData.onBeginEvent.Invoke();

            while (isTyping) // Typing text
            {
                if (IsReady) // If interact key has been pressed while typing, finish the message
                {
                    text_Message.text = "";
                    for (int i = 1; i < lines.Length; i++)
                    {
                        text_Message.text += lines[i];
                        if (i < lines.Length - 1) text_Message.text += "<BR>";
                    }
                    isTyping = false;
                    break;
                }

                text_Message.text += lines[currentLine][currentChar];
                currentChar++;

                // Check for next line
                if (lines[currentLine].Length == currentChar)
                {
                    text_Message.text += "<BR>";
                    currentChar = 0;
                    currentLine++;

                    // Check if all lines done
                    if (currentLine == lines.Length)
                    {
                        isTyping = false;
                        break;
                    }
                }

                yield return wait;
            }
            // Stop playing sound
            typingSource.Stop();

            IsReady = false;
            okButton.transform.localScale = Vector3.zero;
            okButton.gameObject.SetActive(true);
            Tween tween = null;
            okButton.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                okButton.Select();

                // Pulse until ready
                tween = okButton.transform.DOPunchScale(Vector3.one * .15f, 1f, 1)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1);
            });

            while (!IsReady) yield return null; // Wait for interact key to be pressed

            tween.Kill();

            // Hide if told to
            if (currentMessageData.hideOnComplete)
            {
                transform.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    IsReady = false;
                });
            }

            // Play OK sound
            AudioManager.PlayUISound(AudioManager.Instance.clip_ButtonOK);

            // Execute onComplete event
            currentMessageData.onCompleteEvent.Invoke();
        }
    }
}