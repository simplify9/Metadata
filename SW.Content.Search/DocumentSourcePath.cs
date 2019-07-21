using SW.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search
{
    public class DocumentSourcePath
    {
        public ContentPath Path { get; }

        public int Offset { get; }

        public DocumentSourcePath(ContentPath path, int offset)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Offset = offset;
        }

        public override bool Equals(object obj)
        {
            var pos = obj as DocumentSourcePath;
            if (pos == null) return false;
            return pos.Path.Equals(Path) && pos.Offset == Offset;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Path}:{Offset}";
        }
    }
}
