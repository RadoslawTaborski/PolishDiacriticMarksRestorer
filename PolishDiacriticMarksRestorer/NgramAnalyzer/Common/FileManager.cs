using System;
using System.IO;
using System.Linq;
using NgramAnalyzer.Interfaces;
using System.IO.Abstractions;

namespace NgramAnalyzer.Common
{
    public class FileManager : IFileAccess
    {
        private StreamReader _sr;
        private StreamWriter _sw;
        private readonly IFileSystem _fs;
        private FileManagerType _type;
        private readonly string _path;

        public FileManager(IFileSystem fs, string path)
        {
            _type = FileManagerType.Nothing;
            _fs = fs;
            _path = path;
        }

        public bool Open(FileManagerType type)
        {
            if (!_fs.File.Exists(_path)) return false;

            _type = type;

            switch (_type)
            {
                case FileManagerType.Read:
                    _sr = new StreamReader(_fs.File.Open(_path,FileMode.Open));
                    break;
                case FileManagerType.Write:
                    _sw = new StreamWriter(_fs.File.Open(_path, FileMode.Open));
                    break;
                case FileManagerType.Nothing:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;

        }

        public bool Close()
        {
            if (_sr == null && _sw == null) return false;

            switch (_type)
            {
                case FileManagerType.Read:
                    _sr?.Close();
                    break;
                case FileManagerType.Write:
                    _sw?.Close();
                    break;
                case FileManagerType.Nothing:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        public void WriteLine(string str)
        {
            if (_type == FileManagerType.Write)
                _sw.WriteLine(str, true);
        }

        public string ReadLine()
        {
            return _type == FileManagerType.Read ? _sr.ReadLine() : "";
        }

        public int CountLines()
        {
            return _fs.File.ReadLines(_path).Count();
        }

        public void Create()
        {
            var fs = _fs.File.Create(_path);
            fs.Close();
        }
    }
}
