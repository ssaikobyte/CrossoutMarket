using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("itemsynergies")]
    [PrimaryKey("id")]
    public class SynergyPoco
    {
        [JsonIgnore]
        [Column("id")]
        public int Id { get; set; }

        [JsonProperty("itemNumber")]
        [Column("itemnumber")]
        public int ItemNumber { get; set; }

        [JsonProperty("synergyType")]
        [Column("synergy")]
        public string SynergyType { get; set; }

        [JsonIgnore]
        [Column("enabled")]
        public bool Enabled { get; set; }
    }
}
