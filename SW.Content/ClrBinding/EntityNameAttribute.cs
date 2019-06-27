using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.ClrBinding
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class EntityNameAttribute : Attribute
    {
        public string Name { get; }

        public EntityNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
