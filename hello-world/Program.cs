// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using HelloWorld.Options;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using HellowWorld;

try
{
    //
    // Load configuration
    //

    var configuration = new ConfigurationBuilder()
        .AddTomlFile("config.toml", optional: false, reloadOnChange: true)
        .Build();

    AppOptions options = new();
    configuration.Bind(AppOptions.Section, options);

    //
    // Dump the configuration to make sure it's working
    //

    var jsonoptions = new JsonSerializerOptions() { WriteIndented = true };
    Console.WriteLine("CONFIG: {0}", JsonSerializer.Serialize(options, jsonoptions));

    //
    // Get a token
    //

    var myApp = ConfidentialClientApplicationBuilder
        .Create(options.Identity!.AppId.ToString())
        .WithClientSecret(options.Identity!.AppSecret!)
        .WithAuthority($"{options.Login!.Authority!}{options.Identity.TenantId}")
        .Build();

    var authResult = await myApp.AcquireTokenForClient(options.Login!.Scopes).ExecuteAsync();

    var jwtToken = new JwtSecurityToken(authResult.AccessToken);

    //
    // Dump the token for viewing
    //

    Console.WriteLine("TOKEN: {0}", JsonSerializer.Serialize(jwtToken.Payload, jsonoptions));

    //
    // Retrieve connected machines
    //

    var client = new ApiClient(options.Resources!.BaseUri!, jwtToken.RawData);
    var machines = await client.GetMachinesAsync();

    //
    // Dump the response for viewing
    //

    using (var jDoc = JsonDocument.Parse(machines))
    {
        Console.WriteLine("MACHINES: {0}", JsonSerializer.Serialize(jDoc, jsonoptions));
    }

    //
    // Retrieve top recommendations by severity
    //

    var vulnerabilities = await client.GetVulnerabilitiesAsync();

    //
    // Dump the response for viewing
    //

    using (var jDoc = JsonDocument.Parse(vulnerabilities))
    {
        Console.WriteLine("RECOMMENDATIONS: {0}", JsonSerializer.Serialize(jDoc, jsonoptions));
    }

}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
