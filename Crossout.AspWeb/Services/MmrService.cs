using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.Model.Items;
using Crossout.Model.Recipes;
using Crossout.AspWeb.Models;
using Crossout.AspWeb.Models.EditRecipe;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Items;
using Crossout.AspWeb.Models.Recipes;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Data.PremiumPackages;
using Crossout.AspWeb.Models.Changes;
using Crossout.AspWeb.Models.Language;
using Crossout.AspWeb.Models.Info;
using Crossout.AspWeb.Models.Drafts.BadgeExchange;
using Crossout.AspWeb.Models.Drafts.Snipe;
using Crossout.AspWeb.Pocos;
using NPoco;
using System.Data.Common;
using Crossout.AspWeb.Models.Cod;

namespace Crossout.AspWeb.Services
{
    public class MmrService
    {
        protected SqlConnector DB { get; set; }
        protected IDatabase NPoco { get; set; }

        public MmrService(SqlConnector sql)
        {
            DB = sql;
            NPoco = new Database(sql.CreateConnection());
        }
        public class MmrMatch
        {
            public long match_id { get; set; }
            public int uid { get; set; }
            public int team { get; set; }
            public float result { get; set; }
        }

        public Dictionary<int, int> CalculateAllMmr(string match_type)
        {
            NPoco.Connection.Open();
            Dictionary<int, int> mmr = new Dictionary<int, int> { };
            List<MmrMatch> history = SelectGameTypeHistory(match_type);
            long previous_match = history.FirstOrDefault().match_id;
            List<MmrMatch> current_game = new List<MmrMatch> { };
            float k_factor = 32.0f;

            for (int i = 0; i < history.Count; i++)
            {
                if (!mmr.ContainsKey(history[i].uid))
                    mmr.Add(history[i].uid, 1600);

                if (history[i].match_id != previous_match || i == history.Count - 1)
                {
                    foreach (MmrMatch player in current_game)
                    {
                        float opposition_average_mmr = 0.0f;
                        int opposition_mmr_total = 0;
                        int opposition_player_total = 0;

                        foreach (MmrMatch opponent in current_game.Where(x => x.team != player.team))
                        {
                            opposition_mmr_total += mmr[opponent.uid];
                            opposition_player_total += 1;
                        }

                        opposition_average_mmr = (float)Math.Round((double)opposition_mmr_total / (double)opposition_player_total);

                        float expectation = 1 / (1.0f + (float)Math.Pow(10.0f, (opposition_average_mmr - mmr[player.uid]) / 400.0f));

                        mmr[player.uid] += (int)Math.Round(k_factor * (player.result - expectation));
                    }

                    previous_match = history[i].match_id;
                    current_game = new List<MmrMatch> { };
                }

                current_game.Add(history[i]);
            }

            List<RankPoco> ranks = new List<RankPoco> { };
            foreach (KeyValuePair<int, int> player in mmr)
                ranks.Add(new RankPoco { uid = player.Key, group_id = 0, match_type = match_type, mmr = player.Value });

            NPoco.InsertBulk<RankPoco>(ranks);

            NPoco.Connection.Close();
            return mmr;
        }

        public List<MmrMatch> SelectGameTypeHistory(string match_type)
        {
            List<MmrMatch> drill_down_return = new List<MmrMatch> { };
            
            drill_down_return = NPoco.Fetch<MmrMatch>(@"SELECT record.match_id, player.uid, player.team, CASE record.winning_team WHEN 0 THEN 0.5 WHEN player.team THEN 1 ELSE 0 END as result
                                                          FROM crossout.cod_match_records record
                                                    INNER JOIN crossout.cod_player_round_records player ON record.match_id = player.match_id
                                                         WHERE record.match_type = @0
                                                         ORDER BY record.match_id, player.team", match_type);
            return drill_down_return;
        }
    }
}
