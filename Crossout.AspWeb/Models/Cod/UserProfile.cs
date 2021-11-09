using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class UserProfile : BaseViewModel, IViewTitle
    {
        public string Title => "Profile ";
        public int Uid { get; set; }
        public string Nickname { get; set; }
        public List<string> Nicknames { get; set; }
    }

    public class OverviewCard
    {
        public int GamesRecorded { get; set; }
        public int GamesUploaded { get; set; }
        public int PvpGames { get; set; }
        public int WinCount { get; set; }
        public string TimePlayed { get; set; }
        public string WinRate { get; set; }
        public string KPB { get; set; }
        public string MVPRate { get; set; }
    }

    public class OverviewCharts
    {
        public List<DrillDown> gamemode_preference { get; set; }
        public List<DrillDown> weapon_preference { get; set; }
        public List<DrillDown> movement_preference { get; set; }
    }

    public class DrillDown
    {
        public string type { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }

    public class GameModeDetail
    {
        public List<GameMode> game_modes { get; set; }
    }

    public class GameMode
    {
        public string match_classification { get; set; }
        public string match_type { get; set; }
        public int games { get; set; }
        public int rounds { get; set; }
        public int wins { get; set; }
        public int time_spent { get; set; }
        public int medals { get; set; }
        public int kills { get; set; }
        public int assists { get; set; }
        public int drone_kills { get; set; }
        public int deaths { get; set; }
        public double damage { get; set; }
        public double damage_rec { get; set; }
        public int score { get; set; }
    }

    public class MatchHistoryDetail
    {
        public List<UserMatchHistory> match_history { get; set; }
    }

    public class UserMatchHistory
    {
        public long match_id { get; set; }
        public string match_classification { get; set; }
        public string match_type { get; set; }
        public DateTime match_start { get; set; }
        public DateTime match_end { get; set; }
        public string map { get; set; }
        public int power_score { get; set; }
        public int score { get; set; }
        public int kills { get; set; }
        public int assists { get; set; }
        public int drone_kills { get; set; }
        public double damage { get; set; }
        public double damage_rec { get; set; }
        public string result { get; set; }
        public string resources { get; set; }
    }
}
