using System.Net;
using NUnit.Framework;


namespace Rackspace.CloudFiles.Specs.CustomAsserts
{
    public class MatchesValue
    {
        private readonly string _value;

        public MatchesValue(string value)
        {
            _value = value;
        }
        public void HasValueOf(string expectedvalue)
        {
            if(_value!=expectedvalue)
                throw new AssertionException("Expected value of " + expectedvalue + " was actually " + _value);
        }
    }

    public static class WebHeaderMatchers
    {
        public static MatchesValue KeyValueFor(this WebHeaderCollection collection, string key)
        {
            var value = collection.Get(key);
            return new MatchesValue(value);
        }
    }
}