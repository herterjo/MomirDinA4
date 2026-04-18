using System.Diagnostics;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public class Card(
        [JsonProperty("object")] string @object,
        [JsonProperty("id")] string id,
        [JsonProperty("oracle_id")] string oracleId,
        [JsonProperty("multiverse_ids")] List<object> multiverseIds,
        [JsonProperty("tcgplayer_id")] int? tcgplayerId,
        [JsonProperty("name")] string name,
        [JsonProperty("lang")] string lang,
        [JsonProperty("released_at")] string releasedAt,
        [JsonProperty("uri")] string uri,
        [JsonProperty("scryfall_uri")] string scryfallUri,
        [JsonProperty("layout")] string layout,
        [JsonProperty("highres_image")] bool? highresImage,
        [JsonProperty("image_status")] string imageStatus,
        [JsonProperty("image_uris")] ImageUris imageUris,
        [JsonProperty("mana_cost")] string manaCost,
        [JsonProperty("cmc")] double? cmc,
        [JsonProperty("type_line")] string typeLine,
        [JsonProperty("oracle_text")] string oracleText,
        [JsonProperty("power")] string power,
        [JsonProperty("toughness")] string toughness,
        [JsonProperty("colors")] List<string> colors,
        [JsonProperty("color_identity")] List<string> colorIdentity,
        [JsonProperty("keywords")] List<string> keywords,
        [JsonProperty("games")] List<string> games,
        [JsonProperty("reserved")] bool? reserved,
        [JsonProperty("game_changer")] bool? gameChanger,
        [JsonProperty("foil")] bool? foil,
        [JsonProperty("nonfoil")] bool? nonfoil,
        [JsonProperty("finishes")] List<string> finishes,
        [JsonProperty("oversized")] bool? oversized,
        [JsonProperty("promo")] bool? promo,
        [JsonProperty("reprint")] bool? reprint,
        [JsonProperty("variation")] bool? variation,
        [JsonProperty("set_id")] string setId,
        [JsonProperty("set")] string set,
        [JsonProperty("set_name")] string setName,
        [JsonProperty("set_type")] string setType,
        [JsonProperty("set_uri")] string setUri,
        [JsonProperty("set_search_uri")] string setSearchUri,
        [JsonProperty("scryfall_set_uri")] string scryfallSetUri,
        [JsonProperty("rulings_uri")] string rulingsUri,
        [JsonProperty("prints_search_uri")] string printsSearchUri,
        [JsonProperty("collector_number")] string collectorNumber,
        [JsonProperty("digital")] bool? digital,
        [JsonProperty("rarity")] string rarity,
        [JsonProperty("card_back_id")] string cardBackId,
        [JsonProperty("artist")] string artist,
        [JsonProperty("border_color")] string borderColor,
        [JsonProperty("frame")] string frame,
        [JsonProperty("full_art")] bool? fullArt,
        [JsonProperty("textless")] bool? textless,
        [JsonProperty("booster")] bool? booster,
        [JsonProperty("story_spotlight")] bool? storySpotlight,
        [JsonProperty("promo_types")] List<string> promoTypes,
        [JsonProperty("mtgo_id")] int mtgoId,
        [JsonProperty("color_indicator")] List<string> colorIndicator
        )
{
    [JsonProperty("object")]
    public string Object { get; } = @object;

    [JsonProperty("id")]
    public string Id { get; } = id;

    [JsonProperty("oracle_id")]
    public string OracleId { get; } = oracleId;

    [JsonProperty("multiverse_ids")]
    public IReadOnlyList<object> MultiverseIds { get; } = multiverseIds;

    [JsonProperty("tcgplayer_id")]
    public int? TcgplayerId { get; } = tcgplayerId;

    [JsonProperty("name")]
    public string Name { get; } = name;

    [JsonProperty("lang")]
    public string Lang { get; } = lang;

    [JsonProperty("released_at")]
    public string ReleasedAt { get; } = releasedAt;

    [JsonProperty("uri")]
    public string Uri { get; } = uri;

    [JsonProperty("scryfall_uri")]
    public string ScryfallUri { get; } = scryfallUri;

    [JsonProperty("layout")]
    public string Layout { get; } = layout;

    [JsonProperty("highres_image")]
    public bool? HighresImage { get; } = highresImage;

    [JsonProperty("image_status")]
    public string ImageStatus { get; } = imageStatus;

    [JsonProperty("image_uris")]
    public ImageUris ImageUris { get; } = imageUris;

    [JsonProperty("mana_cost")]
    public string ManaCost { get; } = manaCost;

    [JsonProperty("cmc")]
    public double? Cmc { get; } = cmc;

    [JsonProperty("type_line")]
    public string TypeLine { get; } = typeLine;

    [JsonProperty("oracle_text")]
    public string OracleText { get; } = oracleText;

    [JsonProperty("power")]
    public string Power { get; } = power;

    [JsonProperty("toughness")]
    public string Toughness { get; } = toughness;

    [JsonProperty("colors")]
    public IReadOnlyList<string> Colors { get; } = colors;

    [JsonProperty("color_identity")]
    public IReadOnlyList<string> ColorIdentity { get; } = colorIdentity;
    [JsonProperty("color_indicator")]
    public IReadOnlyList<string> ColorIndicator { get; } = colorIndicator;

    [JsonProperty("keywords")]
    public IReadOnlyList<string> Keywords { get; } = keywords;

    [JsonProperty("games")]
    public IReadOnlyList<string> Games { get; } = games;

    [JsonProperty("reserved")]
    public bool? Reserved { get; } = reserved;

    [JsonProperty("game_changer")]
    public bool? GameChanger { get; } = gameChanger;

    [JsonProperty("foil")]
    public bool? Foil { get; } = foil;

    [JsonProperty("nonfoil")]
    public bool? Nonfoil { get; } = nonfoil;

    [JsonProperty("finishes")]
    public IReadOnlyList<string> Finishes { get; } = finishes;

    [JsonProperty("oversized")]
    public bool? Oversized { get; } = oversized;

    [JsonProperty("promo")]
    public bool? Promo { get; } = promo;

    [JsonProperty("reprint")]
    public bool? Reprint { get; } = reprint;

    [JsonProperty("variation")]
    public bool? Variation { get; } = variation;

    [JsonProperty("set_id")]
    public string SetId { get; } = setId;

    [JsonProperty("set")]
    public string Set { get; } = set;

    [JsonProperty("set_name")]
    public string SetName { get; } = setName;

    [JsonProperty("set_type")]
    public string SetType { get; } = setType;

    [JsonProperty("set_uri")]
    public string SetUri { get; } = setUri;

    [JsonProperty("set_search_uri")]
    public string SetSearchUri { get; } = setSearchUri;

    [JsonProperty("scryfall_set_uri")]
    public string ScryfallSetUri { get; } = scryfallSetUri;

    [JsonProperty("rulings_uri")]
    public string RulingsUri { get; } = rulingsUri;

    [JsonProperty("prints_search_uri")]
    public string PrintsSearchUri { get; } = printsSearchUri;

    [JsonProperty("collector_number")]
    public string CollectorNumber { get; } = collectorNumber;

    [JsonProperty("digital")]
    public bool? Digital { get; } = digital;

    [JsonProperty("rarity")]
    public string Rarity { get; } = rarity;

    [JsonProperty("card_back_id")]
    public string CardBackId { get; } = cardBackId;

    [JsonProperty("artist")]
    public string Artist { get; } = artist;

    [JsonProperty("border_color")]
    public string BorderColor { get; } = borderColor;

    [JsonProperty("frame")]
    public string Frame { get; } = frame;

    [JsonProperty("full_art")]
    public bool? FullArt { get; } = fullArt;

    [JsonProperty("textless")]
    public bool? Textless { get; } = textless;

    [JsonProperty("booster")]
    public bool? Booster { get; } = booster;

    [JsonProperty("story_spotlight")]
    public bool? StorySpotlight { get; } = storySpotlight;

    [JsonProperty("promo_types")]
    public IReadOnlyList<string> PromoTypes { get; } = promoTypes;

    [JsonProperty("mtgo_id")]
    public int MtgoId { get; } = mtgoId;
}