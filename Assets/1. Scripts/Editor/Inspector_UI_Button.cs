using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using TMPro;

namespace ScrapGames.UI
{
    [CustomEditor(typeof(UI_Button))]
    public class Inspector_UI_Button : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UI_Button button = target as UI_Button;

            button.text_ButtonText = EditorGUILayout.ObjectField
            (
                "Button Text",
                button.text_ButtonText,
                typeof(TextMeshProUGUI),
                true
            ) as TextMeshProUGUI;

            button.image_Glow = EditorGUILayout.ObjectField
            (
                "Selection Highlight",
                button.image_Glow,
                typeof(Image),
                true
            ) as Image;
        }
    }
}