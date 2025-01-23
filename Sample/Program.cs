using Westfalenfahrplan.NET;

// initialize the API client
using var api = new WestfalenfahrplanClient();

// search for the stop "Bielefeld Jahnplatz" and get its details
var stop = (await api.SearchAsync("Bielefeld, Jahnplatz")).Locations.First();

// fetch real-time location information and serving lines for the stop
var realtimeData = await api.GetRealtimeLocationInfoAsync(stop.Id);
var servingLines = (await api.GetServingLinesAsync(stop.Id)).Lines;

// display the stop's name and serving lines
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"Departures for: {stop.Name}");
Console.WriteLine($"Serving lines: {string.Join(", ", servingLines.Select(line => line.Number))}");
Console.ResetColor();

// display departure information
foreach (var stopEvent in realtimeData.StopEvents)
{
    // display basic departure details
    var transportation = stopEvent.Transportation;
    var departureTime = stopEvent.DepartureTimeBaseTimetable.ToLocalTime().ToShortTimeString();
    var estimatedTime = stopEvent.IsRealtimeControlled
        ? stopEvent.DepartureTimeEstimated.ToLocalTime().ToShortTimeString()
        : "(no real-time information)";

    Console.WriteLine($"{transportation.Number} -> {transportation.Destination.Name}: {departureTime} {estimatedTime}");

    // display additional information, if available
    Console.ForegroundColor = ConsoleColor.Red;
    foreach (var info in stopEvent.Infos)
    {
        foreach (var link in info.InfoLinks)
            Console.WriteLine($"Additional info: {link.Title}");
    }

    Console.ResetColor();
}