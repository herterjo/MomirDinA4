using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using MomirDinA4.ScryfallApiObjects;
using Newtonsoft.Json;

namespace MomirDinA4;

public static class ScryfallApi
{
    private const string CardsFileName = "scryfallCards.json";
    private const string CardsFileNameTemp = "scryfallCards_temp.json";

    private static readonly HttpClient ScryfallClient = new();
    public static Dictionary<uint, List<Card>>? CmcCards { get; private set; }
    public static Card? Momir { get; private set; }

    private static Task<HttpResponseMessage> SendJsonRequest(string requestUri)
    {
        return SendRequest(requestUri, "application/json;q=0.9,*/*;q=0.8");
    }

    public static async Task<HttpResponseMessage> SendRequest(string requestUri, string? accept = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("User-Agent", "MomirDinA4");
        if (accept != null)
        {
            request.Headers.Add("Accept", accept);
        }
        var response = await ScryfallClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            throw new Exception("Could not complete request to scryfall for URL: " + requestUri + "; status code " + response.StatusCode + "; response: " + responseText);
        }

        return response;
    }

    private async static Task DownloadBulkCards()
    {
        if (Config.Instance.SkipScryfallUpdate && File.Exists(CardsFileName))
        {
            return;
        }


        using var bulkItemsResponse = await SendJsonRequest("https://api.scryfall.com/bulk-data");
        var bulkItemsString = await bulkItemsResponse.Content.ReadAsStringAsync();
        var bulkItems = JsonConvert.DeserializeObject<BulkDataItems>(bulkItemsString);
        var bulkCardUrl = (bulkItems?.Data.SingleOrDefault(d => d.Type == "oracle_cards")?.DownloadUri) ?? throw new Exception("Could not get bulk card download url from scryfall");

        using var allItemsResponse = await SendJsonRequest(bulkCardUrl);
        using (Stream output = File.OpenWrite(CardsFileNameTemp))
        using (Stream input = allItemsResponse.Content.ReadAsStream())
        {
            await input.CopyToAsync(output);
        }

        File.Move(CardsFileNameTemp, CardsFileName, true);
    }

    public async static Task InitCardDatabase()
    {
        await DownloadBulkCards();

        using var fileStream = File.OpenRead(CardsFileName);
        var serializer = new JsonSerializer();

        using (var sr = new StreamReader(fileStream))
        using (var jsonTextReader = new JsonTextReader(sr))
        {
            var deserialized = serializer.Deserialize<List<Card>>(jsonTextReader);
            Momir = deserialized?.SingleOrDefault(c => c.MtgoId == Config.Instance.MomirAvatarMtgoId);
            CmcCards = deserialized?
                .Where(c => c.Cmc.HasValue && c.Cmc == (uint)c.Cmc && c.Digital != true && c.TypeLine.Contains("Creature") && c.ImageUris != null && !c.TypeLine.StartsWith("Token"))
                .GroupBy(c => (uint)c.Cmc.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
