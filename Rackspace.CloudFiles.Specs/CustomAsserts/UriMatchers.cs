using System;
using NUnit.Framework;

namespace Rackspace.CloudFiles.Specs.CustomAsserts
{
    public static class UriMatchers
    {
        private static void BoolCheck(bool booleancheck, string message)
        {
            
            if(!booleancheck)
                throw new AssertionException(message);
        }
        public static void StartsWith(this Uri basestring, string comparedto)
        {
            BoolCheck(basestring.ToString().StartsWith(comparedto) ,"Expected to find " + comparedto + " at start of uri.\nUrl actually looks like " + basestring );
        }
        public static void EndsWith(this Uri uri, string comparedto)
        {
            BoolCheck(uri.ToString().EndsWith(comparedto), "Expected to find " + comparedto + " at end of uri.\nUrl actually looks like " + uri);
        }
    }
}