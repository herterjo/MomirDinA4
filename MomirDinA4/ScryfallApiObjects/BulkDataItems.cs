using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public class BulkDataItems(
        [JsonProperty("object")] string @object,
        [JsonProperty("has_more")] bool? hasMore,
        [JsonProperty("data")] List<BulkDataItem> data
        )
{
    [JsonProperty("object")]
    public string Object { get; } = @object;

    [JsonProperty("has_more")]
    public bool? HasMore { get; } = hasMore;

    [JsonProperty("data")]
    public IReadOnlyList<BulkDataItem> Data { get; } = data;
}
