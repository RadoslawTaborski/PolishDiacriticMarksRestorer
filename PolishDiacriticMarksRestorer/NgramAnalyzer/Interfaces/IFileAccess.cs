using System;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IFileAccess : IDisposable
    {
        bool Open(FileManagerType type);
        void Close();
        string ReadLine();
        void WriteLine(string str);
        void Create();
        int CountLines();
    }
}
