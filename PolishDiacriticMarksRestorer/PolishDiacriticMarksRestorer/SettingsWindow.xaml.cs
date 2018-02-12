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
        #region FIELDS
        private readonly bool[] _changes = new bool[9];
        #endregion

        #region PROPERTIES
        public bool ChangeSettings { get; private set; }
        #endregion

        #region CONSTRUCTORS
        public SettingsWindow()
        {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow == null) return;
            Left = mainWindow.Left + (mainWindow.Width) / 2 - Width / 2;
            Top = mainWindow.Top + (mainWindow.Height) / 2 - Height / 2;

            GetSettings();
            BtnApply.IsEnabled = false;
            BtnSave.IsEnabled = false;
            ChangeSettings = false;
        }
        #endregion

        #region  PRIVATE
        private void SetSettings()
        {
            Settings.Type = (NgramType)Enum.Parse(typeof(NgramType), CbType.SelectedValue.ToString());
            Settings.Server = TbServer.Text;
            Settings.DbName = TbDbName.Text;
            Settings.DbUser = TbUserName.Text;
            Settings.DbPassword = TbPassword.Text;
            Settings.TableNames[0] = TbUni.Text;
            Settings.TableNames[1] = TbDi.Text;
            Settings.TableNames[2] = TbTri.Text;
            Settings.TableNames[3] = TbFour.Text;
        }

        private void GetSettings()
        {
            CbType.SelectedIndex = (int)Settings.Type - 1;
            TbServer.Text = Settings.Server;
            TbDbName.Text = Settings.DbName;
            TbUserName.Text = Settings.DbUser;
            TbPassword.Text = Settings.DbPassword;
            TbUni.Text = Settings.TableNames[0];
            TbDi.Text = Settings.TableNames[1];
            TbTri.Text = Settings.TableNames[2];
            TbFour.Text = Settings.TableNames[3];
        }

        private void Update()
        {
            if (_changes.Any(item => item))
            {
                BtnApply.IsEnabled = true;
                return;
            }

            BtnApply.IsEnabled = false;
        }
        #endregion

        #region EVENTS
        private void NgramChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _changes[0] = !Settings.Type.Equals((NgramType)Enum.Parse(typeof(NgramType), CbType.SelectedValue.ToString()));
            Update();
        }

        private void TextBox_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _changes[1] = !Settings.Server.Equals(TbServer.Text);
            _changes[2] = !Settings.DbName.Equals(TbDbName.Text);
            _changes[3] = !Settings.DbUser.Equals(TbUserName.Text);
            _changes[4] = !Settings.DbPassword.Equals(TbPassword.Text);
            _changes[5] = !Settings.TableNames[0].Equals(TbUni.Text);
            _changes[6] = !Settings.TableNames[1].Equals(TbDi.Text);
            _changes[7] = !Settings.TableNames[2].Equals(TbTri.Text);
            _changes[8] = !Settings.TableNames[3].Equals(TbFour.Text);
            Update();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SerializeStatic.Save(typeof(Settings), MainWindow.Path);
            BtnSave.IsEnabled = false;
        }

        private void SettingApply_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();
            ChangeSettings = true;
            BtnSave.IsEnabled = true;
            BtnApply.IsEnabled = false;
        }
        #endregion

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
