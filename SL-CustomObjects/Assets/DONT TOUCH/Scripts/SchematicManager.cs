using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

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
        Instance = GetWindow<SchematicManager>("SchematicManager");

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
        if (!Directory.Exists(Schematic.path))
            Directory.CreateDirectory(Schematic.path);

        System.Diagnostics.Process.Start(Schematic.path);
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            CompileAll();
    }

    // Add menu item named "My Window" to the Window menu
    [MenuItem("SchematicManager/Settings")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        Instance = GetWindow<SchematicManager>("SchematicManager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", BuildTarget);
        BuildOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Build options", BuildOptions);
        OpenDirectoryAfterCompilying = EditorGUILayout.ToggleLeft("Open schematics directory after compilying", OpenDirectoryAfterCompilying);
    }

    [SerializeField]
    public BuildTarget BuildTarget = BuildTarget.StandaloneLinux64;

    [SerializeField]
    public BuildAssetBundleOptions BuildOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode;

    [SerializeField]
    public bool OpenDirectoryAfterCompilying = false;
}
