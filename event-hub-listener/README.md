# Connect an Event Hub to Streaming API

## Prerequisites

* Microsoft Defender for Endpoint instance, with at least one connected machine. Check out the [Microsoft Defender for Endpoint P2 Trial](https://aka.ms/MDEp2OpenTrial), if you need one.
* .NET 8.0 SDK. Download a copy at the [Download .NET](https://dotnet.microsoft.com/en-us/download) page.
* Azure CLI tools with the Bicep extension. See [Azure CLI with Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli)

## Overview

Here are the steps we'll go through:

1. Deploy an Event Hub Namespace
1. Configure Defender to export events to this namespace
1. Configure this sample with Event Hub connection details
1. Run the sample!
1. Dig deeper into the sample

## Deploy an Event Hub Namespace

First, we need an Event Hub. This needs to be in the same tenant which hosts the Defender instance.

You can use my AzDeploy.Bicep templates. This:

1. Creates an Event Hub namespace `ehubns-{unique}`
1. Creates a single hub under it `ehub`
1. Creates listening and sending keys, so we can use connection strings with least permissions. Here we care about `ListenKey`.

Here's how we deploy it:

```PowerShell
git clone https://github.com/jcoliz/AzDeploy.Bicep.git
cd AzDeploy.Bicep\EventHub

$env:RESOURCEGROUP = "pick-a-group-name"
az group create -n $env:RESOURCEGROUP -l "West US 2"
az deployment group create --name "Deploy-$(Get-Random)" --resource-group $env:RESOURCEGROUP --template-file .\ehub.bicep
```

Have a look at the output for the `outputs.id.value`. This is the ID you'll need to give Defender.

```json
    "outputs": {
      "hub": {
        "type": "String",
        "value": "ehub"
      },
      "id": {
        "type": "String",
        "value": "/subscriptions/{something}/resourceGroups/{your-group}/providers/Microsoft.EventHub/namespaces/ehubns-{unique}"
      },
      "namespace": {
        "type": "String",
        "value": "ehubns-{unique}"
      },
    }
```

Later, when we're done experimenting, be sure to tear down the Event Hub namespace.

```Powershell
az group delete -y -g $env:RESOURCEGROUP
```

## Configure Defender to export events to this namespace

TODO!

## Configure the sample

Now we need the connection string for the `ListenKey`. You can run this snippet, being sure to set `$env:EHUBNS` to your unique namespace name.

```Powershell
$env:EHUBNS = "ehubns-{unique}"
az eventhubs namespace authorization-rule keys list --resource-group $env:RESOURCEGROUP --namespace-name $env:EHUBNS --name ListenKey
```

Returns something like this:

```json
{
  "keyName": "ListenKey",
  "primaryConnectionString": "Endpoint=sb://ehubns-{unique}.servicebus.windows.net/;SharedAccessKeyName=ListenKey;SharedAccessKey={something}",
  "primaryKey": "{something}",
  "secondaryConnectionString": "Endpoint=sb://ehubns-{unique}.servicebus.windows.net/;SharedAccessKeyName=ListenKey;SharedAccessKey={something}",
  "secondaryKey": "{something}"
}
```

Copy the file `config.sample.toml` to `config.toml`, then enter your `primaryConnectionString` from above into the `ConnectionString` below.

```toml
[EventHub]
ConnectionString = "--fill me in--"
HubName = "ehub"
```

## Run the sample

With evertthing connected, and the `config.toml` set up correctly, you can now run the sample to watch the events flow in. Press Ctrl-C when you're done.

```dotnetcli
dotnet run
```

## Dig deeper

The sample uses the `Azure.Messaging.EventHubs` library, and itself is a copy/paste of the [README](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/README.md#read-events-from-an-event-hub) of that project.

I'll restate the note from that project: This approach to consuming is intended to improve the experience of exploring the Event Hubs client library and prototyping. It is recommended that it not be used in production scenarios. For production use, we recommend using the [Event Processor Client](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs.Processor), as it provides a more robust and performant experience.

```c#
string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
await using var consumer = new EventHubConsumerClient(consumerGroup, ehubOptions.Value!.ConnectionString, ehubOptions.Value!.HubName);

await foreach (PartitionEvent receivedEvent in consumer.ReadEventsAsync(stoppingToken))
{
    var raw = receivedEvent.Data.EventBody.ToString();
    using var jDoc = JsonDocument.Parse(raw);

    logger.LogInformation("Events: {events}", JsonSerializer.Serialize(jDoc, _jsonoptions));
}
```
