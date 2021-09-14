using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
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

        public List<PlayerRoundPoco> PlayerRoundRecords { get; set; }

        public TimeSpan Duration { get => MatchRecord.match_end.Subtract(MatchRecord.match_start); }

        public int PowerScoreRange { get => MatchRecord.max_power_score - MatchRecord.min_power_score; }

        public List<Team> Teams { get; set; } = new List<Team>();

        public void FormTeams()
        {
            var teamNumbers = PlayerRoundRecords.Select(x => x.team).Distinct().ToList();
            teamNumbers.Sort();
            foreach (var teamNumber in teamNumbers)
            {
                var team = new Team(PlayerRoundRecords.Where(x => x.team == teamNumber).ToList());
                Teams.Add(team);
            }
        }
    }

    public class Team
    {
        public int TeamNumber { get; set; }

        public Dictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();

        public Team(List<PlayerRoundPoco> playerRounds)
        {
            TeamNumber = playerRounds[0].team;

            foreach (var playerRound in playerRounds)
            {
                if (Players.ContainsKey(playerRound.uid))
                {
                    Players[playerRound.uid] = new Player(playerRounds.Where(x => x.uid == playerRound.uid).ToList());
                }
                else
                {
                    Players.Add(playerRound.uid, new Player(playerRounds.Where(x => x.uid == playerRound.uid).ToList()));
                }
            }
        }
    }

    public class Player
    {
        public int UserId { get; set; }

        public PlayerRoundPoco RoundsCombined { get; set; }

        public List<PlayerRoundPoco> Rounds { get; set; }

        public Player(List<PlayerRoundPoco> playerRounds)
        {
            UserId = playerRounds[0].uid;

            Rounds = playerRounds.OrderBy(x => x.round_id).ToList();

            RoundsCombined = CombineRounds(playerRounds);
        }

        private PlayerRoundPoco CombineRounds(List<PlayerRoundPoco> rounds)
        {
            var result = new PlayerRoundPoco();
            foreach (var round in rounds)
            {
                if (result.uid == round.uid)
                {
                    result.damage += round.damage;
                    result.damage_taken += round.damage_taken;
                    result.drone_kills += round.drone_kills;
                    result.kills += round.kills;
                    result.score += round.score;
                    result.assists += round.assists;
                }
                else
                {
                    result = round.ShallowCopy();
                }
            }

            return result;
        }
    }

    public class MatchPlayerDetailData
    {
        [JsonProperty("damageData")]
        public List<RoundDamage> DamageData { get; set; } = new List<RoundDamage>();
    }

    public class RoundDamage
    {
        [JsonIgnore]
        public RoundDamagePoco RoundDamageRecord { get; set; }

        [JsonIgnore]
        public ItemPoco Item { get; set; }

        [JsonProperty("userId")]
        public int UserId { get => RoundDamageRecord.uid; }

        [JsonProperty("roundId")]
        public int RoundId { get => RoundDamageRecord.round_id; }

        [JsonProperty("damage")]
        public float Damage { get => RoundDamageRecord.damage; }

        [JsonProperty("itemId")]
        public int ItemId { get => Item?.Id ?? 0; }

        [JsonProperty("imageExists")]
        public bool ImageExists { get => Item?.ImageExists ?? false; }

        [JsonProperty("weaponDisplayName")]
        public string WeaponDisplayName { get => Item?.AvailableName ?? RoundDamageRecord.weapon; }
    }
}
