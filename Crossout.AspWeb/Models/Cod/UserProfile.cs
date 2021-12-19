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
        public string Nicknames { get; set; }
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

    public class MatchHistoryDetail
    {
        public List<UserMatchHistory> match_history { get; set; }
    }

    public class UserMatchHistory
    {
        public long match_id { get; set; }
        public string host_name { get; set; }
        public string client_version { get; set; }
        public string match_classification { get; set; }
        public string match_type { get; set; }
        public string map { get; set; }
        public string result { get; set; }
        public string build_hash { get; set; }
        public string parts { get; set; }
        public int power_score { get; set; }
        public int rounds { get; set; }
        public DateTime match_start { get; set; }
        public int time_spent { get; set; }
        public int kills { get; set; }
        public int assists { get; set; }
        public int drone_kills { get; set; }
        public int deaths { get; set; }
        public double damage { get; set; }
        public double damage_rec { get; set; }
        public int score { get; set; }
        public string resource_list { get; set; }
        public string medal_list { get; set; }
    }
}
