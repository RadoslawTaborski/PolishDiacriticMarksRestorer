using System;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IFileAccess : IDisposable
    {
        /// <summary>
        /// This method opens a StreamReader or WriteReader.
        /// </summary>
        /// <param name="type">Read or Write or Nothing.</param>
        /// <returns>
        /// If the connection was made with Read or Write type - true.
        /// </returns>
        bool Open(FileManagerType type);
        /// <summary>
        /// This method close StreamReader and StreamWriter.
        /// </summary>
        void Close();
        /// <summary>
        /// If type is Read, this method read line from opened file.
        /// </summary>
        /// <returns>
        /// Read string.
        /// </returns>
        string ReadLine();
        /// <summary>
        /// If type is Write, this method writes line to opened file.
        /// </summary>
        /// <param name="str">string which will be added to file.</param>
        void WriteLine(string str);
        /// <summary>
        /// This method create new file.
        /// </summary>
        void Create();
        /// <summary>
        /// This method count lines in opened file.
        /// </summary>
        /// <returns>
        /// number of lines.
        /// </returns>
        int CountLines();
    }
}
