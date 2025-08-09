﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.Web.Models;
using Crossout.Web.Models.Items;
using Crossout.Web.Services;
using Nancy;
using Zicore.Connector.Base;

namespace Crossout.Web.Modules.Search
{
    public class CompareModule : NancyModule
    {
        public CompareModule()
        {
            Get["/compare/(?<ids>.*)"] = x =>
            {
                return RouteCompare(x);
            };
        }

        SqlConnector sql = new SqlConnector(ConnectionType.MySql);

        private dynamic RouteCompare(dynamic items)
        {
            var result = new List<int>();
            var idsString = (string)items.ids;

            var ids = idsString.Split(',');

            foreach (var id in ids)
            {
                int foundId;
                if (int.TryParse(id, out foundId))
                {
                    if (foundId > 0)
                    {
                        result.Add(foundId);
                    }
                }
            }

            try
            {
                sql.Open(WebSettings.Settings.CreateDescription());

                DataService db = new DataService(sql);

                var itemList = new List<Item>();

                foreach (var id in result)
                {
                    var itemModel = db.SelectItem(id, true);
                    CrossoutDataService.Instance.AddData(itemModel.Item);
                    itemList.Add(itemModel.Item);
                }
                var itemCol = new ItemCollection();
                itemCol.Items = itemList;

                itemCol.CreateStatList();
                itemCol.AllItems = db.SelectAllActiveItems();

                return View["compare", itemCol];
            }
            catch
            {
                return Response.AsRedirect("/");
            }
        }
    }
}
