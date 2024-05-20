# Microsoft Defender for Endpoint API Samples

[![Build+Test](https://github.com/jcoliz/defender-endpoint-samples/actions/workflows/build.yml/badge.svg)](https://github.com/jcoliz/defender-endpoint-samples/actions/workflows/build.yml)

This project showcases samples for: 
* [Microsoft Defender for Endpoint APIs](https://learn.microsoft.com/en-us/defender-endpoint/api/management-apis), and
* [Microsoft Security Graph API](https://learn.microsoft.com/en-us/graph/security-concept-overview) used for Defender for Endpoint.

## What's Here

* [Hello World](./hello-world/): Basic example to ensure your connection works, and you can download some basic information from the service.
* [Start Advanced Hunting](./advanced-hunting/): Simple example of running an advanced hunting query against the Security Graph APIs.
* [Explore Advanced Hunting Queries](./advanced-hunting-yaml/): The Microsoft Defender portal surfaces several community-supplied threat hunting queries. We can use these same queries to hunt using the API!

## Getting Started

All of the samples use the same configuration mechanism, described in the below steps.

### Pre-requisites

* Microsoft Defender for Endpoint instance, with at least one connected machine. Check out the [Microsoft Defender for Endpoint P2 Trial](https://aka.ms/MDEp2OpenTrial), if you need one.
* .NET 8.0 SDK. Download a copy at the [Download .NET](https://dotnet.microsoft.com/en-us/download) page.

### Create an App Registration in Entra ID

Follow along with the guide at [Create an app to access Microsoft Defender for Endpoint without a user](https://learn.microsoft.com/en-us/defender-endpoint/api/exposed-apis-create-app-webapp). You'll need these pieces of information to continue:

* Application (client) ID
* Directory (tenant) ID
* Client secret

### Create a `config.toml` file for your configuration

Each sample requires your app registration details in a `config.toml` file. Copy the `config.sample.toml` file from that folder, rename to `config.toml`, and fill out the details:

```toml
[App.Identity]
TenantId = "00000000-0000-0000-0000-000000000000" # Directory (tenant) ID
AppId = "00000000-0000-0000-0000-000000000000" # Application (client) ID
AppSecret = "--fill me in--" # Client secret value
```

### Set the necessary permissions for the application

Each sample uses slightly different APIs, so each will need permissions specific to that sample.
Please see the individual samples for details on which API permissions are needed.
