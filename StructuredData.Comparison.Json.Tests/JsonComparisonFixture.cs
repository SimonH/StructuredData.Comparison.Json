using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace StructuredData.Comparison.Json.Tests
{
    [TestFixture]
    public class JsonComparisonFixture
    {
        private string LoadEmbeddedResource(string file)
        {
            var array = Assembly.GetAssembly(typeof(JsonComparisonFixture)).GetManifestResourceNames();
            var path = array.FirstOrDefault(s => s.EndsWith("." + file));
            if(path != null)
            {
                var stream = Assembly.GetAssembly(typeof(JsonComparisonFixture)).GetManifestResourceStream(path);
                return stream == null ? null : new StreamReader(stream).ReadToEnd();
            }
            return null;
        }

        [Test]
        [TestCase("SimpleValues.json", "SimpleValues.json", true, TestName = "SameObject")]
        [TestCase("SimpleValues.json", "DifferentSimpleValues.json", false, TestName = "SameObjectDifferentValues")]
        [TestCase("MultipleValues.json", "IgnoredMultipleValues.json", true, TestName = "IgnoringValues")]
        [TestCase("SourceList.json", "SameListDefaultSettings.json", true, TestName = "SameListDefaultSettings")]
        [TestCase("SourceList.json", "ReducedListDefaultSettings.json", true, TestName = "ReducedListDefaultSettings")]
        [TestCase("SourceList.json", "ExtendedListDefaultSettings.json", false, TestName = "ExtendedListDefaultSettings")]
        [TestCase("SourceList.json", "SameListStrictOrderedSettings.json", true, TestName = "SameListStrictOrderedSettings")]
        [TestCase("SourceList.json", "ReducedListStrictOrderedSettings.json", false, TestName = "ReducedListStrictOrderedSettings")]
        [TestCase("ListOfValues.json", "ListOfValuesWithSettings.json", true, TestName = "ListOfValuesAsNodes")]
        [TestCase("ListOfValues.json", "ReducedListOfValuesAsList.json", true, TestName = "ReducedListOfValuesAsList")]
        [TestCase("ListOfValues.json", "CaseSensitiveListOfValues.json", false, TestName = "CaseSensitiveListOfValuesAsList")]
        [TestCase("SimpleValues.json", "CaseSensitiveSimpleValues.json", false, TestName = "CaseSensitiveObject")]
        public void ComparingJson(string source, string result, bool expectNullOrWhitespace)
        {
            var data1 = LoadEmbeddedResource(source);
            var data2 = LoadEmbeddedResource(result);
            if(data1 == null || data2 == null)
            {
                Assert.Fail("Could not load data");
            }
            var candidate = data1.ContentComparison(data2, "application/json");
            Assert.That(string.IsNullOrWhiteSpace(candidate), Is.EqualTo(expectNullOrWhitespace));
        }
    }
}