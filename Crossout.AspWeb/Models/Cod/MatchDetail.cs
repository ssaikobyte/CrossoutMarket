using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class MatchDetail : BaseViewModel, IViewTitle
    {
        public string Title => "Match " + MatchRecord.match_id;

        public MatchPoco MatchRecord { get; set; }

        public MapPoco Map { get; set; }

        public List<RoundPoco> RoundRecords { get; set; }

        public List<RoundDamage> RoundDamages { get; set; }

        public List<PlayerRoundPoco> PlayerRoundRecords { get; set; }

        public TimeSpan Duration { get => MatchRecord.match_end.Subtract(MatchRecord.match_start); }

        public int PowerScoreRange { get => MatchRecord.max_power_score - MatchRecord.min_power_score; }

        public List<PlayerRoundPoco> Team1PlayersCombined { get => CombineRounds(PlayerRoundRecords.Where(x => x.team == 1).ToList()); }

        public List<PlayerRoundPoco> Team2PlayersCombined { get => CombineRounds(PlayerRoundRecords.Where(x => x.team == 2).ToList()); }

        private List<PlayerRoundPoco> CombineRounds(List<PlayerRoundPoco> rounds)
        {
            var result = new Dictionary<int, PlayerRoundPoco>();
            foreach (var round in rounds)
            {
                if (result.ContainsKey(round.uid))
                {
                    result[round.uid].damage += round.damage;
                    result[round.uid].damage_taken += round.damage_taken;
                    result[round.uid].drone_kills += round.drone_kills;
                    result[round.uid].kills += round.kills;
                    result[round.uid].score += round.score;
                    result[round.uid].assists += round.assists;
                }
                else
                {
                    var playerRoundRecordPoco = round.ShallowCopy();
                    result.Add(playerRoundRecordPoco.uid, playerRoundRecordPoco);
                }
            }
            return result.Values.ToList();
        }
    }
}
