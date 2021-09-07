using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class MatchHistory : BaseViewModel, IViewTitle
    {
        public string Title => "Match History";
    }

    public class MatchHistoryData
    {
        [JsonProperty("data")]
        public List<MatchHistoryEntry> Data { get; set; } = new List<MatchHistoryEntry>();
    }

    public class MatchHistoryEntry
    {
        [JsonProperty("matchId")]
        public long MatchId { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        [JsonProperty("matchType")]
        public string MatchType { get; set; }

        [JsonProperty("mapName")]
        public string MapName { get; set; }

        [JsonProperty("minPowerScore")]
        public int MinPowerScore { get; set; }

        [JsonProperty("maxPowerScore")]
        public int MaxPowerScore { get; set; }

        [JsonProperty("powerScoreRange")]
        public int PowerScoreRange { get; set; }

        public MatchHistoryEntry(MatchRecordPoco poco)
        {
            MatchId = poco.match_id;
            Start = poco.match_start;
            End = poco.match_end;
            Duration = End.Subtract(Start);
            MatchType = poco.match_type;
            MapName = poco.map_name;
            MinPowerScore = poco.min_power_score;
            MaxPowerScore = poco.max_power_score;
            PowerScoreRange = MaxPowerScore - MinPowerScore;
        }
    }
}
