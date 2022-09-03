using Newtonsoft.Json;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SchematicManager : EditorWindow
{
    public static Config Config { get; private set; }

    public static string ConfigPath { get; }

    static SchematicManager()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;

        ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "config.json");
        Config = File.Exists(ConfigPath) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath)) : new Config();
        _prevConfig = new Config(Config);
    }

    [MenuItem("SchematicManager/Compile all _F6")]
    private static void CompileAll()
    {
        Debug.ClearDeveloperConsole();

        if (Config.AutoAddSchematicComponent)
        {
            foreach (Transform transform in FindObjectsOfType<Transform>())
            {
                if (transform.root == transform && !transform.gameObject.TryGetComponent<Schematic>(out _))
                {
                    if (transform.tag == "EditorOnly" || transform.name == "DONT TOUCH")
                        continue;

                    transform.gameObject.AddComponent<Schematic>();
                }
            }
        }

        foreach (Schematic schematic in FindObjectsOfType<Schematic>())
        {
            schematic.CompileSchematic();
        }

        if (Config.OpenDirectoryAfterCompilying)
            OpenDirectory();
    }

    [MenuItem("SchematicManager/Open schematics directory")]
    private static void OpenDirectory()
    {
        if (!Directory.Exists(Config.ExportPath))
            Directory.CreateDirectory(Config.ExportPath);

        System.Diagnostics.Process.Start(Config.ExportPath);
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            CompileAll();
    }

    [MenuItem("SchematicManager/Settings")]
    private static void ShowWindow() => GetWindow<SchematicManager>("SchematicManager");

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 2000, 2000));
        GUILayout.Label("<size=30><color=white><b>Settings</b></color></size>", UnityRichTextStyle);

        Config.OpenDirectoryAfterCompilying = EditorGUILayout.ToggleLeft
            ("<color=white><i>Open schematics directory after compiling</i></color>", Config.OpenDirectoryAfterCompilying, UnityRichTextStyle);

        Config.ZipCompiledSchematics = EditorGUILayout.ToggleLeft
            ("<color=white><i>Put compiled schematics directly into .zip archives</i></color>", Config.ZipCompiledSchematics, UnityRichTextStyle);

        Config.AutoAddSchematicComponent = EditorGUILayout.ToggleLeft
            ("<color=white><i>Automatically add schematic component to root objects</i></color>", Config.AutoAddSchematicComponent, UnityRichTextStyle);

        EditorGUILayout.Space();
        /*
        GUILayout.Label("<size=30><color=white><b>Extra assets</b></color></size>", UnityRichTextStyle);

        Config.IncludeSurfaceScene = EditorGUILayout.ToggleLeft
            ("<color=white><i>Include Surface scene</i></color>", Config.IncludeSurfaceScene, UnityRichTextStyle);
        */

        EditorGUILayout.Space();
        GUILayout.Label($"<size=20><color=yellow>Output path: <b>{Config.ExportPath}</b></color></size>", UnityRichTextStyle);
        if (GUI.Button(new Rect(10, 150, 200, 30), "<size=15><color=white><i>Change output directory</i></color></size>", new GUIStyle(GUI.skin.button) { richText = true }))
        {
            string path = EditorUtility.OpenFolderPanel("Select output path", Config.ExportPath, "");

            if (!string.IsNullOrEmpty(path))
                Config.ExportPath = path;
        }

        if (GUI.Button(new Rect(225, 150, 200, 30), "<size=15><color=white><i>Reset output directory</i></color></size>", new GUIStyle(GUI.skin.button) { richText = true }))
            Config.ExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics"); ;

        string progressBarText = "Progress Bar";

        if (!string.IsNullOrEmpty(Updater.UpdaterText))
        {
            progressBarText = Updater.UpdaterText;

            if (Updater.DownloadProgress != null)
                progressBarText += $" {Updater.DownloadProgress.BytesReceived / 1048576} MB / {Updater.DownloadProgress.TotalBytesToReceive / 1048576} MB";
        }

        EditorGUI.ProgressBar(new Rect(350, 500, 500, 20), Updater.DownloadProgress?.ProgressPercentage / 100f ?? 0f, progressBarText);

        GUILayout.EndArea();

        if (Config != _prevConfig)
        {
            _prevConfig = _prevConfig.CopyProperties(Config);
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }
    }

    private void Update()
    {
        if (Updater.UpdaterText != null)
        {
            Repaint();
        }
    }

    public static GUIStyle UnityRichTextStyle
    {
        get
        {
            if (_settingsStyle != null)
                return _settingsStyle;

            _settingsStyle = new GUIStyle
            {
                richText = true,
            };

            return _settingsStyle;
        }
    }

    private static GUIStyle _settingsStyle;
    private static Config _prevConfig;
}