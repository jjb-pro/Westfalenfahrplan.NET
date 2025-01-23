using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Westfalenfahrplan.NET.Model;

namespace Westfalenfahrplan.NET.Converters
{
    internal class RealtimeStatusConverter : JsonConverter<RealtimeStatus>
    {
        public override void WriteJson(JsonWriter writer, RealtimeStatus value, JsonSerializer serializer) => throw new NotImplementedException();

        public override RealtimeStatus ReadJson(JsonReader reader, Type objectType, RealtimeStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = 0;
            var array = JArray.Load(reader);

            foreach (var item in array)
            {
                switch (item.Value<string>())
                {
                    case "MONITORED":
                        result |= (int)RealtimeStatus.Monitored;
                        break;
                    case "EXTRA_TRIP":
                        result |= (int)RealtimeStatus.ExtraTrip;
                        break;
                    case "TRIP_CANCELLED":
                        result |= (int)RealtimeStatus.TripCancelled;
                        break;
                    default:
                        result |= (int)RealtimeStatus.Other;
                        break;
                }
            }

            return (RealtimeStatus)result;
        }
    }
}