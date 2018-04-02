﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using NgramAnalyzer;
using NgramAnalyzer.Common;
using IQueryProvider = NgramAnalyzer.Interfaces.IQueryProvider;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <inheritdoc cref="MainWindow" />
    public partial class MainWindow
    {
        #region FIELDS

        private Analyzer _analyzer;
        private readonly Timer _timer = new Timer();
        private DateTime _start;
        private DateTime _stop;
        public static readonly string Path = "settings.dat";
        private List<List<string>> _lists;
        #endregion

        #region CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();
            RtbResult.IsReadOnly = true;
            Load(Path);
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
            _timer.Elapsed += OnTimerElapsed;
        }
        #endregion

        #region  PRIVATE
        private void Analyze(string text)
        {
            _timer.Start();
            _start = DateTime.Now;
            var resultsArray = _analyzer.AnalyzeString(text);
            _lists = _analyzer.CreateWordsCombinations();

            Dispatcher.Invoke(() =>
            {
                RtbResult.Document.Blocks.Clear();
            });
            _timer.Stop();


            for (var i = 0; i < _analyzer.InputWithWhiteMarks.Count(); ++i)
            {
                var i1 = i;
                Dispatcher.Invoke(() =>
                {
                    switch (_lists[i1].Count())
                    {
                        case 0:
                            RtbResult.AppendTextColors(resultsArray[i1], new SolidColorBrush(Colors.Firebrick), new SolidColorBrush(Colors.White));
                            break;
                        case 1 when _analyzer.InputWithWhiteMarks[i1] != resultsArray[i1]:
                            RtbResult.AppendTextColors(resultsArray[i1], new SolidColorBrush(Colors.LimeGreen), new SolidColorBrush(Colors.Black));
                            break;
                        case 1 when _analyzer.InputWithWhiteMarks[i1] == resultsArray[i1]:
                            RtbResult.AppendTextColors(resultsArray[i1], new SolidColorBrush(Colors.Transparent), (SolidColorBrush)(FindResource("MyAzure")));
                            break;
                        case int n when n > 1 && _analyzer.InputWithWhiteMarks[i1] != resultsArray[i1]:
                            RtbResult.AppendTextColors(resultsArray[i1], new SolidColorBrush(Colors.GreenYellow), new SolidColorBrush(Colors.Black));
                            break;
                        case int n when n > 1 && _analyzer.InputWithWhiteMarks[i1] == resultsArray[i1]:
                            RtbResult.AppendTextColors(resultsArray[i1], new SolidColorBrush(Colors.Gold), new SolidColorBrush(Colors.Black));
                            break;
                    }
                });
            }
            Dispatcher.Invoke(() =>
            {
                LoadingBar.Visibility = Visibility.Hidden;
                BtnStart.IsEnabled = true;
            });
        }

        private void Load(string path)
        {
            if (File.Exists(path))
                SerializeStatic.Load(typeof(Settings), path);
        }

        private Dictionary LoadDictionary()
        {
            if (!File.Exists(Settings.DictionaryPath))
                return new Dictionary(new List<string>());
            var logFile = File.ReadAllLines(Settings.DictionaryPath);
            var logList = new List<string>(logFile);
            return new Dictionary(logList);
        }

        private void ExceptionHandler(Task task1)
        {
            var exception = task1.Exception;
            if (exception != null) MessageBox.Show(exception.Message);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _stop = DateTime.Now;
            var time = _stop - _start;
            Dispatcher.Invoke(() =>
            {
                Info.Content = $"Czas wykonywania: {new DateTime(time.Ticks):HH:mm:ss.f}";
            });
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadingBar.Visibility = Visibility.Visible;
            BtnStart.IsEnabled = false;
            RtbResult.Document.Blocks.Clear();
            var text = new TextRange(RtbInput.Document.ContentStart, RtbInput.Document.ContentEnd).Text;
            var count = _analyzer.SetWords(text);
            Info2.Content = $"Liczba słów: {count}";
            try
            {
                var t = new Task(() => Analyze(text));
                t.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                t.Start();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Nie można połączyć się z serwerem");
                        break;
                    case 1045:
                        MessageBox.Show("Nieprawidłowa nazwa użytkownika lub hasło do bazy danych");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Rtb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RtbResult.SetCarretPosiotion(Mouse.GetPosition((RtbResult)));
            CheckSpeling();
        }

        public delegate void UpdateInterface();

        private void CheckSpeling()
        {
            var word = RtbResult.GetSelectedWord();
            var results = new List<string>();

            foreach (var item in _lists)
            {
                if(item.Contains(word.Text))
                    results = item;
            }

            RtbResult.SetContextMenu(results, new Regex("[ĄĆĘŁŃÓŚŻŹąćęłńóśżź]"), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.LimeGreen), (SolidColorBrush)(FindResource("MyAzure")), new SolidColorBrush(Colors.Transparent));
        }

        #region TITLE_BAR
        /// <summary>
        /// Handles the Click event of the MenuButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var subWindow = new SettingsWindow();
            subWindow.ShowDialog();

            if (!subWindow.ChangeSettings) return;

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
        }

        /// <summary>
        /// Handles the MouseDown event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (e.ClickCount == 2)
            {
                AdjustWindowSize();
            }
            else
            {
                if (Application.Current.MainWindow != null) Application.Current.MainWindow.DragMove();
            }
        }

        /// <summary>
        /// Adjusts the size of the window.
        /// </summary>
        private void AdjustWindowSize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                var resourceUri = new Uri("img/max.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);

                if (streamInfo == null) return;
                var temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush
                {
                    ImageSource = temp
                };
                MaxButton.Background = brush;
            }
            else
            {
                WindowState = WindowState.Maximized;
                var resourceUri = new Uri("img/min.png", UriKind.Relative);
                var streamInfo = Application.GetResourceStream(resourceUri);

                if (streamInfo == null) return;
                var temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush
                {
                    ImageSource = temp
                };
                MaxButton.Background = brush;
            }
        }

        /// <summary>
        /// Handles the Click event of the MinButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the MaxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion
        #endregion
    }
}
