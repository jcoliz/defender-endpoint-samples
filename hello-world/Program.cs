using System.Text.Json;
using HelloWorld.Options;
using Microsoft.Identity.Client;
using  System.IdentityModel.Tokens.Jwt;

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

    var myApp = ConfidentialClientApplicationBuilder
        .Create(options.Identity!.AppId.ToString())
        .WithClientSecret(options.Identity!.AppSecret!)
        .WithAuthority($"{options.Login!.Authority!}{options.Identity.TenantId}")
        .Build();

    var authResult = await myApp.AcquireTokenForClient(options.Login!.Scopes).ExecuteAsync();

    var jwtToken = new JwtSecurityToken(authResult.AccessToken);

    //
    // Dump the token for viewing
    //

    Console.WriteLine("TOKEN: {0}", JsonSerializer.Serialize(jwtToken.Payload, jsonoptions));
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
