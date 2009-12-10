using System.Collections.Generic;
using System.Text;

namespace Rackspace.CloudFiles.utils
{
    public static class ArrayHelpers
    {
        public static string ConvertToString(this IList<string> toconvert)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var line in toconvert)
            {
                builder.AppendLine(line);
            }
            return builder.ToString();
        }
    }
}