using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Rackspace.CloudFiles.utils
{
    public static class StringHelper
    {
        public static string Capitalize(this String wordToCapitalize)
        {
            if (String.IsNullOrEmpty(wordToCapitalize))
                throw new ArgumentNullException();

            return char.ToUpper(wordToCapitalize[0]) + wordToCapitalize.Substring(1);
        }

        public static string Capitalize(this bool booleanValue)
        {
            
            return booleanValue ? "True" : "False";
        }

        public static string Encode(this string stringToEncode)
        {
            if (String.IsNullOrEmpty(stringToEncode))  
                throw new ArgumentNullException();

            return HttpUtility.UrlEncode(stringToEncode).Replace("+", "%20");
        }

        public static string StripSlashPrefix(this string path)
        {
            return path[0] == '/' ? path.Substring(1, path.Length - 1) : path;
        }
		public static DateTime ParseCfDateTime(this string datestring){
			string format = "yyyy-MM-ddThh:mm:ss.ffffff";
			return DateTime.ParseExact( datestring, format,CultureInfo.InvariantCulture);
		}

        public static string MimeType(this string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();

            string mimetype = "";
            Constants.ExtensionToMimeTypeMap.TryGetValue(extension, out mimetype);
            return mimetype ?? "application/octet-stream";
        }

    }
}