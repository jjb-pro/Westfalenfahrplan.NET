#pragma warning disable CS1591

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Westfalenfahrplan.NET.Model
{
    public class ServerInfo
    {
        public string ControllerVersion { get; set; }
        public string ServerID { get; set; }
        public string VirtDir { get; set; }
        public DateTime ServerTime { get; set; }
        public double CalcTime { get; set; }
        public string LogRequestId { get; set; }
    }

    public class SystemMessage
    {
        public string Type { get; set; }
        public string Module { get; set; }
        public int Code { get; set; }
        public string Text { get; set; }
    }

    public class Coordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class UniqueId
    {
        public string Type { get; }
        public uint LocationId { get; }
        public uint ObjectId { get; }

        private readonly string originalString;

        public UniqueId(string id)
        {
            originalString = id;

            var parts = id.Split(':', '|');
            if (parts.Length < 3)
                return;

            Type = parts[0];
            if (uint.TryParse(parts[1], out var value))
                LocationId = value;

            if (uint.TryParse(parts[2], out value))
                ObjectId = value;
        }

        internal string GetIdString() => originalString;
    }

    public class Location
    {
        public UniqueId Id { get; set; }
        public bool IsGlobalId { get; set; }
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        [JsonProperty("coord")]
        public Coordinate Coordinate { get; set; }
        public LocationType Type { get; set; }
        public int MatchQuality { get; set; }
        public bool IsBest { get; set; }
        public Parent Parent { get; set; }
        public List<AssignedStop> AssignedStops { get; set; }
        public Properties Properties { get; set; }
        public List<int> ProductClasses { get; } = new List<int>();
    }

    public class TimedLocation : Location
    {
        public DateTime ArrivalTimePlanned { get; set; }
        public DateTime DepartureTimePlanned { get; set; }
        public DateTime ArrivalTimeEstimated { get; set; }
        public DateTime DepartureTimeEstimated { get; set; }
    }

    public enum LocationType
    {
        Default,
        Stop,
        Suburb,
        Street,
        Platform,
        SingleHouse,
        [EnumMember(Value = "poi")]
        PointOfInterest
    }

    public class Parent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class AssignedStop
    {
        public string Id { get; set; }
        public bool IsGlobalId { get; set; }
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        public string Type { get; set; }
        [JsonProperty("coord")]
        public Coordinate Coordinate { get; set; }
        public Parent AssignedStopParent { get; set; }
        public List<int> ProductClasses { get; set; }
        public int ConnectingMode { get; set; }
        public Properties AssignedStopProperties { get; set; }
    }

    public class Properties
    {
        public string StopId { get; set; }
        public string Area { get; set; }
        public string Platform { get; set; }
        public string PlatformName { get; set; }
        public string PlannedPlatformName { get; set; }
        public string Zone { get; set; }
        public Occupancy Occupancy { get; set; }
    }

    public enum Occupancy
    {
        Unknown,
        [EnumMember(Value = "MANY_SEATS")]
        ManySeats,
        [EnumMember(Value = "FEW_SEATS")]
        FewSeats,
        [EnumMember(Value = "STANDING_ONLY")]
        StandingOnly,
        [EnumMember(Value = "FULL")]
        Full
    }

    public class StopEvent
    {
        public RealtimeStatus RealtimeStatus { get; set; }
        public bool IsRealtimeControlled { get; set; }
        public Location Location { get; set; }
        public DateTime DepartureTimePlanned { get; set; }
        public DateTime DepartureTimeBaseTimetable { get; set; }
        public DateTime DepartureTimeEstimated { get; set; }
        public Transportation Transportation { get; set; }
        public List<TimedLocation> PreviousLocations { get; } = new List<TimedLocation>();
        public List<TimedLocation> OnwardLocations { get; } = new List<TimedLocation>();
        public List<StopEventInfo> Infos { get; } = new List<StopEventInfo>();
        public StopEventProperties Properties { get; set; }
    }

    [Flags]
    public enum RealtimeStatus
    {
        None = 1 << 0,
        Monitored = 1 << 1,
        ExtraTrip = 1 << 2,
        TripCancelled = 1 << 3,
        Other = 1 << 4
    }

    public class StopEventProperties
    {
        public string AVMSTripID { get; set; }
    }

    public class StopEventInfo
    {
        public string Priority { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
        public List<InfoLink> InfoLinks { get; } = new List<InfoLink>();
    }

    public class InfoLink
    {
        public string UrlText { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public string Subtitle { get; set; }
        public string Title { get; set; }
        public string WapText { get; set; }
        public string SmsText { get; set; }
        public InfoProperties Properties { get; set; }
    }

    public class InfoProperties
    {
        public string Publisher { get; set; }
        public string InfoType { get; set; }
        public DateRange IncidentDateTime { get; set; }
    }

    public class DateRange
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public class Transportation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        public string Number { get; set; }
        public Product Product { get; set; }
        public Operator Operator { get; set; }
        public Destination Destination { get; set; }
        public Properties TransportationProperties { get; set; }
        public Origin Origin { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int Class { get; set; }
        public string Name { get; set; }
        public int IconId { get; set; }
    }

    public class Operator
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Destination
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Origin
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class APIResult
    {
        public ServerInfo ServerInfo { get; set; }
        public string Version { get; set; }
        public List<SystemMessage> SystemMessages { get; set; }
    }

    public class RealtimeLocationInfoResult : APIResult
    {
        public List<Location> Locations { get; set; }
        public List<StopEvent> StopEvents { get; } = new List<StopEvent>();
    }

    public class StopProperties
    {
        public string StopId { get; set; }
        public string MainLocality { get; set; }
    }

    public class StopSearchResult : APIResult
    {
        public List<Location> Locations { get; } = new List<Location>();
    }

    public class ServingLinesResult : APIResult
    {
        public List<Line> Lines { get; set; }
    }

    public class Line
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisassembledName { get; set; }
        public string Number { get; set; }
        public Product Product { get; set; }
        public Operator Operator { get; set; }
        public Destination Destination { get; set; }
        public Properties Properties { get; set; }
    }

    public class Validity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
#pragma warning restore CS1591