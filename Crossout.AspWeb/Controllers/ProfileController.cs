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
            model.PvpGames = db.SelectPvpGameCount(id);
            model.WinCount = db.SelectWinCount(id);
            model.WinRate = ((double)model.WinCount / (double)model.GamesRecorded).ToString("P1");
            model.KPB = Math.Round((double)db.SelectPVPKillAssists(id) / (double)model.PvpGames, 2).ToString();
            model.MVPRate = ((double)db.SelectMVPCount(id) / (double)model.PvpGames).ToString("P1");

            TimeSpan time_played = TimeSpan.FromSeconds(db.SelectSecondsPlayed(id));

            if (time_played.Days > 0)
                model.TimePlayed = string.Format("{0}d {1}h", time_played.Days, time_played.Hours);
            else
                model.TimePlayed = string.Format("{0}h {1}m", time_played.Hours, time_played.Minutes);

            Console.WriteLine("done loading user");

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
        public IActionResult OverviewDrilldownData(int id, string l)
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

        [Route("data/profile/gamemodes/{id:long}")]
        public IActionResult ProfileGameModeData(int id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            GameModeDetail model = new GameModeDetail { };

            try
            {
                model = db.PopulateGameModeDetail(id);

                return Json(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MatchPlayerDetailDataController: " + ex.Message);

                return StatusCode(500);
            }
        }

        [Route("data/profile/match_history/{id:long}")]
        public IActionResult ProfileHistoryData(int id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            MatchHistoryDetail model = new MatchHistoryDetail { };

            try
            {
                model = db.PopulateHistoryDetail(id);

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
