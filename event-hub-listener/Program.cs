// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using event_hub_listener;
using HelloWorld.Options;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddTomlFile("config.toml", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context,services) => 
    {
        services.AddHostedService<Worker>();
        services.Configure<EventHubOptions>(
            context.Configuration.GetSection(EventHubOptions.Section)
        );
    })
    .Build();

host.Run();
