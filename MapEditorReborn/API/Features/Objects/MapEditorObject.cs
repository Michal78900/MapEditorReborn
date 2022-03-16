namespace MapEditorReborn.API.Features.Objects
{
    using System.Linq;
    using Components;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using LiteNetLib4Mirror.Open.Nat;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Component added to all MapEditorReborn objects. It contains properties that are common for most of the objects.
    /// </summary>
    public abstract class MapEditorObject : MonoBehaviour
    {
        /// <summary>
        /// Gets a value indicating whether the object can be rotated.
        /// </summary>
        public virtual bool IsRotatable { get; } = true;

        /// <summary>
        /// Gets a value indicating whether the object can be scaled.
        /// </summary>
        public virtual bool IsScalable { get; } = true;

        /// <summary>
        /// Updates object properties after they were changed.
        /// </summary>
        public virtual void UpdateObject()
        {
            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Gets the attached <see cref="IndicatorObject"/> of the object.
        /// </summary>
        public IndicatorObject AttachedIndicator;

        /// <summary>
        /// Gets or sets forced <see cref="Exiled.API.Enums.RoomType"/> of the object.
        /// </summary>
        public RoomType ForcedRoomType
        {
            get => _forcedRoom;
            set
            {
                CurrentRoom = null;
                _forcedRoom = value;
            }
        }

        /// <summary>
        /// Gets or sets the global postion of the object.
        /// </summary>
        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                UpdateObject();
            }
        }

        /// <summary>
        /// Gets or sets the global rotation of the object.
        /// </summary>
        public Quaternion Rotation
        {
            get => transform.rotation;
            set
            {
                if (!IsRotatable)
                    throw new System.InvalidOperationException($"{name} can not be rotated!");

                transform.rotation = value;
                UpdateObject();
            }
        }

        /// <summary>
        /// Gets or sets the scale of the object.
        /// </summary>
        public Vector3 Scale
        {
            get => transform.localScale;
            set
            {
                if (!IsScalable)
                    throw new System.InvalidOperationException($"{name} can not be rescaled!");

                transform.localScale = value;
                UpdateObject();
            }
        }

        /// <summary>
        /// Gets the current room of the object.
        /// </summary>
        public Room CurrentRoom { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the object is a part of schematic block.
        /// </summary>
        public bool IsSchematicBlock { get; internal set; }

        /// <summary>
        /// Gets the relative position of the object to the <see cref="Room"/> it is currently in.
        /// </summary>
        public Vector3 RelativePosition
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                return CurrentRoom.Type == RoomType.Surface ? transform.position : CurrentRoom.transform.InverseTransformPoint(transform.position);
            }
        }

        /// <summary>
        /// Gets the relative rotation of the object to the <see cref="Room"/> it is currently in.
        /// It will also take into account if the object had a random rotation.
        /// </summary>
        public Vector3 RelativeRotation
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                Vector3 rotation = CurrentRoom.Type == RoomType.Surface ? transform.eulerAngles : transform.eulerAngles - CurrentRoom.transform.eulerAngles;

                if (gameObject.TryGetComponent(out ObjectRotationComponent rotationComponent))
                {
                    if (rotationComponent.XisRandom)
                        rotation.x = -1f;

                    if (rotationComponent.YisRandom)
                        rotation.y = -1f;

                    if (rotationComponent.ZisRandom)
                        rotation.z = -1f;
                }

                return rotation;
            }
        }

        /// <summary>
        /// Gets the room type of the object.
        /// </summary>
        public RoomType RoomType
        {
            get
            {
                if (CurrentRoom == null)
                    CurrentRoom = FindRoom();

                return CurrentRoom.Type;
            }
        }

        /// <summary>
        /// Finds the room in which object is. This method is more accurate than <see cref="Map.FindParentRoom(GameObject)"/> because it will also check for distance.
        /// </summary>
        /// <returns>The found <see cref="Room"/>.</returns>
        public Room FindRoom()
        {
            if (ForcedRoomType != RoomType.Unknown)
                return Map.Rooms.Where(x => x.Type == ForcedRoomType).OrderBy(x => (x.Position - Position).sqrMagnitude).First();

            Room room = Map.FindParentRoom(gameObject);

            if (room?.Type == RoomType.Surface && Position.y <= 500f)
                room = Map.Rooms.Where(x => x.Type == ForcedRoomType).OrderBy(x => (x.Position - Position).sqrMagnitude).First();

            return room ?? Map.Rooms.First(x => x.gameObject.name == "Outside");
        }

        /// <summary>
        /// Gets the corresponding <see cref="Color"/> given a specified <see cref="string"/>.
        /// </summary>
        /// <param name="colorText">The specified <see cref="string"/> to convert.</param>
        /// <returns>The corresponding <see cref="Color"/>.</returns>
        public Color GetColorFromString(string colorText)
        {
            Color color = new Color(-1f, -1f, -1f);
            string[] charTab = colorText.Split(new char[] { ':' });

            if (charTab.Length >= 4)
            {
                if (float.TryParse(charTab[0], out float red))
                    color.r = red / 255f;

                if (float.TryParse(charTab[1], out float green))
                    color.g = green / 255f;

                if (float.TryParse(charTab[2], out float blue))
                    color.b = blue / 255f;

                if (float.TryParse(charTab[3], out float alpha))
                    color.a = alpha;

                return color != new Color(-1f, -1f, -1f) ? color : Color.magenta * 3f;
            }

            if (colorText[0] != '#' && colorText.Length == 8)
                colorText = '#' + colorText;

            return ColorUtility.TryParseHtmlString(colorText, out color) ? color : Color.magenta * 3f;
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public void Destroy() => Destroy(gameObject);

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => $"{name} {Position} {Rotation.eulerAngles} {Scale}";

        internal Player prevOwner;

        private RoomType _forcedRoom = RoomType.Unknown;
    }
}
