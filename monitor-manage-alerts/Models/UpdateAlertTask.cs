/// <summary>
/// A single change we'd like to make to an Alert
/// </summary>
public record UpdateAlertTask
{
    /// <summary>
    /// Database identifier for this record
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Which alert would we like to change?
    /// </summary>
    public Alert Subject { get; init; } = new();

    /// <summary>
    /// What sort of change do we want to make in the system?
    /// </summary>
    public UpdateAction Action { get; init; }

    /// <summary>
    /// What do we want the new value to be?
    /// </summary>
    public string Payload { get; init; } = string.Empty;

    /// <summary>
    /// What is the current status of this update?
    /// </summary>
    public UpdateStatus Status { get; init; }
}

/// <summary>
/// The range of possible statuses that an update task can be in
/// </summary>
public enum UpdateStatus 
{
    // Newly entered, not yet seen by the system
    New = 0, 
    // Successfully sent to the system
    Sent, 
    // We have received and stored the updated value, and verified it to be correct
    Confirmed 
};

/// <summary>
/// The range of possible things we can do with an update task
/// </summary>
public enum UpdateAction { Invalid = 0, Comment, Status, Classification, Determination, AssignedTo }