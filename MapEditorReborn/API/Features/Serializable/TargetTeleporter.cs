namespace MapEditorReborn.API.Features.Serializable
{
    public class TargetTeleporter
    {
        public TargetTeleporter()
        {
        }

        public TargetTeleporter(int id, float chance)
        {
            Id = id;
            Chance = chance;
        }

        public int Id { get; set; }

        public float Chance { get; set; } = 100f;
    }
}
