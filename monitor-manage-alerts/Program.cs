using System.Reflection;
using MdeSamples;
using MdeSamples.Data;
using MdeSamples.Graph;
using MdeSamples.Options;
using Microsoft.Graph;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddTomlFile("config.toml", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context,services) => 
    {
        services.AddHostedService<Worker>();
        services.Configure<IdentityOptions>(
            context.Configuration.GetSection(IdentityOptions.Section)
        );
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddDbContext(context);
        services.AddSingleton<IAlertStorage,AlertStorage>();
        services.AddGraphClient();
    })
    .Build();

host.PrepareDatabase();
host.Run();
