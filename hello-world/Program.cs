// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using HelloWorld.Options;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using HellowWorld;
using Azure.Identity;

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

    ClientSecretCredential clientSecretCredential =
        new ClientSecretCredential
        (
            options.Identity!.TenantId.ToString(), 
            options.Identity.AppId.ToString(),
            options.Identity.AppSecret
        ); 

    var token = await clientSecretCredential.GetTokenAsync(new(options.Login!.Scopes.ToArray()));
    var jwtTokenAzure = new JwtSecurityToken(token.Token);

    //
    // Dump the token for viewing
    //

    Console.WriteLine("TOKEN: {0}", JsonSerializer.Serialize(jwtTokenAzure.Payload, jsonoptions));

    //
    // Retrieve recently-reported machines
    //

    var client = new ApiClient(options.Resources!.BaseUri!, jwtTokenAzure.RawData);
    var machines = await client.GetRecentMachinesAsync();

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

    var recommendations = await client.GetRecommendationsAsync();

    //
    // Dump the response for viewing
    //

    using (var jDoc = JsonDocument.Parse(recommendations))
    {
        Console.WriteLine("RECOMMENDATIONS: {0}", JsonSerializer.Serialize(jDoc, jsonoptions));
    }

}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
