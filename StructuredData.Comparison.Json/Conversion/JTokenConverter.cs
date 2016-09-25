using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Newtonsoft.Json.Linq;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.Json.Conversion
{
    public class JTokenConverter
    {
        public static IStructuredDataNode Convert(JToken token)
        {
            return Convert(token, null);
        }
        private static IStructuredDataNode Convert(JToken token, string nameToUse)
        {
            if(token == null)
            {
                return null;
            }
            var path = string.IsNullOrEmpty(token.Path) ? "$" : $"$.{token.Path}";
            if(token is JValue)
            {
                return new StructuredDataNode
                {
                    Children = Enumerable.Empty<IStructuredDataNode>(),
                    IsValue = true,
                    Name = nameToUse,
                    Path = path,
                    Value = GetValue(token)
                };
            }
            var property = token as JProperty;
            if(property != null && property.Count != 1)
            {
                throw new DataComparisonException("json object property is expected to have a single value. Multiple values are expected to be in an array");
            }
            var isValue = property != null && property.First.Type != JTokenType.Array && property.First.Type != JTokenType.Object;
            var name = DetermineName(token, nameToUse);
            IEnumerable<IStructuredDataNode> children;
            if(name == ProcessorDeclarations.Settings && property != null)
            {
                children = token.First.Children().WithoutComment().Select(Convert);
            }
            else if(property != null && property.First.Type == JTokenType.Array)
            {
                children = property.First.Children().WithoutComment().Select(t => Convert(t, $"{property.Name}Item")); 
            }
            else
            {
                children = token.Children().WithoutComment().Select(Convert);
            }

            return new StructuredDataNode
            {
                Children = isValue ? Enumerable.Empty<IStructuredDataNode>() : children,
                IsValue = isValue,
                Name = name,
                Path = path,
                Value = isValue ? GetValue(token) : null
            };
        }

        private static string DetermineName(JToken token, string nameToUse)
        {
            var property = token as JProperty;
            if(IsSettingsNode(token, property != null))
            {
                return ProcessorDeclarations.Settings;
            }
            return string.IsNullOrWhiteSpace(nameToUse) ?
                (property != null ?
                    property.Name
                    : (string.IsNullOrEmpty(token.Path) ?
                        "Root"
                        : token.Path.Substring(token.Path.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1)))
                : nameToUse;
        }

        private static bool IsSettingsNode(JToken token, bool isProperty)
        {
            return string.Equals(isProperty ? ((JProperty)token).Name : token.Value<string>("Name"), ProcessorDeclarations.Settings, StringComparison.Ordinal);
        }

        private static string GetValue(JToken token)
        {
            var property = token as JProperty;
            if(property != null)
            {
                return GetValue(property.Value);
            }
            return token.Type == JTokenType.Date ? token.ToObject<DateTime>().ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") : token.ToString();
        }
    }
}