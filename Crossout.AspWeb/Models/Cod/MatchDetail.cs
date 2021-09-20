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

        public List<Round> Rounds { get; set; } = new List<Round>();

        public void Create()
        {
            var roundIds = PlayerRoundRecords.Select(x => x.round_id).Distinct().ToList();
            roundIds.Sort();
            var combinedRound = new Round(CombineRounds(PlayerRoundRecords), 0);
            Rounds.Add(combinedRound);
            int i = 1;
            foreach (var roundId in roundIds)
            {
                var round = new Round(PlayerRoundRecords.Where(x => x.round_id == roundId).ToList(), i);
                Rounds.Add(round);
                i++;
            }
        }

        private List<PlayerRoundPoco> CombineRounds(List<PlayerRoundPoco> rounds)
        {
            var playerRounds = new Dictionary<int, PlayerRoundPoco>();
            foreach (var round in rounds)
            {
                if (playerRounds.ContainsKey(round.uid))
                {
                    playerRounds[round.uid].damage += round.damage;
                    playerRounds[round.uid].damage_taken += round.damage_taken;
                    playerRounds[round.uid].drone_kills += round.drone_kills;
                    playerRounds[round.uid].kills += round.kills;
                    playerRounds[round.uid].score += round.score;
                    playerRounds[round.uid].assists += round.assists;
                }
                else
                {
                    playerRounds.Add(round.uid, round.ShallowCopy());
                }
            }

            return playerRounds.Values.ToList();
        }
    }

    public class Round
    {
        public int Id { get; }

        public int Counter { get; set; }

        public string Name { get => Counter > 0 ? $"Round {Counter}" : "Combined"; }

        public List<Team> Teams { get; set; } = new List<Team>();

        public Round(List<PlayerRoundPoco> rounds, int counter)
        {
            Id = rounds.FirstOrDefault().round_id;

            Counter = counter;

            var teamNumbers = rounds.Select(x => x.team).Distinct().ToList();
            teamNumbers.Sort();
            foreach (var teamNumber in teamNumbers)
            {
                var team = new Team(rounds.Where(x => x.team == teamNumber).ToList());
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

        public List<PlayerRoundPoco> Rounds { get; set; }

        public PlayerRoundPoco FirstRound { get => Rounds.FirstOrDefault(); }

        public Player(List<PlayerRoundPoco> playerRounds)
        {
            UserId = playerRounds[0].uid;

            Rounds = playerRounds.OrderBy(x => x.round_id).ToList();
        }
    }

    public class MatchPlayerDetailData
    {
        [JsonProperty("damageData")]
        public List<RoundDamage> DamageData { get; set; } = new List<RoundDamage>();

        [JsonProperty("medalData")]
        public List<MatchMedal> MedalData { get; set; } = new List<MatchMedal>();
    }

    public class RoundDamage
    {
        [JsonIgnore]
        public RoundDamagePoco RoundDamageRecordPoco { get; set; }

        [JsonIgnore]
        public ItemPoco Item { get; set; }

        [JsonProperty("userId")]
        public int UserId { get => RoundDamageRecordPoco.uid; }

        [JsonProperty("roundId")]
        public int RoundId { get => RoundDamageRecordPoco.round_id; }

        [JsonProperty("damage")]
        public float Damage { get => RoundDamageRecordPoco.damage; }

        [JsonProperty("itemId")]
        public int ItemId { get => Item?.Id ?? 0; }

        [JsonProperty("imageExists")]
        public bool ImageExists { get => Item?.ImageExists ?? false; }

        [JsonProperty("weaponDisplayName")]
        public string WeaponDisplayName { get => Item?.AvailableName ?? RoundDamageRecordPoco.weapon; }
    }

    public class MatchMedal
    {
        [JsonIgnore]
        public MatchMedalPoco MatchMedalPoco { get; set; }

        [JsonProperty("userId")]
        public int UserId { get => MatchMedalPoco.uid; }

        [JsonProperty("roundId")]
        public int RoundId { get => MatchMedalPoco.round_id; }

        [JsonProperty("medal")]
        public string Medal { get => MatchMedalPoco.medal; }

        [JsonProperty("amount")]
        public int Amount { get => MatchMedalPoco.amount; }
    }
}
