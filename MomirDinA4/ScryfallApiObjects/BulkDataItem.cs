using Newtonsoft.Json;

namespace MomirDinA4.ScryfallApiObjects;

[method: JsonConstructor]
public class BulkDataItem(
    [JsonProperty("object")] string @object,
    [JsonProperty("id")] string id,
    [JsonProperty("type")] string type,
    [JsonProperty("updated_at")] DateTime? updatedAt,
    [JsonProperty("uri")] string uri,
    [JsonProperty("name")] string name,
    [JsonProperty("description")] string description,
    [JsonProperty("size")] long? size,
    [JsonProperty("download_uri")] string downloadUri,
    [JsonProperty("content_type")] string contentType,
    [JsonProperty("content_encoding")] string contentEncoding
    )
{
    [JsonProperty("object")]
    public string Object { get; } = @object;

    [JsonProperty("id")]
    public string Id { get; } = id;

    [JsonProperty("type")]
    public string Type { get; } = type;

    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt { get; } = updatedAt;

    [JsonProperty("uri")]
    public string Uri { get; } = uri;

    [JsonProperty("name")]
    public string Name { get; } = name;

    [JsonProperty("description")]
    public string Description { get; } = description;

    [JsonProperty("size")]
    public long? Size { get; } = size;

    [JsonProperty("download_uri")]
    public string DownloadUri { get; } = downloadUri;

    [JsonProperty("content_type")]
    public string ContentType { get; } = contentType;

    [JsonProperty("content_encoding")]
    public string ContentEncoding { get; } = contentEncoding;
}