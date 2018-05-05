using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static void Main(string[] args)
        {
            const string path = @"D:\PWr\magisterskie\magisterka\Teksty\";
            const string pathR = @"D:\PWr\magisterskie\magisterka\Raporty\";
            var path1 = "";
            var path2 = "";

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

            if (path1 == "" && path2 == "")
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

                File.WriteAllText(pathR + pathRep, GenerateDescription());

                using (var sr = new StreamReader(path + path1))
                {
                    text = sr.ReadToEnd();
                }

                _originalText = TextSpliter.Split(text).ToList();
                text = text.RemoveDiacritics();
                _inputText = TextSpliter.Split(text).ToList();

                var reports = Analyze(text);

                //Console.Write($"\r\n{reports[0]}\r\n Tekst wynikowy:\r\n{reports[1]}");
                File.AppendAllText(pathR + pathRep, reports[0]);
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

            Console.Write("\r\n");
           // Console.Read();
        }

        private static void Initialize()
        {
            var data = new DataBaseManager(new MySqlConnectionFactory(), Settings.Server, Settings.DbName, Settings.DbUser, Settings.DbPassword);
            var queryProvider = Settings.AlphabeticalTables
                ? (IQueryProvider)new SqlQueryProvider2(Settings.TableNames)
                : new SqlQueryProvider(Settings.TableNames);

            _analyzer = Settings.UseDictionary
                ? (Settings.SentenceSpliterOn
                    ? new Analyzer(new DiacriticMarksAdder(), LoadDictionary(), new SentenceSpliter())
                    : new Analyzer(new DiacriticMarksAdder(), LoadDictionary(), null))
                : (Settings.SentenceSpliterOn
                    ? new Analyzer(new DiacriticMarksAdder(), LoadUnigrams(), new SentenceSpliter())
                    : new Analyzer(new DiacriticMarksAdder(), LoadUnigrams(), null));

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

        private static string[] Analyze(string text)
        {
            Timer.Start();
            _start = DateTime.Now;
            var result = _analyzer.AnalyzeString(text);
            Timer.Stop();

            _outputText = _analyzer.Output;
            _reasercher = new Researcher(_originalText, _inputText, _outputText);

            var time = _stop - _start;

            var reports = GenerateReports(time, result);

            return reports;
        }

        private static string[] GenerateReports(TimeSpan time, List<string> output)
        {
            var result = new string[2];
            result[0] = "\r\n";
            result[0] += "\tRaport: \r\n";
            result[0] += $"Czas wykonywania:\t{new DateTime(time.Ticks):HH:mm:ss.f}\r\n";
            result[0] += $"Czas wykonywania/słowo:\t{new DateTime(time.Ticks / _originalText.Count):ss.ffff}s\r\n";

            result[0] += "\r\n\tMacierz pomyłek: \r\n";
            var res = _reasercher.Count();
            result[0] += res;

            result[0] += "\r\n\tLista błędów: \r\n";
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

        private static string GenerateDescription()
        {
            var result = "\tOpis:\r\n";
            result += $"Typ ngramów:\t{Settings.Type}\r\n";
            result += $"Wsparcie unigramami:\t{Settings.UseDictionary}\r\n";
            result += $"Tabele alfabetyczne:\t{Settings.AlphabeticalTables}\r\n";
            result += $"Podział na zdania:\t{Settings.SentenceSpliterOn}\r\n";
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
