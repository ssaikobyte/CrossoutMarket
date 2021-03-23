using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("ocrstats")]
    [PrimaryKey("id")]
    public class OCRStatItemPoco
    {
        [JsonIgnore]
        [Column("id")]
        public int Id { get; set; }

        [JsonProperty("itemNumber")]
        [Column("itemnumber")]
        public int ItemNumber { get; set; }

        [JsonProperty("timestamp")]
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("xoVersion")]
        [Column("xoversion")]
        public string XoVer { get; set; }

        [JsonProperty("author")]
        [Column("author")]
        public string Author { get; set; }

        [JsonProperty("category")]
        [Column("category")]
        public string Category { get; set; }

        [JsonProperty("faction")]
        [Column("faction")]
        public string Faction { get; set; }

        [JsonProperty("level")]
        [Column("level")]
        public int? Level { get; set; }

        [JsonProperty("name")]
        [Column("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        [Column("type")]
        public string Type { get; set; }

        [JsonProperty("rarity")]
        [Column("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("description")]
        [Column("description")]
        public string Description { get; set; }

        [JsonProperty("increasesDurability")]
        [Column("increasesdurability")]
        public int? IncreasesDurability { get; set; }

        [JsonProperty("increasesReputationPercent")]
        [Column("increasesreputation")]
        public int? IncreasesReputationPercent { get; set; }

        [JsonProperty("topSpeed")]
        [Column("topspeed")]
        public int? TopSpeed { get; set; }

        [JsonProperty("ps")]
        [Column("ps")]
        public int? Ps { get; set; }

        [JsonProperty("damage")]
        [Column("damage")]
        public decimal? Damage { get; set; }

        [JsonProperty("fireRate")]
        [Column("firerate")]
        public decimal? FireRate { get; set; }

        [JsonProperty("range")]
        [Column("range")]
        public decimal? Range { get; set; }

        [JsonProperty("accuracy")]
        [Column("accuracy")]
        public decimal? Accuracy { get; set; }

        [JsonProperty("timeToOverheating")]
        [Column("timetooverheating")]
        public decimal? TimeToOverheating { get; set; }

        [JsonProperty("maxAmmo")]
        [Column("maxammo")]
        public int? MaxAmmo { get; set; }

        [JsonProperty("blastPower")]
        [Column("blastpower")]
        public decimal? BlastPower { get; set; }

        [JsonProperty("addsEnergy")]
        [Column("addsenergy")]
        public int? AddsEnergy { get; set; }

        [JsonProperty("tonnage")]
        [Column("tonnage")]
        public int? Tonnage { get; set; }

        [JsonProperty("massLimit")]
        [Column("masslimit")]
        public int? MassLimit { get; set; }

        [JsonProperty("maxCabinSpeed")]
        [Column("maxcabinspeed")]
        public int? MaxCabinSpeed { get; set; }

        [JsonProperty("maxChassisSpeed")]
        [Column("maxchassisspeed")]
        public int? MaxChassisSpeed { get; set; }

        [JsonProperty("power")]
        [Column("power")]
        public int? Power { get; set; }

        [JsonProperty("cabinPower")]
        [Column("cabinpower")]
        public decimal? CabinPower { get; set; }

        [JsonProperty("fuelReserves")]
        [Column("fuelreserves")]
        public int? FuelReserves { get; set; }

        [JsonProperty("featureBulletPercent")]
        [Column("featurebullet")]
        public int? FeatureBulletPercent { get; set; }

        [JsonProperty("featureMeleePercent")]
        [Column("featuremelee")]
        public int? FeatureMeleePercent { get; set; }

        [JsonProperty("featureBlastPercent")]
        [Column("featureblast")]
        public int? FeatureBlastPercent { get; set; }

        [JsonProperty("featureFirePercent")]
        [Column("featurefire")]
        public int? FeatureFirePercent { get; set; }

        [JsonProperty("featurePassthroughPercent")]
        [Column("featurepassthrough")]
        public int? FeaturePassthroughPercent { get; set; }

        [JsonProperty("durability")]
        [Column("durability")]
        public int? Durability { get; set; }

        [JsonProperty("energyDrain")]
        [Column("energydrain")]
        public int? EnergyDrain { get; set; }

        [JsonProperty("mass")]
        [Column("mass")]
        public int? Mass { get; set; }

        [JsonProperty("perks")]
        [Column("perks")]
        public string Perks { get; set; }
    }
}
