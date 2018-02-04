using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IFileAccess
    {
        bool Open(FileManagerType type);
        bool Close();
        string ReadLine();
        void WriteLine(string str);
        void Create();
        int CountLines();
    }
}
