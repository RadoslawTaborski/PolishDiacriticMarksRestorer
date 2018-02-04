using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Common;
using NgramAnalyzer.Interfaces;
using NgramFilter.FilterItems;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    internal class Program
    {
        private static IFilter _filter;
        private static void Main()
        {
            string output = null;
            Console.WriteLine("Przefiltrować dane? (TAK/NIE)");
            var decisionFilter = Console.ReadLine();

            if (decisionFilter != null && decisionFilter.Equals("TAK"))
            {
                _filter = new Filter();
                _filter.Add(new OnlyWords());
                _filter.Add(new MultipleInstances());

                Console.WriteLine("Podaj ścieżkę do pliku z N-gramami: ");
                var input = Console.ReadLine();
                //var input = @"E:\PWr\magisterskie\magisterka\Materiały\1grams";
                Console.WriteLine("Podaj ścieżkę pliku wynikowego: ");
                output = Console.ReadLine();
                //output = @"E:\PWr\magisterskie\magisterka\Materiały\1grams_2";

                Filter(input, output);
            }

            Console.WriteLine("Utworzyć bazę danych? (TAK/NIE)");
            var decisionDb = Console.ReadLine();

            if (decisionDb != null && decisionDb.Equals("TAK"))
            {
                Console.WriteLine("Nazwa bazy danych: ");
                var dbName = Console.ReadLine();
                Console.WriteLine("Nazwa tabeli: ");
                var tableName = Console.ReadLine();

                Console.WriteLine("Liczba słów: ");
                var numberTxt = Console.ReadLine();
                var converted = int.TryParse(numberTxt, out var number);
                number = converted ? number : 1;

                if (output != null) CreateDb(output,dbName,tableName, number);
                else
                {
                    Console.WriteLine("Podaj ścieżkę pliku z danymi: ");
                    output = Console.ReadLine();
                    //output = @"E:\PWr\magisterskie\magisterka\Materiały\1grams_2";
                    CreateDb(output, dbName, tableName, number);
                }
            }

            Console.WriteLine("\nKONIEC\nNaciśnij ENTER by wyjść");
            Console.Read();
        }

        private static void Filter(string input, string output)
        {
            IFileAccess inputManager = new FileManager(input);
            IFileAccess outputManager = new FileManager(output);
            try
            {
                outputManager.Create();
                var numberOfLines = inputManager.CountLines();
                inputManager.Open(FileManagerType.Read);
                outputManager.Open(FileManagerType.Write);
                var counter = 0;

                string str;
                while ((str = inputManager.ReadLine()) != null)
                {
                    var list = str.Split(' ').ToList().Where(s => s != "").ToList();
                    var ngram = new NGram
                    {
                        Value = int.Parse(list[0]),
                        WordsList = list.GetRange(1, list.Count - 1)
                    };
                    var filterResult = _filter.Start(ngram);
                    ++counter;
                    var percent = (double)counter * 100 / numberOfLines;
                    Console.Write(percent.ToString("F3", CultureInfo.InvariantCulture) + "%\r");
                    if (!filterResult) continue;

                    outputManager.WriteLine(ngram.ToString());
                }

                Console.WriteLine("Ukończono pomyślnie\n");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            finally
            {
                inputManager.Close();
                outputManager.Close();
            }
        }

        private static void CreateDb(string input, string dbName, string tableName, int numberOfWords)
        {
            IFileAccess inputManager = new FileManager(input);
            IDataBaseCreator creator = null;
            try
            {
                var numberOfLines = inputManager.CountLines();
                var counter = 0;
                inputManager.Open(FileManagerType.Read);
                var dbManager = new DataBaseManager("localhost", "NGrams", "root", "");
                creator = new DataBaseCreator(dbManager);
                creator.CreateDataBase(dbName);
                creator.CreateTable(dbName, tableName, numberOfWords);
                creator.OpenDb();

                string str;
                while ((str = inputManager.ReadLine()) != null)
                {
                    var list = str.Split(' ').ToList().Where(s => s != "").ToList();
                    var ngram = new NGram
                    {
                        Value = int.Parse(list[0]),
                        WordsList = list.GetRange(1, list.Count - 1)
                    };
                    ngram.ChangeSpecialCharacters();
                    creator.AddNgramToTable(tableName, ngram);
                    ++counter;
                    var percent = (double)counter * 100 / numberOfLines;
                    Console.Write(percent.ToString("F3", CultureInfo.InvariantCulture) + "%\r");
                }

                Console.WriteLine("Ukończono pomyślnie");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            finally
            {
                inputManager.Close();
                creator?.CloseDb();
            }
        }
    }
}
