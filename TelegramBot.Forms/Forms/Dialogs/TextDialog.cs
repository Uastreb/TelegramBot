using System;
using System.Windows.Forms;

namespace TelegramBot.Forms.Forms.Dialogs
{
    public partial class TextDialog : Form
    {
        public TextDialog()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static string Show(string caption, string defaultText = default)
        {
            var textDialog = new TextDialog();

            textDialog.Text = caption;
            textDialog.valueTextBox.Text = defaultText;
            textDialog.ShowDialog();

            return textDialog.valueTextBox.Text;
        }
    }
}
