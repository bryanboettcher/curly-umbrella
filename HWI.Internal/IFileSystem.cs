using System.Collections.Generic;
using System.IO;
using HWI.Internal.Logging;

namespace HWI.Internal
{
    public interface IFileSystem
    {
        void Move(string source, string dest);
        void Copy(string source, string dest, bool doOverwrite = false);
        void Replace(string source, string dest, string backup);
        void Delete(string source);
        void WriteAllBytes(string source, byte[] bytes);
        bool Exists(string source);
        string ReadAllText(string path);
        void WriteAllText(string path, string data);
        void WriteAllLines(string path, IEnumerable<string> lines);
        void CreateFolder(string path);
        IEnumerable<string> GetFiles(string path, string pattern = null, bool recursive = false);
        IEnumerable<string> ReadLines(string path);

        TextReader OpenFileReader(string path);
        TextWriter OpenFileWriter(string path, bool overwrite);
    }
}