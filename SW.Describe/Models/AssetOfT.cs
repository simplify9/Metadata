using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class Asset<TAsset>
    {
        Asset()
        {

        }

        public string Uri { get; private set; }
        
        public TAsset Data { get; private set; }
        
        public Asset(string uri, TAsset data)
        {
            var now = DateTime.UtcNow;
            Uri = uri;
            Data = data;
        }
    }
}
