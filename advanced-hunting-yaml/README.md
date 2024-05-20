# Explore Community-Supported Advanced Hunting Queries

![Community Queries](..\docs\images\community-queries.png)

The Microsoft Defender portal surfaces several community-supplied threat hunting queries. We can use these same queries to
hunt using the API! See [Microsoft Defender Hunting Quieries from Azure Sentinel](https://github.com/Azure/Azure-Sentinel/tree/master/Hunting%20Queries/Microsoft%20365%20Defender).

This sample will load any YAML query definition from that repository, then execute the query via the [Microsoft Security Graph API](https://learn.microsoft.com/en-us/graph/security-concept-overview).