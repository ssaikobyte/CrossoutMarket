using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Pocos
{
    [TableName("premiumpackageitem")]
    [PrimaryKey("id")]
    public class PremiumPackageItemPoco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("packid")]
        public int PackId { get; set; }

        [Column("itemnumber")]
        public int ItemNumber { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }

}
