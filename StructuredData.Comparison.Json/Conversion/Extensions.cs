using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace StructuredData.Comparison.Json.Conversion
{
    internal static class Extensions
    {
        public static IEnumerable<JToken> WithoutComment(this JEnumerable<JToken> source)
        {
            return source.Where(t => t.Type != JTokenType.Comment);
        }

    }
}