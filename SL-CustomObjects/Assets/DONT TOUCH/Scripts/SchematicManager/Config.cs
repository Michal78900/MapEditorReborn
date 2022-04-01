using System;
using System.IO;

public class Config
{
    public bool OpenDirectoryAfterCompilying { get; set; } = false;

    public string ExportPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    public bool ZipCompiledSchematics { get; set; } = false;

    public bool AutoAddSchematicComponent { get; set; } = false;
}
