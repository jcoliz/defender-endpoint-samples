using AutoMapper;

namespace MdeSamples.Models;

/// <summary>
/// Our local copy of an alert
/// </summary>
/// <remarks>
/// Contains a reduced set of information about the alert. Just the minimum 
/// we need for our ticketing system.
/// </remarks>
public record Alert
{
    /// <summary>
    /// The local database ID for this object
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The ID supplied by the Defender service
    /// </summary>
    public string AlertId { get; init; } = string.Empty;
    public string? Category { get; init; }
    public string? AlertDetermination { get; init; }
    public string Severity { get; init; } = string.Empty;
    public DateTimeOffset? CreatedDateTime { get; init; }
    public List<AlertComment> Comments { get; init; } = [];
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Classification { get; init; } = string.Empty;
    public string? AssignedTo { get; init; }
    public string Determination { get; init; } = string.Empty;
}

/// <summary>
/// Our local copy of an alert comment
/// </summary>
public record AlertComment
{
    public int Id { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTimeOffset? CreatedDateTime { get; init; }
    public string CreatedByDisplayName { get; init; } = string.Empty;
}

/// <summary>
/// Automapper profile to enable mapping from defender alerts to our alerts
/// </summary>
public class AlertMapperProfile : Profile 
{
    public AlertMapperProfile()
    {
        CreateMap<Microsoft.Graph.Models.Security.AlertComment, AlertComment>();
        CreateMap<Microsoft.Graph.Models.Security.Alert, Alert>()
            .ForMember(dest => dest.AlertId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForSourceMember(x=>x.Id,opt=>opt.DoNotValidate());
    }
}
