﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ViewEngines.Razor;

namespace Crossout.Web
{
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "Zicore.Settings.Json";
            yield return "Zicore.Connector";
            yield return "Crossout.Model";
            yield return "Crossout.Data";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "Zicore.Settings.Json";
            yield return "Zicore.Connector";
            yield return "Crossout.Model";
            yield return "Crossout.Data";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}
