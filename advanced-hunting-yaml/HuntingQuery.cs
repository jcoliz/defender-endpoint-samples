using System.Text.Json.Serialization;

public record HuntingQuery
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string[] Tactics { get; init; } = [];
    public string[] RelevantTechniques { get; init; } = [];
    public Connector[] RequiredDataConnectors { get; init; } = [];
    public string Query { get; init; } = string.Empty;
}

public record Connector
{
    public string ConnectorId { get; init; } = string.Empty;
    public string[] DataTypes { get; init; } = [];
}
