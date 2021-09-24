using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Cod;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class MatchHistoryController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("matches")]
        public IActionResult MatchHistory()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            MatchHistory model = new MatchHistory();
            //test commit
            model.Localizations = db.SelectFrontendLocalizations(lang.Id, "matchhistory");
            model.Maps = db.SelectMaps();
            model.Types = db.SelectMatchTypes();

            this.RegisterHit("Match History");
            
            return View("matchhistory", model);
        }
    }

    public class MatchHistoryDataController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("data/matches")]
        public IActionResult MatchHistoryData(long from, long to, string l, string[] types, string[] maps, int ps)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            //Language lang = this.VerifyLanguage(sql, l);

            MatchHistoryData model = new MatchHistoryData();

            try
            {
                model.Data = db.SelectMatchHistoryEntries(TimestampConverter.UnixTimeStampToDateTime(from), TimestampConverter.UnixTimeStampToDateTime(to), types, maps, ps);

                return Json(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR in MatchHistoryDataController: " + ex.Message);

                return StatusCode(500);
            }

        }
    }
}