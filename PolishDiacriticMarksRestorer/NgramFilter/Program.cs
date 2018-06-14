using System;
using System.IO;
using System.IO.Abstractions;
using MySql.Data.MySqlClient;
using NgramAnalyzer.Common;
using NgramFilter.FilterItems;

namespace NgramFilter
{
    internal class Program
    {
        #region FIELDS
        private static Bootstrapper _bootstrapper;
        #endregion

        private static void Main()
        {
            var filter = new Filter();
            //filter.Add(new MultipleInstances());
            filter.Add(new OnlyWords());
            filter.Add(new WordsWithoutNonPunctationMarks());
            filter.Add(new NotLongWords());
            var modifier = new Modifier();
            _bootstrapper = null;

            string output = null;
            string dbName = null;
            string tableName = null;
            Console.WriteLine("Przefiltrować dane? (T/N)");
            var decisionFilter = Console.ReadLine();
            Console.WriteLine("Utworzyć bazę danych? (T/N)");
            var decisionDb = Console.ReadLine();
            Console.WriteLine("Czy stworzyć alfabetyczne tabele? (T/N)");
            var decisionTables = Console.ReadLine();

            if (decisionTables != null && decisionTables == "T")
            {
                _bootstrapper = new Bootstrapper(filter, modifier, new FileSystem(), new MySqlConnectionFactory(), new SqlQueryProviderAlpha());
            }
            else
            {
                _bootstrapper = new Bootstrapper(filter, modifier, new FileSystem(), new MySqlConnectionFactory(), new SqlQueryProvider());
            }

            if (decisionDb != null && decisionDb == "T")
            {
                Console.WriteLine("Nazwa bazy danych: ");
                dbName = Console.ReadLine();
                Console.WriteLine("Nazwa tabeli: ");
                tableName = Console.ReadLine();
            }

            try
            {
                RunFilter(decisionFilter, ref output);
                RunDbCreator(decisionDb, output, dbName, tableName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Brak pliku z danymi");
            }
            catch (FormatException)
            {
                Console.WriteLine("Błędna zawartość pliku z danymi");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Błędna ścieżka do pliku");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }

            Console.WriteLine("\nKONIEC\nNaciśnij ENTER by wyjść");
            Console.Read();
        }

        #region PRIVATE
        private static void RunFilter(string decisionFilter, ref string output)
        {
            if (decisionFilter == null || !decisionFilter.Equals("T")) return;

            Console.WriteLine("Podaj ścieżkę do pliku z N-gramami: ");
            var input = Console.ReadLine();
            Console.WriteLine("Podaj ścieżkę pliku wynikowego: ");
            output = Console.ReadLine();
            _bootstrapper.Filter(input, output);
        }

        private static void RunDbCreator(string decisionDb, string output, string dbName, string tableName)
        {
            if (decisionDb == null || !decisionDb.Equals("T") || dbName == null || tableName == null) return;

            if (output != null) _bootstrapper.CreateDb(output, dbName, tableName);
            else
            {
                Console.WriteLine("Podaj ścieżkę pliku z N-gramami: ");
                output = Console.ReadLine();

                _bootstrapper.CreateDb(output, dbName, tableName);
            }
        }
        #endregion
    }
}
