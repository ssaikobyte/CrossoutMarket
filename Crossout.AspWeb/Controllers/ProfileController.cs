using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Cod;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class ProfileController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("profile/{id:int}")]
        public IActionResult Profile(int id)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            UserProfile model = new UserProfile { };

            if (!db.ValidUID(id))
                return View("profile", model);

            model.Uid = id;
            model.Nicknames = db.SelectNicknames(id);
            model.Nickname = model.Nicknames.FirstOrDefault();
            model.GamesRecorded = db.SelectRecordedCount(id);
            model.GamesUploaded = db.SelectUploadedCount(id);
            model.TimePlayed = TimeSpan.FromSeconds(db.SelectSecondsPlayed(id));

            this.RegisterHit("profile");

            return View("profile", model);
        }
    }

    public class ProfileDataController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public ProfileDataController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("data/profile/overview_drilldowns/{id:long}")]
        public IActionResult ProfileGameModeData(int id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            OverviewCharts model = new OverviewCharts();

            try
            {
                model = db.SelectOverviewBreakdowns(id);

                return Json(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MatchPlayerDetailDataController: " + ex.Message);

                return StatusCode(500);
            }
        }
    }
}
