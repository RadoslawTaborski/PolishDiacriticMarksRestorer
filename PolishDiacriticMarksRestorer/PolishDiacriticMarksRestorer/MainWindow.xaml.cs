using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using NgramAnalyzer;
using NgramAnalyzer.Common;

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
        private readonly Analyzer _analyzer = new Analyzer();
        public static readonly string Path = "settings.dat";
        #endregion

        #region CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();
            RtbResult.IsReadOnly = true;
            Load(Path);
            var data = new DataBaseManager(new MySqlConnectionFactory(), Settings.Server, Settings.DbName, Settings.DbUser, Settings.DbPassword);
            var queryProvider = new SqlQueryProvider(Settings.TableNames);
            _analyzer.SetData(data);
            _analyzer.SetQueryProvider(queryProvider);
            _analyzer.SetNgram(Settings.Type);
        }
        #endregion

        #region  PRIVATE
        private void Analyze(string[] stringsArray)
        {
            var resultsArray = _analyzer.AnalyzeStrings(stringsArray);
            Dispatcher.Invoke(() => {
                RtbResult.Document.Blocks.Clear();
                
            });

            foreach (var item in resultsArray)
            {
                Dispatcher.Invoke(() => {
                    RtbResult.AppendText(item + " ");
                });
            }
            Dispatcher.Invoke(() => {
                LoadingBar.Visibility = Visibility.Hidden;
                BtnStart.IsEnabled = true;
            });
        }

        private void Load(string path)
        {
            if (File.Exists(path))
                SerializeStatic.Load(typeof(Settings), path);
        }

        private static void ExceptionHandler(Task task1)
        {
            var exception = task1.Exception;
            if (exception != null) MessageBox.Show(exception.Message);
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
            var stringsArray = text.Split(new[]{
                " ",
                "\r\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                var t = new Task(() => Analyze(stringsArray));
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
            var queryProvider = new SqlQueryProvider(Settings.TableNames);
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
