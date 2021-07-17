namespace MapEditorReborn
{
    /// <summary>
    /// Config with translations for the ToolGun.
    /// </summary>
    public class Translations
    {
        /// <summary>
        /// Gets a text indicating creating mode.
        /// </summary>
        public string ModeCreating { get; private set; } = "<color=yellow>Mode:</color> <color=green>Creating</color>";

        /// <summary>
        /// Gets a text indicating deleting mode.
        /// </summary>
        public string ModeDeleting { get; private set; } = "<color=yellow>Mode:</color> <color=red>Deleting</color>";

        /// <summary>
        /// Gets a text indicating selecting mode.
        /// </summary>
        public string ModeSelecting { get; private set; } = "<color=yellow>Mode:</color> <color=yellow>Selecting</color>";

        /// <summary>
        /// Gets a text indicating copying mode.
        /// </summary>
        public string ModeCopying { get; private set; } = "<color=yellow>Mode:</color> <color=#34B4EB>Copying to the ToolGun</color>";
    }
}
