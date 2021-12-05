using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    private void Start()
    {
        SaveDataObjectList list = new SaveDataObjectList();

        transform.position = Vector3.zero;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.TryGetComponent(out ObjectComponent objectComponent))
            {
                switch (objectComponent.ObjectType)
                {
                    case ObjectType.Primitive:
                        {
                            PrimitiveComponent primitiveComponent = obj.GetComponent<PrimitiveComponent>();

                            PrimitiveObject primitive = new PrimitiveObject()
                            {
                                Type = primitiveComponent.Type,
                                Color = ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color),

                                Position = obj.transform.localPosition,
                                Rotation = obj.transform.eulerAngles,
                                Scale = obj.transform.localScale,
                            };

                            list.Primitives.Add(primitive);

                            break;
                        }

                    case ObjectType.Light:
                        {


                            break;
                        }

                    case ObjectType.Item:
                        {
                            ItemType item = obj.GetComponent<ItemComponent>().ItemType;

                            ItemObject itemObject = new ItemObject()
                            {
                                ItemType = item,

                                Position = GetCorrectPosition(obj.transform.position, item),
                                Rotation = GetCorrectRotation(obj.transform.eulerAngles, item),
                                Scale = obj.transform.localScale,
                            };

                            list.Items.Add(itemObject);

                            break;
                        }

                    case ObjectType.Workstation:
                        {
                            WorkStationObject workStation = new WorkStationObject()
                            {
                                Position = obj.transform.localPosition,
                                Rotation = obj.transform.eulerAngles,
                                Scale = obj.transform.localScale,
                            };

                            list.WorkStations.Add(workStation);

                            break;
                        }
                }
            }
        }
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented));
        Debug.Log($"{name} has been successfully compiled!");
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



