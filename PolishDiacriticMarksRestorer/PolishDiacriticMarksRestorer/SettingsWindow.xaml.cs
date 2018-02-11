using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NgramAnalyzer.Common;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public bool ChangeSettings;
        public SettingsWindow()
        {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow == null) return;
            Left = mainWindow.Left + (mainWindow.Width)/ 2 - Width/2;
            Top = mainWindow.Top + (mainWindow.Height) / 2 - Height/2;

            CbType.SelectedIndex = (int)Settings.Type-1;
            BtnApply.IsEnabled = false;
            ChangeSettings = false;
        }

        private void SettingApply_Click(object sender, RoutedEventArgs e)
        {
            Settings.Type = (NgramType)Enum.Parse(typeof(NgramType), CbType.SelectedValue.ToString());
            ChangeSettings = true;
            Close();
        }

        private void NgramChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            BtnApply.IsEnabled = !Settings.Type.Equals((NgramType)Enum.Parse(typeof(NgramType), CbType.SelectedValue.ToString()));
        }

        #region TITLE_BAR
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var thisCurrentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            thisCurrentWindow?.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
