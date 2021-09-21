using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using NPoco;
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

        [JsonProperty("synergyItems")]
        public List<SynergyItem> SynergyItems { get; set; } = new List<SynergyItem>();
    }

    public class SynergyItem
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public SynergyPoco Synergy { get; set; }

        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public ItemPoco Item { get; set; }

        [JsonProperty("itemNumber")]
        public int ItemId { get => Synergy.ItemNumber; }

        [JsonProperty("synergyType")]
        public string SynergyType { get => Synergy.SynergyType; }

        [JsonProperty("name")]
        public string Name { get => Item.ItemLocalization.LocalizedName; }

        [JsonProperty("rarity")]
        public string Rarity { get => Item.Rarity.Name; }
    }
}
