// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

namespace HelloWorld.Options;

/// <summary>
/// Options describing the identity of the app
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Config file section
    /// </summary>
    public static readonly string Section = "Identity";

    /// <summary>
    /// Directory (tenant) ID
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Application (client) ID
    /// </summary>
    public Guid AppId { get; init; }

    /// <summary>
    /// Client secret value
    /// </summary>
    public string? AppSecret { get; init; }

    /// <summary>
    /// API scopes to request at login
    /// </summary>
    public ICollection<string> Scopes { get; init; } = [];
}
