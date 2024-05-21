// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

namespace HelloWorld.Options;

/// <summary>
/// Configuration options for app-wide behavior
/// </summary>
public record AppOptions
{
    /// <summary>
    /// Config file section
    /// </summary>
    public static readonly string Section = "App";

    public class EventHubOptions
    {
        public string? ConnectionString { get; init; }
        public string? HubName { get; init; }
    }

    public EventHubOptions? EventHub { get; init; }
}
