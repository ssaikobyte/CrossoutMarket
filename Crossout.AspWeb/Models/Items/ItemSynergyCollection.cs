using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Items
{
    public class ItemSynergyCollection
    {
        [JsonProperty("itemNumber")]
        public int ItemNumber { get; set; }

        [JsonProperty("synergies")]
        public List<SynergyPoco> Synergies { get; set; } = new List<SynergyPoco>();

        [JsonProperty("synergyitems")]
        public List<SynergyPoco> SynergyItems { get; set; } = new List<SynergyPoco>();
    }
}
