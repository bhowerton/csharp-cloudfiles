using System;
using NUnit.Framework;

namespace Rackspace.CloudFiles.Specs.CustomAsserts
{
    public static class Asserts
    {
  
        public static void Throws<T>(Action action)where T:Exception,new()
        {
            try
            {
                action.Invoke();
                Assert.Fail(String.Format("exception of {0} was not called", typeof(T).Name));
            }
            catch(Exception ex)
            {
                if (ex.GetType() == typeof(T))
                    return;
                throw;
            }
        }
    }
}