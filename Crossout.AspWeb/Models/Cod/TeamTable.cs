using Crossout.AspWeb.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class TeamTable
    {
        public int Id { get; set; }

        public long MatchId { get; set; }

        public int RoundNumber { get; set; }

        public int? HighlightUid { get; set; }

        public List<Player> Players { get; set; }
    }
}
