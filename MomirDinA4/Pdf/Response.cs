using System;
using System.Collections.Generic;
using System.Text;

namespace MomirDinA4.Pdf;

public class Response(List<string> includedCards, byte[] pdfContent)
{
    public List<string> IncludedCards { get; } = includedCards;
    public byte[] PdfContent { get; } = pdfContent;
}
