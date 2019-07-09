using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

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

            document.TryEvaluate(ContentPath.Parse("$.Name"), out actual);
            Assert.AreEqual(new ContentText(employee.Name), actual);

            document.TryEvaluate(ContentPath.Parse("$.Data.FavColor"), out actual);
            Assert.AreEqual(new ContentText("red"), actual);

            var result = document.TryEvaluate(ContentPath.Parse("$.Data.Start_ingSalary.Amount"), out actual);
            Assert.IsFalse(result);

            document.TryEvaluate(ContentPath.Parse("$.Data.StartingSalary.Amount"), out actual);
            Assert.AreEqual(new ContentNumber(500.57m), actual);

            // Test JSON

            var jsonText = JsonConvert.SerializeObject(employee);

            var jToken = JsonConvert.DeserializeObject<JToken>(jsonText);

            document = ContentFactory.Default.CreateFrom(jToken);

            document.TryEvaluate(ContentPath.Parse("$.Name"), out actual);
            Assert.AreEqual(new ContentText(employee.Name), actual);

            document.TryEvaluate(ContentPath.Parse("$.Data.FavColor"), out actual);
            Assert.AreEqual(new ContentText("red"), actual);

            result = document.TryEvaluate(ContentPath.Parse("$.Data.Start_ingSalary.Amount"), out actual);
            Assert.IsFalse(result);

            document.TryEvaluate(ContentPath.Parse("$.Data.StartingSalary.Amount"), out actual);
            Assert.AreEqual(new ContentNumber(500.57m), actual);
        }

        [TestMethod]
        public void Test_Cycles()
        {
            var o = new TypeWithCycles();
            o.Self = o;
            o.MyProperty = 5;
            o.Child = new Sub1
            {
                Parent = o,
                SubChild = new Sub1
                {
                    Parent = o,
                    Prop1 = 7,
                    SubChild = new Sub1 { }
                }
            };

            var c = ContentFactory.Default.CreateFrom(o);

            var arr = c.Visit().ToArray();

            Assert.IsTrue(arr.Any(p => p.Path.Equals(ContentPath.Parse("$.Child.SubChild.SubChild")) &&
                p.Value is ContentObject));

            Assert.IsTrue(arr.Any(p => p.Path.Equals(ContentPath.Parse("$.Child.SubChild.SubChild.Prop1")) &&
                p.Value is ContentNumber));

            Assert.IsTrue(arr.Any(p => p.Path.Equals(ContentPath.Parse("$.Child.SubChild.Prop1")) &&
                p.Value is ContentNumber));

            Assert.AreEqual(10, arr.Length);
        }
    }
}
