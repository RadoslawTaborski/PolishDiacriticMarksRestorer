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

            const string path = @"D:\PWr\magisterskie\magisterka\Teksty\";
            const string pathR = @"D:\PWr\magisterskie\magisterka\Raporty\";
            Console.Write($"Podaj ścieżkę do tekstu: {path}");
            var path1 = Console.ReadLine()+".txt";
            Console.Write($"Podaj ścieżkę zapisu raportu: {pathR}");
            var path2 = Console.ReadLine();
            var pathRep = $"{path2}-report.txt";
            var pathOut = $"{path2}-output.txt";

            try
            {
                string text;

                using (var sr = new StreamReader(path+path1))
                {
                    text = sr.ReadToEnd();
                }

                _originalText = TextSpliter.Split(text).ToList();
                text = text.RemoveDiacritics();
                _inputText = TextSpliter.Split(text).ToList();
                
                var reports = Analyze(text);

                Console.Write($"\r\n{reports[0]}\r\n Tekst wynikowy:\r\n{reports[1]}");
                File.WriteAllText(pathR + pathRep, reports[0]);
                File.WriteAllText(pathR + pathOut, reports[1]);
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

            Console.WriteLine("Koniec");

            Console.Read();
        }

        private static void Initialize()
        {
            var data = new DataBaseManager(new MySqlConnectionFactory(), Settings.Server, Settings.DbName, Settings.DbUser, Settings.DbPassword);
            var queryProvider = Settings.AlphabeticalTables
                ? (IQueryProvider)new SqlQueryProvider2(Settings.TableNames)
                : new SqlQueryProvider(Settings.TableNames);

            _analyzer = Settings.FileDictionary
                ? (Settings.SentenceSpliterOn ? new Analyzer(new DiacriticMarksAdder(), LoadDictionary(), new SentenceSpliter()) : new Analyzer(new DiacriticMarksAdder(), LoadDictionary(), null))
                : (Settings.SentenceSpliterOn ? new Analyzer(new DiacriticMarksAdder(), new SentenceSpliter()) : new Analyzer(new DiacriticMarksAdder(), null));

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

        private static string[] Analyze(string text)
        {
            Timer.Start();
            _start = DateTime.Now;
            var result =_analyzer.AnalyzeString(text);
            Timer.Stop();

            _outputText = _analyzer.Output;
            _reasercher = new Researcher(_originalText, _inputText, _outputText);

            var time = _stop - _start;

            var reports = GenerateReports(time,result);

            return reports;
        }

        private static string[] GenerateReports(TimeSpan time, List<string> output)
        {
            var result = new string[2];
            result[0] = "";
            result[0] += " Raport: \r\n";
            result[0] += $"Czas wykonywania:\t{new DateTime(time.Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Czas wykonywania/słowo:\t{new DateTime(time.Ticks / _originalText.Count):ss.ffff}s\r\n";

            result[0] += "\r\n Macierz pomyłek: \r\n";
            var res = _reasercher.Count();
            result[0] += res;

            result[0] += "\r\n Lista błędów: \r\n";
            foreach (var error in _reasercher.Errors)
            {
                result[0] += $"{error}\r\n";
            }

            result[1] = "";
            for (var i = 0; i < _analyzer.InputWithWhiteMarks.Count(); ++i)
            {
                result[1] += $"{output[i]}";
            }

            return result;
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _stop = DateTime.Now;
            var time = _stop - _start;
            Console.Write("Czas wykonywania: " + new DateTime(time.Ticks).ToString("HH:mm:ss.f") + "\r");
        }
    }
}
