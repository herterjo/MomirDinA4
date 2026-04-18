using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

[method: JsonConstructor]
public class BasicPrintPdfRequest(PdfSettings pdfSettings, int momirEmblemCount, List<uint> initialCmcs, HashSet<string> alreadyPrintedCards)
{
    public PdfSettings PdfSettings { get; } = pdfSettings;
    public int MomirEmblemCount { get; } = momirEmblemCount;
    public IReadOnlyList<uint> InitialCmcs { get; } = initialCmcs;
    public IReadOnlySet<string> AlreadyPrintedCards { get; } = alreadyPrintedCards;
}
