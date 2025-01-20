![Westfalenfahrplan.NET icon](https://github.com/jjb-pro/Westfalenfahrplan.NET/blob/main/Westfalenfahrplan.NET/assets/icon.png)  
# Westfalenfahrplan.NET

[![NuGet](https://img.shields.io/nuget/v/Westfalenfahrplan.NET.svg)](https://www.nuget.org/packages/Westfalenfahrplan.NET)

Westfalenfahrplan.NET is a .NET library for accessing real-time public transportation data in the Westfalen region of Germany.

## Features

- **Search Locations**: Find public transport stops, point of interests and more by name.  
- **Real-Time Data**: Retrieve real-time departure information for a specific stop.
- **Serving Lines**: Get all lines serving a particular location.

## Usage

To use Westfalenfahrplan.NET, follow these steps:

1. **Install via NuGet**: Add the package to your project using the .NET CLI:
   ```bash
   dotnet add package Westfalenfahrplan.NET
   ```
   Or, search for `Westfalenfahrplan.NET` in the NuGet Package Manager in Visual Studio.
2. **Use the package**: Use the `WestfalenfahrplanClient` class to create your own application.

## Example

A simple example that shows how to retrieve and display real-time departure information for a specific location: 

```csharp  
using Westfalenfahrplan.NET;  

using var api = new WestfalenfahrplanClient();  

var stop = (await api.SearchAsync("Bielefeld Jahnplatz")).Locations.FirstOrDefault();
var realtimeData = await api.GetRealtimeLocationInfoAsync(stop.Id);  

foreach (var stopEvent in realtimeData.StopEvents)  
   Console.WriteLine($"{stopEvent.Transportation.Number} -> {stopEvent.Transportation.Destination.Name}: {stopEvent.DepartureTimeBaseTimetable.ToLocalTime():t}");
```

This example demonstrates:  
- Searching for a stop by name.  
- Retrieving real-time departure data for the stop.  
- Displaying the departure times and destinations.  

The sample can also be found in the repository under **Sample**. 

## Development

### Feature requests are welcome!

If you have specific feature requests or need additional methods, feel free to open an issue.