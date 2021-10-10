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


            this.RegisterHit("Profile");

            return View("profile", model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
