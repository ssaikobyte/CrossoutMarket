using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("premiumpackage")]
    [PrimaryKey("id")]
    public class PremiumPackagePoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("key")]
        public string Key { get; set; }

        [Column("appid")]
        public int SteamAppID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("coins")]
        public int RawCoins { get; set; }
    }

}
