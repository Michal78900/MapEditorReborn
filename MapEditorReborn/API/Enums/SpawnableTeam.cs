namespace MapEditorReborn.API.Enums
{
    /// <summary>
    /// Used to determine the spawnpoint type.
    /// </summary>
    public enum SpawnableTeam
    {
        /// <summary>
        /// Represents <see cref="RoleType.None"/>.
        /// </summary>
        None = -1,

        /// <summary>
        /// Represents <see cref="RoleType.Scp049"/>.
        /// </summary>
        Scp049,

        /// <summary>
        /// Represents <see cref="RoleType.Scp0492"/>.
        /// </summary>
        Scp0492,

        /// <summary>
        /// Represents <see cref="RoleType.Scp079"/>.
        /// </summary>
        Scp079,

        /// <summary>
        /// Represents <see cref="RoleType.Scp096"/>.
        /// </summary>
        Scp096,

        /// <summary>
        /// Represents <see cref="RoleType.Scp106"/>.
        /// </summary>
        Scp106,

        /// <summary>
        /// Represents <see cref="RoleType.Scp173"/>.
        /// </summary>
        Scp173,

        /// <summary>
        /// Represents <see cref="RoleType.Scp93953"/> and <see cref="RoleType.Scp93989"/>.
        /// </summary>
        Scp939,

        /// <summary>
        /// Represents <see cref="RoleType.ClassD"/>.
        /// </summary>
        ClassD,

        /// <summary>
        /// Represents <see cref="RoleType.Scientist"/>.
        /// </summary>
        Scientist,

        /// <summary>
        /// Represents <see cref="RoleType.FacilityGuard"/>.
        /// </summary>
        FacilityGuard,

        /// <summary>
        /// Represents <see cref="RoleType.NtfPrivate"/>, <see cref="RoleType.NtfSergeant"/>, <see cref="RoleType.NtfSpecialist"/> and <see cref="RoleType.NtfCaptain"/>.
        /// </summary>
        MTF,

        /// <summary>
        /// Represents <see cref="RoleType.ChaosRifleman"/>, <see cref="RoleType.ChaosConscript"/>, <see cref="RoleType.ChaosMarauder"/>, <see cref="RoleType.ChaosRepressor"/>.
        /// </summary>
        Chaos,

        /// <summary>
        /// Represents <see cref="RoleType.Tutorial"/>.
        /// </summary>
        Tutorial,
    }
}
