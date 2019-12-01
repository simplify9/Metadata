using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SW.Eval.Binding.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class PayloadReaderTests
    {

        [TestMethod]
        public void Test_Evaluation()
        {
            var employee = Employee.Sample;

            var reader = new PayloadReader(new[] { new FromJToken() });
            var converter = new PayloadConverter(new[] { new ToJToken() });

            var payload = reader.Read(employee);
            Assert.IsInstanceOfType(payload, typeof(IObject));

            var result = converter.TryConvert(payload, out Employee clone);
            Assert.IsNull(result);

            Assert.AreEqual(employee.ContractType, clone.ContractType);

            //payload.ValueOf("$.Name").

            //var document = ContentFactory.Default.CreateFrom(employee);

            //IContentNode actual = null;

            //document.TryEvaluate(PayloadPath.Parse("$.Name"), out actual);
            //Assert.AreEqual(new ContentText(employee.Name), actual);

            //document.TryEvaluate(PayloadPath.Parse("$.Data.FavColor"), out actual);
            //Assert.AreEqual(new ContentText("red"), actual);

            //var result = document.TryEvaluate(PayloadPath.Parse("$.Data.Start_ingSalary.Amount"), out actual);
            //Assert.IsFalse(result);

            //document.TryEvaluate(PayloadPath.Parse("$.Data.StartingSalary.Amount"), out actual);
            //Assert.AreEqual(new ContentNumber(500.57m), actual);

            //// Test JSON

            //var jsonText = JsonConvert.SerializeObject(employee);

            //var jToken = JsonConvert.DeserializeObject<JToken>(jsonText);

            //document = ContentFactory.Default.CreateFrom(jToken);

            //document.TryEvaluate(PayloadPath.Parse("$.Name"), out actual);
            //Assert.AreEqual(new ContentText(employee.Name), actual);

            //document.TryEvaluate(PayloadPath.Parse("$.Data.FavColor"), out actual);
            //Assert.AreEqual(new ContentText("red"), actual);

            //result = document.TryEvaluate(PayloadPath.Parse("$.Data.Start_ingSalary.Amount"), out actual);
            //Assert.IsFalse(result);

            //document.TryEvaluate(PayloadPath.Parse("$.Data.StartingSalary.Amount"), out actual);
            //Assert.AreEqual(new ContentNumber(500.57m), actual);
        }

        [TestMethod]
        public void Test_Cycles()
        {
            //var o = new TypeWithCycles();
            //o.Self = o;
            //o.MyProperty = 5;
            //o.Child = new Sub1
            //{
            //    Parent = o,
            //    SubChild = new Sub1
            //    {
            //        Parent = o,
            //        Prop1 = 7,
            //        SubChild = new Sub1 { }
            //    }
            //};

            //var c = ContentFactory.Default.CreateFrom(o);

            //var arr = c.Visit().ToArray();

            //Assert.IsTrue(arr.Any(p => p.Path.Equals(PayloadPath.Parse("$.Child.SubChild.SubChild")) &&
            //    p.Value is ContentObject));

            //Assert.IsTrue(arr.Any(p => p.Path.Equals(PayloadPath.Parse("$.Child.SubChild.SubChild.Prop1")) &&
            //    p.Value is ContentNumber));

            //Assert.IsTrue(arr.Any(p => p.Path.Equals(PayloadPath.Parse("$.Child.SubChild.Prop1")) &&
            //    p.Value is ContentNumber));

            //Assert.AreEqual(10, arr.Length);
        }
    }
}
