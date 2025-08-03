// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using MdEndpoint.Models;
using MdEndpoint.Options;
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

    // Define a sliding window of time
    var endTime = DateTime.UtcNow;
    var startTime = endTime - TimeSpan.FromDays(30);

    {
        //
        // Request events in the window we're looking for
        //

        var timespan = $"{startTime:o}/{endTime:o}";
        Console.WriteLine($"Timespan: {timespan}");
        var result = await graphClient.Security.MicrosoftGraphSecurityRunHuntingQuery.PostAsync(
            new()
            {
                Query = $"ExposureGraphNodes | where NodeLabel == \"user\"",
                Timespan = timespan
            }
        );

        foreach (var line in result.Results)
        {
            var userId = line.AdditionalData["AccountObjectId"]?.ToString();
            Console.WriteLine("{0} {1}", userId, line.AdditionalData["EmailAddress"]);

            // Retrieve the user object
            var user = await graphClient.Users[userId].GetAsync();

            Console.Write(System.Text.Json.JsonSerializer.Serialize(user, jsonoptions));
        }

    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
