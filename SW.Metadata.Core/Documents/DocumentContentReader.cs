using SW.Metadata.Core.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class DocumentContentReader
    {


        static readonly IDocumentValueFactory[] _defaultFactories =
        {
            // ORDER IS IMPORTANT !!
            new FromClrNullFactory(),
            new FromDocumentValueFactory(),
            new FromJTokenFactory(),
            new FromClrStringFactory(),
            new FromClrBooleanFactory(),
            new FromClrDateTimeFactory(),
            new FromClrNumberTypeFactory(),
            new FromClrDictionaryFactory(),
            new FromClrEnumerableFactory(),
            new FromClrPocoFactory()
        };

        readonly IEnumerable<IDocumentValueFactory> _factories;

        readonly IDictionary<DocumentPath, IDocumentValue> _valueCache;

        IDocumentValue CreateValue(object obj)
        {
            var value = _factories
                .Select(f_ => f_.CreateFrom(obj))
                .Where(v => v != null)
                .FirstOrDefault();

            if (value != null) return value;

            throw new NotSupportedException($"Cannot create a document value from type '{obj?.GetType()}'");
        }

        public DocumentContentReader(object data, IEnumerable<IDocumentValueFactory> valueFactories = null)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _valueCache = new Dictionary<DocumentPath, IDocumentValue>();

            _factories = valueFactories == null
                ? _defaultFactories 
                : _defaultFactories.Concat(valueFactories);

            Root = CreateValue(data);
        }

        public IDocumentValue Root { get; }

        public bool TryEvaluate(DocumentPath path, out IDocumentValue result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!_valueCache.TryGetValue(path, out result))
            {
                var success = Root.TryEvaluate(path, CreateValue, out result);
                if (success) _valueCache.Add(path, result);
                return success;
            }

            return true;
        }

        public IEnumerable<IDocumentValue> AsEnumerable(IDocumentValue list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            return list.AsEnumerable(CreateValue);
        }

        public DocumentContentReader CreateSubReader(IDocumentValue root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            return new DocumentContentReader(root, _factories);
        }

        // TODO ... more relevant properties (e.g. document version perhaps? )
    }
}
