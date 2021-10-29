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
        public int GamesRecorded { get; set; }
        public int GamesUploaded { get; set; }
        public int PvpGames { get; set; }
        public int WinCount { get; set; }
        public string TimePlayed { get; set; }
        public string WinRate { get; set; }
        public string KPB { get; set; }
        public string MVPRate { get; set; }
        public List<string> Nicknames { get; set; }
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
        public int match_classification { get; set; }
        public string game_type { get; set; }
        public int games_recorded { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public string time_spent { get; set; }
        public int medals { get; set; }
        public int kills { get; set; }
        public int assists { get; set; }
        public int drone_kills { get; set; }
        public int deaths { get; set; }
        public double score_avg { get; set; }
        public double kill_avg { get; set; }
        public double assist_avg { get; set; }
        public double damage_avg { get; set; }
        public double damage_rec_avg { get; set; }
    }
}
