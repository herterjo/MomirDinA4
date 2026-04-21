using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

public record CmcPdfRequest(PdfSettings PdfSettings, uint Cmc, IReadOnlySet<string> AlreadyPrintedCards)
{
    [method: JsonConstructor]
    public CmcPdfRequest(PdfSettings PdfSettings, uint Cmc, HashSet<string> AlreadyPrintedCards) : this(PdfSettings, Cmc, (IReadOnlySet<string>)AlreadyPrintedCards)
    {
    }
}
