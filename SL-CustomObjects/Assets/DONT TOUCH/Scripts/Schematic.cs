using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    private void Start()
    {
        SchematicObjectDataList list = new SchematicObjectDataList()
        {
            RootObjectId = transform.GetInstanceID(),
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
                                    AnimatorName = obj.TryGetComponent(out Animator animator) ? animator.runtimeAnimatorController.name.ToLower() : string.Empty,

                                    Position = obj.localPosition,
                                    Rotation = obj.localEulerAngles,
                                    Scale = primitiveComponent.Collidable ? obj.localScale : obj.localScale * -1f,

                                    BlockType = BlockType.Primitive,
                                    Properties = new Dictionary<string, object>()
                                    {
                                        { "PrimitiveType", primitiveComponent.Type },
                                        { "Color", ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color) },
                                    }
                                };

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
                    AnimatorName = obj.TryGetComponent(out Animator animator) ? animator.runtimeAnimatorController.name.ToLower() : string.Empty,

                    Position = obj.localPosition,
                    Rotation = obj.localEulerAngles,
                    Scale = obj.localScale,

                    BlockType = BlockType.Empty,
                };

                list.Blocks.Add(block);

                BuildPipeline.BuildAssetBundle();
                Selection.ac
      
            }
        }
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log($"{name} has been successfully compiled!");
    }

    /*
    private Vector3 GetCorrectPosition(Vector3 position, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.SCP268:
                position += Vector3.back * 0.15f;
                position += Vector3.up * 0.1f;
                break;

            case ItemType.ArmorLight:
            case ItemType.ArmorCombat:
            case ItemType.ArmorHeavy:
                position += -Vector3.right * 0.05f;
                break;

        }

        return position;
    }

    private Vector3 GetCorrectRotation(Vector3 rotation, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.SCP268:
                rotation += Vector3.up * -90f;
                break;

            case ItemType.ArmorLight:
            case ItemType.ArmorCombat:
            case ItemType.ArmorHeavy:
                rotation += new Vector3(-20f, 150f, -55f);
                break;
        }

        return rotation;
    }
    */

    /*

case ObjectType.Primitive:
    {
        PrimitiveComponent primitiveComponent = obj.GetComponent<PrimitiveComponent>();

        PrimitiveObject primitive = new PrimitiveObject()
        {
            PrimitiveType = primitiveComponent.Type,
            Color = ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color),

            Position = obj.transform.position,
            Rotation = obj.transform.eulerAngles,
            Scale = primitiveComponent.Collidable ? obj.transform.localScale : obj.transform.localScale * -1f,
        };

        list.Primitives.Add(primitive);

        break;
    }

case ObjectType.Light:
    {
        Light lightComponent = obj.GetComponent<Light>();

        LightSourceObject lightSource = new LightSourceObject()
        {
            Color = ColorUtility.ToHtmlStringRGBA(lightComponent.color),
            Intensity = lightComponent.intensity,
            Range = lightComponent.range,
            Shadows = lightComponent.shadows != LightShadows.None,

            Position = obj.transform.position,
        };

        list.LightSources.Add(lightSource);

        break;
    }
*/

    private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
}



