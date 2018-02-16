using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MySql.Data.MySqlClient;
using NgramAnalyzer;
using NgramAnalyzer.Common;

namespace EffectivenessResearch
{
    internal class Program
    {
        //private static readonly Timer Timer= new Timer();
        //private static DateTime _start;
        //private static DateTime _stop;
        //private static readonly Analyzer Analyzer=new Analyzer(new DiacriticMarksAdder());

        private static void Main()
        {
            //var names = new List<string>
            //{
            //    "1grams",
            //    "2grams",
            //    "3grams",
            //    "4grams"
            //};
            //var data = new DataBaseManager(new MySqlConnectionFactory(), "localhost", "ngrams", "root", "");
            //var queryProvider = new SqlQueryProvider(names);
            //Analyzer.SetData(data);
            //Analyzer.SetQueryProvider(queryProvider);
            //Analyzer.SetNgram(NgramType.Trigram);
            //Timer.Interval = 90;
            //Timer.Elapsed += OnTimerElapsed;

            //const string text = "jest za przyjeciem uchwaly z dnia wczorajszego";
            //var stringsArray = text.Split(new[]{
            //    " ",
            //    "\r\n"
            //}, StringSplitOptions.RemoveEmptyEntries);
            //try
            //{
            //    var t = new Task(() => Analyze(stringsArray.ToList()));
            //    t.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            //    t.Start();
            //}
            //catch (MySqlException ex)
            //{
            //    switch (ex.Number)
            //    {
            //        case 0:
            //            Console.WriteLine("Nie można połączyć się z serwerem");
            //            break;
            //        case 1045:
            //            Console.WriteLine("Nieprawidłowa nazwa użytkownika lub hasło do bazy danych");
            //            break;
            //        default:
            //            Console.WriteLine(ex.Message);
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //Console.Read();
        }

        //private static void Analyze(List<string> stringsList)
        //{
        //    Console.WriteLine("START");
        //    Timer.Start();
        //    _start = DateTime.Now;
        //    var resultsArray = Analyzer.AnalyzeStrings(stringsList);
        //    Timer.Stop();
        //    Console.WriteLine();
        //    foreach (var item in resultsArray)
        //    {
        //        Console.Write(item+ " ");
        //    }
        //}

        //private static void ExceptionHandler(Task task1)
        //{
        //    var exception = task1.Exception;
        //    if (exception != null) Console.WriteLine(exception.Message);
        //}

        //private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    _stop = DateTime.Now;
        //    var time = _stop - _start;
        //    Console.Write("Czas wykonywania: " + new DateTime(time.Ticks).ToString("HH:mm:ss.f")+"\r");
        //}
    }
}
