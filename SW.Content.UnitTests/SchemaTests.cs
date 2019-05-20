using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class SchemaTests
    {
        [TestMethod]
        public void Test_Validation()
        {
            
            var employeeSchema = new ContentSchema(new MustBeObject(
                new ContentProperty[]
                {
                    new ContentProperty("Id", new MustBeWholeNumber(), true),
                    new ContentProperty("Name", new MustHaveType<ContentText>(), true),
                    new ContentProperty("Phones", new MustBeList(
                        new MustHaveType<ContentText>(), 1, 3), false),
                    new ContentProperty("Salary", new MustBeObject(new ContentProperty[]
                    {
                        new ContentProperty("Amount", new MustHaveType<ContentNumber>(), true),
                        new ContentProperty("Currency", new MustMatchRegex("^[A-Z]{3,3}$"), true)
                    }), true)
                }));

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

            // object(id*:wholenumber[1:3],name:text,phones:list[1:3](text)
        }
    }
}
