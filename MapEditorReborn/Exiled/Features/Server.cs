// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using Mirror;

namespace MapEditorReborn.Exiled.Features;

/// <summary>
/// A set of tools to easily work with the server.
/// </summary>
public static class Server
{
    private static MethodInfo sendSpawnMessage;

    /// <summary>
    /// Gets the cached <see cref="SendSpawnMessage"/> <see cref="MethodInfo"/>.
    /// </summary>
    public static MethodInfo SendSpawnMessage
    {
        get => sendSpawnMessage ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);
    }
}