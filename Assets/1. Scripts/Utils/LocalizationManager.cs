using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;


public class LocalizationManager : Singleton<LocalizationManager>
{
#pragma warning disable CS0649
    public static event System.Action LanguageSet;

    [SerializeField] private int idColumn;
    [SerializeField] private TextAsset textCSV;

    public SystemLanguage currentLanguage;
    [SerializeField] private SupportedLanguage[] supportedLanguages;
    private Dictionary<SystemLanguage, int> supportedLanguagesDatabase;

    private void Awake()
    {
        supportedLanguagesDatabase = new Dictionary<SystemLanguage, int>();

        for (int i = 0; i < supportedLanguages.Length; i++)
        {
            supportedLanguagesDatabase.Add(supportedLanguages[i].language, supportedLanguages[i].columnID);
        }

        SetLanguage(currentLanguage);
    }

    public static Dictionary<string, string> GameText { get; private set; }

    public void SetLanguage(SystemLanguage language)
    {
        if (!supportedLanguagesDatabase.TryGetValue(language, out int languageID))
        {
            return;
        }

        currentLanguage = language;

        GameText = new Dictionary<string, string>();

        string[] rows = textCSV.text.Split("\n"[0]);

        Regex seperator = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for (int i = 0; i < rows.Length; i++)
        {
            // Read each line and send into database
            string[] row = seperator.Split(rows[i]);

            // Check ID column is valid
            string id = row[idColumn];
            if (id == "")
                continue;

            // Add to database
            string txt = row[languageID];

            // Remove /r suffix
            if (txt.Length > 2)
            {
                string suffixCheck = txt.Substring(txt.Length - 2);
                if (suffixCheck == "\r")
                    txt.Remove(txt.Length - 2, 2);
            }
            GameText.Add(id, txt);
        }

        OnLanguageSet();
    }

    private void OnLanguageSet()
    {
        LanguageSet?.Invoke();
    }

    public static string GetText(string id)
    {
        if (GameText.TryGetValue(id, out string txt))
        {
            return txt;
        }

        Debug.LogErrorFormat("Unable to get localization text for ID <B>{0}</B>", id);
        return "ERROR";
    }

    [System.Serializable]
    public struct SupportedLanguage
    {
        public SystemLanguage language;
        public int columnID;
    }
}
