using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using EffectivenessResearch.Interfaces;
using NgramAnalyzer;
using NgramAnalyzer.Interfaces;
using NgramAnalyzer.Common;
using NgramAnalyzer.Common.Dictionaries;
using NgramAnalyzer.Common.IInterpunctionManager;
using NgramAnalyzer.Common.NgramsConnectors;
using NgramAnalyzer.Common.SentenceSplitters;
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
        private static IDictionary _dictionary;
        private static IDictionary _main;
        private static IDictionary _unigrams;
        private static DiacriticMarksAdder _diacriticMarksAdder;
        private static SentenceSpliter _spliter;
        private static InclusionManager _iManager;
        private static INgramsConnector _connector;

        private static void Main(string[] args)
        {
            const string path = @"D:\PWr\magisterskie\magisterka\Teksty\";
            const string pathR = @"D:\PWr\magisterskie\magisterka\Raporty\";
            var path1 = "";
            var path2 = "";
            var path3 = "";

            for (var index = 0; index < args.Length; index++)
            {
                var argument = args[index];
                switch (argument)
                {
                    case "-i":
                        if (index + 1 < args.Length)
                        {
                            index++;
                            path1 = args[index] + ".txt";
                        }
                        break;
                    case "-o":
                        if (index + 1 < args.Length)
                        {
                            index++;
                            path2 = args[index];
                        }
                        break;
                    case "-io":
                        if (index + 1 < args.Length)
                        {
                            index++;
                            path3 = args[index] + ".txt";
                        }
                        break;
                    case "-n1":
                        Settings.Type = NgramType.Unigram;
                        break;
                    case "-n2":
                        Settings.Type = NgramType.Bigram;
                        break;
                    case "-n3":
                        Settings.Type = NgramType.Trigram;
                        break;
                    case "-n4":
                        Settings.Type = NgramType.Quadrigram;
                        break;
                    case "-udt":
                        Settings.UseDictionary = true;
                        break;
                    case "-udf":
                        Settings.UseDictionary = false;
                        break;
                    case "-att":
                        Settings.AlphabeticalTables = true;
                        break;
                    case "-atf":
                        Settings.AlphabeticalTables = false;
                        break;
                    case "-sst":
                        Settings.SentenceSpliterOn = true;
                        break;
                    case "-ssf":
                        Settings.SentenceSpliterOn = false;
                        break;
                    case "-ipt":
                        Settings.IgnorePunctationMarks = true;
                        break;
                    case "-ipf":
                        Settings.IgnorePunctationMarks = false;
                        break;
                    case "-m1":
                        Settings.NoOfMethod = 0;
                        break;
                    case "-m2":
                        Settings.NoOfMethod = 1;
                        break;
                    case "-db":
                        if (index + 1 < args.Length)
                        {
                            index++;
                            Settings.DbName = args[index];
                        }
                        break;
                    case "-tab":
                        if (index + 4 < args.Length)
                        {
                            index++;
                            Settings.TableNames[0] = args[index];
                            index++;
                            Settings.TableNames[1] = args[index];
                            index++;
                            Settings.TableNames[2] = args[index];
                            index++;
                            Settings.TableNames[3] = args[index];
                        }
                        break;
                }
            }

            Initialize();

            if (path1 == "" || path2 == "")
            {
                Console.Write($"Podaj ścieżkę do tekstu: {path}");
                path1 = Console.ReadLine() + ".txt";
                Console.Write($"Podaj ścieżkę zapisu raportu: {pathR}");
                path2 = Console.ReadLine();
            }

            var pathRep = $"{path2}-report.txt";
            var pathOut = $"{path2}-output.txt";

            try
            {
                string text;
                string[] reports;

                File.WriteAllText(pathR + pathRep, GenerateDescription());

                using (var sr = new StreamReader(path + path1))
                {
                    text = sr.ReadToEnd();
                }

                _originalText = TextSpliter.Split(text).ToList();
                text = text.RemoveDiacritics();
                _inputText = TextSpliter.Split(text).ToList();

                if (path3 != "")
                {
                    using (var sr = new StreamReader(path + path3))
                    {
                        var outputText = sr.ReadToEnd();
                        _outputText = TextSpliter.Split(outputText).ToList();
                        var result = TextSpliter.SplitAndKeep(outputText).ToList();
                        var times = new List<TimeSpan>();
                        for (var i = 0; i < 10; ++i)
                        {
                            _start = _stop = DateTime.Now;
                            times.Add(_stop - _start);
                        }
                        var counts = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
                        reports = Research(times, counts, result);
                    }
                }
                else
                {
                    var result = Analyze(text, out var times, out var counts);
                    reports = Research(times, counts, result);
                }

                //Console.Write($"\r\n{reports[0]}\r\n Tekst wynikowy:\r\n{reports[1]}");
                File.AppendAllText(pathR + pathRep, reports[0]);
                File.WriteAllText(pathR + pathOut, reports[1]);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Brak pliku z danymi");
            }


            Console.Write("\r\n");
            //Console.Read();
        }

        private static void Initialize()
        {
            var data = new DataBaseManager(new MySqlConnectionFactory(), Settings.Server, Settings.DbName, Settings.DbUser, Settings.DbPassword);
            var queryProvider = Settings.AlphabeticalTables
                ? (IQueryProvider)new SqlQueryProvider2(Settings.TableNames)
                : new SqlQueryProvider(Settings.TableNames);

            if (Settings.UseDictionary)
            {
                if (_dictionary == null)
                {
                    _dictionary = LoadDictionary();
                    _main = _dictionary;
                }
            }
            else
            {
                if (_unigrams == null)
                {
                    _unigrams = LoadUnigrams();
                    _main = _unigrams;
                }
            }

            _diacriticMarksAdder = new DiacriticMarksAdder();
            _spliter = Settings.SentenceSpliterOn ? new SentenceSpliter() : null;
            _iManager = Settings.IgnorePunctationMarks ? new InclusionManager() : null;
            _connector = (Settings.NoOfMethod == 0) ? (INgramsConnector)new Variant1() : new Variant2();

            _analyzer = new Analyzer(_diacriticMarksAdder, _main, _spliter, _iManager, _connector);

            _analyzer.SetData(data);
            _analyzer.SetQueryProvider(queryProvider);
            _analyzer.SetNgram(Settings.Type);
            Timer.Interval = 90;
            Timer.Elapsed += OnTimerElapsed;
        }
        private static IDictionary LoadDictionary()
        {
            if (!File.Exists(Settings.DictionaryPath))
                return new Dict(new Dictionary<string, int>());
            var logFile = File.ReadAllLines(Settings.DictionaryPath);
            var logList = new List<string>(logFile);
            var result = new Dictionary<string, int>();
            foreach (var item in logList)
            {
                result.Add(item, 0);
            }
            return new Dict(result);
        }

        private static IDictionary LoadUnigrams()
        {
            if (!File.Exists(Settings.UnigramPath))
                return new Dict(new Dictionary<string, int>());
            var logFile = File.ReadAllLines(Settings.UnigramPath);
            var logList = new List<string>(logFile);
            var result = new Dictionary<string, int>();
            foreach (var item in logList)
            {
                var str = item.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                result.Add(str[1], int.Parse(str[0]));
            }
            return new Dict(result);
        }

        private static List<string> Analyze(string text, out List<TimeSpan> times, out List<int> counts)
        {
            Timer.Start();
            _start = DateTime.Now;
            var result = _analyzer.AnalyzeString(text, out times, out counts);
            _stop = DateTime.Now;
            Timer.Stop();

            _outputText = _analyzer.Output;

            times[9] = _stop - _start;

            return result;
        }

        private static string[] Research(List<TimeSpan> times, List<int> counts, List<string> result)
        {
            _reasercher = new Researcher(_originalText, _inputText, _outputText);

            var reports = GenerateReports(times, counts, result);

            return reports;
        }

        private static string[] GenerateReports(List<TimeSpan> times, List<int> counts, List<string> output)
        {
            var result = new string[2];
            result[0] = "\r\n";
            result[0] += "\tRaport: \r\n";
            result[0] += $"Czas wykonywania:\t{new DateTime(times[9].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Czas wykonywania/słowo:\t{new DateTime(times[9].Ticks / _originalText.Count):ss.ffff}\r\n";
            result[0] += $"Podział na zdania:\t{new DateTime(times[0].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Tworzenie kombinacji:\t{new DateTime(times[1].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Sprawdzanie w słowniku:\t{new DateTime(times[2].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Tworzenie ngramów:\t{new DateTime(times[3].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Wykluczanie jednoznacznych ngramów:\t{new DateTime(times[4].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Pobieranie danych:\t{new DateTime(times[5].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Aktualizacja ngramów:\t{new DateTime(times[6].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Analiza ngramów:\t{new DateTime(times[7].Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Przywracanie formatu:\t{new DateTime(times[8].Ticks):HH:mm:ss.f}\r\n";

            result[0] += $"Liczba zdań:\t{counts[0]}\r\n";
            result[0] += $"Liczba kombinacji:\t{counts[1]}\r\n";
            result[0] += $"Liczba poprawnych kombinacji:\t{counts[2]}\r\n";
            result[0] += $"Liczba utworzonych ngramów:\t{counts[3]}\r\n";
            result[0] += $"Liczba niejednoznacznych ngramów:\t{counts[4]}\r\n";
            result[0] += $"Liczba ngramów w bazie:\t{counts[5]}\r\n";
            result[0] += $"Liczba wybranych ngramów:\t{counts[6]}\r\n";

            result[0] += "\r\n\tMacierz pomyłek: \r\n";
            var res = _reasercher.Count();
            result[0] += res;

            result[0] += "\r\n\tLista błędów: \r\n";
            foreach (var error in _reasercher.Errors)
            {
                result[0] += $"{error}\r\n";
            }

            result[1] = "";
            for (var i = 0; i < output.Count(); ++i)
            {
                result[1] += $"{output[i]}";
            }

            return result;
        }

        private static string GenerateDescription()
        {
            var result = "\tOpis:\r\n";
            result += $"Typ ngramów:\t{Settings.Type}\r\n";
            result += $"Użycie słownika:\t{Settings.UseDictionary}\r\n";
            result += $"Tabele alfabetyczne:\t{Settings.AlphabeticalTables}\r\n";
            result += $"Podział na zdania:\t{Settings.SentenceSpliterOn}\r\n";
            result += $"Ignorowanie znaków interpunkcyjnych:\t{Settings.IgnorePunctationMarks}\r\n";
            result += $"Serwer:\t{Settings.Server}\r\n";
            result += $"Użytkownik:\t{Settings.DbUser}\r\n";
            result += $"Baza danych:\t{Settings.DbName}\r\n";
            result += $"\t{Settings.TableNames[0]}\r\n";
            result += $"\t{Settings.TableNames[1]}\r\n";
            result += $"\t{Settings.TableNames[2]}\r\n";
            result += $"\t{Settings.TableNames[3]}\r\n";

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
