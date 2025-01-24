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
    var startTime = endTime - TimeSpan.FromMinutes(10);

    // All the events we've received in this session
    var eventsReceived = new HashSet<DeviceEvent>();

    // All the events we've received since we've moved the window forward
    var eventsReceivedThisWindow = new HashSet<DeviceEvent>();

    while(true)
    {
        //
        // Request events in the window we're looking for
        //

        var timespan = $"{startTime:o}/{endTime:o}";
        Console.WriteLine($"Timespan: {timespan}");
        var result = await graphClient.Security.MicrosoftGraphSecurityRunHuntingQuery.PostAsync(
            new()
            {
                Query = $"DeviceEvents | order by Timestamp asc | project Timestamp, DeviceId, DeviceName, ActionType, ReportId | limit 10",
                Timespan = timespan
            }
        );
        //| where Timestamp between (datetime({startTime:o}) .. datetime({endTime:o}) ) 

        // Transform to DeviceEvent objects    
        var events = result!.Results!.Select(x=>x.AdditionalData).Select(DeviceEvent.FromDictionary).ToHashSet();

        Console.WriteLine($"Received: {events.Count}");

        //
        // Remove events we already have received this window
        //

        events.ExceptWith(eventsReceivedThisWindow);

        // If we received new events in this last request
        if (events.Count > 0)
        {
            // Update the time window to the time of the last event, while leaving the end time unchanged 
            startTime = events.OrderBy(x=>x.Timestamp).Last()!.Timestamp!.Value;

            // Add them to events this session
            eventsReceivedThisWindow.UnionWith(events);

            // Now remove events we already received
            events.ExceptWith(eventsReceived);

            if (events.Count > 0)
            {
                // Dump the new events
                Console.WriteLine("DEVICE EVENTS: {0}", JsonSerializer.Serialize(events, jsonoptions));

                // Add them to known events
                eventsReceived.UnionWith(events);
            }
        }
        else
        {
            // We have successfully received all events in the last desired window.
            // Wait a bit, and then move the entire window forward
            Console.WriteLine("WAIT");
            await Task.Delay(TimeSpan.FromSeconds(15));
            endTime = DateTime.UtcNow;
            startTime = endTime - TimeSpan.FromMinutes(10);
            eventsReceivedThisWindow = new HashSet<DeviceEvent>();
        }
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
