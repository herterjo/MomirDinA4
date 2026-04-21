using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public record class BulkDataItems(
        [property: JsonProperty("object")] string Object,
        [property: JsonProperty("has_more")] bool? HasMore,
        [property: JsonProperty("data")] IReadOnlyList<BulkDataItem> Data
        )
{
}
