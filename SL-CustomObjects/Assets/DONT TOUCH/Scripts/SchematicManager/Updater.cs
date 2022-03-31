using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Updater
{
    public static HttpClient HttpClient { get; } = new HttpClient(new HttpClientHandler() { Proxy = null, UseProxy = false });

    public static readonly string DownloadedZipPath = Path.Combine(Directory.GetCurrentDirectory(), "NewMapEditorReborn.zip");
    public static readonly string ExtractedDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "NewMapEditorReborn");

    [MenuItem("SchematicManager/Update SL-CustomObject")]
    public static async Task DownloadNewVersion()
    {
        Debug.Log("Downloading new version...");

        byte[] fileBytes = System.Array.Empty<byte>();
        try
        {
            fileBytes = await HttpClient.GetByteArrayAsync("https://github.com/Michal78900/MapEditorReborn/archive/refs/heads/dev.zip");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error while downloading new version of SL-CustomObject\n" + e);
        }

        Debug.Log("Successfully downloaded!");
        File.WriteAllBytes(DownloadedZipPath, fileBytes);

        Debug.Log("Extracting...");
        ZipFile.ExtractToDirectory(DownloadedZipPath, ExtractedDirectoryPath);

        Debug.Log("Successfully extracted!");
        string dontTouchPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "DONT TOUCH");
        string resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources");

        DeleteDirectory(dontTouchPath);
        DeleteDirectory(resourcesPath);

        Directory.Move(Path.Combine(ExtractedDirectoryPath, "MapEditorReborn-dev", "SL-CustomObjects", "Assets", "DONT TOUCH"), dontTouchPath);
        Directory.Move(Path.Combine(ExtractedDirectoryPath, "MapEditorReborn-dev", "SL-CustomObjects", "Assets", "Resources"), resourcesPath);

        string myProjectsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "My Projects");

        if (!Directory.Exists(myProjectsPath))
        {
            Directory.CreateDirectory(myProjectsPath);
            Directory.CreateDirectory(Path.Combine(myProjectsPath, "Animations"));
            Directory.CreateDirectory(Path.Combine(myProjectsPath, "Scenes"));
        }

        File.Delete(DownloadedZipPath);
        DeleteDirectory(ExtractedDirectoryPath);
        EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }

    public static void DeleteDirectory(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(path, false);
    }
}
