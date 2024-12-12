// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using HelloWorld.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

try
{
    //
    // Load configuration
    //

    var configuration = new ConfigurationBuilder()
        .AddTomlFile("config.toml", optional: false, reloadOnChange: true)
        .Build();

    IdentityOptions options = new();
    configuration.Bind(IdentityOptions.Section, options);

    if (options is null)
    {
        Console.Error.WriteLine("ERROR: Unable to load identity options");
        return;
    }

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
            options.TenantId.ToString(), 
            options.AppId.ToString(),
            options.AppSecret
        ); 

    GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential);

    //
    // Make a hunting query
    //

    var result = await graphClient.Security.MicrosoftGraphSecurityRunHuntingQuery.PostAsync(new() {
        Query = "DeviceInfo | order by Timestamp desc | project Timestamp, DeviceId, ReportId, ExposureLevel | limit 5" 
    });

    //
    // Dump the result
    //

    Console.WriteLine("HUNTING: {0}", JsonSerializer.Serialize(result!, jsonoptions));
}
catch (Exception ex)
{
    Console.Error.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
