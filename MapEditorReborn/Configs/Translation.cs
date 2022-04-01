namespace MapEditorReborn
{
    using Exiled.API.Interfaces;

    /// <summary>
    /// Config with translations for the ToolGun.
    /// </summary>
    public sealed class Translation : ITranslation
    {
        /// <summary>
        /// Gets a text which represents the creating mode.
        /// </summary>
        public string ModeCreating { get; private set; } = "<color=yellow>Mode:</color> <color=green>Creating</color>";

        /// <summary>
        /// Gets a text which represents the deleting mode.
        /// </summary>
        public string ModeDeleting { get; private set; } = "<color=yellow>Mode:</color> <color=red>Deleting</color>";

        /// <summary>
        /// Gets a text which represents the selecting mode.
        /// </summary>
        public string ModeSelecting { get; private set; } = "<color=yellow>Mode:</color> <color=yellow>Selecting</color>";

        /// <summary>
        /// Gets a text which represents the copying mode.
        /// </summary>
        public string ModeCopying { get; private set; } = "<color=yellow>Mode:</color> <color=#34B4EB>Copying to the ToolGun</color>";
    }
}
