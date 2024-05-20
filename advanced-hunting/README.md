# Advanced Threat Hunting using C#

Next up, we will explore some of the community hunting queries for MDE.

* [Microsoft Defender for Endpoint Commonly Used Queries and Examples](https://techcommunity.microsoft.com/t5/core-infrastructure-and-security/microsoft-defender-for-endpoint-commonly-used-queries-and/ba-p/1795046)
* [Microsoft Defender Hunting Quieries from Azure Sentinel](https://github.com/Azure/Azure-Sentinel/tree/master/Hunting%20Queries/Microsoft%20365%20Defender)
* [Advanced Hunting using Python](https://learn.microsoft.com/en-us/defender-endpoint/api/run-advanced-query-sample-python?view=o365-worldwide)

The difference from the Hello World sample is that we will be using the Microsoft Graph APIs. The legacy APIs have not been [updated since 2022](https://learn.microsoft.com/en-us/defender-endpoint/api/api-release-notes). All new work is going into the [Microsoft Graph Security API](https://learn.microsoft.com/en-us/defender-endpoint/api/api-release-notes).

This means we'll need to give our app the correct permissions and we'll need a new SDK! We'll be using the [Microsoft Graph .NET Client Library](https://github.com/microsoftgraph/msgraph-sdk-dotnet)