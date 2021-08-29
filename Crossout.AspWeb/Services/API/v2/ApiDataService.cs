using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.AspWeb.Models.API.v2;
using Crossout.AspWeb.Models.Filter;
using ZicoreConnector.Zicore.Connector.Base;
using Crossout.Data.PremiumPackages;
using Crossout.AspWeb.Pocos;
using NPoco;

namespace Crossout.AspWeb.Services.API.v2
{
    public class ApiDataService
    {
        protected SqlConnector DB { get; set; }
        protected IDatabase NPocoDB { get; set; }

        public ApiDataService(SqlConnector sql)
        {
            DB = sql;
            NPocoDB = new Database(sql.CreateConnection());
            NPocoDB.Connection.Open();
        }

        public List<T> CreateApiEntryBase<T>(List<object[]> ds) where T : ApiEntryBase, new()
        {
            var result = new List<T>();

            foreach (var row in ds)
            {
                var entry = new T
                {
                    Id = row[0].ConvertTo<int>(),
                    Name = row[1].ConvertTo<string>(),
                };

                result.Add(entry);
            }

            return result;
        }

        public List<ApiRarityEntry> GetRarities()
        {
            string query = "SELECT rarity.id, rarity.name, rarity.order, rarity.primarycolor, rarity.secondarycolor FROM rarity ORDER BY rarity.id ASC;";
            var ds = DB.SelectDataSet(query);
            List<ApiRarityEntry> list = new List<ApiRarityEntry>();
            foreach (var row in ds)
            {
                int i = 0;
                ApiRarityEntry entry = new ApiRarityEntry
                {
                    Id = Convert.ToInt32(row[i++]),
                    Name = Convert.ToString(row[i++]),
                    Order = Convert.ToInt32(row[i++]),
                    PrimaryColor = Convert.ToString(row[i++]),
                    SecondaryColor = Convert.ToString(row[i++])
                };
                list.Add(entry);
            }
            return list;
        }

        public List<ApiFactionEntry> GetFactions()
        {
            string query = "SELECT id, name FROM faction ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiFactionEntry>(ds);
            return list;
        }

        public List<ApiCategoryEntry> GetCategories()
        {
            string query = "SELECT id, name FROM category ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiCategoryEntry>(ds);
            return list;
        }

        public List<ApiCategoryEntry> GetItemTypes()
        {
            string query = "SELECT id, name FROM type ORDER BY id ASC;";
            var ds = DB.SelectDataSet(query);
            var list = CreateApiEntryBase<ApiCategoryEntry>(ds);
            return list;
        }

        public List<ApiPackEntry> GetPacks()
        {
            List<ApiPackEntry> list = new List<ApiPackEntry>();
            List<PremiumPackage> packages = CrossoutDataService.Instance.PremiumPackagesCollection.Packages;
            List<int> containedItemIDs = new List<int>();
            foreach (var pack in packages)
            {
                containedItemIDs.AddRange(pack.MarketPartIDs);
            }
            Dictionary<int, ContainedItem> containedItems = SelectItemsByID(DB, containedItemIDs);
            string query = "SELECT steamprices.id, steamprices.appid, steamprices.priceusd, steamprices.priceeur, steamprices.pricegbp, steamprices.pricerub, steamprices.discount, steamprices.successtimestamp FROM steamprices ORDER BY steamprices.id ASC";
            var ds = DB.SelectDataSet(query);
            foreach (var row in ds)
            {
                var apiPackEntry = new ApiPackEntry();
                apiPackEntry.Create(packages, row, containedItems);

                list.Add(apiPackEntry);
            }
            return list;
        }

        public List<OCRStatItemPoco> GetOCRStats(bool onlynewest, int? id = null)
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            if (id != null)
                sqlBuilder.Where("t1.itemnumber = @0", id);
            if (onlynewest)
                sqlBuilder.Join("(SELECT itemnumber, MAX(timestamp) timestamp FROM ocrstats GROUP BY itemnumber) t2 ON t1.itemnumber = t2.itemnumber AND t1.timestamp = t2.timestamp");
            var template = sqlBuilder.AddTemplate("SELECT t1.* FROM crossout.ocrstats t1 /**join**/ WHERE /**where**/");
            var ocrStatItems = NPocoDB.Fetch<OCRStatItemPoco>(template);
            ocrStatItems.ForEach(x => x.CreateDisplayStats());
            return ocrStatItems;
        }

        public List<SynergyPoco> GetSynergies(int? id = null)
        {
            if (id == null)
                return NPocoDB.Fetch<SynergyPoco>();
            else
                return NPocoDB.Fetch<SynergyPoco>("WHERE itemnumber = @0", id);
        }

