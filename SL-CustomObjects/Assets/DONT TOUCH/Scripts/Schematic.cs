using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

public class Schematic : MonoBehaviour
{
    public void CompileSchematic()
    {
        string parentDirectoryPath = SchematicManager.Instance.ExportPath;
        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (!Directory.Exists(schematicDirectoryPath))
            Directory.CreateDirectory(schematicDirectoryPath);

        SchematicObjectDataList list = new SchematicObjectDataList()
        {
            RootObjectId = transform.GetInstanceID(),
        };

        transform.localScale = Vector3.one;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.tag == "EditorOnly" || obj == transform)
                continue;

            SchematicBlockData block = new SchematicBlockData()
            {
                Name = obj.name,
                ObjectId = obj.transform.GetInstanceID(),
                ParentId = obj.parent.GetInstanceID(),
                Position = obj.localPosition,
            };

            if (obj.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
                block.AnimatorName = animator.runtimeAnimatorController.name;

            if (obj.TryGetComponent(out SchematicBlock schematicBlock))
            {
                switch (schematicBlock.BlockType)
                {
                    case BlockType.Primitive:
                        {
                            if (obj.TryGetComponent(out PrimitiveComponent primitiveComponent))
                            {
                                block.Rotation = obj.localEulerAngles;
                                block.Scale = primitiveComponent.Collidable ? obj.localScale : obj.localScale * -1f;

                                block.BlockType = BlockType.Primitive;
                                block.Properties = new Dictionary<string, object>()
                                {
                                    { "PrimitiveType", (PrimitiveType)Enum.Parse(typeof(PrimitiveType), obj.tag) },
                                    { "Color", ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color) },
                                };
                            }

                            break;
                        }

                    case BlockType.Pickup:
                        {
                            if (obj.TryGetComponent(out PickupComponent pickupComponent))
                            {
                                block.Rotation = obj.localEulerAngles;
                                block.Scale = obj.localScale;

                                block.BlockType = BlockType.Pickup;
                                block.Properties = new Dictionary<string, object>()
                                {
                                    { "ItemType",  pickupComponent.ItemType},
                                };

                                if (!pickupComponent.UseGravity)
                                    block.Properties.Add("Kinematic", string.Empty);

                                if (!pickupComponent.CanBePickedUp)
                                    block.Properties.Add("Locked", string.Empty);
                            }

                            break;
                        }

                    case BlockType.Workstation:
                        {
                            if (obj.TryGetComponent(out WorkstationComponent workstationComponent))
                            {
                                block.Rotation = obj.localEulerAngles;
                                block.Scale = obj.localScale;

                                block.BlockType = BlockType.Workstation;
                                block.Properties = new Dictionary<string, object>()
                                {
                                    { "IsInteractable",  workstationComponent.IsInteractable},
                                };
                            }

                            break;
                        }
                }
            }
            else
            {
                if (obj.TryGetComponent(out Light lightComponent))
                {
                    block.BlockType = BlockType.Light;
                    block.Properties = new Dictionary<string, object>()
                    {
                        { "Color", ColorUtility.ToHtmlStringRGBA(lightComponent.color) },
                        { "Intensity", lightComponent.intensity },
                        { "Range", lightComponent.range },
                        { "Shadows", lightComponent.shadows != LightShadows.None },
                    };
                }
                else
                {
                    obj.localScale = Vector3.one;

                    block.Rotation = obj.localEulerAngles;
                    block.BlockType = BlockType.Empty;
                }
            }

            if (animator != null)
                BuildPipeline.BuildAssetBundle(animator.runtimeAnimatorController, animator.runtimeAnimatorController.animationClips, Path.Combine(schematicDirectoryPath, animator.runtimeAnimatorController.name), AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);

            list.Blocks.Add(block);
        }

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log($"{name} has been successfully compiled!");
    }

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode;
}



