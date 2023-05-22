// -----------------------------------------------------------------------
// <copyright file="MapEditorReborn.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Drawing;
using FMOD;
using MapEditorReborn.Events.Handlers.Internal;
using MapEditorReborn.Exiled.Features.Config;
using MapEditorReborn.Factories;
using MapGeneration;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using PluginAPI.Helpers;
using Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace MapEditorReborn;

using System;
using System.IO;
using System.IO.Compression;
using Configs;
using HarmonyLib;
using EventHandler = Events.Handlers.Internal.EventHandler;

/// <summary>
/// The main <see cref="MapEditorReborn"/> plugin class.
/// </summary>
public class MapEditorReborn
{
    private Harmony _harmony;
    private FileSystemWatcher _fileSystemWatcher;
        
    /// <summary>
    /// MapEditorReborn configuration
    /// </summary>
    [PluginConfig("config.yml")] public Config Config;

    /// <summary>
    /// MapEditorReborn translation
    /// </summary>
    [PluginConfig("translations.yml")] public Translation Translation;

    /// <summary>
    /// Gets the <see langword="static"/> instance of the <see cref="MapEditorReborn"/>.
    /// </summary>
    public static MapEditorReborn Singleton { get; private set; }

    /// <summary>
    /// Gets the MapEditorReborn parent folder path.
    /// </summary>
    public static string PluginDir { get; } = Path.Combine(Paths.Configs, "MapEditorReborn");

    /// <summary>
    /// Gets the folder path in which the maps are stored.
    /// </summary>
    public static string MapsDir { get; } = Path.Combine(PluginDir, "Maps");

    /// <summary>
    /// Gets the folder path in which the schematics are stored.
    /// </summary>
    public static string SchematicsDir { get; } = Path.Combine(PluginDir, "Schematics");
    
    /// <summary>
    /// Gets or sets the serializer for configs and translations.
    /// </summary>
    public static ISerializer Serializer { get; set; } = new SerializerBuilder()
        .WithTypeConverter(new VectorsConverter())
        .WithTypeConverter(new ColorConverter())
        .WithTypeConverter(new AttachmentIdentifiersConverter())
        .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
        .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreFields()
        .Build();

    /// <summary>
    /// Gets or sets the deserializer for configs and translations.
    /// </summary>
    public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
        .WithTypeConverter(new VectorsConverter())
        .WithTypeConverter(new ColorConverter())
        .WithTypeConverter(new AttachmentIdentifiersConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
        .IgnoreFields()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// NorthwoodAPI Plugin Handler
    /// </summary>
    public static PluginHandler Handler { get; private set; }

    [PluginEntryPoint("MapEditorReborn-NW", "2.1.2-NW", "MapEditorReborn - NWAPI edition", "Michal78900 (original idea by Killers0992), Access Community")]
    public void LoadPlugin()
    {
        Singleton = this;
        Handler = PluginHandler.Get(this);
            
        ValidateConfig();
        
        EventManager.RegisterEvents<EventHandler>(this);
        EventManager.RegisterEvents<GravityGunHandler>(this);
        EventManager.RegisterEvents<ToolGunHandler>(this);

        //SeedSynchronizer.OnMapGenerated += EventHandler.OnGenerated;

        _harmony = new Harmony($"access-community.mapEditorReborn-nw-{DateTime.Now.Ticks}");
        _harmony.PatchAll();

        TryEnableFileWatcher();

        FactoryManager.RegisterPlayerFactory<MERPlayerFactory>(this);
    }

    private void ValidateConfig()
    {
        if (!Directory.Exists(PluginDir))
        {
            Log.Warning("MapEditorReborn parent directory does not exist. Creating...");
            Directory.CreateDirectory(PluginDir);
        }

        if (!Directory.Exists(MapsDir))
        {
            Log.Warning("Maps directory does not exist. Creating...");
            Directory.CreateDirectory(MapsDir);
        }

        if (!Directory.Exists(SchematicsDir))
        {
            Log.Warning("Schematics directory does not exist. Creating...");
            Directory.CreateDirectory(SchematicsDir);
        }
        else
        {
            foreach (var path in Directory.GetFiles(SchematicsDir))
            {
                if (path.EndsWith(".json"))
                {
                    var schematicName = Path.GetFileNameWithoutExtension(path);
                    var directoryPath = Path.Combine(SchematicsDir, schematicName);
                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    File.Move(path, Path.Combine(directoryPath, schematicName) + ".json");

                    Log.Warning($"{schematicName}.json has been moved to its own folder. Please put an entire schematic directory, not a single file!");
                    continue;
                }

                if (Config.AutoExtractSchematics && path.EndsWith(".zip"))
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        var schematicName = Path.GetFileNameWithoutExtension(path);
                        var directoryPath = Path.Combine(SchematicsDir, schematicName);
                        if (Directory.Exists(directoryPath))
                            Directory.Delete(directoryPath, true);

                        Log.Warning($"Extracting {schematicName}.zip...");
                        ZipFile.ExtractToDirectory(path, SchematicsDir);

                        Log.Warning($"{schematicName}.zip has been successfully extracted!");
                        File.Delete(path);
                    });
                }
            }
        }
    }

    private void TryEnableFileWatcher()
    {
        if (!Config.EnableFileSystemWatcher) return;
            
        _fileSystemWatcher = new FileSystemWatcher(MapsDir)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*.yml",
            EnableRaisingEvents = true,
        };
            
        //_fileSystemWatcher.Changed += EventHandler.OnFileChanged;
            
        Log.Info("FileSystemWatcher enabled!");
    }

    // public override void OnEnabled()
    // {
    //     Singleton = this;
    //
    //
    //
    //     MapEvent.Generated += EventHandler.OnGenerated;
    //     ServerEvent.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
    //     ServerEvent.RoundStarted += EventHandler.OnRoundStarted;
    //     WarheadEvent.Detonated += EventHandler.OnWarheadDetonated;
    //
    //     PlayerEvent.DroppingItem += EventHandler.OnDroppingItem;
    //     PlayerEvent.Shooting += EventHandler.OnShooting;
    //     PlayerEvent.InteractingShootingTarget += EventHandler.OnInteractingShootingTarget;
    //     PlayerEvent.AimingDownSight += EventHandler.OnAimingDownSight;
    //     PlayerEvent.DamagingShootingTarget += EventHandler.OnDamagingShootingTarget;
    //     PlayerEvent.TogglingWeaponFlashlight += EventHandler.OnTogglingWeaponFlashlight;
    //     PlayerEvent.UnloadingWeapon += EventHandler.OnUnloadingWeapon;
    //     PlayerEvent.SearchingPickup += EventHandler.OnSearchingPickup;
    //     PlayerEvent.PickingUpItem += EventHandler.OnPickingUpItem;
    //     PlayerEvent.InteractingLocker += EventHandler.OnInteractingLocker;
    //
    //     PlayerEvent.ChangingItem += GravityGunHandler.OnChangingItem;
    //     PlayerEvent.TogglingFlashlight += GravityGunHandler.OnTogglingFlashlight;
    // }
}