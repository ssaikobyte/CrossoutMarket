using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    public class MatchHistoryEntryPoco
    {
        [JsonIgnore]
        public MatchRecordPoco MatchRecord { get; set; }

        [JsonIgnore]
        public MapPoco Map { get; set; }

        [Ignore]
        [JsonProperty("matchId")]
        public long MatchId { get => MatchRecord.match_id; }

        [Ignore]
        [JsonProperty("start")]
        public DateTime Start { get => MatchRecord.match_start; }

        [Ignore]
        [JsonProperty("end")]
        public DateTime End { get => MatchRecord.match_end; }

        [Ignore]
        [JsonProperty("matchType")]
        public string MatchType { get => MatchRecord.match_type; }

        [Ignore]
        [JsonProperty("mapName")]
        public string MapName { get => Map.map_display_name; }

        [Ignore]
        [JsonProperty("mapKey")]
        public string MapKey { get => Map.map_name; }

        [Ignore]
        [JsonProperty("minPowerScore")]
        public int MinPowerScore { get => MatchRecord.min_power_score; }

        [Ignore]
        [JsonProperty("maxPowerScore")]
        public int MaxPowerScore { get => MatchRecord.max_power_score; }

        [Ignore]
        [JsonProperty("duration")]
        public TimeSpan Duration { get => End.Subtract(Start); }

        [Ignore]
        [JsonProperty("powerScoreRange")]
        public int PowerScoreRange { get => MaxPowerScore - MinPowerScore; }
    }
}
