using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScrapGames.UI;

namespace Buildings
{
    public class UI_BlueprintPanel : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Transform container;
        [SerializeField] private UI_Button buttonPrefab;
        [HideInInspector] public UI_Button[] buttons;
        [SerializeField] protected UI_PreviewBase infoPanel;


        public void Init()
        {
            buttons = new UI_Button[8];

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i]) Destroy(buttons[i]);

                UI_Button button = Instantiate(buttonPrefab, container);
                button.gameObject.SetActive(false);
                buttons[i] = button;
            }
            //infoPanel.InitComponents();
        }

        public UI_Button SetButtonData(int id, BuildingData data, System.Action selectCallback)
        {
            UI_Button button = buttons[id];
            button.image.sprite = data.icon;
            button.text_ButtonText.text = data.LocalizedName;
            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => { selectCallback(); ShowInfoPanel(); });
            return button;
        }

        private void ShowInfoPanel()
        {
            if (!infoPanel.gameObject.activeSelf)
                infoPanel.ShowPanel(true);
        }

        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}