using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class FactoriesTests
    {
        [TestMethod]
        public void Test_Evaluation()
        {
            var employee = Employee.Sample;

            var document = ContentFactory.Default.CreateFrom(employee);

            IContentNode actual = null;

            document.TryEvaluate(ContentPath.Parse("Name"), out actual);
            Assert.AreEqual(new ContentText(employee.Name), actual);

            document.TryEvaluate(ContentPath.Parse("Data.FavColor"), out actual);
            Assert.AreEqual(new ContentText("red"), actual);

            var result = document.TryEvaluate(ContentPath.Parse("Data.Start ingSalary.Amount"), out actual);
            Assert.IsFalse(result);

            document.TryEvaluate(ContentPath.Parse("Data.StartingSalary.Amount"), out actual);
            Assert.AreEqual(new ContentNumber(500.57m), actual);

            // Test JSON

            var jsonText = JsonConvert.SerializeObject(employee);

            var jToken = JsonConvert.DeserializeObject<JToken>(jsonText);

            document = ContentFactory.Default.CreateFrom(jToken);

            document.TryEvaluate(ContentPath.Parse("Name"), out actual);
            Assert.AreEqual(new ContentText(employee.Name), actual);

            document.TryEvaluate(ContentPath.Parse("Data.FavColor"), out actual);
            Assert.AreEqual(new ContentText("red"), actual);

            result = document.TryEvaluate(ContentPath.Parse("Data.Start ingSalary.Amount"), out actual);
            Assert.IsFalse(result);

            document.TryEvaluate(ContentPath.Parse("Data.StartingSalary.Amount"), out actual);
            Assert.AreEqual(new ContentNumber(500.57m), actual);
        }
    }
}
