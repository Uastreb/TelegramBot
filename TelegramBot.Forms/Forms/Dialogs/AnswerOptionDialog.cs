using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramBot.Forms.Models;

namespace TelegramBot.Forms.Forms.Dialogs
{
    public partial class AnswerOptionDialog : Form
    {
        public AnswerOptionDialog()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static Answer Show(int defaultPoints = 0, string defaultValue = default)
        {
            var answerOptionDialog = new AnswerOptionDialog();

            answerOptionDialog.pointsNumericUD.Value = defaultPoints;
            answerOptionDialog.valueTextBox.Text = defaultValue;
            answerOptionDialog.ShowDialog();

            return new Answer()
            {
                AnswerPoints = Convert.ToDouble(answerOptionDialog.pointsNumericUD.Value),
                AnswerOption = answerOptionDialog.valueTextBox.Text
            };
        }
    }
}
