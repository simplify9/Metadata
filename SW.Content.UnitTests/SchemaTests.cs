using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Filters;
using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SW.Content.Serialization.Schema;
using SW.Content.Expressions;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class SchemaTests
    {
        static readonly IContentSchemaConstraint[] _noRules = new IContentSchemaConstraint[] { };

        [TestMethod]
        public void Test_TypeToSchema()
        {
            var f = ContentSchemaNodeFactory.Default;
            var schema = f.CreateSchemaNodeFrom(typeof(Employee));

            Assert.IsInstanceOfType(schema, typeof(TypeDef<ContentObject>));

            var objSchema = schema as TypeDef<ContentObject>;

            var emp = ContentFactory.Default.CreateFrom(Employee.Sample);

            var issues = objSchema.FindIssues(emp).ToArray();

            Assert.AreEqual(0, issues.Length);
        }

        [TestMethod]
        public void Test_Validation()
        {

            var employeeSchema = new TypeDef<ContentObject>()
                .WithProperty("Id", true, new TypeDef<ContentNumber>())
                .WithProperty("Name", true, new TypeDef<ContentText>())
                .WithProperty("Phones", false, new TypeDef<ContentList>()
                    .WithItemsOfType(new TypeDef<ContentText>())
                    .WithConstraint("_range", new ListItemCountConstraint(1, 3)))
                .WithProperty("Salary", true, new TypeDef<ContentObject>()
                    .WithProperty("Amount", true, new TypeDef<ContentNumber>())
                    .WithProperty("Currency", true, new TypeDef<ContentText>()
                        .WithConstraint("_regex", new RegexConstraint("^[A-Z]{3,3}$"))));
                
            var issues = employeeSchema
                .FindIssues(ContentFactory.Default.CreateFrom(Employee.Sample))
                .ToArray();
            
            Assert.AreEqual(0, issues.Length);

            var invalidSample = Employee.Sample;
            invalidSample.Salary.Currency = "_F3";

            issues = employeeSchema
                .FindIssues(ContentFactory.Default.CreateFrom(invalidSample))
                .ToArray();

            Assert.AreEqual(1, issues.Length);

            var expectedSubject = ContentPath.Root.Append(nameof(Employee.Salary)).Append(nameof(Money.Currency));
            Assert.AreEqual(expectedSubject, issues[0].SubjectPath);

            //var dto = employeeSchema.ToDto();

            
        }
    }
}
