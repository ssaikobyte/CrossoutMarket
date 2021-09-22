using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.AspWeb.Helper;
using Crossout.Data.PremiumPackages;
using Crossout.Model.Formatter;
using Crossout.Web;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Services;
using Microsoft.AspNetCore.Mvc;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.AspWeb.Models.Language;

namespace Crossout.AspWeb.Controllers
{
    public class PremiumPackagesController : Controller
    {
        private readonly RootPathHelper pathProvider;

        public PremiumPackagesController(RootPathHelper pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        [Route("packs")]
        public IActionResult Packages(int id)
        {
            Language lang = this.ReadLanguageCookie(sql);
            this.RegisterHit("Packs");

            sql.Open(WebSettings.Settings.CreateDescription());

            DataService db = new DataService(sql);

            try
            {
                PremiumPackageColletion packagesCollection = db.SelectAllPremiumPackages(lang.Id);
                packagesCollection.Packages.ForEach(x => x.Create());

                packagesCollection.Localizations = db.SelectFrontendLocalizations(lang.Id, "packs");

                packagesCollection.Status = db.SelectStatus();
                return View("packages", packagesCollection);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Redirect("/");
            }
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);
    }
}