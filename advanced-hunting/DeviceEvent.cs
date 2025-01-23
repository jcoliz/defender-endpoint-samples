// Copyright (C) 2025 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

namespace MdEndpoint.Models;

public record DeviceEvent
{
    public DateTime? Timestamp { get; init; }
    public string? DeviceName { get; init; }
    public string? ActionType { get; init; }
    public Int64 ReportId { get; init; }

    public static DeviceEvent FromDictionary(IDictionary<string,object> dictionary)
    {
        return new DeviceEvent() 
        {
            Timestamp = dictionary["Timestamp"] as DateTime?,
            DeviceName = dictionary["DeviceName"] as string,
            ActionType = dictionary["ActionType"] as string,
            ReportId = Int64.Parse( dictionary["ReportId"] as string ?? "0" )
        };
    }
}
