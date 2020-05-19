using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScrapGames.UI
{
    [CreateAssetMenu(fileName = "UI_ColorThemeData", menuName = "NewTerra/UI/Color Theme Data")]
    public class UI_ColorThemeData : ScriptableObject
    {
#pragma warning disable CS0649
        public ColorSet colorSet;
        [Header("Shared Sprites")]
        public Sprite primaryBackground;
        public Sprite secondaryBackground;

        [Header("Type Settings")]
        public float titleFontSize;
        public float headerFontSize, subheaderFontSize, regularFontSize, buttonFontSize;

        [System.Serializable]
        public struct ColorSet
        {
            [Header("Element Colors")]
            public Color primaryColor;
            public Color secondaryColor;
            public Color tertiaryColor;
            public Color buttonHighlightColor;
            public Color disabledColor;
            public Color accentColor;
            public Color titlePrimaryColor;
            public Color titleSecondaryColor;

            [Header("Font Colors")]
            public Color regularTextColor;
            public Color headerTextColor;
            public Color subheaderTextColor;
            public Color titleTextColor;
            public Color buttonTextColor;
        }
    }
}