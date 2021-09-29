using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Cod;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class MatchRewardsOverviewController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("matchrewards")]
        public IActionResult MatchRewardsOverview()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            MatchRewardsOverview model = new MatchRewardsOverview();

            model.Localizations = db.SelectFrontendLocalizations(lang.Id, "matchrewards");
            model.Missions = db.SelectMissions();

            this.RegisterHit("Match Rewards");

            return View("matchrewardsoverview", model);
        }
    }

    public class MatchRewardsOverviewDataController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("data/matchrewards")]
        public IActionResult MatchRewardsOverviewData(int newerthan, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            MatchRewardsOverviewData model = new MatchRewardsOverviewData();

            try
            {
                model.MatchRewards = db.SelectMatchRewards(newerthan);
                model.ResourceBundles = db.SelectResourceBundles(lang.Id);

                return Json(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MatchRewardsOverviewDataController: " + ex.Message);

                return StatusCode(500);
            }
        }
    }
}
