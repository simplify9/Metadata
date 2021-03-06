﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class CanBeAnything : IMust
    {
        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            yield break;
        }

        public bool TryGetSchema(ContentPath path, out IMust schema)
        {
            schema = new CanBeAnything();
            return true;
        }
    }
}
