using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Westfalenfahrplan.NET.Model;

namespace Westfalenfahrplan.NET.Converters
{
    internal class CoordinateConverter : JsonConverter<Coordinate>
    {
        public override Coordinate ReadJson(JsonReader reader, Type objectType, Coordinate existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return new Coordinate(array[0].Value<double>(), array[1].Value<double>());
        }

        public override void WriteJson(JsonWriter writer, Coordinate value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}