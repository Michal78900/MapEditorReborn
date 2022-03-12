using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[InitializeOnLoad]
public static class Updater
{
    public static HttpClient HttpClient { get; } = new HttpClient(new HttpClientHandler() { Proxy = null, UseProxy = false });

    [MenuItem("SchematicManager/Update SL-CustomObject")]
    public static async Task DownloadNewVersion()
    {
        Debug.Log("Downloading new version...");

        var response = await HttpClient.GetAsync("https://github.com/Michal78900/MapEditorReborn/archive/refs/heads/dev.zip", HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
        File.WriteAllBytes(@"C:\Users\micha\Downloads\MapEditorReborn-dev.zip", fileBytes);

        Debug.Log("Successfully downloaded!");

        Debug.Log("Extracting...");
        ZipFile.ExtractToDirectory(@"C:\Users\micha\Downloads\MapEditorReborn-dev.zip", @"C:\Users\micha\Downloads\MapEditorReborn-dev");

        Debug.Log("Successfully extracted!");
        File.Delete(@"C:\Users\micha\Downloads\MapEditorReborn-dev.zip");
        await Task.Delay(1000);
        Debug.Log(Directory.GetCurrentDirectory());
        // EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
}
