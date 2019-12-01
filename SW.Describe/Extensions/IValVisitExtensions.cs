
using SW.Describe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Extensions
{
    public static class IValVisitExtensions
    {
        static IEnumerable<KeyValuePair<EPath,IPayload>> ProcessVisit(this IPayload n)
        {
            yield return new KeyValuePair<EPath, IPayload>(EPath.Root, n);
            
            if (n is EObject o)
            {
                var descendents = o
                    .SelectMany(prop => 
                        prop.Value.ProcessVisit()
                            .Select(subProp => 
                                new KeyValuePair<EPath, IPayload>(prop.Key.Append(subProp.Key), subProp.Value)));

                foreach (var i in descendents) yield return i;
            }

            //else if (n is ESet list)
            //{
            //    var descendents = list.SelectMany((item, idx) =>
            //        item.ProcessVisit().Select(pair => 
            //            new KeyValuePair<EPath, IPayload>(
            //                EPath.Root.Append(idx).Append(pair.Key),
            //                pair.Value)));

            //    foreach (var i in descendents) yield return i;
            //}
        }

        public static IEnumerable<KeyValuePair<EPath, IPayload>> Visit(this IPayload n)
        {
            return ProcessVisit(n);
        }
        
    }
}
