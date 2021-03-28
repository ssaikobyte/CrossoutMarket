using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Items
{
    public class OCRDisplayStat
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DisplayType DisplayType { get; set; }
        public bool Displayed = true;
        public int Order { get; set; } = 0;
    }

    public enum DisplayType
    {
        HIDDEN,
        RAW,
        POWERSCORE,
        PERCENT,
        TIMESTAMP,
        VERSION,
        RARITY,
        RATING
    }
}
