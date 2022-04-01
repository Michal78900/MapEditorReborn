using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SchematicManager : EditorWindow
{
    static SchematicManager()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    public static SchematicManager Instance { get; private set; }

    [MenuItem("SchematicManager/Compile all _F6")]
    private static void CompileAll()
    {
        if (Instance == null)
            Instance = GetWindow<SchematicManager>("SchematicManager", false, new Type[] { typeof(SceneView) });

        Debug.ClearDeveloperConsole();

        foreach (Schematic schematic in FindObjectsOfType<Schematic>())
        {
            schematic.CompileSchematic();
        }

        if (Instance.OpenDirectoryAfterCompilying)
            OpenDirectory();
    }

    [MenuItem("SchematicManager/Open schematics directory")]
    private static void OpenDirectory()
    {
        if (Instance == null)
            Instance = GetWindow<SchematicManager>("SchematicManager", false, new Type[] { typeof(SceneView) });

        if (!Directory.Exists(Instance.ExportPath))
            Directory.CreateDirectory(Instance.ExportPath);

        System.Diagnostics.Process.Start(Instance.ExportPath);
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            CompileAll();
    }

    [MenuItem("SchematicManager/Settings")]
    private static void ShowWindow() => Instance = GetWindow<SchematicManager>("SchematicManager");

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        OpenDirectoryAfterCompilying = EditorGUILayout.ToggleLeft("Open schematics directory after compiling", OpenDirectoryAfterCompilying);

        GUILayout.Label($"Output path: {ExportPath}", EditorStyles.largeLabel);

        if (GUILayout.Button("Change output directory"))
        {
            string path = EditorUtility.OpenFolderPanel("Select output path", "", "");

            if (!string.IsNullOrEmpty(path))
                ExportPath = path;
        }

        if (string.IsNullOrEmpty(ExportPath))
            ExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
    }

    [SerializeField]
    public bool OpenDirectoryAfterCompilying;

    [SerializeField]
    public string ExportPath;
}