        public List<UploadRecordPoco> GetCodUploadRecords(int uid)
        {
            return NPocoDB.Fetch<UploadRecordPoco>("WHERE uid = @0", uid);
        }

        public int UploadMatchs(List<MatchEntry> match_list)
        {
            int uploader_id = 0;

            foreach (MatchEntry match in match_list)
            {
                if (NPocoDB.Fetch<UploadRecordPoco>("WHERE MATCH_ID=@0 AND UID=@1", match.match_id, match.uploader_uid).Any())
                    continue;

                uploader_id = match.uploader_uid;

                string status = "I";

                if (!ValidMatchUpload(match))
                    status = "C";

                Console.WriteLine("match_id:" + match.match_id.ToString());

                UploadRecordPoco new_uploader = PopulateUploadPoco(match);
                MatchRecordPoco new_match = PopulateMatchPoco(match, status);
                List<RoundRecordPoco> new_rounds = PopulateRoundsPoco(match);

                UploadUploadRecords(new_uploader);
                UploadMatch(new_match);//test
                UploadRounds(new_rounds, match);

                List<PlayerRoundRecordPoco> new_players = PopulatePlayerRoundRecordPoco(match);
                UploadPlayerRoundRecords(new_players);
            }

            return NPocoDB.ExecuteScalar<int>("SELECT COUNT(*) FROM CROSSOUT.COD_UPLOAD_RECORDS WHERE UID = @0", uploader_id);
        }

        public void UploadMatch(MatchRecordPoco match)
        {
            try
            {
                NPocoDB.Insert(match);
            }
            catch (Exception ex)
            {
                Console.WriteLine("db upload match error:" + ex.Message);
            }
        }

        public void UploadRounds(List<RoundRecordPoco> rounds, MatchEntry match)
        {
            try
            {
                foreach (RoundRecordPoco round in rounds)
                    NPocoDB.Insert(round);

                MatchRecordPoco poco_match = NPocoDB.SingleOrDefaultById<MatchRecordPoco>(match.match_id);
                List<RoundRecordPoco> poco_rounds = NPocoDB.Fetch<RoundRecordPoco>("WHERE MATCH_ID = @0", match.match_id);

                foreach (RoundRecordPoco poco_round in poco_rounds)
                {
                    if (poco_match.round_id_1 == 0)
                        poco_match.round_id_1 = poco_round.round_id;
                    else
                    if (poco_match.round_id_2 == 0)
                        poco_match.round_id_2 = poco_round.round_id;
                    else
                    if (poco_match.round_id_3 == 0)
                        poco_match.round_id_3 = poco_round.round_id;
                    else
                        Console.WriteLine("populated 3 rounds");
                }

                NPocoDB.Update(poco_match);
            }
            catch (Exception ex)
            {
                Console.WriteLine("db upload round error:" + ex.Message);
            }
        }

        public void UploadUploadRecords(UploadRecordPoco upload_record)
        {
            try
            {
                NPocoDB.Insert(upload_record);
            }
            catch (Exception ex)
            {
                Console.WriteLine("db upload upload error:" + ex.Message);
            }
        }

