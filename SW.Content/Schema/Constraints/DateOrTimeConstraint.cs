using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.Schema
{
    public class DateOrTimeConstraint : IContentSchemaConstraint
    {
        static readonly Regex _dateOnly = new Regex(@"^\d\d\d\d-(0?[1-9]|1[0-2])-(0?[1-9]|[12][0-9]|3[01])$");

        static readonly Regex _timeOnly = new Regex(@"^(00|[0-9]|1[0-9]|2[0-3]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])$");

        static readonly Regex _dateTimeRegex = new Regex($"^{ContentDateTime.Regex}$");

        public bool HasDate { get; private set; }

        public bool HasTime { get; private set; }

        public DateOrTimeConstraint(bool hasDate, bool hasTime)
        {
            HasDate = hasDate;
            HasTime = hasTime;
            if (!HasDate && !HasTime)
            {
                throw new ArgumentException("DateTime schema node must either have date or time or both, but cannot be both false");
            }
        }

        public IEnumerable<SchemaIssue> FindIssues(IContentNode node)
        {
            if (node is ContentText text)
            {
                var regex = HasDate && HasTime
                    ? _dateTimeRegex
                    : (HasDate ? _dateOnly : _timeOnly);

                if (!regex.IsMatch(text.Value))
                {
                    yield return new SchemaIssue(ContentPath.Root, $"Value '{text.Value}' is not a valid date, time or date time");
                }
            }
        }
    }
}
