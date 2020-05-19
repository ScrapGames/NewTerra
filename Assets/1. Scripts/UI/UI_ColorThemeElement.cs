using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ScrapGames.UI
{
    public class UI_ColorThemeElement : MonoBehaviour
    {
#pragma warning disable CS0649
        public UI_ColorThemeData theme;
        public bool keepAfterInitialize;
        [HideInInspector] public bool controlledExternally;
        [Header("Image Links")] public Image[] primaryElements;
        public Image[] secondaryElements, tertiaryElements, accentElements, titlePrimaryElements,
            titleSecondaryElements, primaryBackgrounds, secondaryBackgrounds;
        [Header("Text Links")] public bool changeTextSize = true;
        public TextMeshProUGUI[] regularText, headerText, subheaderText, titleText, buttonText;

        [SerializeField, Header("Overrides")] private bool overridePrimaryElements;
        [SerializeField]
        private bool overrideSecondaryElements, overrideTertiaryElements, overrideAccentElements, overrideTitleElements,
            overrideRegularText, overrideHeaderText, overrideSubheaderText, overrideTitleText, overrideButtonText;

        [SerializeField] private UI_ColorThemeData.ColorSet overrideColors;

        private void Awake()
        {
            if (!controlledExternally) SetTheme();
        }

        public void SetTheme()
        {
            if (theme == null) return;

            // Primary Elements
            SetColors(primaryElements, overridePrimaryElements
                ? overrideColors.primaryColor
                : theme.colorSet.primaryColor
            );

            // Secondary Elements
            SetColors(secondaryElements, overrideSecondaryElements
                ? overrideColors.secondaryColor
                : theme.colorSet.secondaryColor
            );

            // Tertiary Elements
            SetColors(tertiaryElements, overrideTertiaryElements
                ? overrideColors.tertiaryColor
                : theme.colorSet.tertiaryColor
            );

            // Accent Elements
            SetColors(accentElements, overrideAccentElements
                ? overrideColors.accentColor
                : theme.colorSet.accentColor
            );

            // Title Primary Elements
            SetColors(titlePrimaryElements, overrideTitleElements
                ? overrideColors.titlePrimaryColor
                : theme.colorSet.titlePrimaryColor
            );

            // Title Secondary Elements
            SetColors(titleSecondaryElements, overrideTitleElements
                ? overrideColors.titleSecondaryColor
                : theme.colorSet.titleSecondaryColor
            );

            // Regular Text
            SetColors(regularText, overrideRegularText
                ? overrideColors.regularTextColor
                : theme.colorSet.regularTextColor,
                theme.regularFontSize
            );

            // Header Text
            SetColors(headerText, overrideHeaderText
                ? overrideColors.headerTextColor
                : theme.colorSet.headerTextColor,
                theme.headerFontSize
            );

            // Subheader Text
            SetColors(titleText, overrideSubheaderText
                ? overrideColors.subheaderTextColor
                : theme.colorSet.subheaderTextColor,
                theme.subheaderFontSize
            );

            // Title Text
            SetColors(titleText, overrideTitleText
                ? overrideColors.titleTextColor
                : theme.colorSet.titleTextColor,
                theme.titleFontSize
            );

            // Button Text
            SetColors(buttonText, overrideButtonText
                ? overrideColors.buttonTextColor
                : theme.colorSet.buttonTextColor,
                theme.buttonFontSize
            );

            // BackgroundImage
            SetImages(primaryBackgrounds, theme.primaryBackground);
            SetImages(secondaryBackgrounds, theme.secondaryBackground);

            // Remove component - ENSURE NO OTHER SCRIPTS REFERENCE THIS!!!
            if (keepAfterInitialize) return;
            if (Application.isEditor) return;
            Destroy(this);
        }

        private void SetColors(Image[] elements, Color color)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                float alpha = elements[i].color.a;
                color.a = alpha;
                elements[i].color = color;
            }
        }

        private void SetColors(TextMeshProUGUI[] elements, Color color, float fontSize)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                TextMeshProUGUI tmp = elements[i];
                float alpha = tmp.color.a;
                color.a = alpha;
                tmp.color = color;
                if(changeTextSize) tmp.fontSize = fontSize;
            }
        }

        private void SetImages(Image[] elements, Sprite sprite)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].sprite = sprite;
            }
        }
    }
}