using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class SchemaIssue
    {
        public ContentPath SubjectPath { get; }

        public string Error { get; }

        public SchemaIssue(ContentPath subjectPath, string error)
        {
            SubjectPath = subjectPath ?? throw new ArgumentNullException(nameof(subjectPath));
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }
    }
}
