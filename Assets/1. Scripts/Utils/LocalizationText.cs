using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationText : MonoBehaviour
{
#pragma warning disable CS0649

    [SerializeField] private LocalizedText[] localizedTexts;


    private void Start()
    {
        SetText();
        LocalizationManager.LanguageSet += SetText;
    }

    private void OnDestroy()
    {
        LocalizationManager.LanguageSet -= SetText;
    }


    private void SetText()
    {
        for (int i = 0; i < localizedTexts.Length; i++)
        {
            localizedTexts[i].textObject.text = LocalizationManager.GetText(localizedTexts[i].localizationID);
        }
    }

    [System.Serializable]
    public struct LocalizedText
    {
        public TextMeshProUGUI textObject;
        public string localizationID;
    }
}
