using System.Reflection;
using HelloWorld.Options;
using monitor_manage_alerts;

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
    })
    .Build();

host.PrepareDatabase();
host.Run();
