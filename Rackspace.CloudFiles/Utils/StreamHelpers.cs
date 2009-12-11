using System.IO;
using System.Text;

namespace Rackspace.CloudFiles.utils
{
    public static class StreamHelpers
    {
        public static string ConvertToString(this Stream stream)
        {
            using(var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }

        }
    }
}