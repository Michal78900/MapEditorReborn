using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Updater
{
    public static WebClient WebClient { get; } = new WebClient();

    public static DownloadProgressChangedEventArgs DownloadProgress { get; private set; }

    public static string UpdaterText
    {
        get => _updaterText;
        set
        {
            _updaterText = value;

            if (!string.IsNullOrEmpty(value) && value != "Progress Bar")
                Debug.Log(_updaterText);
        }
    }

    static Updater()
    {
        WebClient.DownloadProgressChanged += (sender, args) =>
        {
            DownloadProgress = args;
        };
    }

    public static readonly string DownloadedZipPath = Path.Combine(Directory.GetCurrentDirectory(), "NewMapEditorReborn.zip");
    public static readonly string ExtractedDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "NewMapEditorReborn");

    [MenuItem("SchematicManager/Update SL-CustomObject")]
    public static async Task DownloadNewVersion()
    {
        UpdaterText = "Downloading SL-CustomObject...";

        try
        {
            await WebClient.DownloadFileTaskAsync("https://github.com/Michal78900/MapEditorReborn/releases/latest/download/SL-CustomObjects.zip", DownloadedZipPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error while downloading new version of SL-CustomObject!\n" + e);
        }

        UpdaterText = "Successfully downloaded!";
        DownloadProgress = null;

        UpdaterText = "Extracting...";
        await Task.Run(() => ZipFile.ExtractToDirectory(DownloadedZipPath, ExtractedDirectoryPath));

        UpdaterText = "Successfully extracted!";

        UpdaterText = "Progress Bar";
        UpdaterText = null;

        /*
        File.Delete(DownloadedZipPath);
        DeleteDirectory(ExtractedDirectoryPath);
        return;
        */

        string dontTouchPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "DONT TOUCH");
        string resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Resources");
        
        DeleteDirectory(dontTouchPath);
        DeleteDirectory(resourcesPath);

        Directory.Move(Path.Combine(ExtractedDirectoryPath, "SL-CustomObjects", "Assets", "DONT TOUCH"), dontTouchPath);
        Directory.Move(Path.Combine(ExtractedDirectoryPath, "SL-CustomObjects", "Assets", "Resources"), resourcesPath);

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

    private static void DeleteDirectory(string path)
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

    private static string _updaterText;
}
