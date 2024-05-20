using System.Text.Json.Serialization;

/// <summary>
/// Detections and Hunting Queries
/// </summary>
/// <remarks>
/// "This document aims to create a uniform style for Microsoft Sentinel and 
/// Microsoft 365 Defender content provided to and by Microsoft. We encourage 
/// external contributors to follow this same guidance, but this is not 
/// enforced. Microsoft will review and update any query that is pulled into 
/// the Microsoft Sentinel UX with the requirements below as needed. Queries
/// for Microsoft 365 Defender will flow into both Microsoft Sentinel and 
/// Microsoft 365 Defender."
/// </remarks>
/// <see href="https://github.com/Azure/Azure-Sentinel/wiki/Query-Style-Guide" /> 
public record HuntingQuery
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    /// <remarks>
    /// This is a standard GUID. You can generate from just about any 
    /// development tool, online GUID generator, or from PowerShell via the 
    /// New-GUID cmdlet.
    /// </remarks>
    public Guid Id { get; init; }
    public QueryKind? Kind { get; init; }
    public QuerySeverity? Severity { get; init; }
    /// <summary>
    /// A short name of the detection in the form of a label 
    /// </summary>
    /// <remarks>
    /// This should include what the detection is about without reading the 
    /// full description. Note that you can use alertDetailsOverride to 
    /// provide a dynamic name that will make it easier for analysts to 
    /// understand the alert.
    /// </remarks>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Details the purpose of the query
    /// </summary>
    /// <remarks>
    /// Include and any references such as EventID explanations or URL 
    /// references. Note that you can use alertDetailsOverride to provide a 
    /// dynamic description that will make it easier for analysts to 
    /// understand the alert.
    /// </remarks>
    public string Description { get; init; } = string.Empty;
    public string[] Tactics { get; init; } = [];
    public string[] RelevantTechniques { get; init; } = [];
    public Connector[] RequiredDataConnectors { get; init; } = [];

    /// <summary>
    /// Query in Kusto Query Language (KQL)
    /// </summary>
    /// <remarks>
    /// This is the query that will run every "QueryFrequency" time, and trigger an 
    /// alert if the number of results from the query meets the condition 
    /// defined in "triggerThreshold" and "triggerOperator".
    /// </remarks>
    /// <see href="https://github.com/Azure/Azure-Sentinel/wiki/Query-Style-Guide#query"/>
    public string Query { get; init; } = string.Empty;
}

public enum QueryKind { Invalid = 0, Scheduled, NRT }

public enum QuerySeverity { Invalid = 0, Informational, Low, Medium, High }

public record Connector
{
    public string ConnectorId { get; init; } = string.Empty;
    public string[] DataTypes { get; init; } = [];
}
