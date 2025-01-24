// Copyright (C) 2025 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

namespace MdEndpoint.Models;

public class DeviceEvent
{
    public DateTime? Timestamp { get; init; }
    public string? DeviceName { get; init; }
    public string? ActionType { get; init; }
    public Int64 ReportId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public TimeSpan Age => Timestamp.HasValue ? CreatedAt - Timestamp.Value : TimeSpan.Zero;

    public override bool Equals(object? obj) =>
        obj is DeviceEvent other
        && Timestamp.HasValue && DeviceName is not null
        && Timestamp.Equals(other.Timestamp) && DeviceName.Equals(other.DeviceName) && ReportId.Equals(other.ReportId);
    
    public override int GetHashCode() => HashCode.Combine(Timestamp, DeviceName, ReportId);

    public static DeviceEvent FromDictionary(IDictionary<string,object> dictionary)
    {
        return new DeviceEvent() 
        {
            Timestamp = dictionary["Timestamp"] as DateTime?,
            DeviceName = dictionary["DeviceName"] as string,
            ActionType = dictionary["ActionType"] as string,
            ReportId = (Int64)(decimal)dictionary["ReportId"]
        };
    }
}
