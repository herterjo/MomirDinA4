using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

[method: JsonConstructor]
public class CmcPdfRequest(PdfSettings pdfSettings, uint cmc, HashSet<string> alreadyPrintedCards)
{
    public PdfSettings PdfSettings { get; } = pdfSettings;
    public uint Cmc { get; } = cmc;
    public IReadOnlySet<string> AlreadyPrintedCards { get; } = alreadyPrintedCards;
}
