using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

[ExecuteInEditMode]
public class Schematic : SchematicBlock
{
    public override BlockType BlockType => BlockType.Schematic;

    public void CompileSchematic()
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath)
            ? Config.ExportPath
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MapEditorReborn_CompiledSchematics");
        
        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (Directory.Exists(schematicDirectoryPath))
            DeleteDirectory(schematicDirectoryPath);

        if (File.Exists($"{schematicDirectoryPath}.zip"))
            File.Delete($"{schematicDirectoryPath}.zip");

        Directory.CreateDirectory(schematicDirectoryPath);

        int rootObjectId = transform.GetInstanceID();
        /*
        SchematicObjectDataList blockList = new SchematicObjectDataList(rootObjectId);
        Dictionary<int, SerializableRigidbody> rigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
        List<SerializableTeleport> teleports = new List<SerializableTeleport>();
        */
        BlockList.RootObjectId = rootObjectId;
        BlockList.Blocks.Clear();
        RigidbodyDictionary.Clear();
        Teleports.Clear();

        if (TryGetComponent(out Rigidbody rigidbody))
            RigidbodyDictionary.Add(rootObjectId, new SerializableRigidbody(rigidbody));

        // transform.localScale = Vector3.one;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.CompareTag("EditorOnly") || obj == transform)
                continue;

            int objectId = obj.transform.GetInstanceID();

            SchematicBlockData block = new SchematicBlockData
            {
                Name = obj.name,
                ObjectId = objectId,
                ParentId = obj.parent.GetInstanceID(),
                Position = Quaternion.Euler(obj.parent.eulerAngles) * obj.localPosition,
            };

            if (obj.TryGetComponent(out SchematicBlock schematicBlock))
            {
                if (!schematicBlock.Compile(block, this))
                    continue;
            }
            else
            {
                // Light
                if (obj.TryGetComponent(out Light lightComponent))
                {
                    block.BlockType = BlockType.Light;
                    block.Properties = new Dictionary<string, object>
                    {
                        { "Color", ColorUtility.ToHtmlStringRGBA(lightComponent.color) },
                        { "Intensity", lightComponent.intensity },
                        { "Range", lightComponent.range },
                        { "Shadows", lightComponent.shadows != LightShadows.None },
                    };
                }
                else // Empty transform
                {
                    obj.localScale = Vector3.one;

                    block.BlockType = BlockType.Empty;
                    block.Rotation = obj.localEulerAngles;
                }
            }

            if (obj.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                block.AnimatorName = runtimeAnimatorController.name;

                BuildPipeline.BuildAssetBundle(runtimeAnimatorController,
                    runtimeAnimatorController.animationClips,
                    Path.Combine(schematicDirectoryPath, runtimeAnimatorController.name),
                    AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
            }

            if (obj.TryGetComponent(out rigidbody))
                RigidbodyDictionary.Add(objectId, new SerializableRigidbody(rigidbody));

            BlockList.Blocks.Add(block);
        }

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}.json"),
            JsonConvert.SerializeObject(BlockList, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (RigidbodyDictionary.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Rigidbodies.json"),
                JsonConvert.SerializeObject(RigidbodyDictionary, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Teleports.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Teleports.json"),
                JsonConvert.SerializeObject(Teleports, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Config.ZipCompiledSchematics)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(schematicDirectoryPath, $"{schematicDirectoryPath}.zip",
                System.IO.Compression.CompressionLevel.Optimal, true);
            Directory.Delete(schematicDirectoryPath, true);
        }

        Debug.Log($"<color=green><b>{name}</b> has been successfully compiled!</color>");
    }

    public void Update()
    {
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
            Debug.LogError("<color=red>Do not change the scale of the root object or any other empty transform!</color>");
        }

        if (name.Contains(" "))
        {
            name = name.Replace(" ", string.Empty);
            Debug.LogError("<color=red>Schematic name cannot contain spaces!</color>");
        }
    }

    // This is only used in nested schematics (schematics inside other schematics)
    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;

        block.BlockType = BlockType.Schematic;
        block.Properties = new Dictionary<string, object>
        {
            { "SchematicName", name }
        };

        return false;
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

    internal readonly SchematicObjectDataList BlockList = new SchematicObjectDataList();
    internal readonly Dictionary<int, SerializableRigidbody> RigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
    internal readonly List<SerializableTeleport> Teleports = new List<SerializableTeleport>();

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression |
                                                                      BuildAssetBundleOptions.ForceRebuildAssetBundle |
                                                                      BuildAssetBundleOptions.StrictMode;

    private static readonly Config Config = SchematicManager.Config;
}