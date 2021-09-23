using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Items;
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
    public class SynergyController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("/data/synergy/{id:int}")]
        public IActionResult Synergy(int id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            Language lang = this.VerifyLanguage(sql, l);

            DataService db = new DataService(sql);

            try
            {
                var model = db.SelectItemSynergy(id, lang.Id);

                return Json(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
