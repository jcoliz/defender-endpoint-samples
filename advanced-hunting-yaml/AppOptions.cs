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

    /// <summary>
    /// Options describing the identity of the app
    /// </summary>
    public class IdentityOptions
    {
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
        /// User ID of a sample user to get to validate connection
        /// </summary>
        public string? UserId { get; init; }
    }

    /// <summary>
    /// Options for logging into the system
    /// </summary>
    public class LoginOptions
    {
        /// <summary>
        /// Authority providing the login service
        /// </summary>
        public Uri? Authority { get; init; }

        /// <summary>
        /// API scopes to request at login
        /// </summary>
        public ICollection<string> Scopes { get; init; } = [];
    }

    /// <summary>
    /// Options for connecting with the resource server
    /// </summary>
    public class ResourceOptions
    {
        /// <summary>
        /// Base URI for all resource requests
        /// </summary>
        public Uri? BaseUri { get; init; }
    }

    public IdentityOptions? Identity { get; init; }
    public LoginOptions? Login { get; init; }
    public ResourceOptions? Resources { get; init; }
}
