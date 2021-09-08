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
            model.RoundDamages = db.SelectRoundDamage(matchId);
            model.PlayerRoundRecords = db.SelectPlayerRoundRecords(matchId);

            this.RegisterHit("Match History");

            return View("matchdetail", model);
        }
    }
}
