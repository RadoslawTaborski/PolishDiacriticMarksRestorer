using NgramAnalyzer.Common;
using System;
using System.IO;

namespace PolishDiacriticMarksRemover
{
    class Program
    {
        static void Main()
        {
            var path = @"D:\PWr\magisterskie\magisterka\Teksty\";
            Console.Write($"Podaj ścieżkę do tekstu: {path}");

            var path1 = Console.ReadLine();
            Console.Write($"Podaj ścieżkę do zapisu: {path}");
            var path2 = Console.ReadLine();

            try
            {
                string text;

                using (var sr = new StreamReader(path+path1))
                {
                    text = sr.ReadToEnd();
                }

                text = text.RemoveDiacritics();

                File.WriteAllText(path+path2, text);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Brak pliku z danymi");
            }
            catch (FormatException)
            {
                Console.WriteLine("Błędna zawartość pliku z danymi");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Koniec");
            Console.Read();
        }
    }
}
