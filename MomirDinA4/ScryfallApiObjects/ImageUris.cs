using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public class ImageUris(
            [JsonProperty("small")] string small,
            [JsonProperty("normal")] string normal,
            [JsonProperty("large")] string large,
            [JsonProperty("png")] string png,
            [JsonProperty("art_crop")] string artCrop,
            [JsonProperty("border_crop")] string borderCrop
            )
{
    [JsonProperty("small")]
    public string Small { get; } = small;

    [JsonProperty("normal")]
    public string Normal { get; } = normal;

    [JsonProperty("large")]
    public string Large { get; } = large;

    [JsonProperty("png")]
    public string Png { get; } = png;

    [JsonProperty("art_crop")]
    public string ArtCrop { get; } = artCrop;

    [JsonProperty("border_crop")]
    public string BorderCrop { get; } = borderCrop;
}
