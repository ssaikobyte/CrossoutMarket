using Crossout.AspWeb.Models.View;
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

        public List<string> Resources { get; set; }

        public List<string> MatchTypes { get; set; }
    }

    public class MatchRewardsOverviewData
    {
        [JsonProperty("data")]
        public List<MatchReward> MatchRewards { get; set; }
    }

    public class MatchReward
    {
        [Column("resource")]
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [Column("match_type")]
        [JsonProperty("matchType")]
        public string MatchType { get; set; }

        [Column("datacount")]
        [JsonProperty("dataCount")]
        public int DataCount { get; set; }

        [Column("averageamount")]
        [JsonProperty("averageAmount")]
        public decimal AverageAmount { get; set; }

        [Ignore]
        [JsonProperty("formatAverageAmount")]
        public string FormatAverageAmount { get => AverageAmount.ToString("0.00", CultureInfo.InvariantCulture); }
    }
}
