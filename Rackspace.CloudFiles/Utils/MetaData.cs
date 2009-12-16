using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace Rackspace.CloudFiles.utils
{
    public static class MetaData
    {
        public static void AddMetaDict(this WebHeaderCollection webheaders, IDictionary<string, string> metadata)
        {

            foreach (var kvp in metadata)
            {
                webheaders.Add(Constants.META_DATA_HEADER + kvp.Key, kvp.Value);
            }

        }
    }
}