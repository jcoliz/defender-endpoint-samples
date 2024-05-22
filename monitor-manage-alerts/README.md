# Monitor and Manage Alerts

This sample will demonstrate a ticketing system, which monitors Microsoft Defender for alerts, stores them in a local database, and
syncronizes changes back to Defender when they are made locally. This sample runs in a container, and works with a Postgres database
running in container.

## Initial configuration

Please see the [README](../README.md) at the top if this repository for necessary pre-requisites
and initial configuration before running the sample.

In particular, this sample requires these API permissions:

* Microsoft Graph: SecurityAlert.Read.All

## Running locally

Once you have `config.toml` set up, feel free to run the sample locally in this folder.

```powershell
dotnet run

info: monitor_manage_alerts.Worker[0]
      Starting
info: monitor_manage_alerts.Worker[0]
      Client OK
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: .\defender-endpoint-samples\monitor-manage-alerts
info: monitor_manage_alerts.Worker[0]
      Received 2 alerts
info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
info: monitor_manage_alerts.Worker[0]
      Cancelled
```

Right now, the sample connects to the Graph Security APIs, and fetches all the alerts
every 30 seconds.

## Building the container

From a terminal window in this folder:

```powershell
.scripts/Build-Container.ps1
```

## Running the container

Once the container is built, and you have `config.toml` set up:

```powershell
docker compose -f .docker/docker-compose.yaml up

[+] Running 1/0
 ✔ Container monitor  Recreated                                                                                    0.1s
Attaching to monitor
monitor  | info: monitor_manage_alerts.Worker[0]
monitor  |       Starting
monitor  | info: monitor_manage_alerts.Worker[0]
monitor  |       Client OK
monitor  | info: Microsoft.Hosting.Lifetime[0]
monitor  |       Application started. Press Ctrl+C to shut down.
monitor  | info: Microsoft.Hosting.Lifetime[0]
monitor  |       Hosting environment: Production
monitor  | info: Microsoft.Hosting.Lifetime[0]
monitor  |       Content root path: /app
monitor  | info: monitor_manage_alerts.Worker[0]
monitor  |       Received 2 alerts
```
