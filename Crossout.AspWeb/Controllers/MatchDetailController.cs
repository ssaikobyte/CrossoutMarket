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
    public class MatchDetailController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("match/{id:long}")]
        public IActionResult MatchDetail(long id)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            MatchDetail model = new MatchDetail();

            model.Localizations = db.SelectFrontendLocalizations(lang.Id, "matchdetail");
            model.MatchRecord = db.SelectMatchRecord(id);
            model.Map = db.SelectMap(model.MatchRecord.map_name);
            var matchId = model.MatchRecord.match_id;
            model.RoundRecords = db.SelectRoundRecords(matchId);
            model.PlayerRoundRecords = db.SelectPlayerRoundRecords(matchId);
            model.FormTeams();

            this.RegisterHit("Match History");

            return View("matchdetail", model);
        }
    }

    public class MatchPlayerDetailDataController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public MatchPlayerDetailDataController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("data/match/{id:long}")]
        public IActionResult MatchPlayerDetailData(long id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            MatchPlayerDetailData model = new MatchPlayerDetailData();

            try
            {
                model.DamageData = db.SelectRoundDamage(id, lang.Id);
                model.DamageData.ForEach(x => x.Item?.SetImageExists(pathProvider));

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
