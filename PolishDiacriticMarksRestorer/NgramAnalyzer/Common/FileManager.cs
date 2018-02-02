using System;
using System.IO;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common
{
    public class FileManager : IFileAccess
    {
        private StreamReader _sr;
        private StreamWriter _sw;
        private FileManagerType _type;
        private readonly string _path;

        public FileManager(string path)
        {
            _type = FileManagerType.Nothing;
            _path = path;
        }

        public bool Open(FileManagerType type)
        {
            if (!File.Exists(_path)) return false;

            _type = type;

            switch (_type)
            {
                case FileManagerType.Read:
                    _sr=new StreamReader(_path);
                    break;
                case FileManagerType.Write:
                    _sw = new StreamWriter(_path);
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
            if (_sr == null) return false;

            _sr.Close();
            return true;
        }

        public void WriteLine(string str)
        {
            if(_type == FileManagerType.Write)
                _sw.WriteLine(str);
        }

        public string ReadLine()
        {
            return _type==FileManagerType.Read ? _sr.ReadLine() : "";
        }

        public void Create()
        {
            File.Create(_path);
        }
    }
}
