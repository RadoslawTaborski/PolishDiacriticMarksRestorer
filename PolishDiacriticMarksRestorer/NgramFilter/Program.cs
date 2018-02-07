using System;
using System.IO.Abstractions;
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
            var bootstrapper = new Bootstrapper(filter, new FileSystem(), new MySqlConnectionFactory());

            string output = null;
            Console.WriteLine("Przefiltrować dane? (T/N)");
            var decisionFilter = Console.ReadLine();
            Console.WriteLine("Utworzyć bazę danych? (T/N)");
            var decisionDb = Console.ReadLine();

            if (decisionFilter != null && decisionFilter.Equals("T"))
            {
                Console.WriteLine("Podaj ścieżkę do pliku z N-gramami: ");
                var input = Console.ReadLine();
                //var input = @"E:\PWr\magisterskie\magisterka\Materiały\1grams";
                Console.WriteLine("Podaj ścieżkę pliku wynikowego: ");
                output = Console.ReadLine();
                //output = @"E:\PWr\magisterskie\magisterka\Materiały\1grams_2";

                bootstrapper.Filter(input, output);
            }

            if (decisionDb != null && decisionDb.Equals("T"))
            {
                Console.WriteLine("Nazwa bazy danych: ");
                var dbName = Console.ReadLine();
                Console.WriteLine("Nazwa tabeli: ");
                var tableName = Console.ReadLine();

                Console.WriteLine("Liczba słów: ");
                var numberTxt = Console.ReadLine();
                var converted = int.TryParse(numberTxt, out var number);
                number = converted ? number : 1;

                if (output != null) bootstrapper.CreateDb(output, dbName, tableName, number);
                else
                {
                    Console.WriteLine("Podaj ścieżkę pliku z danymi: ");
                    output = Console.ReadLine();
                    //output = @"E:\PWr\magisterskie\magisterka\Materiały\1grams_2";
                    bootstrapper.CreateDb(output, dbName, tableName, number);
                }
            }

            Console.WriteLine("\nKONIEC\nNaciśnij ENTER by wyjść");
            Console.Read();
        }
    }
}
