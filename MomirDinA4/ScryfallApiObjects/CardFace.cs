using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public record CardFace(
    [property: JsonProperty("object")] string Object,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("mana_cost")] string ManaCost,
    [property: JsonProperty("type_line")] string TypeLine,
    [property: JsonProperty("oracle_text")] string OracleText,
    [property: JsonProperty("power")] string Power,
    [property: JsonProperty("toughness")] string Toughness,
    [property: JsonProperty("artist")] string Artist,
    [property: JsonProperty("artist_id")] string ArtistId,
    [property: JsonProperty("illustration_id")] string IllustrationId
);