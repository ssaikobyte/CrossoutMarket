using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Pocos;
using Crossout.Model.Formatter;
using Crossout.Model.Items;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.General
{
    public class PremiumPackageColletionNew : BaseViewModel, IViewTitle
    {
        public string Title => "Packs";

        [JsonProperty("packages")]
        public List<PremiumPackageNew> Packages { get; set; } = new List<PremiumPackageNew>();

        [JsonProperty("containeditems")]
        public Dictionary<int, Item> ContainedItems { get; set; } = new Dictionary<int, Item>();

        [JsonIgnore]
        public StatusModel Status { get; set; } = new StatusModel();
    }
    public class PremiumPackageNew
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public PremiumPackagePoco PremiumPackage { get; set; }

        [JsonIgnore]
        public int Id { get => PremiumPackage.Id; }

        [JsonProperty("key")]
        public string Key { get => PremiumPackage.Key; }

        [JsonProperty("steamappid")]
        public int SteamAppID { get => PremiumPackage.SteamAppID; }

        [JsonProperty("containeditems")]
        public List<ContainedItemNew> ContainedItems { get; set; }

        [JsonProperty("name")]
        public string Name { get => PremiumPackage.Name; }

        [JsonProperty("category")]
        public int Category { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("sellsum")]
        public int SellSum { get; set; }

        [JsonProperty("buysum")]
        public int BuySum { get; set; }

        [JsonProperty("formatsellsum")]
        public string FormatSellSum => PriceFormatter.FormatPrice(SellSum);

        [JsonProperty("formatbuysum")]
        public string FormatBuySum => PriceFormatter.FormatPrice(BuySum);

        [JsonProperty("totalsellsum")]
        public int TotalSellSum => SellSum + RawCoins * 100;

        [JsonProperty("totalbuysum")]
        public int TotalBuySum => BuySum + RawCoins * 100;

        [JsonProperty("formattotalsellsum")]
        public string FormatTotalSellSum => PriceFormatter.FormatPrice(TotalSellSum);

        [JsonProperty("formattotalbuysum")]
        public string FormatTotalBuySum => PriceFormatter.FormatPrice(TotalBuySum);

        [JsonProperty("rawcoins")]
        public int RawCoins { get; set; }

        [JsonProperty("appprices")]
        [Reference(ReferenceType.OneToOne)]
        public AppPricesNew AppPrices { get; set; }

    }

    public class AppPricesNew
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("steamappid")]
        public int SteamAppId { get; set; }

        [JsonProperty("prices")]
        public List<CurrencyNew> Prices { get; set; }

        [JsonProperty("discount")]
        public int Discount { get; set; }

        [JsonProperty("successtimestamp")]
        public DateTime SuccessTimestamp { get; set; }

        public string FormatSuccessTimestamp => SuccessTimestamp.ToString("yyyy-MM-dd HH:mm:ss");
        public bool OlderThan(int minutes)
        {
            return DateTime.UtcNow - SuccessTimestamp > new TimeSpan(0, minutes, 0);
        }
    }

    public class CurrencyNew
    {
        [JsonProperty("currencyabbriviation")]
        public string CurrencyAbbriviation { get; set; }

        [JsonProperty("final")]
        public int Final { get; set; }

        [JsonProperty("formatfinal")]
        public string FormatFinal => PriceFormatter.FormatPrice(Final);

        public string SteamCurrencyAbbriviation { get; set; }
        public int Initial { get; set; }
        public int DiscountPercent { get; set; }
        public string FormatSellPriceDividedByCurrency { get; set; }
        public string FormatBuyPriceDividedByCurrency { get; set; }
    }

    public class ContainedItemNew
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public PremiumPackageItemPoco PremiumPackageItem { get; set; }

        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public ItemPoco Item { get; set; }

        [JsonProperty("id")]
        public int Id { get => PremiumPackageItem.ItemNumber; }

        [JsonProperty("name")]
        public string Name { get => Item.ItemLocalization.LocalizedName; }

        [JsonProperty("sellprice")]
        public int SellPrice { get; set; }

        [JsonProperty("buyprice")]
        public int BuyPrice { get; set; }

        [JsonProperty("formatsellprice")]
        public string FormatSellPrice => PriceFormatter.FormatPrice(SellPrice);

        [JsonProperty("formatbuyprice")]
        public string FormatBuyPrice => PriceFormatter.FormatPrice(BuyPrice);
    }
}
