using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Json.Conversion;

namespace StructuredData.Comparison.Json
{
    public class JsonDataWalker : IEnumerable<IStructuredDataNode>
    {
        private readonly JToken _jToken;

        public JsonDataWalker(string jsonData)
        {
            try
            {
                _jToken = JToken.Parse(jsonData);
            }
            catch (Exception)
            {
                _jToken = null;
            }
        }

        public IEnumerator<IStructuredDataNode> GetEnumerator()
        {
            if(_jToken == null)
            {
                yield break;
            }
            yield return JTokenConverter.Convert(_jToken);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}