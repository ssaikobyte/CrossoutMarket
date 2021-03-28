using Crossout.AspWeb.Models.Items;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

        [JsonProperty("levelType")]
        [Column("leveltype")]
        public string LevelType { get; set; }

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

        [JsonIgnore]
        [Ignore]
        public string FormatDamagePercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Damage * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatDamage { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Damage); }

        [JsonProperty("fireRate")]
        [Column("firerate")]
        public decimal? FireRate { get; set; }

        [JsonIgnore]
        [Ignore]
        public string FormatFireRatePercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", FireRate * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatFireRate { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", FireRate); }

        [JsonProperty("range")]
        [Column("range")]
        public decimal? Range { get; set; }

        [JsonIgnore]
        [Ignore]
        public string FormatRangePercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Range * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatRange { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Range); }

        [JsonProperty("accuracy")]
        [Column("accuracy")]
        public decimal? Accuracy { get; set; }

        [JsonIgnore]
        [Ignore]
        public string FormatAccuracyPercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Accuracy * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatAccuracy { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Accuracy); }

        [JsonProperty("timeToOverheating")]
        [Column("timetooverheating")]
        public decimal? TimeToOverheating { get; set; }

        [JsonIgnore]
        [Ignore]
        public string FormatTimeToOverheatingPercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", TimeToOverheating * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatTimeToOverheating { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", TimeToOverheating); }

        [JsonProperty("maxAmmo")]
        [Column("maxammo")]
        public int? MaxAmmo { get; set; }

        [JsonProperty("blastPower")]
        [Column("blastpower")]
        public decimal? BlastPower { get; set; }

        [JsonIgnore]
        [Ignore]
        public string FormatBlastPowerPercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", BlastPower * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatBlastPower { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", BlastPower); }

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

        [JsonIgnore]
        [Ignore]
        public string FormatCabinPowerPercent { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", CabinPower * 10m); }

        [JsonIgnore]
        [Ignore]
        public string FormatCabinPower { get => string.Format(CultureInfo.InvariantCulture, "{0:0.00}", CabinPower); }

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

        [JsonProperty("displayStats")]
        [Ignore]
        public List<OCRDisplayStat> DisplayStats { get; set; } = new List<OCRDisplayStat>();

        public void CreateDisplayStats()
        {
            DisplayStats.Add(new OCRDisplayStat { Name = "Item ID", Value = ItemNumber.ToString(), DisplayType = DisplayType.HIDDEN, Displayed = false });
            if (Timestamp != null) DisplayStats.Add(new OCRDisplayStat { Name = "Timestamp", Value = Timestamp.ToString(), DisplayType = DisplayType.TIMESTAMP, Displayed = false });
            if (XoVer != null) DisplayStats.Add(new OCRDisplayStat { Name = "Crossout Version", Value = XoVer.ToString(), DisplayType = DisplayType.VERSION, Displayed = false });
            if (Author != null) DisplayStats.Add(new OCRDisplayStat { Name = "Author", Value = Author.ToString(), DisplayType = DisplayType.RAW, Displayed = false }); ;
            if (Category != null) DisplayStats.Add(new OCRDisplayStat { Name = "Category", Value = Category.ToString(), DisplayType = DisplayType.RAW, Order = 5 });
            if (Faction != null) DisplayStats.Add(new OCRDisplayStat { Name = "Faction", Value = Faction.ToString(), DisplayType = DisplayType.RAW, Order = 2 });
            if (Level != null) DisplayStats.Add(new OCRDisplayStat { Name = "Level", Value = Level.ToString(), DisplayType = DisplayType.RAW, Order = 3 });
            if (LevelType != null) DisplayStats.Add(new OCRDisplayStat { Name = "Level Type", Value = LevelType.ToString(), DisplayType = DisplayType.RAW, Order = 4 });
            if (Name != null) DisplayStats.Add(new OCRDisplayStat { Name = "Name", Value = Name.ToString(), DisplayType = DisplayType.RAW, Order = 1 });
            if (Type != null) DisplayStats.Add(new OCRDisplayStat { Name = "Type", Value = Type.ToString(), DisplayType = DisplayType.RAW, Order = 6 });
            if (Rarity != null) DisplayStats.Add(new OCRDisplayStat { Name = "Rarity", Value = Rarity.ToString(), DisplayType = DisplayType.RARITY, Order = 7 });
            if (Description != null) DisplayStats.Add(new OCRDisplayStat { Name = "Description", Value = Description.ToString(), DisplayType = DisplayType.RAW, Displayed = false });
            if (IncreasesDurability != null) DisplayStats.Add(new OCRDisplayStat { Name = "Adds Durability", Value = IncreasesDurability.ToString(), DisplayType = DisplayType.RAW, Order = 19 });
            if (IncreasesReputationPercent != null) DisplayStats.Add(new OCRDisplayStat { Name = "Adds Reputation", Value = IncreasesReputationPercent.ToString(), DisplayType = DisplayType.PERCENT, Order = 20 });
            if (TopSpeed != null) DisplayStats.Add(new OCRDisplayStat { Name = "Top Speed", Value = TopSpeed.ToString(), DisplayType = DisplayType.RAW, Order = 21 });
            if (Ps != null) DisplayStats.Add(new OCRDisplayStat { Name = "Powerscore", Value = Ps.ToString(), DisplayType = DisplayType.POWERSCORE, Order = 8 });
            if (Damage != null) DisplayStats.Add(new OCRDisplayStat { Name = "Damage", Value = string.Format(CultureInfo.InvariantCulture, "{0:00}", Damage), DisplayType = DisplayType.RATING, Order = 12 });
            if (FireRate != null) DisplayStats.Add(new OCRDisplayStat { Name = "Fire Rate", Value = FireRate.ToString(), DisplayType = DisplayType.RATING, Order = 13 });
            if (Range != null) DisplayStats.Add(new OCRDisplayStat { Name = "Range", Value = Range.ToString(), DisplayType = DisplayType.RATING, Order = 14 });
            if (Accuracy != null) DisplayStats.Add(new OCRDisplayStat { Name = "Accuracy", Value = Accuracy.ToString(), DisplayType = DisplayType.RATING, Order = 15 });
            if (TimeToOverheating != null) DisplayStats.Add(new OCRDisplayStat { Name = "Time to Overheating", Value = TimeToOverheating.ToString(), DisplayType = DisplayType.RATING, Order = 16 });
            if (MaxAmmo != null) DisplayStats.Add(new OCRDisplayStat { Name = "Max. Ammo", Value = MaxAmmo.ToString(), DisplayType = DisplayType.RAW, Order = 22 });
            if (BlastPower != null) DisplayStats.Add(new OCRDisplayStat { Name = "Blast Power", Value = BlastPower.ToString(), DisplayType = DisplayType.RATING, Order = 17 });
            if (AddsEnergy != null) DisplayStats.Add(new OCRDisplayStat { Name = "Adds Energy", Value = AddsEnergy.ToString(), DisplayType = DisplayType.RAW, Order = 23 });
            if (Tonnage != null) DisplayStats.Add(new OCRDisplayStat { Name = "Tonnage", Value = Tonnage.ToString(), DisplayType = DisplayType.RAW, Order = 24 });
            if (MassLimit != null) DisplayStats.Add(new OCRDisplayStat { Name = "Mass Limit", Value = MassLimit.ToString(), DisplayType = DisplayType.RAW, Order = 25 });
            if (MaxCabinSpeed != null) DisplayStats.Add(new OCRDisplayStat { Name = "Max. Cabin Speed", Value = MaxCabinSpeed.ToString(), DisplayType = DisplayType.RAW, Order = 26 });
            if (MaxChassisSpeed != null) DisplayStats.Add(new OCRDisplayStat { Name = "Max. Chassis Speed", Value = MaxChassisSpeed.ToString(), DisplayType = DisplayType.RAW, Order = 27 });
            if (Power != null) DisplayStats.Add(new OCRDisplayStat { Name = "Power", Value = Power.ToString(), DisplayType = DisplayType.PERCENT, Order = 28 });
            if (CabinPower != null) DisplayStats.Add(new OCRDisplayStat { Name = "Cabin Power", Value = CabinPower.ToString(), DisplayType = DisplayType.RATING, Order = 18 });
            if (FuelReserves != null) DisplayStats.Add(new OCRDisplayStat { Name = "Fuel Reserves", Value = FuelReserves.ToString(), DisplayType = DisplayType.RAW, Order = 29 });
            if (BlastPower != null) DisplayStats.Add(new OCRDisplayStat { Name = "Feature Bullet", Value = BlastPower.ToString(), DisplayType = DisplayType.PERCENT, Order = 30 });
            if (FeatureMeleePercent != null) DisplayStats.Add(new OCRDisplayStat { Name = "Feature Melee", Value = FeatureMeleePercent.ToString(), DisplayType = DisplayType.PERCENT, Order = 31 });
            if (FeatureBlastPercent != null) DisplayStats.Add(new OCRDisplayStat { Name = "Feature Blast", Value = FeatureBlastPercent.ToString(), DisplayType = DisplayType.PERCENT, Order = 32 });
            if (FeatureFirePercent != null) DisplayStats.Add(new OCRDisplayStat { Name = "Feature Fire", Value = FeatureFirePercent.ToString(), DisplayType = DisplayType.PERCENT, Order = 33 });
            if (FeaturePassthroughPercent != null) DisplayStats.Add(new OCRDisplayStat { Name = "Feature Passthrough", Value = FeaturePassthroughPercent.ToString(), DisplayType = DisplayType.PERCENT, Order = 34 });
            if (Durability != null) DisplayStats.Add(new OCRDisplayStat { Name = "Durability", Value = Durability.ToString(), DisplayType = DisplayType.RAW, Order = 10 });
            if (EnergyDrain != null) DisplayStats.Add(new OCRDisplayStat { Name = "Energy Drain", Value = EnergyDrain.ToString(), DisplayType = DisplayType.RAW, Order = 11 });
            if (Mass != null) DisplayStats.Add(new OCRDisplayStat { Name = "Mass", Value = Mass.ToString(), DisplayType = DisplayType.RAW, Order = 9 });
            if (Perks != null) DisplayStats.Add(new OCRDisplayStat { Name = "Perks", Value = Perks.ToString(), DisplayType = DisplayType.RAW, Order = 35 });

            DisplayStats.Sort(CompareOrder);
        }

        public int CompareOrder(OCRDisplayStat x, OCRDisplayStat y)
        {
            return x.Order.CompareTo(y.Order);
        }
    }
}
