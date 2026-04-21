using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

public record BasicPrintPdfRequest(PdfSettings PdfSettings, int MomirEmblemCount, IReadOnlyList<uint> InitialCmcs, IReadOnlySet<string> AlreadyPrintedCards)
{
    [method: JsonConstructor]
    public BasicPrintPdfRequest(PdfSettings PdfSettings, int MomirEmblemCount, List<uint> InitialCmcs, HashSet<string> AlreadyPrintedCards) : this(PdfSettings, MomirEmblemCount, (IReadOnlyList<uint>)InitialCmcs, (IReadOnlySet<string>)AlreadyPrintedCards)
    {

    }
}
