using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SW.Content.Schema
{
    public class TypeDef<TContent> : ITypeDef where TContent : IContentNode
    {

        static readonly Dictionary<string,IContentSchemaConstraint> _empty 
            = new Dictionary<string, IContentSchemaConstraint> { };

        public IReadOnlyDictionary<string, IContentSchemaConstraint> Rules { get; }

        public TypeDef(IReadOnlyDictionary<string,IContentSchemaConstraint> rules = null)
        {
            Rules = rules ?? _empty;
        }

        public virtual IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (!(node is TContent))
            {
                yield return new SchemaIssue(ContentPath.Root,
                    $"Expected type {typeof(TContent)}, found {node.GetType()}");
            }
            else
            {
                var issues = Rules.SelectMany(rule => rule.Value.FindIssues(node));
                foreach (var i in issues) yield return i;
            }
        }
        
    }
}
