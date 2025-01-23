using Newtonsoft.Json;
using System;
using System.Globalization;
using Westfalenfahrplan.NET.Model;

namespace Westfalenfahrplan.NET.Converters
{
    internal class DateRangeConverter : JsonConverter<DateRange>
    {
        private const string DateFormat = "dd/MM/yyyy HH:mm";

        public override void WriteJson(JsonWriter writer, DateRange value, JsonSerializer serializer) => throw new NotImplementedException();

        public override DateRange ReadJson(JsonReader reader, Type objectType, DateRange existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dateString = reader.Value.ToString().Split('-');
            return new DateRange(DateTime.ParseExact(dateString[0].Trim(), DateFormat, CultureInfo.InvariantCulture), DateTime.ParseExact(dateString[1].Trim(), DateFormat, CultureInfo.InvariantCulture));
        }
    }
}