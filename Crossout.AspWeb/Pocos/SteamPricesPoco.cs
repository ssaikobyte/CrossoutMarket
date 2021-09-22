using Crossout.Model.Formatter;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("steamprices")]
    [PrimaryKey("id")]
    public class SteamPricesPoco
    {
        [JsonProperty("id")]
        [Column("id")]
        public int Id { get; set; }

        [JsonProperty("steamappid")]
        [Column("appid")]
        public int SteamAppId { get; set; }

        [JsonIgnore]
        [Column("priceusd")]
        public int PriceUsd { get; set; }

        [JsonIgnore]
        [Column("priceeur")]
        public int PriceEur { get; set; }

        [JsonIgnore]
        [Column("pricegbp")]
        public int PriceGbp { get; set; }

        [JsonIgnore]
        [Column("pricerub")]
        public int PriceRub { get; set; }

        [JsonProperty("prices")]
        [Ignore]
        public List<CurrencyNew> Prices { get; private set; }

        [JsonProperty("discount")]
        [Column("discount")]
        public int Discount { get; set; }

        [JsonProperty("successtimestamp")]
        [Column("successtimestamp")]
        public DateTime SuccessTimestamp { get; set; }

        public string FormatSuccessTimestamp => SuccessTimestamp.ToString("yyyy-MM-dd HH:mm:ss");
        public bool OlderThan(int minutes)
        {
            return DateTime.UtcNow - SuccessTimestamp > new TimeSpan(0, minutes, 0);
        }

        public void Create()
        {
            Prices = new List<CurrencyNew>
                {
                    new CurrencyNew {  CurrencyAbbriviation = "USD", Final = PriceUsd, DiscountPercent = Discount },
                    new CurrencyNew {  CurrencyAbbriviation = "EUR", Final = PriceEur, DiscountPercent = Discount },
                    new CurrencyNew {  CurrencyAbbriviation = "GBP", Final = PriceGbp, DiscountPercent = Discount },
                    new CurrencyNew {  CurrencyAbbriviation = "RUB", Final = PriceRub, DiscountPercent = Discount }
                };
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

        [JsonIgnore]
        public string SteamCurrencyAbbriviation { get; set; }

        [JsonIgnore]
        public int Initial { get; set; }

        [JsonIgnore]
        public int DiscountPercent { get; set; }

        [JsonIgnore]
        public string FormatSellPriceDividedByCurrency { get; set; }

        [JsonIgnore]
        public string FormatBuyPriceDividedByCurrency { get; set; }
    }
}
