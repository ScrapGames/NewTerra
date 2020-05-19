using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace EPM
{
    [CustomEditor(typeof(EpicPoolManager))]
    public class InspectorEPM : Editor
    {
        const int NUM_COL_SIZE = 50;
        const int DEL_COL_SIZE = 20;
        const int SCRIPT_FIELD_SIZE = 100;
        const int PREVIEW_SIZE = 64;
        const int ITEMS_PER_PAGE = 10;
        const int GROW_WARNING_AMOUNT = 6;

        EpicPoolManager epm;
        bool hasChanged;
        bool showStatus;
        bool isOverPrefabs;
        Rect dropBoxRect, scriptBoxRect;
        int currentPage;


        private void OnEnable()
        {
            epm = (EpicPoolManager)target;
            epm.EditorInit();
            hasChanged = false;
            showStatus = true;

            // Check objects exist - ie; may have been deleted
            for (int i = epm.poolableObjects.Count - 1; i >= 0; i--)
            {
                PoolInfo info = epm.poolableObjects[i];

                if (info.prefab == null)
                {
                    Debug.LogError("Prefab Missing From Disk! ID: " + info.id + " Removing from database.");
                    epm.RemoveObject(info);
                    hasChanged = true;
                }
            }
            if (hasChanged)
                Repaint();

            currentPage = 0;
        }

        private void OnDisable()
        {
            if (hasChanged)
            {
                EditorUtility.SetDirty(epm);
                AssetDatabase.SaveAssets();
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Height(128));
            GUILayout.Box(EditorEPM.Textures.Logo, EditorEPM.Styles.Logo);
            if (Application.isPlaying)
            {
                GUI.color = Color.red;
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.color = Color.white;
                GUILayout.Label("Game is playing!" + Environment.NewLine + "Unable to modify!", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
                showStatus = GUILayout.Toggle(showStatus, "Show Debug Status");
                DrawRuntimeStatus();
                return;
            }
            else
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Default Pool Size");
                string poolSize = GUILayout.TextField(epm.defaultPoolSize.ToString(), GUILayout.Width(NUM_COL_SIZE));
                if (poolSize != epm.defaultPoolSize.ToString() && int.TryParse(poolSize, out int result))
                {
                    Undo.RecordObject(epm, "Default Pool Size");
                    hasChanged = true;
                    epm.defaultPoolSize = Mathf.Clamp(result, 1, 2000);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Default Grow Size");
                string growSize = GUILayout.TextField(epm.defaultGrowSize.ToString(), GUILayout.Width(NUM_COL_SIZE));
                if (growSize != epm.defaultGrowSize.ToString() && int.TryParse(growSize, out result))
                {
                    Undo.RecordObject(epm, "Default Grow Size");
                    hasChanged = true;
                    epm.defaultGrowSize = Mathf.Clamp(result, 1, 2000);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                GUILayout.Label("Ref Script Name");
                if (epm.scriptPath != "")
                    GUI.enabled = false;
                string scriptName = EpicPoolManager.SanitizeName(GUILayout.TextField(epm.scriptName, GUILayout.Width(SCRIPT_FIELD_SIZE)));
                if (epm.scriptPath != "")
                    GUI.enabled = true;
                if (scriptName != epm.scriptName)
                {
                    Undo.RecordObject(epm, "Script Name");
                    hasChanged = true;
                    epm.scriptName = scriptName;
                }

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            if (isOverPrefabs)
                GUI.color = Color.green;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Drop Prefabs Here", EditorEPM.Styles.DropboxText);
            GUILayout.Box(EditorEPM.Textures.DropBox, EditorEPM.Styles.Dropbox);
            dropBoxRect = GUILayoutUtility.GetLastRect();
            if (isOverPrefabs)
                GUI.color = Color.white;
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            if (epm.poolableObjects.Count > 0)
            {
                GUILayout.Label("Poolable Objects", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                int lastPage = epm.poolableObjects.Count / ITEMS_PER_PAGE;
                if (epm.poolableObjects.Count % ITEMS_PER_PAGE != 0)
                    lastPage++;

                GUILayout.Label(String.Format("Page {0} of {1}", currentPage + 1, lastPage));
                if (currentPage == 0)
                    GUI.enabled = false;
                if (GUILayout.Button("<"))
                {
                    currentPage--;
                    Repaint();
                }
                if (currentPage == 0)
                    GUI.enabled = true;
                if (currentPage == lastPage - 1)
                    GUI.enabled = false;
                if (GUILayout.Button(">"))
                {
                    currentPage++;
                    Repaint();
                }
                if (currentPage == lastPage - 1)
                    GUI.enabled = true;

                GUILayout.EndHorizontal();

                //GUI.color = Color.yellow;
                GUILayout.BeginVertical(EditorEPM.Styles.PoolArea);
                //GUI.color = Color.white;                

                for (int i = currentPage * ITEMS_PER_PAGE; i < (currentPage * ITEMS_PER_PAGE) + ITEMS_PER_PAGE - 1; i++)
                {
                    if (i == epm.poolableObjects.Count) break;

                    if (DrawPoolRow(epm.poolableObjects[i]))
                    {
                        hasChanged = true;
                        Repaint();
                        break;
                    }
                }
                GUILayout.EndVertical();
            }
            else { GUILayout.EndHorizontal(); }

            if (epm.poolableObjects.Count > 0)
            {
                GUILayout.Label("Script Generation", EditorStyles.boldLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Script Path (Location to your base script path)");
                if (epm.scriptPath == "")
                {
                    GUILayout.Label("NO SCRIPT PATH SELECTED", EditorStyles.boldLabel);
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.TextArea(epm.scriptPath);
                    GUI.enabled = true;
                }

                GUILayout.BeginHorizontal();
                if (epm.scriptPath != "")
                    GUI.enabled = false;
                if (GUILayout.Button("Set Script Folder"))
                {
                    EditorUtility.DisplayDialog("Path Warning", "NOTE: This can not be changed later - choose wisely!", "Gotchya");
                    epm.scriptPath = EditorUtility.OpenFolderPanel("Choose Script Folder", "Assets", "Folder");
                }
                if (epm.scriptPath != "")
                    GUI.enabled = true;
                if (epm.scriptPath == "")
                    GUI.enabled = false;
                if (GUILayout.Button("Generate Reference Script"))
                {
                    GenerateScript();
                }
                if (epm.scriptPath == "")
                    GUI.enabled = true;

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            CheckMouseOver();
        }

        private void DrawRuntimeStatus()
        {
            if (!epm.IsActive) return;
            GUILayout.Label("Pool Status", EditorStyles.boldLabel);
            GUILayout.BeginVertical(EditorEPM.Styles.StatusArea);
            foreach (EpicPool info in epm.Pools)
            {
                PoolStatus status = info.GetPoolStatus();
                if (status.GrownCount > 0)
                    GUI.backgroundColor = status.GrownCount >= GROW_WARNING_AMOUNT ? Color.red : Color.yellow;
                GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(30));
                if (status.GrownCount > 0)
                    GUI.backgroundColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label(status.id);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Start #", EditorEPM.Styles.CenterText);
                GUILayout.Label(status.startSize.ToString(), EditorEPM.Styles.CenterText);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Grow #", EditorEPM.Styles.CenterText);
                GUILayout.Label(status.growSize.ToString(), EditorEPM.Styles.CenterText);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Current #", EditorEPM.Styles.CenterText);
                GUILayout.Label(status.currentSize.ToString(), EditorEPM.Styles.CenterText);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Times Grown", EditorEPM.Styles.CenterText);
                GUILayout.Label(status.GrownCount.ToString(), EditorEPM.Styles.CenterText);
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void GenerateScript()
        {
            string nl = Environment.NewLine;
            string filePath = epm.scriptPath;

            if (epm.scriptPath[epm.scriptPath.Length - 1] != Char.Parse("/"))
            {
                filePath += "/";
            }
            filePath += epm.scriptName + ".cs";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }


            FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
            string stringData = "public struct " + epm.scriptName + nl +
                "{" + nl;
            foreach (PoolInfo info in epm.poolableObjects)
            {
                string constName = info.id.ToUpper();

                stringData += "    public const string " + constName + " = \"" + info.id + "\";" + nl;
            }

            stringData += "}";
            byte[] data = new System.Text.UTF8Encoding(true).GetBytes(stringData);
            fs.Write(data, 0, data.Length);
            fs.Close();
            AssetDatabase.Refresh();
            Debug.Log("Wrote new EPMPools.cs file at: " + filePath);
        }

        private bool DrawPoolRow(PoolInfo info)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(PREVIEW_SIZE));
            {
                bool hasChanged = false;
                if (GUILayout.Button(AssetPreview.IsLoadingAssetPreview(info.prefab.GetInstanceID())
                    ? EditorEPM.Textures.Question : AssetPreview.GetAssetPreview(info.prefab),
                    GUILayout.Width(PREVIEW_SIZE), GUILayout.Height(PREVIEW_SIZE)))
                {
                    Selection.activeObject = info.prefab;
                }

                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("ID: " + info.id);
                    GUILayout.EndHorizontal();
                    bool sendReset = GUILayout.Toggle(info.sendReset, "Send Reset?");
                    if (sendReset != info.sendReset)
                    {
                        hasChanged = true;
                        info.sendReset = sendReset;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Pool" + Environment.NewLine + "Size", EditorEPM.Styles.CenterText, GUILayout.Width(NUM_COL_SIZE));
                    string poolSize = GUILayout.TextField(info.initialSize.ToString(), GUILayout.Width(NUM_COL_SIZE));
                    if (poolSize != info.initialSize.ToString() && int.TryParse(poolSize, out int result))
                    {
                        info.initialSize = Mathf.Clamp(result, 1, 2000);
                        hasChanged = true;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }

                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Grow" + Environment.NewLine + "Size", EditorEPM.Styles.CenterText, GUILayout.Width(NUM_COL_SIZE));

                    string growSize = GUILayout.TextField(info.growSize.ToString(), GUILayout.Width(NUM_COL_SIZE));
                    if (growSize != info.growSize.ToString() && int.TryParse(growSize, out int result))
                    {
                        info.growSize = Mathf.Clamp(result, 1, 2000);
                        hasChanged = true;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                if (hasChanged)
                {
                    epm.UpdateInfo(info);
                }
                if (GUILayout.Button("x", EditorEPM.Styles.DeleteButton))
                {
                    if (EditorUtility.DisplayDialog("Remove Object From Pool Manager",
                        "Are you sure you want to remove " + info.prefab.name + " from the pool manager?", "Yes", "No"))
                    {
                        Undo.RecordObject(epm, "Remove " + info.prefab.name + " from pool manager");
                        epm.RemoveObject(info);
                        hasChanged = true;
                    }
                }
                GUILayout.EndHorizontal();
                return hasChanged;
            }

        }

        private void CheckMouseOver()
        {
            Event e = Event.current;
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Check if prefabs in dropbox                          
                if (dropBoxRect.Contains(e.mousePosition))
                {
                    bool hasPrefabs = false;
                    for (int i = 0; i < DragAndDrop.paths.Length; i++)
                    {
                        if (DragAndDrop.paths[i].EndsWith(".prefab"))
                        { hasPrefabs = true; isOverPrefabs = true; break; }
                        else isOverPrefabs = false;
                    }

                    if (hasPrefabs)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        if (e.type == EventType.DragPerform)
                        {
                            for (int i = 0; i < DragAndDrop.paths.Length; i++)
                            {
                                string path = DragAndDrop.paths[i];
                                if (path.EndsWith(".prefab"))
                                {
                                    PoolInfo info = new PoolInfo()
                                    {
                                        growSize = epm.defaultGrowSize,
                                        initialSize = epm.defaultPoolSize,
                                        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path)
                                    };
                                    AssetPreview.GetAssetPreview(info.prefab);
                                    info.id = EpicPoolManager.SanitizeName(info.prefab.name);
                                    // Check if name exists
                                    if (epm.CheckIfEntryExists(info.id))
                                    {
                                        if (EditorUtility.DisplayDialog("Duplicate Entry", "The pool manager already contains an object with this name \""
                                            + info.prefab.name + "\" or is very similar to one which has been sanitized." + Environment.NewLine +
                                            "Please consider renaming the prefab and adding it again." + Environment.NewLine + Environment.NewLine +
                                            "Do you want to highlight the prefab now?" +
                                            " (This will cancel processing any additional objects if this was a multidrop)", "Yes", "No"))
                                        {
                                            Selection.activeObject = info.prefab;
                                            isOverPrefabs = false;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        epm.AddPoolData(info);
                                        hasChanged = true;
                                    }
                                }
                            }
                            isOverPrefabs = false;
                            e.Use();
                        }
                    }
                }
                else
                    isOverPrefabs = false;
            }
        }
    }
}