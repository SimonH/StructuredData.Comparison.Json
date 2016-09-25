using System.ComponentModel.Composition;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Json.Exports
{
    [Export(typeof(IFileMimeType))]
    [ExportMetadata("Extension", ".json")]
    public class JsonMimeType : IFileMimeType
    {
        public string MimeType => "application/json";
    }
}