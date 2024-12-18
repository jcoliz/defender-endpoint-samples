# Hello World

This sample connects to the Defender for Endpoint APIs, then makes a simple request to gets recently-reported machines.

## Getting Started

Step one is to ensure you have an Entra ID app registration, and the necessary details from it in a `config.toml` file. This is described in the main page [README.md](../README.md).

Ensure your app registration has permission to the following `WindowsDefenderATP` APIs:

* `Machine.Read.All`

## Running the Sample

From a developer command prompt, simply run

```powershell
PS> dotnet run
```

If all is working well, you will see the following information:

### Configuration

As loaded from `config.toml`

```json
{
  "Identity": {
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "AppId": "00000000-0000-0000-0000-000000000000",
    "AppSecret": "---fill me in---"
  },
  "Login": {
    "Authority": "https://login.microsoftonline.com/",
    "Scopes": [
      "https://api.securitycenter.microsoft.com/.default"
    ]
  },
  "Resources": {
    "BaseUri": "https://api.securitycenter.microsoft.com/api/"
  }
}
```

### Authorization Token

As returned from `login.microsoftonline.com`. In particular, confirm that the `roles` section is as shown here.

```json
{
  "aud": "https://api.securitycenter.microsoft.com",
  "iss": "https://sts.windows.net/00000000-0000-0000-0000-000000000000/",
  "iat": 1714510676,
  "nbf": 1714510676,
  "exp": 1714514576,
  "aio": "E2NgYFedKTVnUe8ySqY60cef9BQA=",
  "app_displayname": "MDE-HelloWorld",
  "appid": "00000000-0000-0000-0000-000000000000",
  "appidacr": "1",
  "idp": "https://sts.windows.net/00000000-0000-0000-0000-000000000000/",
  "oid": "00000000-0000-0000-0000-000000000000",
  "rh": "0.AQcAFgzpbcuwWUeGs1YBrsSbUGUEePwXINRAoMUwcCJHG5L_AAA.",
  "roles": [
    "Machine.Read.All"
  ],
  "sub": "00000000-0000-0000-0000-000000000000",
  "tenant_region_scope": "NA",
  "tid": "00000000-0000-0000-0000-000000000000",
  "uti": "Xt8ehY95jjNdvnE3y5qSjodAA",
  "ver": "1.0"
}
```

### Machines list

As returned from [List machines API](https://learn.microsoft.com/en-us/defender-endpoint/api/get-machines).

```json
{
  "@odata.context": "https://api.securitycenter.microsoft.com/api/$metadata#Machines",
  "value": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "mergedIntoMachineId": null,
      "isPotentialDuplication": false,
      "isExcluded": false,
      "exclusionReason": null,
      "computerDnsName": "computerDnsName",
      "firstSeen": "2024-04-29T21:49:18.9544144Z",
      "lastSeen": "2024-04-30T15:37:40.8512254Z",
      "osPlatform": "Windows11",
      "osVersion": null,
      "osProcessor": "x64",
      "version": "23H2"
    }
  ]
}
```

## What's next?

Congratulations, you've now successfully connected to the Microsoft Defender for Endppoint service.
Now you're ready to call any of the [Supported Microsoft Defender for Endpoint APIs](https://learn.microsoft.com/en-us/defender-endpoint/api/exposed-apis-list).
Of course, you may need to grant additional permissions to your Entra ID app registration.
The required permission is described on the documentation page for each API.
