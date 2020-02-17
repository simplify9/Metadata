using SW.Content.UnitTests.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{

    public enum GenderType
    {
        Male, 
        Female
    }

    public enum EmploymentType
    {
        Permenant,
        Temporary
    }

    public class Money
    {
        public decimal? Amount { get; set; }

        public string Currency { get; set; }
    }

    public class TypeWithPrivateSetters
    {
        public int Prop1 { get; private set; }
    }

    

    public class Sub1
    {
        public int Prop1 { get; set; }

        public TypeWithCycles Parent { get; set; }

        public Sub1 SubChild { get; set; }
    }

    public class TypeWithCycles
    {
        public TypeWithCycles Self { get; set; }

        public int MyProperty { get; set; }

        public Sub1 Child { get; set; }
    }

    public class Employee
    {
        public static Employee Sample = new Employee
            {
                Data = new Dictionary<string, object>
                {
                    { "FavColor", "red" },
                    { "StartingSalary", new Money { Amount = 500.57m, Currency = "JOD" }
}
                },
                Contracts = new AccountContractDto[] { new AccountContractDto { Product="ddd",Ratesheet ="dd" ,ChargeType = "dd",InActive =true} },
                Id = 7,

                EndDate = DateTime.UtcNow,

                Gender = GenderType.Female,

                ContractType = EmploymentType.Permenant,

                Name = "John Smith",

                Phones = new[] { "12345", "67890" },

                Salary = new Money {  Amount = 1000, Currency = "JOD" },

                StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(365))
            };

        public int Id { get; set; }

        public string Name { get; set; }

        public GenderType? Gender { get; set; }

        public string[] Phones { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        public Money Salary { get; set; }

        public EmploymentType? ContractType { get; set; }

        public AccountContractDto[]  Contracts { get; set; }

        public IDictionary<string,object> Data { get; set; }
    }
}
