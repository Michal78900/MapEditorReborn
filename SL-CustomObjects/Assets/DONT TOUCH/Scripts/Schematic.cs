using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    public void CompileSchematic()
    {
        string schematicPath = Path.Combine(path, name);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!Directory.Exists(schematicPath))
            Directory.CreateDirectory(schematicPath);

        SchematicObjectDataList list = new SchematicObjectDataList()
        {
            RootObjectId = transform.GetInstanceID(),
            Scale = transform.localScale,
        };

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj == transform)
                continue;

            if (obj.TryGetComponent(out ObjectComponent objectComponent))
            {
                switch (objectComponent.ObjectType)
                {
                    case BlockType.Primitive:
                        {
                            if (obj.TryGetComponent(out PrimitiveComponent primitiveComponent))
                            {
                                SchematicBlockData block = new SchematicBlockData()
                                {
                                    Name = obj.name,

                                    ObjectId = obj.transform.GetInstanceID(),
                                    ParentId = obj.parent.GetInstanceID(),
                                    AnimatorName = obj.TryGetComponent(out Animator animator) ? animator.runtimeAnimatorController.name : string.Empty,

                                    Position = obj.localPosition,
                                    Rotation = obj.localEulerAngles,
                                    Scale = primitiveComponent.Collidable ? obj.localScale : obj.localScale * -1f,

                                    BlockType = BlockType.Primitive,
                                    Properties = new Dictionary<string, object>()
                                    {
                                        { "PrimitiveType", (PrimitiveType)Enum.Parse(typeof(PrimitiveType), obj.tag) },
                                        { "Color", ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color) },
                                    }
                                };

                                if (animator != null)
                                    BuildPipeline.BuildAssetBundle(animator.runtimeAnimatorController, animator.runtimeAnimatorController.animationClips, Path.Combine(schematicPath, animator.runtimeAnimatorController.name), AssetBundleBuildOptions, SchematicManager.Instance.BuildTarget);

                                list.Blocks.Add(block);
                            }

                            break;
                        }
                }
            }
            else
            {
                SchematicBlockData block = new SchematicBlockData()
                {
                    Name = obj.name,

                    ObjectId = obj.transform.GetInstanceID(),
                    ParentId = obj.parent.GetInstanceID(),
                    AnimatorName = obj.TryGetComponent(out Animator animator) ? animator.runtimeAnimatorController.name : string.Empty,

                    Position = obj.localPosition,
                    Rotation = obj.localEulerAngles,
                    Scale = obj.localScale,

                    BlockType = BlockType.Empty,
                };

                if (animator != null)
                    BuildPipeline.BuildAssetBundle(animator.runtimeAnimatorController, animator.runtimeAnimatorController.animationClips, Path.Combine(schematicPath, animator.runtimeAnimatorController.name), AssetBundleBuildOptions, SchematicManager.Instance.BuildTarget);

                list.Blocks.Add(block);
            }
        }

        File.WriteAllText(Path.Combine(schematicPath, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log($"{name} has been successfully compiled!");
    }

    public static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode;
}



