using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ScrapGames.UI
{
    public class UI_Button : Button
    {
        public event System.Action Selected;
        public TextMeshProUGUI text_ButtonText;
        public Image image_Glow;

        protected override void Awake()
        {

            base.Awake();
            image_Glow?.gameObject.SetActive(false);
            //SetTheme();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            //SetTheme();
        }
#endif

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            image_Glow?.gameObject.SetActive(true);
            Selected?.Invoke();
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            image_Glow?.gameObject.SetActive(false);
            base.OnDeselect(eventData);
        }

    }
}