using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    public List<AnimationFrame> AnimationFrames = new List<AnimationFrame>();
    public AnimationEndAction AnimationEndAction;

    private void Start()
    {
        transform.position = Vector3.zero;

        if (AnimationFrames.Count > 0)
        {
            originalPosition = transform.position;
            originalRotation = transform.eulerAngles;
            StartCoroutine(UpdateAnimation());
        }
        else
        {
            AnimationFrames = null;
        }

        SaveDataObjectList list = new SaveDataObjectList()
        {
            ParentAnimationFrames = ConvertToSerializableForm(AnimationFrames),
            AnimationEndAction = this.AnimationEndAction,
        };

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
                                PrimitiveType = primitiveComponent.Type,
                                Color = ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color),

                                Position = obj.transform.position,
                                Rotation = obj.transform.eulerAngles,
                                Scale = obj.transform.localScale,

                                AnimationFrames = ConvertToSerializableForm(primitiveComponent.AnimationFrames),
                                AnimationEndAction = primitiveComponent.AnimationEndAction,
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

                    case ObjectType.Item:
                        {
                            ItemType item = obj.GetComponent<ItemComponent>().ItemType;

                            ItemObject itemObject = new ItemObject()
                            {
                                Item = item.ToString(),

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

        File.WriteAllText(Path.Combine(path, $"{name}.json"), JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        Debug.Log($"{name} has been successfully compiled!");
    }

    private IEnumerator<YieldInstruction> UpdateAnimation()
    {
        foreach (AnimationFrame frame in AnimationFrames)
        {
            Vector3 remainingPosition = frame.PositionAdded;
            Vector3 remainingRotation = frame.RotationAdded;
            Vector3 deltaPosition = remainingPosition / Mathf.Abs(frame.PositionRate);
            Vector3 deltaRotation = remainingRotation / Mathf.Abs(frame.RotationRate);

            yield return new WaitForSeconds(frame.Delay);

            int i = 0;

            while (true)
            {
                if (remainingPosition != Vector3.zero)
                {
                    transform.position += deltaPosition;
                    remainingPosition -= deltaPosition;
                }

                if (remainingRotation != Vector3.zero)
                {
                    transform.Rotate(deltaRotation, Space.World);
                    remainingRotation -= deltaRotation;
                }

                if (remainingPosition.sqrMagnitude <= 1 && remainingRotation.sqrMagnitude <= 1)
                    break;

                yield return new WaitForSeconds(frame.FrameLength);

                i++;
            }
        }

        if (AnimationEndAction == AnimationEndAction.Destroy)
        {
            Destroy(gameObject);
        }
        else if (AnimationEndAction == AnimationEndAction.Loop)
        {
            transform.position = originalPosition;
            transform.eulerAngles = originalRotation;
            StartCoroutine(UpdateAnimation());
        }
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

    private List<SerializableAnimationFrame> ConvertToSerializableForm(List<AnimationFrame> frames)
    {
        if (frames == null)
            return null;

        List<SerializableAnimationFrame> serializableFrames = new List<SerializableAnimationFrame>();

        foreach (AnimationFrame frame in frames)
        {
            serializableFrames.Add(new SerializableAnimationFrame(frame));
        }

        return serializableFrames;
    }

    private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    private Vector3 originalPosition;
    private Vector3 originalRotation;
}



