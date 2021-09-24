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

        public UploadReturn GetCodUploadRecords(int uid)
        {
            UploadReturn upload_return = new UploadReturn { };

            upload_return.uploaded_matches = NPocoDB.Fetch<UploadPoco>("WHERE uid = @0", uid).Select(x => x.match_id).ToList();
            upload_return.uploaded_builds = NPocoDB.ExecuteScalar<int>("SELECT COUNT(*) FROM CROSSOUT.COD_BUILD_UPLOAD_RECORD WHERE UID = @0", uid);

            return upload_return;
        }

        public UploadReturn UploadMatchsAndBuilds(UploadEntry upload_entry)
        {
            UploadReturn upload_return = new UploadReturn { uploaded_matches = new List<long> { }, uploaded_builds = 0 };

            if (upload_entry.match_list.Count == 0 && upload_entry.build_list.Count == 0)
                return upload_return;

            NPocoDB.BeginTransaction();

            foreach (MatchEntry match in upload_entry.match_list)
            {
                if (UploadExists(match.match_id, upload_entry.uploader_uid))
                    continue;

                UploadResources(match, upload_entry.uploader_uid);

                if (MatchExists(match.match_id))
                {
                    UploadUploadRecord(match, ValidMatch(match) ? "V" : "C", upload_entry.uploader_uid);
                    continue;
                }

                UploadMap(match);
                UploadMatch(match);
                UploadRounds(match);
                UploadPlayerRoundRecords(match);
                UploadDamageRecords(match);
                UploadScores(match);
                UploadMedals(match);
                UploadGroups(match);
                UploadUploadRecord(match, MatchContainsError(match.match_id) ? "C" : "I", upload_entry.uploader_uid);
            }

            foreach (BuildEntry build in upload_entry.build_list)
            {
                if (!BuildExists(build.build_hash, build.power_score))
                    UploadBuild(build);

                UploadBuildParts(build);
                UploadBuildUploadRecord(build, upload_entry.uploader_uid);
            }

            upload_return = GetCodUploadRecords(upload_entry.uploader_uid);

            NPocoDB.CompleteTransaction();

            return upload_return;
        }

        public int GetUploadCount(int uploader_uid)
        {
            int upload_count = 0;

            try
            {
                upload_count = NPocoDB.ExecuteScalar<int>("SELECT COUNT(*) FROM CROSSOUT.COD_UPLOAD_RECORDS WHERE UID = @0", uploader_uid);
            }
            catch (Exception ex)
            {
                WriteErrorLog(0,"db upload count error:" + ex.Message);
            }

            return upload_count;
        }

        public int GetGroupId(GroupPoco group)
        {
            int group_id = 0;

            try
            {
                group_id = NPocoDB.Fetch<GroupPoco>("WHERE UID_1 = @0 AND UID_2 = @1 AND UID_3 = @2 AND UID_4 = @3", group.uid_1, group.uid_2, group.uid_3, group.uid_4).FirstOrDefault().group_id;
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db group id error:" + ex.Message);
            }

            return group_id;
        }

        public bool MatchContainsError(long match_id)
        {
            bool match_error = false;

            try
            {
                if (NPocoDB.Fetch<UploadPoco>("WHERE MATCH_ID = @0", match_id).Any())
                    match_error = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(match_id, "db match check error:" + ex.Message);
            }

            return match_error;
        }

        public bool UploadExists(long match_id, int uploader_uid)
        {
            bool upload_exists = false;

            try
            {
                if (NPocoDB.Fetch<UploadPoco>("WHERE MATCH_ID = @0 AND UID = @1", match_id, uploader_uid).Any())
                    upload_exists = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(match_id, "db upload existance error:" + ex.Message);
            }

            return upload_exists;
        }

        public bool BuildExists(string build_hash, int power_score)
        {
            bool build_exists = false;

            try
            {
                if (NPocoDB.Fetch<BuildPoco>("WHERE BUILD_HASH = @0 AND POWER_SCORE = @1", build_hash, power_score).Any())
                    build_exists = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db build existance error:" + ex.Message);
            }

            return build_exists;
        }

        public bool MatchExists(long match_id)
        {
            bool match_exists = false;

            try
            {
                if (NPocoDB.SingleOrDefaultById<MatchPoco>(match_id) != null)
                    match_exists = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(match_id, "db match existance error:" + ex.Message);
            }

            return match_exists;
        }
        public bool GroupExists(GroupPoco group)
        {
            bool group_exists = false;

            try
            {
                if (NPocoDB.Fetch<GroupPoco>("WHERE UID_1 = @0 AND UID_2 = @1 AND UID_3 = @2 AND UID_4 = @3", group.uid_1, group.uid_2, group.uid_3, group.uid_4).Any())
                    group_exists = true;
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db group existance error:" + ex.Message);
            }

            return group_exists;
        }

        private class group_record
        {
            public int group_id { get; set; }
            public List<int> uids { get; set; }
        }

        public void UploadGroups(MatchEntry match)
        {
            try
            {
                MatchPoco poco_match = NPocoDB.SingleOrDefaultById<MatchPoco>(match.match_id);
                List<PlayerRoundPoco> poco_players = NPocoDB.Fetch<PlayerRoundPoco>("WHERE MATCH_ID = @0", match.match_id);
                List<group_record> groups = new List<group_record> { };

                foreach (RoundEntry round in match.rounds)
                {
                    foreach (MatchPlayerEntry player in round.players.Where(x => x.group_id > 0))
                    {
                        if (!groups.Any(x => x.group_id == player.group_id))
                            groups.Add(new group_record { group_id = player.group_id, uids = new List<int> { } });

                        if (!groups.Any(x => x.group_id == player.group_id && x.uids.Contains(player.uid)))
                            groups.First(x => x.group_id == player.group_id).uids.Add(player.uid);
                    }
                }

                foreach (group_record group in groups)
                {
                    group.uids.Sort();

                    MatchGroupPoco match_group_poco = new MatchGroupPoco { };
                    GroupPoco group_poco = new GroupPoco { };
                    int group_id = 0;
                    
                    group_poco.uid_1 = 0;
                    group_poco.uid_2 = 0;
                    group_poco.uid_3 = 0;
                    group_poco.uid_4 = 0;

                    foreach (int uid in group.uids)
                    {
                        if (group_poco.uid_1 == 0)
                            group_poco.uid_1 = uid;
                        else
                        if (group_poco.uid_2 == 0)
                            group_poco.uid_2 = uid;
                        else
                        if (group_poco.uid_3 == 0)
                            group_poco.uid_3 = uid;
                        else
                        if (group_poco.uid_4 == 0)
                            group_poco.uid_4 = uid;
                    }

                    if (!GroupExists(group_poco))
                        NPocoDB.Insert(group_poco);

                    group_id = GetGroupId(group_poco);

                    if (group_id != 0)
                    {
                        match_group_poco.match_id = match.match_id;
                        match_group_poco.group_id = group_id;
                        NPocoDB.Insert(match_group_poco);
                    }

                    foreach (PlayerRoundPoco poco_player in poco_players)
                    {
                        if (!group.uids.Contains(poco_player.uid))
                            continue;

                        poco_player.group_id = group_id;
                        NPocoDB.Update(poco_player);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload group error:" + ex.Message);
            }
        }

        public void UploadMap(MatchEntry match)
        {
            try
            {
                MapPoco map = NPocoDB.SingleOrDefaultById<MapPoco>(match.map_name);

                if (map == null)
                {
                    map = new MapPoco { };
                    map.map_name = match.map_name;
                    map.map_display_name = match.map_display_name;
                    NPocoDB.Insert(map);
                }
                else
                if (map.map_name == map.map_display_name && match.map_name != match.map_display_name)
                {
                    map.map_display_name = match.map_display_name;
                    NPocoDB.Update(map);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload map error:" + ex.Message);
            }
        }

        public void UploadScores(MatchEntry match)
        {
            try
            {
                foreach (RoundEntry round in match.rounds)
                {
                    foreach (MatchPlayerEntry player in round.players)
                    {
                        if (player.uid == 0)
                            continue;

                        foreach (ScoreEntry score in player.scores)
                        {
                            MatchScorePoco poco_score = new MatchScorePoco { };
                            poco_score.match_id = match.match_id;
                            poco_score.round_id = round.round_id;
                            poco_score.uid = player.uid;
                            poco_score.score_type = score.score_type;
                            poco_score.score = score.points;
                            NPocoDB.Insert(poco_score);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload score error:" + ex.Message);
            }
        }

        public void UploadMedals(MatchEntry match)
        {
            try
            {
                foreach (RoundEntry round in match.rounds)
                {
                    foreach (MatchPlayerEntry player in round.players)
                    {
                        if (player.uid == 0)
                            continue;

                        foreach (MedalEntry medal in player.medals)
                        {
                            MatchMedalPoco poco_medal = new MatchMedalPoco { };
                            poco_medal.match_id = match.match_id;
                            poco_medal.round_id = round.round_id;
                            poco_medal.uid = player.uid;
                            poco_medal.medal = medal.medal;
                            poco_medal.amount = medal.amount;
                            NPocoDB.Insert(poco_medal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload medal error:" + ex.Message);
            }
        }

        public void UploadResources(MatchEntry match, int uploader_uid)
        {
            try
            {
                foreach (ResourceEntry resource in match.resources)
                {
                    MatchResourcePoco poco_resource = new MatchResourcePoco { };
                    poco_resource.match_id = match.match_id;
                    poco_resource.uid = uploader_uid;
                    poco_resource.resource = resource.resource;
                    poco_resource.amount = resource.amount;
                    NPocoDB.Insert(poco_resource);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload resource error:" + ex.Message);
            }
        }

        public void UploadDamageRecords(MatchEntry match)
        {
            try
            {
                foreach (RoundEntry round in match.rounds)
                {
                    foreach(RoundDamageEntry damage_record in round.damage_records)
                    {
                        RoundDamagePoco damage_record_poco = new RoundDamagePoco { };
                        damage_record_poco.match_id = match.match_id;
                        damage_record_poco.round_id = round.round_id;
                        damage_record_poco.uid = damage_record.uid;
                        damage_record_poco.weapon = damage_record.weapon;
                        damage_record_poco.damage = (float)damage_record.damage;
                        NPocoDB.Insert(damage_record_poco);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload damage error:" + ex.Message);
            }
        }

        public void UploadBuild(BuildEntry build)
        {
            try
            {
                BuildPoco build_poco = new BuildPoco { };
                build_poco.build_hash = build.build_hash;
                build_poco.power_score = build.power_score;

                NPocoDB.Insert(build_poco);
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db upload round error:" + ex.Message);
            }
        }

        public int GetBuildId(BuildEntry build)
        {
            int build_id = 0;

            try
            {
                BuildPoco build_poco = NPocoDB.First<BuildPoco>("WHERE BUILD_HASH = @0 AND POWER_SCORE = @1", build.build_hash, build.power_score);
                build_id = build_poco.build_id;
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db get build id error:" + ex.Message);
            }

            return build_id;
        }

        public void UploadBuildParts(BuildEntry build)
        {
            try
            {
                BuildPoco build_poco = NPocoDB.First<BuildPoco>("WHERE BUILD_HASH = @0 AND POWER_SCORE = @1", build.build_hash, build.power_score);
                List<BuildPartPoco> build_parts_poco = NPocoDB.Fetch<BuildPartPoco>("WHERE BUILD_ID = @0", build_poco.build_id);
                
                foreach (string part in build.parts)
                {
                    if (build_parts_poco.Any(x => x.part_name == part))
                        continue;

                    if (!NPocoDB.Fetch<ItemPoco>("WHERE externalKey = @0", part).Any())
                        continue;

                    BuildPartPoco build_part_poco = new BuildPartPoco { };
                    build_part_poco = new BuildPartPoco { };
                    build_part_poco.build_id = build_poco.build_id;
                    build_part_poco.part_name = part;
                    NPocoDB.Insert(build_part_poco);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db upload build part error:" + ex.Message);
            }
        }

        public void UploadBuildUploadRecord(BuildEntry build, int uid)
        {
            try
            {
                BuildUploadPoco build_upload_poco = new BuildUploadPoco { };
                build_upload_poco.uid = uid;
                build_upload_poco.build_hash = build.build_hash;
                build_upload_poco.power_score = build.power_score;
                build_upload_poco.part_count = build.parts.Count;
                
                NPocoDB.Insert(build_upload_poco);
            }
            catch (Exception ex)
            {
                WriteErrorLog(0, "db upload build upload error:" + ex.Message);
            }
        }

        public void UploadMatch(MatchEntry match)
        {
            try
            {
                MatchPoco poco_match = new MatchPoco { };
                poco_match.match_id = match.match_id;
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

                NPocoDB.Insert(poco_match);
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload round error:" + ex.Message);
            }
        }

        public void UploadRounds(MatchEntry match)
        {
            try
            {
                MatchPoco poco_match = NPocoDB.SingleOrDefaultById<MatchPoco>(match.match_id);

                foreach (RoundEntry round in match.rounds)
                {
                    RoundPoco poco_round = new RoundPoco { };
                    poco_round.match_id = match.match_id;
                    poco_round.round_start = round.round_start;
                    poco_round.round_end = round.round_end;
                    poco_round.winning_team = round.winning_team;
                    NPocoDB.Insert(poco_round);

                    if (poco_match.round_id_1 == 0)
                        poco_match.round_id_1 = poco_round.round_id;
                    else
                    if (poco_match.round_id_2 == 0)
                        poco_match.round_id_2 = poco_round.round_id;
                    else
                    if (poco_match.round_id_3 == 0)
                        poco_match.round_id_3 = poco_round.round_id;

                }

                NPocoDB.Update(poco_match);
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload round error:" + ex.Message);
            }
        }

        public void UploadUploadRecord(MatchEntry match, string status, int uploader_uid)
        {
            try
            {
                UploadPoco poco_upload = new UploadPoco { };
                poco_upload.match_id = match.match_id;
                poco_upload.uid = uploader_uid;
                poco_upload.upload_time = DateTime.Now.ToUniversalTime();
                poco_upload.status = status;
                NPocoDB.Insert(poco_upload);
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload upload error:" + ex.Message);
            }
        }

        public void UploadPlayerRoundRecords(MatchEntry match)
        {
            try
            {
                MatchPoco poco_match = NPocoDB.SingleOrDefaultById<MatchPoco>(match.match_id);

                foreach (RoundEntry round in match.rounds)
                {
                    foreach (MatchPlayerEntry player in round.players)
                    {
                        if (player.uid == 0)
                            continue;

                        PlayerRoundPoco poco_player = new PlayerRoundPoco { };
                        poco_player.match_id = match.match_id;
                        poco_player.round_id = poco_match.round_id_1;
                        poco_player.uid = player.uid;
                        poco_player.nickname = player.nickname;
                        poco_player.group_id = 0;
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

                        NPocoDB.Insert(poco_player);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db upload record error:" + ex.Message);
            }
        }

        public bool ValidMatch(MatchEntry match)
        {
            bool valid_match = true;
            try
            {
                MatchPoco poco_match = NPocoDB.SingleOrDefaultById<MatchPoco>(match.match_id);

                if (match.map_name != poco_match.map_name)
                {
                    Console.WriteLine(string.Format("map_name discrepency ({0})-({1})", match.map_name, poco_match.map_name));
                    valid_match = false;
                }

                if (match.match_type != poco_match.match_type)
                {
                    Console.WriteLine(string.Format("match_type discrepency ({0})-({1})", match.match_type, poco_match.match_type));
                    valid_match = false;
                }

                if (match.match_start.Date != poco_match.match_start.Date)
                {
                    Console.WriteLine(string.Format("match_start discrepency ({0})-({1})", match.match_start, poco_match.match_start));
                    valid_match = false;
                }

                if (match.match_end.Date != poco_match.match_end.Date)
                {
                    Console.WriteLine(string.Format("match_end discrepency ({0})-({1})", match.match_end, poco_match.match_end));
                    valid_match = false;
                }

                if (match.winning_team != poco_match.winning_team)
                {
                    Console.WriteLine(string.Format("winning_team discrepency ({0})-({1})", match.winning_team, poco_match.winning_team));
                    valid_match = false;
                }

                if (match.rounds.ElementAtOrDefault(0) != null)
                {
                    if (!ValidRound(match.rounds[0], poco_match.round_id_1))
                    {
                        Console.WriteLine(string.Format("round validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }

                    if (!ValidPlayers(match.rounds[0].players, poco_match.match_id, poco_match.round_id_1))
                    {
                        Console.WriteLine(string.Format("player validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }
                }

                if (match.rounds.ElementAtOrDefault(1) != null)
                {
                    if (!ValidRound(match.rounds[1], poco_match.round_id_2))
                    {
                        Console.WriteLine(string.Format("round validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }

                    if (!ValidPlayers(match.rounds[1].players, poco_match.match_id, poco_match.round_id_2))
                    {
                        Console.WriteLine(string.Format("player validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }
                }

                if (match.rounds.ElementAtOrDefault(2) != null)
                {
                    if (!ValidRound(match.rounds[2], poco_match.round_id_3))
                    {
                        Console.WriteLine(string.Format("round validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }

                    if (!ValidPlayers(match.rounds[2].players, poco_match.match_id, poco_match.round_id_3))
                    {
                        Console.WriteLine(string.Format("player validation discrepency on {0},{1}", match.match_id, poco_match.round_id_1));
                        valid_match = false;
                    }
                }

                NPocoDB.Update(poco_match);
            }
            catch (Exception ex)
            {
                WriteErrorLog(match.match_id, "db match validation error:" + ex.Message);
            }

            return valid_match;
        }
        public bool ValidPlayers(List<MatchPlayerEntry> players, long match_id, int round_id)
        {
            bool valid_players = true;

            try
            {
                List<PlayerRoundPoco> poco_players = NPocoDB.Fetch<PlayerRoundPoco>("WHERE MATCH_ID = @0 AND ROUND_ID = @1", match_id, round_id);

                foreach (PlayerRoundPoco poco_player in poco_players)
                {
                    MatchPlayerEntry player = players.SingleOrDefault(x => x.uid == poco_player.uid);

                    if (player == null)
                    {
                        Console.WriteLine(string.Format("unable to find player {0},{1}", poco_player.nickname, poco_player.uid));
                        valid_players = false;
                    }

                    if (!valid_players)
                        break;

                    if (player.build_hash != poco_player.build_hash)
                    {
                        Console.WriteLine(string.Format("build hash disagreement ({0},{1})-({2},{3})", player.nickname, player.build_hash, poco_player.nickname, poco_player.build_hash));
                        valid_players = false;
                    }

                    if (player.team != poco_player.team)
                    {
                        Console.WriteLine(string.Format("team disagreement ({0},{1})-({2},{3})", player.nickname, player.team, poco_player.nickname, poco_player.team));
                        valid_players = false;
                    }

                    if (player.nickname != poco_player.nickname)
                    {
                        Console.WriteLine(string.Format("team disagreement ({0},{1})-({2},{3})", player.nickname, player.uid, poco_player.nickname, poco_player.uid));
                        valid_players = false;
                    }

                    if (Math.Round((float)player.damage, 1) != Math.Round(poco_player.damage, 1))
                    {
                        Console.WriteLine(string.Format("damage disagreement ({0},{1})-({2},{3})", player.nickname, player.damage, poco_player.nickname, poco_player.damage));
                        valid_players = false;
                    }

                    if (Math.Round((float)player.damage_taken, 1) != Math.Round(poco_player.damage_taken, 1))
                    {
                        Console.WriteLine(string.Format("damage taken disagreement ({0},{1})-({2},{3})", player.nickname, player.damage_taken, poco_player.nickname, poco_player.damage_taken));
                        valid_players = false;
                    }

                    if (player.kills != poco_player.kills)
                    {
                        Console.WriteLine(string.Format("kills disagreement ({0},{1})-({2},{3})", player.nickname, player.kills, poco_player.nickname, poco_player.kills));
                        valid_players = false;
                    }

                    if (player.assists != poco_player.assists)
                    {
                        Console.WriteLine(string.Format("assists disagreement ({0},{1})-({2},{3})", player.nickname, player.assists, poco_player.nickname, poco_player.assists));
                        valid_players = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(match_id, "db player validation error:" + ex.Message);
            }

            return valid_players;
        }

        public bool ValidRound(RoundEntry round, int round_id)
        {
            bool valid_round = true;

            try
            {
                RoundPoco poco_round = NPocoDB.SingleOrDefaultById<RoundPoco>(round_id);

                if (round.winning_team != poco_round.winning_team)
                {
                    Console.WriteLine(string.Format("winning_team disagreement ({0})-({1})", round.winning_team, poco_round.winning_team));
                    valid_round = false;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(round.match_id, "db round validation error:" + ex.Message);
            }

            return valid_round;
        }

        public void WriteErrorLog(long match_id, string log)
        {
            try
            {
                ErrorLogPoco error_log = new ErrorLogPoco { };
                error_log.match_id = match_id;
                error_log.error_log = log;
                NPocoDB.Insert(error_log);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error writing log" + log + ex.Message);
            }
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
