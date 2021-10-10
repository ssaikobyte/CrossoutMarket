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
        public List<string> Nicknames { get; set; }
        public List<Tuple<string, int>> GameModeTrack { get; set; }
        public List<Tuple<string, int>> MovementTrack { get; set; }
        public List<Tuple<string, int>> WeaponTrack { get; set; }
    }

    public class ProfileOverview
    {

    }


}
