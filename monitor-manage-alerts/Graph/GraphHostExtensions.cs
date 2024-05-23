using Azure.Identity;
using MdeSamples.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace MdeSamples.Graph;

/// <summary>
/// Host builder extensions for adding an MS Graph API client
/// </summary>
public static class GraphHostExtensions
{
    /// <summary>
    /// Add a MS Graph API client factory 
    /// </summary>
    /// <param name="services">Where to add</param>
    public static void AddGraphClient(this IServiceCollection services)
    {
        services.AddSingleton(sp => 
        {
            var options = sp.GetRequiredService<IOptions<IdentityOptions>>();

            ClientSecretCredential clientSecretCredential =
                new ClientSecretCredential
                (
                    options.Value.TenantId.ToString(), 
                    options.Value.AppId.ToString(),
                    options.Value.AppSecret
                ); 

            return new GraphServiceClient(clientSecretCredential, options.Value.Scopes);
        });
    }
}
