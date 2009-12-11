using System.IO;

namespace Rackspace.CloudFiles.Specs.Utils
{
    public class TextStreamFactory
    {
        public static Stream MakeFromString(string stringtowrite)
        {
            var memorystream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memorystream);
             
                writer.WriteLine(stringtowrite);
                writer.Flush();
                memorystream.Seek(0, 0);
                return memorystream;
             
        }
    }
}