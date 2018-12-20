using System.IO;

namespace HWI.Internal
{
    public class StreamProvider:IStreamProvider
    {
        public FileStream GetFileStream(string pathToFile)
        {
            return new FileStream(pathToFile, FileMode.Open);
        }

        public void Dispose()
        {
        }
    }
}