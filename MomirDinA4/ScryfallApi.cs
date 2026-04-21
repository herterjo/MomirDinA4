using System;
using System.Collections.Generic;
using System.Text;
using MomirDinA4.ScryfallApiObjects;
using Newtonsoft.Json;

namespace MomirDinA4;

public static class ScryfallApi
{
    private const string CardsFileName = "scryfallDefaultCards.json";
    private const string CardsFileNameTemp = "scryfallDefaultCards_temp.json";

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
            Console.WriteLine("Skipping download, using existing file");
            return;
        }

        using var bulkItemsResponse = await SendJsonRequest("https://api.scryfall.com/bulk-data");
        var bulkItemsString = await bulkItemsResponse.Content.ReadAsStringAsync();
        var bulkItems = JsonConvert.DeserializeObject<BulkDataItems>(bulkItemsString);
        var bulkCardUrl = (bulkItems?.Data.SingleOrDefault(d => d.Type == "default_cards")?.DownloadUri) ?? throw new Exception("Could not get bulk card download url from scryfall");

        Console.WriteLine("Downloading database from " + bulkCardUrl);

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

        Console.WriteLine("Reading card database");

        using var fileStream = File.OpenRead(CardsFileName);
        var serializer = new JsonSerializer();

        using (var sr = new StreamReader(fileStream))
        using (var jsonTextReader = new JsonTextReader(sr))
        {
            var deserialized = serializer.Deserialize<List<Card>>(jsonTextReader);
            Momir = deserialized?.SingleOrDefault(c => c.MtgoId == Config.Instance.MomirAvatarMtgoId);

            Console.WriteLine("Filtering card database");

            var withCmc = deserialized?.Where(c => c.Cmc.HasValue && c.Cmc == (uint)c.Cmc);

            if (String.IsNullOrWhiteSpace(Config.Instance.CubeFilename))
            {
                Console.WriteLine("Using default card filter with DefaultFilterIncludesDigitalCards=" + Config.Instance.DefaultFilterIncludesDigitalCards + " and DefaultFilterIncludesFunnyCards=" + Config.Instance.DefaultFilterIncludesFunnyCards);
                withCmc = AddDefaultCardFilter(withCmc);
            }
            else if (!File.Exists(Config.Instance.CubeFilename))
            {
                throw new Exception("File " + Config.Instance.CubeFilename + " does not exist");
            }
            else
            {
                Console.WriteLine("Using cube file " + Config.Instance.CubeFilename + " with CubeCardsAreScryfallIdsInsteadOfNames=" + Config.Instance.CubeCardsAreScryfallIdsInsteadOfNames);
                withCmc = AddCubeCardFilter(withCmc);
            }

            if (Config.Instance.CubeCardsAreScryfallIdsInsteadOfNames)
            {
                SetCmcDict(withCmc);
            }
            else
            {
                withCmc = withCmc.Where(c => c.Lang == "en")
                    .GroupBy(c => c.Name)
                    .Select(g => g.OrderByDescending(c => c.ReleasedAt).First());
                SetCmcDict(withCmc);
            }
        }
    }

    private static void SetCmcDict(IEnumerable<Card>? withCmc)
    {
        CmcCards = withCmc?
            .GroupBy(c => (uint)c.Cmc.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static IEnumerable<Card>? AddCubeCardFilter(IEnumerable<Card>? withCmc)
    {
        var cubeCardList = File.ReadLines(Config.Instance.CubeFilename).Where(c => !String.IsNullOrWhiteSpace(c)).ToHashSet();
        if (cubeCardList.Count == 0)
        {
            throw new Exception("File " + Config.Instance.CubeFilename + " was empty");
        }
        else
        {
            withCmc = AddCubeCardFilter(withCmc, cubeCardList, Config.Instance.CubeCardsAreScryfallIdsInsteadOfNames ? c => c.Id : c => c.Name);
        }

        return withCmc;
    }

    private static IEnumerable<Card>? AddCubeCardFilter(IEnumerable<Card>? cards, HashSet<string> toInclude, Func<Card, string> getCardIdentifier)
    {
        if (cards == null)
        {
            return cards;
        }

        var filteredCards = cards.Where(c => toInclude.Contains(getCardIdentifier(c))).ToList();

        if (filteredCards.Count == 0)
        {
            throw new Exception("Could not find any cards from list");
        }

        var filteredCardIds = filteredCards.Select(getCardIdentifier).ToHashSet();
        var missingCards = toInclude.Except(filteredCardIds).ToList();

        if (missingCards.Count == 0)
        {
            return filteredCards;
        }

        Console.WriteLine("Could not find the following cards: \n" + String.Join("\n", missingCards));
        return filteredCards;
    }

    private static IEnumerable<Card>? AddDefaultCardFilter(IEnumerable<Card>? cards)
    {
        if (cards == null)
        {
            return cards;
        }

        var illegalLayouts = new HashSet<string>() { "token", "double_faced_token", "meld", "transform", "modal_dfc", "augment" };
        var illegalNames = new HashSet<string>() { "Common Curve Filler", "Enolc, Perfect Clone", "Awoken Nephilim", "B.F.M. (Big Furry Monster)" };
        cards = cards.Where(c => c.TypeLine.Contains("Creature") && !illegalLayouts.Contains(c.Layout) && !illegalNames.Contains(c.Name));
        if (!Config.Instance.DefaultFilterIncludesDigitalCards)
        {
            cards = cards.Where(c => c.Digital != true);
        }
        if (!Config.Instance.DefaultFilterIncludesFunnyCards)
        {
            cards = cards.Where(c => c.SetType != "funny");
        }
        return cards;
    }
}
