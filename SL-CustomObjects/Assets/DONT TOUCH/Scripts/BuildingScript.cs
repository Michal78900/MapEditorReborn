using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildingScript
{
    [MenuItem("Schematic/Compile")]
    static void Compile()
    {
        Debug.ClearDeveloperConsole();

        foreach (Schematic schematic in Object.FindObjectsOfType<Schematic>())
        {
            schematic.CompileSchematic();
        }
    }

    [MenuItem("Schematic/CompileAndRun")]
    static void CompileAndRun()
    {
        Compile();
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("Schematic/OpenDirectory")]
    static void OpenDirectory()
    {
        if (!Directory.Exists(Schematic.path))
            Directory.CreateDirectory(Schematic.path);

        System.Diagnostics.Process.Start(Schematic.path);
    }
}
