using System;
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
    /// <inheritdoc cref="MainWindow" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly Analyzer _analyzer = new Analyzer();

        public MainWindow()
        {
            InitializeComponent();
            var data = new DataBaseManager("localhost","testowa","root","");
            _analyzer.SetData(data);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(RtbInput.Document.ContentStart, RtbInput.Document.ContentEnd).Text;
            var stringsArray = text.Split(' ');

            try
            {
                var resultsArray = _analyzer.AnalyzeStrings(stringsArray);
                RtbResult.Document.Blocks.Clear();
                foreach (var item in resultsArray)
                {
                    RtbResult.AppendText(item + " ");
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
            }
        }

        #region TITLE_BAR
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

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var subWindow = new SettingsWindow();
            subWindow.ShowDialog();
        }
        #endregion
    }
}
