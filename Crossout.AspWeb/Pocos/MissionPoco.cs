using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    [TableName("mission")]
    public class MissionPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("match_type")]
        public string MatchType { get; set; }

        [Column("resource")]
        public string Resource { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
