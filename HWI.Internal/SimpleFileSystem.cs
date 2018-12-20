using System.Collections.Generic;
using System.IO;
using HWI.Internal.Logging;

namespace HWI.Internal
{
    public class SimpleFileSystem : IFileSystem
    {
        public SimpleFileSystem(ILogger logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;

        public void Move(string source, string dest)
        {
            _logger.Debug($"About to copy <{source}> to <{dest}>...");
            File.Move(source, dest);
        }

        public void Copy(string source, string dest, bool doOverwrite = false)
        {
            var destIsValidDirectory = (Directory.Exists(dest));
            var sourceFi = new FileInfo(source);
            if (destIsValidDirectory && sourceFi.Exists)
            {
                _logger.Debug($"Trying to copy file <{source}> to directory <{dest}>");
                dest = Path.Combine(dest, sourceFi.Name);
            }
            _logger.Debug($"About to copy <{source}> to <{dest}>...");
            File.Copy(source, dest, doOverwrite);
        }

        public void Replace(string source, string dest, string backup)
        {
            _logger.Debug($"About to backup file <{dest}> to <{backup}> and write {source.Length} characters to the original file...");
            File.Replace(source, dest, backup);
        }

        public void Delete(string source)
        {
            _logger.Debug($"About to delete file <{source}>...");
            File.Delete(source);
        }

        public void WriteAllBytes(string source, byte[] bytes)
        {
            _logger.Debug($"Writing {bytes.Length} bytes to <{source}>...");
            File.WriteAllBytes(source, bytes);
        }

        public bool Exists(string source)
        {
            var fileExists = File.Exists(source);
            _logger.Debug($"The file <{source}> does {(fileExists ? "" : "NOT ")}exist.");
            return fileExists;
        }

        public string ReadAllText(string path)
        {
            _logger.Debug($"Reading <{path}>...");
            var content = File.ReadAllText(path);
            _logger.Debug($"Read {content.Length} characters from {path}");
            return content;
        }

        public void WriteAllText(string path, string data)
        {
            _logger.Debug($"Wirting {data.Length} characters to <{path}>...");
            File.WriteAllText(path, data);
        }

        public void WriteAllLines(string path, IEnumerable<string> lines)
        {
            File.WriteAllLines(path, lines);
        }

        public void CreateFolder(string path)
        {
            if (Directory.Exists(path))
            {
                _logger.Debug($"Folder <{path}> already exists.");
                return;
            }
            _logger.Debug($"Creating folder <{path}>...");
            Directory.CreateDirectory(path);
        }

        public IEnumerable<string> GetFiles(string path, string pattern = null, bool recursive = false)
        {
            return pattern == null
                ? Directory.GetFiles(path)
                : Directory.GetFiles(path, pattern,
                    recursive 
                        ? SearchOption.TopDirectoryOnly 
                        : SearchOption.AllDirectories
            );
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }

        public TextReader OpenFileReader(string path)
        {
            return File.OpenText(path);
        }

        public TextWriter OpenFileWriter(string path, bool overwrite)
        {
            return overwrite
                ? File.CreateText(path)
                : File.AppendText(path);
        }
    }
}