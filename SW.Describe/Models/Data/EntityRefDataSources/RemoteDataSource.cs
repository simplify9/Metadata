using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class RemoteDataSource : IEntityRefDataSource
    {
        RemoteDataSource()
        {

        }

        public string Url { get; private set; }

        public RemoteDataSource(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}
