using NgramAnalyzer.Interfaces;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// FileManager Class allows to manage file.
    /// </summary>
    /// <seealso cref="NgramAnalyzer.Interfaces.IFileAccess" />
    public class FileManager : IFileAccess
    {
        #region FIELDS
        private StreamReader _sr;
        private StreamWriter _sw;
        private readonly IFileSystem _fileSystem;
        private FileManagerType _type;
        private readonly string _path;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="fileSystem">The fileSystem.</param>
        /// <param name="path">The path to file.</param>
        public FileManager(IFileSystem fileSystem, string path)
        {
            _type = FileManagerType.Nothing;
            _fileSystem = fileSystem;
            _path = path;
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method opens a StreamReader or WriteReader.
        /// </summary>
        /// <param name="type">Read or Write or Nothing.</param>
        /// <returns>
        /// if the connection was made with Read or Write type - true.
        /// </returns>
        /// <inheritdoc />
        public bool Open(FileManagerType type)
        {
            if (!_fileSystem.File.Exists(_path)) return false;

            _type = type;

            switch (_type)
            {
                case FileManagerType.Read:
                    _sr = new StreamReader(_fileSystem.File.Open(_path, FileMode.Open));
                    break;
                case FileManagerType.Write:
                    _sw = new StreamWriter(_fileSystem.File.Open(_path, FileMode.Open));
                    break;
                case FileManagerType.Nothing:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This method close StreamReader and StreamWriter.
        /// </summary>
        /// <inheritdoc />
        public void Close()
        {
            _sr?.Close();
            _sw?.Close();
        }

        /// <summary>
        /// If type is Write, this method writes line to opened file.
        /// </summary>
        /// <param name="str">string which will be added to file.</param>
        /// <inheritdoc />
        public void WriteLine(string str)
        {
            if (_type == FileManagerType.Write)
                _sw.WriteLine(str, true);
        }

        /// <summary>
        /// If type is Read, this method read line from opened file.
        /// </summary>
        /// <returns>
        /// Read string.
        /// </returns>
        /// <inheritdoc />
        public string ReadLine()
        {
            return _type == FileManagerType.Read ? _sr.ReadLine() : null;
        }

        /// <summary>
        /// This method count lines in opened file.
        /// </summary>
        /// <returns>
        /// Number of lines.
        /// </returns>
        /// <inheritdoc />
        public int CountLines()
        {
            return _fileSystem.File.ReadLines(_path).Count();
        }

        /// <summary>
        /// This method create new file.
        /// </summary>
        /// <inheritdoc />
        public void Create()
        {
            var fs = _fileSystem.File.Create(_path);
            fs.Close();
        }

        /// <summary>
        /// This method close StreamReader and StreamWriter.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            Close();
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}
