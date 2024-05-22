# Monitor and Manage Alerts

This sample will demonstrate a ticketing system, which monitors Microsoft Defender for alerts, stores them in a local database, and
syncronizes changes back to Defender when they are made locally. This sample runs in a container, and works with a Postgres database
running in container.

## Building the container

From a terminal window in this folder:

```powershell
.scripts/Build-Container.ps1
```

## Running the container

Once it's built:

```powershell
docker run monitor-manage-alerts:local

info: monitor_manage_alerts.Worker[0]
      Worker running at: 05/22/2024 16:17:39 +00:00
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app
info: monitor_manage_alerts.Worker[0]
      Worker running at: 05/22/2024 16:17:40 +00:00
info: monitor_manage_alerts.Worker[0]
      Worker running at: 05/22/2024 16:17:41 +00:00
```

It doesn't do much just yet, but we're just getting started!!
