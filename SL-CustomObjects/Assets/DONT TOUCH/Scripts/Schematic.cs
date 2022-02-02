using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
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
        };

        transform.localScale = Vector3.one;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj == transform)
                continue;

            SchematicBlockData block = new SchematicBlockData()
            {
                Name = obj.name,
                ObjectId = obj.transform.GetInstanceID(),
                ParentId = obj.parent.GetInstanceID(),
                Position = obj.localPosition,
            };

            if (obj.TryGetComponent(out Animator animator))
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

                    case BlockType.Light:
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
            {
                BuildPipeline.BuildAssetBundle(animator.runtimeAnimatorController, animator.runtimeAnimatorController.animationClips, Path.Combine(schematicPath, animator.runtimeAnimatorController.name), AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log(AssetBundle.LoadFromFile(Path.Combine(schematicPath, animator.runtimeAnimatorController.name)).LoadAllAssets().First(x => x is RuntimeAnimatorController));
            }

            list.Blocks.Add(block);
        }

        File.WriteAllText(Path.Combine(schematicPath, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log($"{name} has been successfully compiled!");
    }


    public static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode;
}



