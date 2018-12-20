using System;
using System.IO;

namespace HWI.Internal
{
    public interface IStreamProvider : IDisposable
    {
        FileStream GetFileStream(string pathToFile);


    }
}