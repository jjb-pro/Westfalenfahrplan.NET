using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Westfalenfahrplan.NET.Converters;
using Westfalenfahrplan.NET.Helper;
using Westfalenfahrplan.NET.Model;

namespace Westfalenfahrplan.NET
{
    public class WestfalenfahrplanClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings() { Converters = new List<JsonConverter>() { new CoordinateConverter(), new UniqueIdConverter(), new DateRangeConverter(), new RealtimeStatusConverter() } };

        private const string BaseUrl = "https://westfalenfahrplan.de/nwl-efa/";

        private const string SearchUrl = "XML_STOPFINDER_REQUEST?coordOutputFormat=WGS84[dd.ddddd]&language=de&locationInfoActive=1&locationServerActive=1&nwlStopFinderMacro=1&outputFormat=rapidJSON&serverInfo=1&sl3plusStopFinderMacro=dm&type_sf=any&version=10.6.14.22";
        private const string RealtimeLocationInfoUrl = "XML_DM_REQUEST?coordOutputFormat=WGS84[dd.ddddd]&deleteAssignedStops_dm=1&depSequence=30&depType=stopEvents&doNotSearchForStops=1&genMaps=0&imparedOptionsActive=1&inclMOT_1=true&inclMOT_10=true&inclMOT_11=true&inclMOT_13=true&inclMOT_14=true&inclMOT_15=true&inclMOT_16=true&inclMOT_17=true&inclMOT_18=true&inclMOT_19=true&inclMOT_2=true&inclMOT_3=true&inclMOT_4=true&inclMOT_5=true&inclMOT_6=true&inclMOT_7=true&inclMOT_8=true&inclMOT_9=true&includeCompleteStopSeq=1&includedMeans=checkbox&itOptionsActive=1&language=de&locationServerActive=1&maxTimeLoop=1&mode=direct&nwlDMMacro=1&outputFormat=rapidJSON&ptOptionsActive=1&serverInfo=1&sl3plusDMMacro=1&type_dm=any&useAllStops=1&useProxFootSearch=0&useRealtime=1&version=10.6.14.22";
        private const string ServingLinesUrl = "XML_SERVINGLINES_REQUEST?deleteAssignedStops_sl=1&language=de&lineReqType=1&lsShowTrainsExplicit=1&mergeDir=true&mode=odv&outputFormat=rapidJSON&serverInfo=1&sl3plusServingLinesMacro=1&type_sl=stop&version=10.6.14.22&withoutTrains=0";

        public WestfalenfahrplanClient()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        }

        /// <summary>
        /// Searches for locations by name.
        /// </summary>
        /// <param name="name">The name of the locations to search for.</param>
        /// <returns>A list of search results.</returns>
        public Task<StopSearchResult> SearchAsync(string name, CancellationToken cancellationToken = default)
        {
            var url = new UrlBuilder(BaseUrl, true)
                .AddPathSegment(SearchUrl)
                .AddQueryParameter("name_sf", name)
                .Build();

            return FetchDataAsync<StopSearchResult>(url, cancellationToken);
        }

        /// <summary>
        /// Searches for locations by name.
        /// </summary>
        /// <param name="name">The name of the locations to search for.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        public Task<RealtimeLocationInfoResult> GetRealtimeLocationInfoAsync(UniqueId locationId, CancellationToken cancellationToken = default)
        {
            var url = new UrlBuilder(BaseUrl, true)
                .AddPathSegment(RealtimeLocationInfoUrl)
                .AddQueryParameter("name_dm", locationId.GetIdString())
                .Build();

            return FetchDataAsync<RealtimeLocationInfoResult>(url, cancellationToken);
        }

        /// <summary>
        /// Retrieves real-time departures for a specified location at a specified time.
        /// </summary>
        /// <param name="stopId">The ID of the location.</param>
        /// <param name="dateTime">The date and time for which to retrieve departures.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        public Task<RealtimeLocationInfoResult> GetRealtimeLocationInfoAsync(UniqueId locationId, DateTime dateTime, CancellationToken cancellationToken = default)
        {
            var url = new UrlBuilder(BaseUrl, true)
                .AddPathSegment(RealtimeLocationInfoUrl)
                .AddQueryParameter("name_dm", locationId.GetIdString())
                .AddQueryParameter("itdDateDayMonthYear", dateTime.ToString("dd.MM.yyyy"))
                .AddQueryParameter("itdDateTimeDepArr", "dep")
                .AddQueryParameter("itdTime", dateTime.ToString("HH:mm"))
                .Build();

            return FetchDataAsync<RealtimeLocationInfoResult>(url, cancellationToken);
        }

        /// <summary>
        /// Retrieves the serving lines for a specific location.
        /// </summary>
        /// <param name="locationId">The ID of the location.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        public Task<ServingLinesResult> GetServingLinesAsync(UniqueId locationId, CancellationToken cancellationToken = default)
        {
            var url = new UrlBuilder(BaseUrl, true)
                .AddPathSegment(ServingLinesUrl)
                .AddQueryParameter("name_sl", locationId.GetIdString())
                .Build();

            return FetchDataAsync<ServingLinesResult>(url, cancellationToken);
        }

        private async Task<T> FetchDataAsync<T>(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var decompressedStream = stream;

            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                decompressedStream = new GZipStream(stream, mode: CompressionMode.Decompress);
            }
            else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
            {
                decompressedStream = new DeflateStream(stream, mode: CompressionMode.Decompress);
            }

            using (var reader = new StreamReader(decompressedStream))
            {
                var decompressedJson = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(decompressedJson, _jsonSettings);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _httpClient.Dispose();
        }
    }
}