        public void UploadPlayerRoundRecords(List<PlayerRoundRecordPoco> players)
        {
            try
            {
                foreach (PlayerRoundRecordPoco player in players)
                {
                    if (player.uid == 0)
                        continue;

                    NPocoDB.Insert(player);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("db upload player error:" + ex.Message);

                foreach (PlayerRoundRecordPoco player in players)
                    Console.WriteLine(string.Format("{0},{1},{2},{3}", player.nickname, player.uid, player.round_id, player.match_id));

            }
        }

        public MatchRecordPoco PopulateMatchPoco(MatchEntry match, string status)
        {
            MatchRecordPoco poco_match = new MatchRecordPoco { };
            poco_match.match_id = match.match_id;
            poco_match.upload_status = status;
            poco_match.validation_count = 1;
            poco_match.match_type = match.match_type;
            poco_match.match_start = match.match_start;
            poco_match.match_end = match.match_end;
            poco_match.map_name = match.map_name;
            poco_match.winning_team = match.winning_team;
            poco_match.round_id_1 = 0;
            poco_match.round_id_2 = 0;
            poco_match.round_id_3 = 0;
            poco_match.client_version = match.client_version;
            poco_match.co_driver_version = match.co_driver_version;
            poco_match.server_ip = match.game_server;

            int min_power_score = int.MaxValue;
            int max_power_score = int.MinValue;

            foreach (RoundEntry round in match.rounds)
            {
                foreach (MatchPlayerEntry player in round.players)
                {
                    if (player.power_score == 0)
                        continue;

                    if (player.power_score < min_power_score)
                        min_power_score = player.power_score;

                    if (player.power_score > max_power_score)
                        max_power_score = player.power_score;
                }
            }

            poco_match.min_power_score = min_power_score;
            poco_match.max_power_score = max_power_score;

            return poco_match;
        }

        public List<RoundRecordPoco> PopulateRoundsPoco(MatchEntry match)
        {
            List<RoundRecordPoco> poco_rounds = new List<RoundRecordPoco> { };

            foreach (RoundEntry round in match.rounds)
            {
                RoundRecordPoco poco_round = new RoundRecordPoco { };
                poco_round.match_id = match.match_id;
                poco_round.round_start = round.round_start;
                poco_round.round_end = round.round_end;
                poco_round.winning_team = round.winning_team;
                poco_rounds.Add(poco_round);
            }

            return poco_rounds;
        }

        public UploadRecordPoco PopulateUploadPoco(MatchEntry match)
        {
            UploadRecordPoco poco_upload = new UploadRecordPoco { };
            poco_upload.match_id = match.match_id;
            poco_upload.uid = match.uploader_uid;
            poco_upload.upload_time = DateTime.Now.ToUniversalTime();
            return poco_upload;
        }

        public List<PlayerRoundRecordPoco> PopulatePlayerRoundRecordPoco(MatchEntry match)
        {
            List<PlayerRoundRecordPoco> poco_players = new List<PlayerRoundRecordPoco> { };
            MatchRecordPoco poco_match = NPocoDB.SingleOrDefaultById<MatchRecordPoco>(match.match_id);

            foreach (RoundEntry round in match.rounds)
            {
                foreach (MatchPlayerEntry player in round.players)
                {
                    PlayerRoundRecordPoco poco_player = new PlayerRoundRecordPoco { };
                    poco_player.match_id = match.match_id;
                    poco_player.round_id = poco_match.round_id_1;
                    poco_player.uid = player.uid;
                    poco_player.nickname = player.nickname;
                    poco_player.team = player.team;
                    poco_player.build_hash = player.build_hash;
                    poco_player.power_score = player.power_score;
                    poco_player.kills = player.kills;
                    poco_player.assists = player.assists;
                    poco_player.drone_kills = player.drone_kills;
                    poco_player.score = player.score;
                    poco_player.damage = (float)player.damage;
                    poco_player.damage_taken = (float)player.damage_taken;

                    if (player.round_id == 1)
                        poco_player.round_id = poco_match.round_id_2;
                    else
                    if (player.round_id == 2)
                        poco_player.round_id = poco_match.round_id_3;

                    if (player.team == match.winning_team)
                        poco_player.game_result = "W";
                    else
                    if (match.winning_team == 0)
                        poco_player.game_result = "D";
                    else
                        poco_player.game_result = "L";

                    poco_players.Add(poco_player);
                }
            }

            return poco_players;
        }

        public bool ValidMatchUpload(MatchEntry match)
        {
            try
            {
                if (match.match_id <= 0)
                    return false;

                if (match.uploader_uid <= 0)
                    return false;

                if (match.match_start < new DateTime(2015, 4, 5))
                    return false;

                if (match.match_start > DateTime.Today.AddDays(3))
                    return false;

                if (match.match_end < new DateTime(2015, 4, 5))
                    return false;

                if (match.match_end > DateTime.Today.AddDays(3))
                    return false;

                if (String.IsNullOrWhiteSpace(match.map_name))
                    return false;

                if (match.winning_team < -1)
                    return false;

                if (match.win_conidtion < 0)
                    return false;

                if (String.IsNullOrWhiteSpace(match.client_version))
                    return false;

                if (String.IsNullOrWhiteSpace(match.co_driver_version))
                    return false;

                if (String.IsNullOrWhiteSpace(match.game_server))
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static ApiItemEntry CreateApiItem(object[] row)
        {
            int i = 0;
            ApiItemEntry item = new ApiItemEntry
            {
                Id = row[i++].ConvertTo<int>(),
                Name = row[i++].ConvertTo<string>(),
                SellPrice = row[i++].ConvertTo<decimal>(),
                BuyPrice = row[i++].ConvertTo<decimal>(),
                SellOffers = row[i++].ConvertTo<int>(),
                BuyOrders = row[i++].ConvertTo<int>(),
                Timestamp = row[i++].ConvertTo<DateTime>(),
                RarityId = row[i++].ConvertTo<int>(),
                RarityName = row[i++].ConvertTo<string>(),
                CategoryId = row[i++].ConvertTo<int>(),
                CategoryName = row[i++].ConvertTo<string>(),
                TypeId = row[i++].ConvertTo<int>(),
                TypeName = row[i++].ConvertTo<string>(),
                RecipeId = row[i++].ConvertTo<int>(),
                Removed = row[i++].ConvertTo<int>(),
                FactionNumber = row[i++].ConvertTo<int>(),
                Faction = row[i++].ConvertTo<string>(),
                Popularity = row[i++].ConvertTo<int>(),
                WorkbenchRarity = row[i].ConvertTo<int>(),
            };

            return item;
        }

        // Search

        public static List<RarityItem> SelectRarities(SqlConnector sql)
        {
            List<RarityItem> items = new List<RarityItem>();

            var ds = sql.SelectDataSet("SELECT rarity.id, rarity.name, rarity.order, rarity.primarycolor, rarity.secondarycolor FROM rarity ORDER BY rarity.id ASC");

            foreach (var row in ds)
            {
                items.Add(RarityItem.Create(row));
            }

            return items;
        }

        public static List<FilterItem> SelectFactions(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM faction");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }

        public static List<FilterItem> SelectCategories(SqlConnector sql)
        {
            List<FilterItem> items = new List<FilterItem>();

            var ds = sql.SelectDataSet("SELECT id,name FROM category");

            foreach (var row in ds)
            {
                items.Add(FilterItem.Create(row));
            }

            return items;
        }

        public static Dictionary<int, ContainedItem> SelectItemsByID(SqlConnector sql, List<int> ids)
        {
            Dictionary<int, ContainedItem> items = new Dictionary<int, ContainedItem>();
            string query = BuildItemsQueryFromIDList(ids);
            var ds = sql.SelectDataSet(query);

            foreach (var row in ds)
            {
                ContainedItem item = new ContainedItem();
                int i = 0;
                item.Id = row[i++].ConvertTo<int>();
                item.Name = row[i++].ConvertTo<string>();
                item.SellPrice = row[i++].ConvertTo<int>();
                item.BuyPrice = row[i++].ConvertTo<int>();
                items.Add(item.Id, item);
            }

            return items;
        }

        public static string BuildSearchQuery(bool hasFilter, bool limit, bool count, bool hasId, bool hasRarity, bool hasCategory, bool hasFaction, bool showRemovedItems, bool showMetaItems)
        {
            string selectColumns = "item.id,item.name,item.sellprice,item.buyprice,item.selloffers,item.buyorders,item.datetime,rarity.id,rarity.name,category.id,category.name,type.id,type.name,recipe.id,item.removed,faction.id,faction.name,item.popularity,item.workbenchrarity";
            if (count)
            {
                selectColumns = "count(*)";
            }
            string query = $"SELECT {selectColumns} FROM item LEFT JOIN rarity on rarity.id = item.raritynumber LEFT JOIN category on category.id = item.categorynumber LEFT JOIN type on type.id = item.typenumber LEFT JOIN recipe ON recipe.itemnumber = item.id LEFT JOIN faction ON faction.id = recipe.factionnumber ";

            if (!hasId)
            {
                if (hasFilter)
                {
                    query += "WHERE item.name LIKE @filter ";
                }
                else
                {
                    query += "WHERE 1=1 ";
                }
            }
            else
            {
                query += "WHERE item.id = @id ";
            }

            if (hasRarity)
            {
                query += " AND rarity.id = @rarity ";
            }

            if (hasCategory)
            {
                query += " AND category.id = @category ";
            }

            if (hasFaction)
            {
                query += " AND faction.id = @faction ";
            }

            if (!showRemovedItems)
            {
                query += " AND item.removed = 0 ";
            }

            if (!showMetaItems)
            {
                query += " AND item.meta = 0 ";
            }

            if (!count)
            {
                query += "ORDER BY item.id asc, item.name asc ";
            }

            return query;
        }

        public static string BuildItemsQueryFromIDList(List<int> ids)
        {
            StringBuilder sb = new StringBuilder();
            string query = "SELECT item.id, item.name, item.sellprice, item.buyprice FROM item WHERE ";
            sb.Append(query);
            int i = 0;
            foreach (var id in ids)
            {
                if (i == 0)
                {
                    sb.Append("id=");
                    sb.Append(id);
                }
                else
                {
                    sb.Append(" OR id=");
                    sb.Append(id);
                }
                i++;
            }
            query = sb.ToString();
            return query;
        }
    }
}
