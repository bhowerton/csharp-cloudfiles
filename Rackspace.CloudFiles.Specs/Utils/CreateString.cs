namespace Rackspace.CloudFiles.Specs.Utils
{
    static class CreateString
    {
        public static string WithLength(int length)
        {
            return new string('a', length);
        }
    }
}