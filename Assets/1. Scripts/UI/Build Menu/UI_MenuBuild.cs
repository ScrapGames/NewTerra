using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ScrapGames.UI;
using DG.Tweening;

namespace Buildings
{

    public class UI_MenuBuild : MonoBehaviour
    {
        public event System.Action<BuildingData> BuildStarted;

        #region Variables
#pragma warning disable CS0649
        [SerializeField, Header("Categories")] private UI_Button button_Harvester;
        [SerializeField] private UI_Button button_Refinery, button_Utility, button_Terraform;
        public UI_Button CurrentCategoryButton { get { return GetCategoryButton(currentCategory); } }
        private BuildingType currentCategory;


        [SerializeField, Header("Blueprint Panels")]
        private UI_BlueprintPanel panel_Harvester;
        [SerializeField] private UI_BlueprintPanel panel_Refinery, panel_Utility, panel_Terraform;
        public UI_BlueprintPanel CurrentBlueprintPanel { get { return GetCategoryPanel(currentCategory); } }

        private BuildingData CurrentBuildingData { get; set; }


        [Header("UI Links")]
        [SerializeField] private UI_PreviewBase infoPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        private UI_ColorThemeData colorTheme;

        #endregion



        public void Show(bool show)
        {
            gameObject.SetActive(show);
            AudioManager.PlayUIShow(show);

            if (show)
            {
                RectTransform rt = transform as RectTransform;
                float x = (rt.anchoredPosition.x);
                rt.anchoredPosition = new Vector2(-2000, rt.anchoredPosition.y);
                rt.DOAnchorPosX(x, 0.5f).SetEase(Ease.OutBack);

                // Select harvester button
                button_Harvester.Select();
                button_Harvester.onClick.Invoke();
            }
            else
            {
                infoPanel.ShowPanel(false);
                GameManager.Instance.BuildingPreview.ReleaseBuilding();
            }

        }

        private void Awake()
        {
            colorTheme = GameManager.Instance.colorThemeData;
            IconDatabase iconDatabase = GameManager.Instance.gameSettings.iconDatabase;
            infoPanel.gameObject.SetActive(false);

            currentCategory = BuildingType.Harvest;

            // Set harvester button
            button_Harvester.onClick.AddListener(() =>
            {
                SetCategory(BuildingType.Harvest);
            });
            button_Harvester.image.sprite = iconDatabase.harvesterIcon;

            // Set refinery button
            button_Refinery.onClick.AddListener(() =>
            {
                SetCategory(BuildingType.Refine);
            });
            button_Refinery.image.sprite = iconDatabase.refineryIcon;


            // Init Harvester
            panel_Harvester.Init();

            // Create buttons for each harvester blueprint
            int i = 0;
            foreach (var blueprint in DataManager.Instance.HarvesterBlueprintDatabase.Values)
            {
                HarvesterData data = blueprint as HarvesterData;
                panel_Harvester.SetButtonData(i, data, () =>
                {
                    CurrentBuildingData = data;
                    infoPanel.ShowPreview(data);
                });
                i++;
            }

            // Init Refinery
            panel_Refinery.Init();

            // Create buttons for each refinery blueprint
            i = 0;
            foreach (var blueprint in DataManager.Instance.RefineryBlueprintDatabase.Values)
            {
                RefineryData data = blueprint as RefineryData;
                panel_Refinery.SetButtonData(i, data, () =>
                {
                    CurrentBuildingData = data;
                    infoPanel.ShowPreview(data);
                });
                i++;
            }
        }

        private void SetCategory(BuildingType type)
        {
            // Disable Fade
            UI_IconPanel iconPanel = CurrentCategoryButton.GetComponent<UI_IconPanel>();
            iconPanel.Fade(false);

            // Change highlight color of current button
            CurrentCategoryButton.image.color = iconPanel.image_Corner.color = colorTheme.colorSet.primaryColor;

            // Hide current panel
            CurrentBlueprintPanel.Show(false);

            // Set category
            currentCategory = type;

            // Enable Fade
            iconPanel = CurrentCategoryButton.GetComponent<UI_IconPanel>();
            iconPanel.Fade(true);

            // Set highlight color of current button
            CurrentCategoryButton.image.color = iconPanel.image_Corner.color = colorTheme.colorSet.accentColor;

            // Show panel
            CurrentBlueprintPanel.Show(true);

            // Set title text
            titleText.text = LocalizationManager.GetText(BuildingTextIDs.GetBuildingTextID(type));
        }


        private UI_BlueprintPanel GetCategoryPanel(BuildingType type)
        {
            switch (type)
            {
                default:
                case BuildingType.Harvest: return panel_Harvester;
                case BuildingType.Refine: return panel_Refinery;
                case BuildingType.Utility: return panel_Utility;
                case BuildingType.Terraform: return panel_Terraform;
            }
        }
        private UI_Button GetCategoryButton(BuildingType type)
        {
            switch (type)
            {
                default:
                case BuildingType.Harvest: return button_Harvester;
                case BuildingType.Refine: return button_Refinery;
                case BuildingType.Utility: return button_Utility;
                case BuildingType.Terraform: return button_Terraform;
            }
        }

        public void Build()
        {
            if (CurrentBuildingData == null)
            {
                Debug.LogError("Build button pressed but no blueprint is selected!");
                return;
            }

            BuildStarted?.Invoke(CurrentBuildingData);
            AudioManager.PlayUISound(AudioManager.Instance.clip_ButtonOK);
        }
    }
}