using System.Collections.Generic;
using System.ComponentModel.Composition;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Json.Exports
{
    [Export(typeof(ICreateStructuredDataWalkers))]
    [ExportMetadata("MimeType", "application/json")]
    public class WalkerCreator : ICreateStructuredDataWalkers
    {
        public IEnumerable<IStructuredDataNode> CreateWalker(string data)
        {
            return new JsonDataWalker(data);
        }
    }
}