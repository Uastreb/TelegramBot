using System;
using System.Windows.Forms;
using TelegramBot.Forms.Models;

namespace TelegramBot.Forms.Forms.Dialogs
{
    public partial class UserDataDialog : Form
    {
        public UserDataDialog()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static UserData Show(string defaultName = default, string defaultValue = default)
        {
            var textWithNameDialog = new UserDataDialog();

            textWithNameDialog.nameTextBox.Text = defaultName;
            textWithNameDialog.valueTextBox.Text = defaultValue;
            textWithNameDialog.ShowDialog();

            return new UserData()
            {
                Name = textWithNameDialog.nameTextBox.Text,
                Text = textWithNameDialog.valueTextBox.Text
            };
        }
    }
}
