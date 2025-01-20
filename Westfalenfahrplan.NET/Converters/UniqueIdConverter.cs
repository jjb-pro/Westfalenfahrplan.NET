using Newtonsoft.Json;
using System;
using Westfalenfahrplan.NET.Model;

namespace Westfalenfahrplan.NET.Converters
{
    internal class UniqueIdConverter : JsonConverter<UniqueId>
    {
        public override UniqueId ReadJson(JsonReader reader, Type objectType, UniqueId existingValue, bool hasExistingValue, JsonSerializer serializer) => new UniqueId(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, UniqueId value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}