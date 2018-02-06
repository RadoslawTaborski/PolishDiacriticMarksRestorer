using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IFileAccess
    {
        bool Open(FileManagerType type);
        void Close();
        string ReadLine();
        void WriteLine(string str);
        void Create();
        int CountLines();
    }
}
