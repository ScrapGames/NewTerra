using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ScrapGames.UI
{

    [CustomEditor(typeof(UI_ColorThemeElement))]
    public class Inspector_UI_ColorThemeElement : Editor
    {
        override public void OnInspectorGUI()
        {
            UI_ColorThemeElement element = (target as UI_ColorThemeElement);
            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = bg;
            if (GUILayout.Button("Set Theme Colors"))
            {
                element.SetTheme();
                Repaint();
            }

            if (GUILayout.Button("Fix Missing"))
            {
                Undo.RecordObject(element, "Fix missing elements");
                // Check each array of images and remove null references
                element.primaryElements = ClearNull(element.primaryElements);
                element.secondaryElements = ClearNull(element.secondaryElements);
                element.tertiaryElements = ClearNull(element.tertiaryElements);
                element.titlePrimaryElements = ClearNull(element.titlePrimaryElements);
                element.titleSecondaryElements = ClearNull(element.titleSecondaryElements);
                element.accentElements = ClearNull(element.accentElements);
                element.primaryBackgrounds = ClearNull(element.primaryBackgrounds);
                element.secondaryBackgrounds = ClearNull(element.secondaryBackgrounds);
                element.titleText = ClearNull(element.titleText);
                element.buttonText = ClearNull(element.buttonText);
                element.headerText = ClearNull(element.headerText);
                element.regularText = ClearNull(element.regularText);
                element.subheaderText = ClearNull(element.subheaderText);
                Repaint();
            }
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        private Image[] ClearNull(Image[] images)
        {
            List<Image> arr = new List<Image>(images);
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (arr[i] == null)
                    arr.RemoveAt(i);
            }

            return arr.ToArray();
        }

        private TMPro.TextMeshProUGUI[] ClearNull(TMPro.TextMeshProUGUI[] text)
        {
            List<TMPro.TextMeshProUGUI> arr = new List<TMPro.TextMeshProUGUI>(text);
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (arr[i] == null)
                    arr.RemoveAt(i);
            }

            return arr.ToArray();
        }
    }
}