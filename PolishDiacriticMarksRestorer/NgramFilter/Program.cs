using System;
using System.IO;
using System.Linq;
using NgramAnalyzer.Common;
using NgramFilter.FilterItems;

namespace NgramFilter
{
    internal class Program
    {

        private static void Main()
        {
            var filter = new Filter();
            filter.Add(new OnlyWords());
            filter.Add(new MultipleInstances());

            Console.WriteLine("Podaj ścieżkę do pliku z N-gramami: ");
            var inp = Console.ReadLine();
            var input = new FileManager(inp);
            Console.WriteLine("Podaj ścieżkę pliku wynikowego: ");
            var outp = Console.ReadLine();
            var output = new FileManager(outp);

            try
            {
                output.Create();
                input.Open(FileManagerType.Read);
                output.Open(FileManagerType.Write);

                string str;
                while ((str = input.ReadLine()) != null)
                {
                    var list = str.Split(' ').ToList().Where(s => s != "").ToList();
                    var ngram = new NGram
                    {
                        Value = int.Parse(list[0]),
                        WordsList = list.GetRange(1, list.Count - 1)
                    };
                    var result = filter.Start(ngram);

                    if (!result) continue;

                    output.WriteLine(ngram.ToString());
                }

                input.Close();
                output.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("End");
            Console.Read();
        }
    }
}
