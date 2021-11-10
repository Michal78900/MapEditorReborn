using Assets;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    private void Awake()
    {
        SaveDataObjectList listOfObjectsToSave = new SaveDataObjectList();

        transform.position = Vector3.zero;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.TryGetComponent(out ObjectComponent objectComponent))
            {
                switch (objectComponent.ObjectType)
                {
                    case ObjectType.Item:
                        {
                            if (obj.TryGetComponent(out ItemComponent item))
                            {
                                SchematicBlockData block = new SchematicBlockData
                                {
                                    ObjectType = objectComponent.ObjectType,
                                    ItemType = item.ItemType,

                                    Position = GetCorrectPosition(obj.transform.position, item.ItemType),
                                    Rotation = GetCorrectRotation(obj.transform.eulerAngles, item.ItemType),
                                    Scale = obj.transform.localScale

                                };

                                listOfObjectsToSave.Blocks.Add(block);
                            }

                            break;
                        }

                    case ObjectType.Workstation:
                        {
                            SchematicBlockData block = new SchematicBlockData
                            {
                                ObjectType = objectComponent.ObjectType,

                                Position = obj.transform.localPosition,
                                Rotation = obj.transform.eulerAngles,
                                Scale = obj.transform.localScale,
                            };

                            listOfObjectsToSave.Blocks.Add(block);

                            break;
                        }
                }
            }
        }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText(Path.Combine(path, $"{name}.json"), JsonConvert.SerializeObject(listOfObjectsToSave, Formatting.Indented));
    }

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

    private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
}



