using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crossout.Data.PremiumPackages;
using NLog;
using Newtonsoft.Json;

namespace Crossout.Data
{
    //TODO: Remove references from Crossout.Web project
    public class PremiumPackagesColletionOLD
    {
        private Logger Log = LogManager.GetCurrentClassLogger();

        public List<PremiumPackageOLD> Packages { get; private set; } = new List<PremiumPackageOLD>();
        
        public PremiumPackagesColletionOLD()
        {
            
        }

        public void ReadPackages(string directory)
        {
            PremiumPackageOLD package = new PremiumPackageOLD();
            DirectoryInfo packageDir = new DirectoryInfo(directory);

            foreach (var file in packageDir.GetFiles())
            {
                StreamReader sr = new StreamReader(file.FullName);

                string output = sr.ReadToEnd();

                package = JsonConvert.DeserializeObject<PremiumPackageOLD>(output);
                Packages.Add(package);
            }

            Packages = Packages.OrderBy(x => x.Name).ToList();
        }
    }
}
