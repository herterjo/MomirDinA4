using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MomirDinA4.Pdf;

[method: JsonConstructor]
public class PdfSettings(bool duplexPrintingEnabled, bool includeQrCode, bool cropArt, bool noArt)
{
    public bool DuplexPrintingEnabled { get; } = duplexPrintingEnabled;
    public bool IncludeQrCode { get; } = includeQrCode;
    public bool CropArt { get; } = cropArt;
    public bool NoArt { get; } = noArt;
}
