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
}
