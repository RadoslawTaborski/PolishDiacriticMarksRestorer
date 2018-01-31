using System.Windows;
using System.Windows.Input;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow == null) return;
            Left = mainWindow.Left + (mainWindow.Width)/ 2 - Width/2;
            Top = mainWindow.Top + (mainWindow.Height) / 2 - Height/2;
        }

        #region MyRegion
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            if (Application.Current.Windows[2] != null) Application.Current.Windows[2].DragMove();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
