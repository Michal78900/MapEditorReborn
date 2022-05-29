using System;
using System.IO;

public class Config
{
    public Config()
    {
    }

    public Config(Config other) =>
        CopyProperties(other);
    
    public Config CopyProperties(Config source)
    {
        OpenDirectoryAfterCompilying = source.OpenDirectoryAfterCompilying;
        ExportPath = source.ExportPath;
        ZipCompiledSchematics = source.ZipCompiledSchematics;
        AutoAddSchematicComponent = source.AutoAddSchematicComponent;

        return this;
    }

    public bool OpenDirectoryAfterCompilying { get; set; } = false;

    public string ExportPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");

    public bool ZipCompiledSchematics { get; set; } = false;

    public bool AutoAddSchematicComponent { get; set; } = false;

    public bool IncludeSurfaceScene { get; set; } = false;

    public static bool operator ==(Config config, Config other) =>
        config.OpenDirectoryAfterCompilying == other.OpenDirectoryAfterCompilying &&
        config.ExportPath == other.ExportPath &&
        config.ZipCompiledSchematics == other.ZipCompiledSchematics &&
        config.AutoAddSchematicComponent == other.AutoAddSchematicComponent &&
        config.IncludeSurfaceScene == other.IncludeSurfaceScene;

    public static bool operator !=(Config config, Config other) =>
        config.OpenDirectoryAfterCompilying != other.OpenDirectoryAfterCompilying ||
        config.ExportPath != other.ExportPath ||
        config.ZipCompiledSchematics != other.ZipCompiledSchematics ||
        config.AutoAddSchematicComponent != other.AutoAddSchematicComponent ||
        config.IncludeSurfaceScene != other.IncludeSurfaceScene;
}
