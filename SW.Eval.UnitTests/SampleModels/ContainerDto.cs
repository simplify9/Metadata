using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    public class ContainerDto
    {
        public AuditDto Audit { get; set; }

        public DateTime? ClosedOn { get; set; }

        public ContainerNumberDto ContainerNumber { get; set; }

        public AttachmentDto[] Attachments { get; set; }

        public ParcelNumberDto[] Containments { get; set; }


        public ReferenceDto[] References { get; set; }

        /// <summary>
        /// BAG, LST, FRT
        /// </summary>

        public string Type { get; set; }

        public string Description { get; set; }

        public CarrierDto AssignedTo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is ContainerDto dto &&
                   ContainerNumber.Number == dto.ContainerNumber.Number &&
                   ArraysAreEqual(References, dto.References) &&
                   Type == dto.Type &&
                   Description == dto.Description;
        }

        private bool ArraysAreEqual(object[] a, object[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i])) return false;
            }

            return true;

        }
        public override int GetHashCode()
        {
            var hashCode = 1582903641;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContainerNumber.Number);
            hashCode = hashCode * -1521134295 + EqualityComparer<ReferenceDto[]>.Default.GetHashCode(References);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }

        public override string ToString()
        {
            return $"Number: {ContainerNumber.Number}, Description: {Description}, Type: {Type}, References: ({References.Length})";
        }
        public class CarrierDto
        {
            public string Code { get; set; }

            public string Name { get; set; }
        }
        public class ReferenceDto : IEquatable<ReferenceDto>
        {
            public string Type { get; set; }

            public string Value { get; set; }

            public string CompositeReference => $"{Type}:{Value}";




            public override bool Equals(object obj)
            {
                return Equals(obj as ReferenceDto);
            }

            public bool Equals(ReferenceDto other)
            {
                return other != null &&
                       Type == other.Type &&
                       Value == other.Value;
            }

            public override int GetHashCode()
            {
                var hashCode = 1265339359;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
                return hashCode;
            }
        }
    }
}
