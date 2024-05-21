// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

namespace HelloWorld.Options;

public class EventHubOptions
{
    /// <summary>
    /// Config file section
    /// </summary>
    public static readonly string Section = "EventHub";

    public string? ConnectionString { get; init; }
    public string? HubName { get; init; }
}
