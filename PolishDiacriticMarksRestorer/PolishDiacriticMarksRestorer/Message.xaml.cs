using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace PolishDiacriticMarksRestorer
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message
    {
        public Message(string header, string body)
        {
            InitializeComponent();
            RtbInfo.VerticalAlignment = VerticalAlignment.Center;

            InitializeComponent();
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow == null) return;
            Left = mainWindow.Left + (mainWindow.Width) / 2 - Width / 2;
            Top = mainWindow.Top + (mainWindow.Height) / 2 - Height / 2;

            Name.Text = header;
            RtbInfo.Document.Blocks.Clear();
            var paragraph = new Paragraph(new Run(body)) {TextAlignment = TextAlignment.Center};
            RtbInfo.Document.Blocks.Add(paragraph);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

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
        #endregion
    }
}
