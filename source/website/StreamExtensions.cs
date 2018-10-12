using System.IO;

namespace Website
{
    public static class StreamExtensions
    {
        public static byte[] ReadFullyToArray(this Stream input)
        {
            using (var memoryStream = new MemoryStream()) {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
