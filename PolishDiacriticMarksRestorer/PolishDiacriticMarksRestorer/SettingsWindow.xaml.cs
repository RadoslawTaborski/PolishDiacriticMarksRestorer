using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NgramAnalyzer.Common;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml.
    /// </summary>
    /// <seealso cref="Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class SettingsWindow
    {
        #region FIELDS
        private readonly bool[] _changes = new bool[11];
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets a value indicating whether changes have occurred.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changes have occurred; otherwise, <c>false</c>.
        /// </value>
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
            //if (FileDict.IsChecked != null) Settings.FileDictionary = (bool)FileDict.IsChecked;
            //Settings.TableNames[0] = TbUni.Text;
            Settings.TableNames[1] = TbDi.Text;
            Settings.TableNames[2] = TbTri.Text;
            Settings.TableNames[3] = TbFour.Text;
            if (AlphaTables.IsChecked != null) Settings.AlphabeticalTables = (bool)AlphaTables.IsChecked;
        }

        private void GetSettings()
        {
            CbType.SelectedIndex = (int)Settings.Type - 2;
            TbServer.Text = Settings.Server;
            TbDbName.Text = Settings.DbName;
            TbUserName.Text = Settings.DbUser;
            TbPassword.Text = Settings.DbPassword;
            //TbUni.Text = Settings.TableNames[0];
            TbDi.Text = Settings.TableNames[1];
            TbTri.Text = Settings.TableNames[2];
            TbFour.Text = Settings.TableNames[3];
            AlphaTables.IsChecked = Settings.AlphabeticalTables;
            //FileDict.IsChecked = Settings.FileDictionary;
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
        /// <summary>
        /// Ngrams changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void NgramChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _changes[0] = !Settings.Type.Equals((NgramType)Enum.Parse(typeof(NgramType), CbType.SelectedValue.ToString()));
            Update();
        }

        /// <summary>
        /// Handles the Checked event of the AlphaTables control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AlphaTables_Checked(object sender, RoutedEventArgs e)
        {
            _changes[9] = !Settings.AlphabeticalTables.Equals(AlphaTables.IsChecked);
            Update();
        }

        /// <summary>
        /// Handles the Changed event of the TextBox controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs" /> instance containing the event data.</param>
        private void TextBoxs_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _changes[1] = !Settings.Server.Equals(TbServer.Text);
            _changes[2] = !Settings.DbName.Equals(TbDbName.Text);
            _changes[3] = !Settings.DbUser.Equals(TbUserName.Text);
            _changes[4] = !Settings.DbPassword.Equals(TbPassword.Text);
            //_changes[5] = !Settings.TableNames[0].Equals(TbUni.Text);
            _changes[6] = !Settings.TableNames[1].Equals(TbDi.Text);
            _changes[7] = !Settings.TableNames[2].Equals(TbTri.Text);
            _changes[8] = !Settings.TableNames[3].Equals(TbFour.Text);
            Update();
        }

        /// <summary>
        /// Handles the Click event of the BtnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SerializeStatic.Save(typeof(Settings), MainWindow.Path);
            BtnSave.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Click event of the SettingApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SettingApply_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();
            ChangeSettings = true;
            BtnSave.IsEnabled = true;
            BtnApply.IsEnabled = false;
        }

        ///// <summary>
        ///// Handles the Checked event of the FileDict control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        //private void FileDict_Checked(object sender, RoutedEventArgs e)
        //{
        //    _changes[10] = !Settings.FileDictionary.Equals(FileDict.IsChecked);
        //    Update();
        //    if (FileDict.IsChecked == null) return;
        //    if ((bool)FileDict.IsChecked)
        //        TbUni.IsEnabled = false;
        //    else
        //        TbUni.IsEnabled = true;
        //}

        #region TITLE_BAR
        /// <summary>
        /// Handles the MouseDown event of the TitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var thisCurrentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            thisCurrentWindow?.DragMove();
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #endregion
    }
}
