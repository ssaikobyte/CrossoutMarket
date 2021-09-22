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
    public class PremiumPackageColletion : BaseViewModel, IViewTitle
    {
        public string Title => "Packs";

        [JsonProperty("packages")]
        public List<PremiumPackage> Packages { get; set; } = new List<PremiumPackage>();

        [JsonIgnore]
        public StatusModel Status { get; set; } = new StatusModel();
    }
    public class PremiumPackage
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public PremiumPackagePoco Package { get; set; }

        [JsonIgnore]
        [Ignore]
        public int Id { get => Package.Id; }

        [JsonProperty("key")]
        [Ignore]
        public string Key { get => Package.Key; }

        [JsonProperty("steamappid")]
        [Ignore]
        public int SteamAppID { get => Package.SteamAppID; }

        [JsonProperty("containeditems")]
        [Ignore]
        public List<PremiumPackageItem> ContainedItems { get; set; }

        [JsonProperty("name")]
        [Ignore]
        public string Name { get => Package.Name; }

        [JsonProperty("category")]
        [Ignore]
        public int Category { get; set; }

        [JsonProperty("description")]
        [Ignore]
        public string Description { get; set; }

        [JsonProperty("sellsum")]
        [Ignore]
        public int SellSum { get; set; }

        [JsonProperty("buysum")]
        [Ignore]
        public int BuySum { get; set; }

        [JsonProperty("formatsellsum")]
        [Ignore]
        public string FormatSellSum => PriceFormatter.FormatPrice(SellSum);

        [JsonProperty("formatbuysum")]
        [Ignore]
        public string FormatBuySum => PriceFormatter.FormatPrice(BuySum);

        [JsonProperty("totalsellsum")]
        [Ignore]
        public int TotalSellSum => SellSum + RawCoins * 100;

        [JsonProperty("totalbuysum")]
        [Ignore]
        public int TotalBuySum => BuySum + RawCoins * 100;

        [JsonProperty("formattotalsellsum")]
        [Ignore]
        public string FormatTotalSellSum => PriceFormatter.FormatPrice(TotalSellSum);

        [JsonProperty("formattotalbuysum")]
        [Ignore]
        public string FormatTotalBuySum => PriceFormatter.FormatPrice(TotalBuySum);

        [JsonProperty("rawcoins")]
        [Ignore]
        public int RawCoins { get => Package.RawCoins; }

        [JsonProperty("appprices")]
        [Reference(ReferenceType.OneToOne)]
        public SteamPricesPoco AppPrices { get; set; }

        public void Create()
        {
            foreach (var item in ContainedItems)
            {
                SellSum += item.SellPrice;
                BuySum += item.BuyPrice;
            }
            //TODO: Add null checking
            AppPrices.Create();
            foreach (var price in AppPrices.Prices)
            {
                if (price != null && price.Final != 0)
                {
                    price.FormatSellPriceDividedByCurrency = PriceFormatter.FormatPrice(TotalSellSum / ((decimal)price.Final / 100));
                    price.FormatBuyPriceDividedByCurrency = PriceFormatter.FormatPrice(TotalBuySum / ((decimal)price.Final / 100));
                }
            }
        }
    }

    public class PremiumPackageItem
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public PremiumPackageItemPoco PackageItem { get; set; }

        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public ItemPoco Item { get; set; }

        [JsonProperty("id")]
        [Ignore]
        public int Id { get => PackageItem.ItemNumber; }

        [JsonProperty("name")]
        [Ignore]
        public string Name { get => Item.AvailableName; }

        [JsonProperty("sellprice")]
        [Ignore]
        public int SellPrice { get => Item.SellPrice; }

        [JsonProperty("buyprice")]
        [Ignore]
        public int BuyPrice { get => Item.BuyPrice; }

        [JsonProperty("formatsellprice")]
        [Ignore]
        public string FormatSellPrice => PriceFormatter.FormatPrice(SellPrice);

        [JsonProperty("formatbuyprice")]
        [Ignore]
        public string FormatBuyPrice => PriceFormatter.FormatPrice(BuyPrice);
    }
}
