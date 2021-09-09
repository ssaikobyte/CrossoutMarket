using Crossout.AspWeb.Pocos;
using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Models.Cod
{
    public class RoundDamage
    {
        public RoundDamagePoco RoundDamageRecord { get; set; }

        public ItemPoco Item { get; set; }

        public ItemLocalizationPoco ItemLocalization { get; set; }
    }
}
