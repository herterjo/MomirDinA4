using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public record ImageUris(
    [property: JsonProperty("small")] string Small, 
    [property: JsonProperty("normal")] string Normal,
    [property: JsonProperty("large")] string Large, 
    [property: JsonProperty("png")] string Png,
    [property: JsonProperty("art_crop")] string ArtCrop,
    [property: JsonProperty("border_crop")] string BorderCrop
    );
