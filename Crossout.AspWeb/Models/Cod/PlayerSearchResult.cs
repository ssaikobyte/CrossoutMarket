using Crossout.AspWeb.Pocos;
using Newtonsoft.Json;
using NPoco;

namespace Crossout.AspWeb.Models.Cod
{
    public class PlayerSearchResult
    {
        [JsonIgnore]
        [Reference(ReferenceType.OneToOne)]
        public PlayerRoundPoco Poco { get; set; }

        [Ignore]
        [JsonProperty("userId")]
        public int UserId { get => Poco.uid; }

        [Ignore]
        [JsonProperty("nickname")]
        public string Nickname { get => Poco.nickname; }
    }
}
