
using System.Net;
using System.Net.Sockets;
using MomirDinA4;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

Console.WriteLine("Initializing card database...");
await ScryfallApi.InitCardDatabase();
if (ScryfallApi.CmcCards == null || ScryfallApi.CmcCards.Count == 0)
{
    throw new Exception("No cards could be loaded");
}
if (ScryfallApi.Momir == null)
{
    Console.WriteLine("Momir avatar could not be loaded!!! Check card database and momir id in config");
}
Console.WriteLine("Card database initialized, loaded " + ScryfallApi.CmcCards.Sum(c => c.Value.Count) + " cards");

WebServer.Start();