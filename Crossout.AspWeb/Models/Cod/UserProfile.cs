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
        public TimeSpan TimePlayed { get; set; }
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
        public int count { get; set; }
        public List<DrillDownSeries> series { get; set; }
    }

    public class DrillDownSeries
    {
        public string name { get; set; }
        public int count { get; set; }
    }

    public class DrillDownSelect
    {
        public string type { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }
}
