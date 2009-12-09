using System;

namespace Rackspace.CloudFiles.Specs.CustomAsserts
{
    public class AssertIt
    {
        public static void should(string message, Action action)
        {
            try
            {
                action.Invoke();
            }
            catch 
            {
                Console.WriteLine("The following assert failed : " + message);
                throw;
            }
           
        }
    }
}