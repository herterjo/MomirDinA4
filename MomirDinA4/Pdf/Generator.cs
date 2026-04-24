using System;
using System.Collections.Generic;
using System.Text;
using MomirDinA4.ScryfallApiObjects;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MomirDinA4.Pdf;

public static class Generator
{
    private const int CardsOnPage = 9;
    private const int CardsInRow = 3;
    private const int ColumnWidth = 175;
    private const int TablePadding = 5;
    private const int PageMargin = 1;
    private const int DefaultRowHeight = 247;
    private static readonly QRCodeGenerator QrGenerator = new();

    public static Response GetBasicPrintPdf(BasicPrintPdfRequest request)
    {
        var momirImageUrl = ScryfallApi.Momir?.ImageUris?.BorderCrop;
        byte[]? momirImage;

        if (momirImageUrl != null)
        {
            momirImage = GetImageData(momirImageUrl).Result;
        }
        else
        {
            momirImage = null;
        }

        var includedCards = new List<string>();
        var pdf = Document.Create(container =>
        {
            if (request.MomirEmblemCount > 0 && momirImage != null)
            {
                var momirImages = new List<(byte[] Data, Card Card)>(request.MomirEmblemCount);
                for (var i = 0; i < request.MomirEmblemCount; i++)
                {
                    momirImages.Add((momirImage, ScryfallApi.Momir));
                }

                AddImages(container, momirImages, new PdfSettings(false, false, false, false), 0);

                if (request.PdfSettings.DuplexPrintingEnabled)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);
                    });
                }
            }

            foreach (var cmc in request.InitialCmcs)
            {
                includedCards.AddRange(AddCards(container, cmc, request.AlreadyPrintedCards, request.PdfSettings));
            }
        })
        .GeneratePdf();

        return new Response(includedCards, pdf);
    }

    private static List<Card> GetCardsForPage(uint cmc, IReadOnlySet<string> notIncludes)
    {
        if (!ScryfallApi.CmcCards.TryGetValue(cmc, out var allCards))
        {
            return [];
        }

        return allCards.Where(c => !notIncludes.Contains(c.Id)).Shuffle().Take(CardsOnPage).ToList();
    }

    private static List<string> AddCards(IDocumentContainer container, uint cmc, IReadOnlySet<string> notIncludes, PdfSettings settings)
    {
        var cards = GetCardsForPage(cmc, notIncludes);

        if (cards.Count == 0)
        {
            return [];
        }

        List<(Task<byte[]?> Data, Card Card)> imageTasks;

        if (settings.NoArt)
        {
            imageTasks = cards.Select(GetNoCardArt).ToList();
        }
        else
        {
            imageTasks = new(cards.Count);
            foreach (var card in cards)
            {
                var url = GetImageUrl(card, settings.CropArt);
                if (url == null)
                {
                    var noCardArt = GetNoCardArt(card);
                    imageTasks.Add(noCardArt);
                }
                else
                {
                    var imageTask = GetImageData(url);
                    imageTasks.Add((imageTask, card));
                }
            }
        }
        Task.WhenAll(imageTasks.Select(c => c.Data)).Wait();
        AddImages(container, imageTasks.Select(c => (c.Data.Result, c.Card)).ToList(), settings, cmc);
        return cards.Select(c => c.Id).ToList();
    }

    private static (Task<byte[]?>, Card card) GetNoCardArt(Card card)
    {
        return (Task.Run(() => (byte[]?)null), card);
    }

    private static string? GetImageUrl(Card card, bool cropArt)
    {
        if (card.ImageUris == null)
        {
            return null;
        }
        if (cropArt && !String.IsNullOrWhiteSpace(card.ImageUris.ArtCrop))
        {
            return card.ImageUris.ArtCrop;
        }
        return String.IsNullOrWhiteSpace(card.ImageUris.BorderCrop) ? card.ImageUris.Normal : card.ImageUris.BorderCrop;
    }

    private async static Task<byte[]?> GetImageData(string url)
    {
        using var response = await ScryfallApi.SendRequest(url);
        var responseStream = response.Content.ReadAsStream();
        var buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }

    private static void AddImages(IDocumentContainer container, List<(byte[]? Data, Card Card)> images, PdfSettings settings, uint cmc)
    {
        if (images.Count == 0)
        {
            return;
        }

        foreach (var cardsOnPage in images.Chunk(CardsOnPage))
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(PageMargin, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                var content = page.Content();

                content
                .AlignCenter()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < CardsInRow; i++)
                        {
                            columns.ConstantColumn(ColumnWidth);
                        }
                    });
                    foreach (var imagcardsInRowsInRow in cardsOnPage.Chunk(CardsInRow))
                    {
                        foreach (var card in imagcardsInRowsInRow)
                        {
                            var noArt = card.Data == null;
                            if ((settings.CropArt && card.Card.ImageUris.ArtCrop != null) || noArt)
                            {
                                table.Cell()
                                    .Element(e => e.Border(1).Height(DefaultRowHeight))
                                    .AlignCenter()
                                    .Padding(TablePadding)
                                    .Column(c =>
                                    {
                                        int remainingHeightForOracle;
                                        c.Item().Height(15).ScaleToFit().Text(card.Card.Name).Bold();
                                        c.Item().Height(12).ScaleToFit().Text("Cost: " + GetManaCost(card.Card));
                                        var addQrCodeOnFront = settings.IncludeQrCode && !settings.DuplexPrintingEnabled;

                                        if (noArt && addQrCodeOnFront)
                                        {
                                            var qrCode = GetQrCode(card.Card);
                                            c.Item().Height(64).Image(qrCode);
                                            remainingHeightForOracle = 110;
                                        }
                                        else if (noArt)
                                        {
                                            remainingHeightForOracle = 174;
                                        }
                                        else if (addQrCodeOnFront)
                                        {
                                            c.Item().AlignCenter().Height(64).ScaleToFit().Row(r =>
                                            {
                                                r.ConstantItem((int)(ColumnWidth * 0.5)).Image(card.Data);
                                                var qrCode = GetQrCode(card.Card);
                                                r.ConstantItem((int)(ColumnWidth * 0.35)).Image(qrCode);
                                            });
                                            remainingHeightForOracle = 110;
                                        }
                                        else
                                        {
                                            c.Item().AlignCenter().Height(100).Image(card.Data);
                                            remainingHeightForOracle = 64;
                                        }

                                        c.Item().Height(15).ScaleToFit().Text(card.Card.TypeLine).Bold();
                                        c.Item().Height(remainingHeightForOracle).ScaleToFit().Text(GetOracleText(card.Card));
                                        c.Item().Height(12).ScaleToFit().AlignBottom().AlignRight().Text(card.Card.Power + " / " + card.Card.Toughness);
                                    });
                            }
                            else
                            {
                                table.Cell()
                                    .AlignCenter()
                                    .Padding(TablePadding)
                                    .Image(Image.FromBinaryData(card.Data));
                            }
                        }
                    }
                });
            });

            if (settings.DuplexPrintingEnabled)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(PageMargin, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    var content = page.Content();

                    content
                    .AlignCenter()
                    .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (var i = 0; i < CardsInRow; i++)
                                {
                                    columns.ConstantColumn(ColumnWidth);
                                }
                            });
                            foreach (var cardsInRow in cardsOnPage.Chunk(CardsInRow))
                            {
                                for (var i = CardsInRow - 1; i >= 0; i--)
                                {
                                    if (i < cardsInRow.Length)
                                    {
                                        var card = cardsInRow[i];
                                        table.Cell()
                                            .Element(s => s.Border(1).Height(DefaultRowHeight))
                                            .AlignCenter()
                                            .Padding(TablePadding)
                                            .Column(c =>
                                            {
                                                c.Spacing(20);
                                                c.Item().Text(cmc.ToString()).FontSize(40).AlignCenter();
                                                if (settings.IncludeQrCode)
                                                {
                                                    var qrCodeBytes = GetQrCode(card.Card);
                                                    c.Item().Image(qrCodeBytes);
                                                }
                                            });
                                    }
                                    else
                                    {
                                        table.Cell()
                                            .Element(s => s.Height(DefaultRowHeight))
                                            .Padding(TablePadding);
                                    }
                                }
                            }
                        });
                });
            }
        }
    }

    private static string GetOracleText(Card card)
    {
        if (!String.IsNullOrEmpty(card.OracleText))
        {
            return card.OracleText;
        }

        if (card.CardFaces != null && card.CardFaces.Count > 0 && card.CardFaces.Any(cf => !String.IsNullOrWhiteSpace(cf.OracleText)))
        {
            var oracleText = String.Join("\n\n//\n\n", card.CardFaces.Select(cf => cf.TypeLine + "\n" + cf.OracleText));
            return oracleText;
        }
        return "";
    }

    private static byte[] GetQrCode(Card card)
    {
        using var qrData = QrGenerator.CreateQrCode(card.ScryfallUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }

    public static string GetManaCost(Card card)
    {
        var colorIndicator = card.ColorIndicator == null ? "" : (" | Color Indicator: " + String.Join(", ", card.ColorIndicator));
        var hasManaCost = !String.IsNullOrWhiteSpace(card.ManaCost);
        if (hasManaCost && card.Cmc != 0)
        {
            return card.ManaCost + colorIndicator;
        }
        return (hasManaCost ? card.ManaCost : "None") + (String.IsNullOrWhiteSpace(colorIndicator) ? " | Colors: " + String.Join(", ", card.Colors ?? []) : colorIndicator);
    }

    public static Response GetCmcPdf(CmcPdfRequest request)
    {
        List<string> includedCards = [];
        var pdf = Document.Create(container =>
        {
            includedCards = AddCards(container, request.Cmc, request.AlreadyPrintedCards, request.PdfSettings);
        })
        .GeneratePdf();

        return new Response(includedCards, pdf);
    }
}
