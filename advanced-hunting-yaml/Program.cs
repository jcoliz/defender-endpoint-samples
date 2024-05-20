using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using HelloWorld.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

try
{
    //
    // Parse command-line arguments
    //

    if (args.Length != 1)
    {
        throw new ArgumentException("Please supply path to a query yaml file as the single argument");
    }

    //
    // Deserialize the query
    //

    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    var reader = File.OpenText(args[0]);
    var hq = deserializer.Deserialize<HuntingQuery>(reader);

    var jsonoptions = new JsonSerializerOptions() 
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault 
    };

    Console.WriteLine("QUERY: {0}", JsonSerializer.Serialize(hq, jsonoptions));

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
    // Make a hunting query
    //

    Microsoft.Graph.Security.MicrosoftGraphSecurityRunHuntingQuery.RunHuntingQueryPostRequestBody body = new() { Query = hq.Query };
    var result = await graphClient.Security.MicrosoftGraphSecurityRunHuntingQuery.PostAsync(body);

    //
    // Dump the result
    //

    Console.WriteLine("HUNTING: {0}", JsonSerializer.Serialize(result!, jsonoptions));

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);

    return -1;
}