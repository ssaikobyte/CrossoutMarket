using Crossout.AspWeb.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.General
{
    public class CalendarModel : BaseViewModel, IViewTitle
    {
        public string Title => "Calendar";
    }
}
