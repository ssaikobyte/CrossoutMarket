using Crossout.WorkerCore.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.WorkerCore.Models.SteamAPI
{
    public class PriceOverview
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("initial")]
        public int Initial { get; set; }

        [JsonProperty("final")]
        public int Final { get; set; }

        [JsonProperty("discount_percent")]
        public int DiscountPercent { get; set; }

        [JsonProperty("initial_formatted")]
        public string InitialFormatted { get; set; }

        [JsonProperty("final_formatted")]
        public string FinalFormatted { get; set; }
    }

    public class Data
    {
        [JsonProperty("price_overview")]
        public PriceOverview PriceOverview { get; set; }
    }

    public class Response
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        [JsonConverter(typeof(SteamJsonConverter<Data>))]
        public Data Data { get; set; }
    }

    public class AppPrices
    {
        public Dictionary<string, Response> Apps { get; set; }
    }
}
