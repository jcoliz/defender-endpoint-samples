# Explore Community-Supported Advanced Hunting Queries

![Community Queries](..\docs\images\community-queries.png)

The Microsoft Defender portal surfaces several community-supplied threat hunting queries. We can use these same queries to
hunt using the API! See [Microsoft Defender Hunting Quieries from Azure Sentinel](https://github.com/Azure/Azure-Sentinel/tree/master/Hunting%20Queries/Microsoft%20365%20Defender).

This sample will load any YAML query definition from that repository, then execute the query via the [Microsoft Security Graph API](https://learn.microsoft.com/en-us/graph/security-concept-overview).

## Initial configuration

Please see the [README](../README.md) at the top if this repository for necessary pre-requisites
and initial configuration before running the sample.

In particular, this sample requires these API permissions:

* Microsoft Graph: ThreatHunting.Read.All

## Running the sample

From a terminal window in the `advanced-hunting-yaml` folder, first build the sample:

```dotnetcli
dotnet build
```

Then run the sample using one of the supplied queries. Here are some to get you started!

```dotnetcli
dotnet run -- '.\queries\Troubleshooting\Connectivity Failures by Device.yaml'
dotnet run -- '.\queries\Network\Defender for Endpoint Telemetry.yaml'
dotnet run -- '.\queries\Device Inventory\Most Common Services.yaml'
```
