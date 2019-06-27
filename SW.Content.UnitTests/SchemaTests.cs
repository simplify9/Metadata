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
        static readonly ContentSchemaRule[] _noRules = new ContentSchemaRule[] { };

        [TestMethod]
        public void Test_TypeToSchema()
        {
            var f = ContentSchemaNodeFactory.Default;
            var schema = f.CreateSchemaNodeFrom(typeof(Employee));

            Assert.IsInstanceOfType(schema, typeof(MustBeObject));

            var objSchema = schema as MustBeObject;

            var emp = ContentFactory.Default.CreateFrom(Employee.Sample);

            var issues = objSchema.FindIssues(emp).ToArray();

            Assert.AreEqual(0, issues.Length);
        }

        [TestMethod]
        public void Test_Validation()
        {
             
            var employeeSchema = new ContentSchema(new MustBeObject(
                new ContentProperty[]
                {
                    new ContentProperty("Id", new MustHaveType<ContentNumber>(_noRules), true),
                    new ContentProperty("Name", new MustHaveType<ContentText>(_noRules), true),
                    new ContentProperty("Phones", new MustBeList(
                        new MustHaveType<ContentText>(_noRules), 1, 3, _noRules), false),
                    new ContentProperty("Salary", new MustBeObject(new ContentProperty[]
                    {
                        new ContentProperty("Amount", new MustHaveType<ContentNumber>(_noRules), true),
                        new ContentProperty("Currency", new MustHaveType<ContentText>(new[] {new ContentSchemaRule("regex", new RegexFilter(new ScopeRootExpression(), "^[A-Z]{3,3}$")) }), true)
                    }, _noRules), true)
                }, _noRules));

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

            var expectedSubject = new ContentPath(new[] { nameof(Employee.Salary), nameof(Money.Currency) });
            Assert.AreEqual(expectedSubject, issues[0].SubjectPath);

            var dto = employeeSchema.Root.ToDto();

            
        }
    }
}
