using System;
using System.Collections.Generic;
using System.Text;

namespace Crossout.WorkerCore.Models.SteamAPI
{
    public class AppPriceCollection
    {
        public Dictionary<string, PriceOverview> PriceOverviewByCurrency { get; set; }
    }
}
