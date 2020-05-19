using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Notifications
{
    public class UI_NotificationIndicator : MonoBehaviour
    {
#pragma warning disable CS0649
        public Image border, icon;
        [SerializeField] private TextMeshProUGUI countText, countTextShadow;
        private const float FADE_VALUE = 0.25f;
        private const float FADE_DELAY = 3.5f;
        private const float FADE_DURATION = 2f;
        private const float POSITION_OFFSET = 150;
        private const float POSITION_IN_DURATION = 0.2f;
        private int count = 0;

        private void Start()
        {
            SetAlpha(FADE_VALUE);
            border.transform.Translate(Vector3.down * POSITION_OFFSET, Space.Self);
        }


        public void ChangeCount(int value)
        {
            ChangeCount(value, false);
        }

        public void ChangeCount(int value, bool setCount)
        {
            // Clear any existing tweens
            border.DOKill();
            icon.DOKill();
            countText.DOKill();
            countTextShadow.DOKill();
            border.transform.DOKill(true);

            // Set count
            count = setCount ? value : count + value;
            countText.text = countTextShadow.text = count.ToString();

            // Reset alpha
            SetAlpha(1);

            // Reposition
            /*
            Vector3 pos = border.transform.localPosition;
            pos.y = 0;
            border.transform.localPosition = pos;
            */
            border.transform.DOLocalMoveY(0, POSITION_IN_DURATION).SetEase(Ease.OutBack);

            // Add tween to fade
            border.DOFade(FADE_VALUE, FADE_DURATION).SetDelay(FADE_DELAY);
            icon.DOFade(FADE_VALUE, FADE_DURATION).SetDelay(FADE_DELAY);
            countText.DOFade(FADE_VALUE, FADE_DURATION).SetDelay(FADE_DELAY);
            countTextShadow.DOFade(FADE_VALUE, FADE_DURATION).SetDelay(FADE_DELAY);

            // Add tween to position
            border.transform.DOLocalMoveY(-POSITION_OFFSET, FADE_DURATION).SetDelay(FADE_DELAY);
        }

        private void SetAlpha(float value)
        {
            Color c = border.color;
            c.a = value;
            border.color = c;
            c = icon.color;
            c.a = value;
            icon.color = c;
            c = countText.color;
            c.a = value;
            countText.color = c;
            c = countTextShadow.color;
            c.a = value;
            countTextShadow.color = c;
        }
    }
}