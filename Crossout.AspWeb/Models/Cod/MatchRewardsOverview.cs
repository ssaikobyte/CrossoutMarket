using Crossout.AspWeb.Models.View;
using Crossout.Model.Formatter;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class MatchRewardsOverview : BaseViewModel, IViewTitle
    {
        public string Title => "Match Rewards";

        public List<MissionPoco> Missions { get; set; }

        public List<string> Resources { get => Missions.Select(x => x.Resource).Distinct().ToList(); }

        public List<string> MatchTypes { get => Missions.Select(x => x.MatchType).Distinct().ToList(); }
    }

    public class MatchRewardsOverviewData
    {
        [JsonProperty("data")]
        public List<MatchReward> MatchRewards { get; set; }

        [JsonProperty("resources")]
        public List<ResourceBundle> ResourceBundles { get; set; }
    }

    public class MatchReward
    {
        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonProperty("missionName")]
        public string MissionName { get; set; }

        [Column("resource")]
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [Column("match_type")]
        [JsonProperty("matchType")]
        public string MatchType { get; set; }

        [Column("datacount")]
        [JsonProperty("dataCount")]
        public int DataCount { get; set; }

        [Column("averageresources")]
        [JsonProperty("averageResources")]
        public decimal AverageResources { get; set; }

        [Column("averagebaseexp")]
        [JsonProperty("averageBaseExp")]
        public decimal AverageBaseExp { get; set; }

        [Ignore]
        [JsonProperty("formatAverageBaseExp")]
        public string FormatAverageBaseExp { get => AverageBaseExp.ToString("0.00", CultureInfo.InvariantCulture); }

        [Ignore]
        [JsonProperty("formatAverageResources")]
        public string FormatAverageResources { get => AverageResources.ToString("0.00", CultureInfo.InvariantCulture); }
    }

    public class ResourceBundle
    {
        [Column("id")]
        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [Column("resourcekey")]
        [JsonProperty("resourceKey")]
        public string ResourceKey { get; set; }

        [Column("localizedname")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Column("name")]
        [JsonProperty("resourceName")]
        public string ResourceName { get; set; }

        [Column("amount")]
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [Column("sellprice")]
        [JsonProperty("sellPrice")]
        public int SellPrice { get; set; }

        [Column("buyprice")]
        [JsonProperty("buyPrice")]
        public int BuyPrice { get; set; }

        [Ignore]
        [JsonProperty("formatSellPrice")]
        public string FormatSellPrice { get => PriceFormatter.FormatPrice(SellPrice); }

        [Ignore]
        [JsonProperty("formatBuyPrice")]
        public string FormatBuyPrice { get => PriceFormatter.FormatPrice(BuyPrice); }

        [Ignore]
        [JsonProperty("sellPricePerUnit")]
        public decimal SellPricePerUnit { get => (SellPrice / 100m) / Amount; }

        [Ignore]
        [JsonProperty("buyPricePerUnit")]
        public decimal BuyPricePerUnit { get => (BuyPrice / 100m) / Amount; }
    }
}
