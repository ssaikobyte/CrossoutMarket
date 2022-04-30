using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.WorkerCore.Models.SteamAPI;
using System.Threading;
using Newtonsoft.Json;
using Crossout.WorkerCore.Helpers;

namespace Crossout.WorkerCore.Tasks
{
    public class SteamAPITask : BaseTask
    {
        public SteamAPITask(string key) : base(key) { }

        private static List<string> currencysToGet = new List<string> { "us", "de", "uk", "ru" };
        private static HttpClient client = new HttpClient();
        private static List<int> appIDsToGet = new List<int>();
        private static Dictionary<int, Dictionary<string, PriceOverview>> appPricesCollection = new Dictionary<int, Dictionary<string, PriceOverview>>();
        private static bool isRunning = false;

        public override async void Workload(SqlConnector sql)
        {
            if (!isRunning)
            {
                isRunning = true;
                string query = "SELECT appid FROM steamprices";
                List<object[]> dataset = new List<object[]>();
                try
                {
                    dataset = sql.SelectDataSet(query);
                }
                catch
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed.");
                    isRunning = false;
                    return;
                }
                appIDsToGet.Clear();
                foreach (var row in dataset)
                {
                    var idToGet = (int)row[0];
                    if (idToGet != 0)
                        appIDsToGet.Add(idToGet);
                }

                await CollectAppPrices();

                foreach (var app in appPricesCollection)
                {
                    if (app.Value.Count == 4)
                    {
                        List<Parameter> parameters = new List<Parameter>();
                        parameters.Add(new Parameter { Identifier = "@appid", Value = app.Key });
                        parameters.Add(new Parameter { Identifier = "@priceusd", Value = app.Value["us"].Final });
                        parameters.Add(new Parameter { Identifier = "@priceeur", Value = app.Value["de"].Final });
                        parameters.Add(new Parameter { Identifier = "@pricegbp", Value = app.Value["uk"].Final });
                        parameters.Add(new Parameter { Identifier = "@pricerub", Value = app.Value["ru"].Final });
                        parameters.Add(new Parameter { Identifier = "@discount", Value = app.Value["us"].DiscountPercent });
                        parameters.Add(new Parameter { Identifier = "@successtimestamp", Value = DateTime.UtcNow });
                        try
                        {
                            var result = sql.ExecuteSQL("UPDATE steamprices SET steamprices.priceusd = @priceusd, steamprices.priceeur = @priceeur, steamprices.pricegbp = @pricegbp, steamprices.pricerub = @pricerub, steamprices.discount = @discount, steamprices.successtimestamp = @successtimestamp WHERE steamprices.appid = @appid", parameters);
                        }
                        catch
                        {
                            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} failed to update DB for app {app.Key}");
                            isRunning = false;
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} couldn't collect all prices from API for app {app.Key}");
                    }
                }
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} finished!");
                isRunning = false;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key} already running, skipping.");
            }
        }

        private async Task<AppPrices> GetAppDetailsAsync(List<int> ids, string currency)
        {
            AppPrices appDetails = new AppPrices();
            var idStringList = string.Join(',', ids);
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://store.steampowered.com/api/appdetails?appids=" + idStringList + "&filters=price_overview&cc=" + currency);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    appDetails.Apps = JsonConvert.DeserializeObject<Dictionary<string, Response>>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {Key}: Couldn't connect to Steam API.");
                Console.WriteLine(e);
            }

            return appDetails;
        }

        private async Task CollectAppPrices(CancellationToken token = new CancellationToken())
        {
            appPricesCollection.Clear();
            foreach (var currencystring in currencysToGet)
            {
                var appDetails = await GetAppDetailsAsync(appIDsToGet, currencystring);

                foreach (var app in appDetails.Apps)
                {
                    if (app.Value.Data != null)
                    {
                        var appId = Convert.ToInt32(app.Key);
                        var priceOverview = app.Value.Data.PriceOverview;
                        if (appPricesCollection.ContainsKey(appId))
                        {
                            appPricesCollection[appId].Add(currencystring, priceOverview);
                        }
                        else
                        {
                            var currencyOverview = new Dictionary<string, PriceOverview>();
                            currencyOverview.Add(currencystring, priceOverview);
                            appPricesCollection.Add(appId, currencyOverview);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(3), token);
            }
        }
    }
}
