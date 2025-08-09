using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zicore.Connector.Base;

namespace Crossout.AspWeb.Controllers
{
    public class CalendarController : Controller
    {
        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        [Route("calendar")]
        public IActionResult Calendar()
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            Language lang = this.ReadLanguageCookie(sql);

            CalendarModel model = new CalendarModel();
            model.Localizations = db.SelectFrontendLocalizations(lang.Id, "calendar");

            this.RegisterHit("Calendar");

            return View("calendar", model);
        }
    }
}
