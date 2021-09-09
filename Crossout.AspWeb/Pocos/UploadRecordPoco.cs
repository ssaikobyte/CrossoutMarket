using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("cod_upload_records")]
    public class UploadPoco
    {
        [Column("uid")]
        public int uid { get; set; }

        [Column("match_id")]
        public long match_id { get; set; }

        [Column("upload_time")]
        public DateTime upload_time { get; set; }
        [Column("status")]
        public string status { get; set; }
    }

    [TableName("cod_match_records")]
    [PrimaryKey("match_id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MatchPoco
    {
        [Column("match_id")]
        public long match_id { get; set; }

        [Column("match_type")]
        public string match_type { get; set; }

        [Column("match_start")]
        public DateTime match_start { get; set; }

        [Column("match_end")]
        public DateTime match_end { get; set; }

        [Column("map_name")]
        public string map_name { get; set; }

        [Column("winning_team")]
        public int winning_team { get; set; }

        [Column("win_condition")]
        public int win_condition { get; set; }
        [Column("min_power_score")]
        public int min_power_score { get; set; }
        [Column("max_power_score")]
        public int max_power_score { get; set; }

        [Column("round_id_1")]
        public int round_id_1 { get; set; }

        [Column("round_id_2")]
        public int round_id_2 { get; set; }

        [Column("round_id_3")]
        public int round_id_3 { get; set; }

        [Column("client_version")]
        public string client_version { get; set; }

        [Column("co_driver_version")]
        public string co_driver_version { get; set; }

        [Column("server_ip")]
        public string server_ip { get; set; }
    }

    [TableName("cod_round_records")]
    [PrimaryKey("round_id", AutoIncrement = true)]
    [ExplicitColumns]
    public class RoundPoco
    {
        [Column("round_id")]
        public int round_id { get; set; }
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("round_start")]
        public DateTime round_start { get; set; }
        [Column("round_end")]
        public DateTime round_end { get; set; }
        [Column("winning_team")]
        public int winning_team { get; set; }
    }

    [TableName("cod_player_round_records")]
    [ExplicitColumns]
    public class PlayerRoundPoco
    {
        [Column("round_id")]
        public int round_id { get; set; }
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("uid")]
        public int uid { get; set; }
        [Column("nickname")]
        public string nickname { get; set; }
        [Column("team")]
        public int team { get; set; }
        [Column("build_hash")]
        public string build_hash { get; set; }
        [Column("game_result")]
        public string game_result { get; set; }
        [Column("power_score")]
        public int power_score { get; set; }
        [Column("kills")]
        public int kills { get; set; }
        [Column("assists")]
        public int assists { get; set; }
        [Column("drone_kills")]
        public int drone_kills { get; set; }
        [Column("score")]
        public int score { get; set; }
        [Column("damage")]
        public float damage { get; set; }
        [Column("damage_taken")]
        public float damage_taken { get; set; }

        public PlayerRoundPoco ShallowCopy()
        {
            return (PlayerRoundPoco)this.MemberwiseClone();
        }
    }

    [TableName("cod_player_round_damage_records")]
    [PrimaryKey("match_id", AutoIncrement = false)]
    [ExplicitColumns]
    public class RoundDamagePoco
    {
        [Column("round_id")]
        public int round_id { get; set; }
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("uid")]
        public int uid { get; set; }
        [Column("weapon")]
        public string weapon { get; set; }
        [Column("damage")]
        public float damage { get; set; }
    }

    [TableName("cod_player_match_resource_records")]
    [PrimaryKey("match_id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MatchResourcePoco
    {
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("uid")]
        public int uid { get; set; }
        [Column("resource")]
        public string resource { get; set; }
        [Column("amount")]
        public int amount { get; set; }
    }

    [TableName("cod_player_match_score_records")]
    [PrimaryKey("match_id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MatchScorePoco
    {
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("round_id")]
        public int round_id { get; set; }
        [Column("uid")]
        public int uid { get; set; }
        [Column("score_type")]
        public string score_type { get; set; }
        [Column("score")]
        public int score { get; set; }
    }

    [TableName("cod_player_match_medal_records")]
    [PrimaryKey("match_id", AutoIncrement = false)]
    [ExplicitColumns]
    public class MatchMedalPoco
    {
        [Column("match_id")]
        public long match_id { get; set; }
        [Column("round_id")]
        public int round_id { get; set; }
        [Column("uid")]
        public int uid { get; set; }
        [Column("medal")]
        public string medal { get; set; }
        [Column("amount")]
        public int amount { get; set; }
    }

    [TableName("cod_maps")]
    [PrimaryKey("map_name", AutoIncrement = false)]
    [ExplicitColumns]
    public class MapPoco
    {
        [Column("map_name")]
        public string map_name { get; set; }
        [Column("map_display_name")]
        public string map_display_name { get; set; }
    }

}
