// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using Azure.Identity;
using HelloWorld.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    var jsonoptions = new JsonSerializerOptions() 
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault 
    };

    Console.WriteLine("CONFIG: {0}", JsonSerializer.Serialize(options, jsonoptions));

    //
    // Setup authentication for Microsoft Graph Service
    //

    ClientSecretCredential clientSecretCredential =
        new ClientSecretCredential
        (
            options.Identity!.TenantId.ToString(), 
            options.Identity.AppId.ToString(),
            options.Identity.AppSecret
        ); 

    GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential, options.Login!.Scopes);

    //
    // Make a test call to retrieve one user's details
    //

    var user = await graphClient.Users[options.Identity.UserId].GetAsync();

    //
    // Dump the result
    //

    Console.WriteLine("USER: {0}", JsonSerializer.Serialize(user!, jsonoptions));
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
