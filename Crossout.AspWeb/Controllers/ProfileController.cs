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

            List<string> nicknames = db.SelectNicknames(id);

            model.Uid = id;
            model.Nickname = nicknames.FirstOrDefault();

            if (nicknames.Count > 1)
                model.Nicknames = String.Join(", ", nicknames.Skip(1));
            else
                model.Nicknames = "";

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

        [Route("data/profile/mmr/{id:long}")]
        public IActionResult ReturnPlayerMmr(int id, string l)
        {
            sql.Open(WebSettings.Settings.CreateDescription());

            Console.WriteLine("CALLED MMR SERVICE");

            MmrService db = new MmrService(sql);

            Language lang = this.VerifyLanguage(sql, l);

            try
            {
                Dictionary<int, int> model = db.CalculateAllMmr("8v8");
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
