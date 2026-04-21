using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method:JsonConstructor]
public record BulkDataItem(
    [property: JsonProperty("object")] string Object,
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("type")] string Type,
    [property: JsonProperty("updated_at")] DateTime? UpdatedAt,
    [property: JsonProperty("uri")] string Uri,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("description")] string Description,
    [property: JsonProperty("size")] long? Size,
    [property: JsonProperty("download_uri")] string DownloadUri,
    [property: JsonProperty("content_type")] string ContentType,
    [property: JsonProperty("content_encoding")] string ContentEncoding
    );
