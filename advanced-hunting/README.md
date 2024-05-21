# Advanced Threat Hunting in .NET

Next up, we will explore some of the community hunting queries for MDE.

The difference from the Hello World sample is that we will be using the Microsoft Graph APIs. The legacy APIs have not been [updated since 2022](https://learn.microsoft.com/en-us/defender-endpoint/api/api-release-notes). All new work is going into the [Microsoft Graph Security API](https://learn.microsoft.com/en-us/defender-endpoint/api/api-release-notes).

## Prerequisities

This means we'll need to give our app the correct permissions. For the purpose of this sample, we'll want to ensure our application has the following permissions:

* User.ReadBasic.All
* ThreatHunting.Read.All

## SDK

We'll be using the [Microsoft Graph .NET Client Library](https://github.com/microsoftgraph/msgraph-sdk-dotnet). The Microsoft Graph APIs are a modern well-defined API set with a proper SDK backing them.

Using the SDK is easy for our simple use case. First, we set up the client with needed credentials and scopes:

```csharp
ClientSecretCredential clientSecretCredential =
    new ClientSecretCredential
    (
        options.Identity!.TenantId.ToString(), 
        options.Identity.AppId.ToString(),
        options.Identity.AppSecret
    ); 

GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential, options.Login!.Scopes);
```

Then we simply make calls using the SDK:

```c#
var body = new RunHuntingQueryPostRequestBody() 
{ 
    Query = "DeviceInfo | order by Timestamp desc | project Timestamp, DeviceId, ReportId, ExposureLevel | limit 5" 
};
var result = await graphClient.Security.MicrosoftGraphSecurityRunHuntingQuery.PostAsync(body);
```