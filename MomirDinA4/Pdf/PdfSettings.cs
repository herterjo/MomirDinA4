using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

[method: JsonConstructor]
public record class PdfSettings(bool DuplexPrintingEnabled, bool IncludeQrCode, bool CropArt, bool NoArt)
{
}
