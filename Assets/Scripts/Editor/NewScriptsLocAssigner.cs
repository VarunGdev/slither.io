using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;
using Mono.Cecil;

[ExecuteInEditMode]
public class NewScriptsLocAssigner : EditorWindow
{
    //Future Note: Add odin File Field locator
    public string _newScriptPath;
    public bool IsAutomated = false;
    private bool RanOnce = false;

    private GUISkin CustomSkin;

    
    [MenuItem("Editor Tools/NewScript Address Allocator")]
    public static void ShowWindow()
    {
        EditorWindow window = (EditorWindow)GetWindow(typeof(NewScriptsLocAssigner));
        window.minSize = new Vector2(500, 500);
    }

    private void SubmitAddress()
    {
        string[] _csFiles = Directory.GetFiles("Assets/", "*.cs");


        foreach (string file in _csFiles)
        {
            DateTime fileLastUpdated = System.IO.File.GetLastWriteTime(file);
            TimeSpan fileLastUpdationTimeLimit = new TimeSpan(0, 0, 30, 0);

            if (TimeSpan.Compare(DateTime.Now - fileLastUpdated, fileLastUpdationTimeLimit) == -1)
            {

                if (Directory.Exists(_newScriptPath + "/"))
                {
                    try
                    {
                      FileUtil.MoveFileOrDirectory(file, _newScriptPath + "/" + Path.GetFileName(file));
                      File.Delete(file);
                      
                    }catch{

                        Debug.LogError("Error moving file");
                    }

                }
                AssetDatabase.Refresh();
                AssetDatabase.OpenAsset((MonoScript)AssetDatabase.LoadAssetAtPath(_newScriptPath + "/" + Path.GetFileName(file), typeof(MonoScript)));

            }

        }

    }

    private void OnGUI()
    {   
        CustomSkin = Resources.Load<GUISkin>("CustomEditorWindow GUISkin");
        GUI.skin = CustomSkin;

        GUILayout.Label("Folder Address", EditorStyles.boldLabel);
        IsAutomated = EditorGUILayout.Toggle("Update Realtime", IsAutomated);
        GUILayout.Space(10);
        _newScriptPath = EditorGUILayout.TextField("Folder Path", _newScriptPath);
        var DataScript = Resources.Load<GameObject>("DataTransmitterPrefab");



        if (!RanOnce)
        {
            _newScriptPath = DataScript.GetComponent<DataTransmitter>().data;
            IsAutomated = DataScript.GetComponent<DataTransmitter>().UpdateRealTimeData;
            RanOnce = true;
        }


        if (DataScript != null)
        {


            if (GUILayout.Button("Select Folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select Destination Folder", "Assets", "");
                if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                {
                    _newScriptPath = "Assets" + path.Substring(Application.dataPath.Length);
                }

            }

            GUILayout.Space(10);
            if (!IsAutomated)
            {
                if (GUILayout.Button("Save Changes", GUILayout.Height(45)))
                {
                    DataScript.GetComponent<DataTransmitter>().data = _newScriptPath;
                    DataScript.GetComponent<DataTransmitter>().UpdateRealTimeData = IsAutomated;
                    SubmitAddress();
                    this.Close();
                }
            }
            else
            {

                DataScript.GetComponent<DataTransmitter>().data = _newScriptPath;
                DataScript.GetComponent<DataTransmitter>().UpdateRealTimeData = IsAutomated;
                SubmitAddress();

                if (GUILayout.Button("Save Changes", GUILayout.Height(45)))
                {
                    this.Close();

                }

            }

        }


    }
}


