using System.Text.Json;
using HelloWorld.Options;

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

    var json = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });

    Console.WriteLine("CONFIG: {0}", json);
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
