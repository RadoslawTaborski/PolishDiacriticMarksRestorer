using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using EffectivenessResearch.Interfaces;
using MySql.Data.MySqlClient;
using NgramAnalyzer;
using NgramAnalyzer.Interfaces;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace EffectivenessResearch
{
    internal class Program
    {
        private static readonly Timer Timer = new Timer();
        private static DateTime _start;
        private static DateTime _stop;
        private static IAnalyzer _analyzer;
        private static List<string> _originalText;
        private static List<string> _inputText;
        private static List<string> _outputText;
        private static IResearcher _reasercher;

        private static void Main()
        {
            Initialize();

            Console.WriteLine("Podaj ścieżkę do tekstu: ");
            var path = Console.ReadLine();

            try
            {
                string text;

                using (var sr = new StreamReader(path))
                {
                    text = sr.ReadToEnd();
                }

                _originalText = TextSpliter.Split(text).ToList();
                text = text.RemoveDiacritics();
                _inputText = TextSpliter.Split(text).ToList();

                var task = new Task(() => Analyze(text));
                task.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                task.Start();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Brak pliku z danymi");
            }
            catch (FormatException)
            {
                Console.WriteLine("Błędna zawartość pliku z danymi");
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Nie można połączyć się z serwerem");
                        break;
                    case 1045:
                        Console.WriteLine("Nieprawidłowa nazwa użytkownika lub hasło do bazy danych");
                        break;
                    default:
                        Console.WriteLine(ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }

        private static void Initialize()
        {
            var data = new DataBaseManager(new MySqlConnectionFactory(), Settings.Server, Settings.DbName, Settings.DbUser, Settings.DbPassword);
            var queryProvider = Settings.AlphabeticalTables
                ? (IQueryProvider)new SqlQueryProvider2(Settings.TableNames)
                : new SqlQueryProvider(Settings.TableNames);

            _analyzer = Settings.FileDictionary
                ? new Analyzer(new DiacriticMarksAdder(), LoadDictionary())
                : new Analyzer(new DiacriticMarksAdder());

            _analyzer.SetData(data);
            _analyzer.SetQueryProvider(queryProvider);
            _analyzer.SetNgram(Settings.Type);
            Timer.Interval = 90;
            Timer.Elapsed += OnTimerElapsed;
        }

        private static Dictionary LoadDictionary()
        {
            if (!File.Exists(Settings.DictionaryPath))
                return new Dictionary(new List<string>());
            var logFile = File.ReadAllLines(Settings.DictionaryPath);
            var logList = new List<string>(logFile);
            return new Dictionary(logList);
        }

        private static void Analyze(string text)
        {
            Timer.Start();
            _start = DateTime.Now;
            _analyzer.AnalyzeString(text);
            _outputText = _analyzer.Output;
            Timer.Stop();

            for (var i = 0; i < _analyzer.InputWithWhiteMarks.Count(); ++i)
            {
                Console.Write(_analyzer.InputWithWhiteMarks[i]);
            }
            Console.WriteLine();

            _reasercher = new Researcher(_originalText, _inputText, _outputText);
            var result = _reasercher.Count();
            Console.WriteLine(result);
        }

        private static void ExceptionHandler(Task task1)
        {
            var exception = task1.Exception;
            if (exception != null) Console.WriteLine(exception.Message);
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _stop = DateTime.Now;
            var time = _stop - _start;
            Console.Write("Czas wykonywania: " + new DateTime(time.Ticks).ToString("HH:mm:ss.f") + "\r");
        }
    }
}
