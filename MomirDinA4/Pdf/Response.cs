using System;
using System.Collections.Generic;
using System.Text;

namespace MomirDinA4.Pdf;

public record class Response(List<string> IncludedCards, byte[] PdfContent)
{
}
