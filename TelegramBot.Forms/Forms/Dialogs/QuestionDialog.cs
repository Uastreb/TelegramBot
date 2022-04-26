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
    public partial class QuestionDialog : Form
    {
        public QuestionDialog()
        {
            InitializeComponent();
        }

        private void addNodeButton_Click(object sender, EventArgs e)
        {
            if (questionTreeView.SelectedNode.Name == "AnswerOptions" ||
                questionTreeView.SelectedNode.Parent?.Name == "AnswerOptions")
            {
                var answerOption = AnswerOptionDialog.Show();

                var selectedNode = questionTreeView.SelectedNode.Name == "AnswerOptions" ? questionTreeView.SelectedNode : questionTreeView.SelectedNode.Parent;
                var answerOptionNode = selectedNode.Nodes.Add(answerOption.AnswerOption);

                answerOptionNode.Nodes.Add(answerOption.AnswerPoints.ToString());
            }
        }

        private void questionTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Name == "Question" || e.Node.Parent?.Name == "Question")
            {
                addNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "AnswerOptions")
            {
                deleteNodeButton.Enabled = false;
                editNodeButton.Enabled = false;

                addNodeButton.Enabled = true;

                return;
            }

            editNodeButton.Enabled = false;
            addNodeButton.Enabled = false;
            deleteNodeButton.Enabled = false;
        }

        public static TreeNodeCollection Show(string defaultQuestion = default, Dictionary<string, int> defaultAnswerOptions = default)
        {
            var questionDialog = new QuestionDialog();

            //questionDialog.questionTreeView.Nodes["Question"].Nodes["QuestionText"].Text = defaultQuestion;

            //var answerOptionsNodes = questionDialog.questionTreeView.Nodes["AnswerOptions"];

            //foreach (var defaultAnswerOption in defaultAnswerOptions)
            //{
            //    var answerOptionNode = answerOptionsNodes.Nodes.Add(defaultAnswerOption.Key);
            //    answerOptionNode.Nodes.Add(defaultAnswerOption.Value.ToString());
            //}

            questionDialog.ShowDialog();


            //var answerOptions = new List<Answer>();

            //foreach(TreeNode answerOptionNode in questionDialog.questionTreeView.Nodes["AnswerOptions"].Nodes)
            //{
            //    var answerOption = new Answer()
            //    {
            //        AnswerOption = answerOptionNode.Text,
            //        AnswerPoints = int.Parse(answerOptionNode.Nodes[0].Text)
            //    };

            //    answerOptions.Add(answerOption);
            //}

            //var question = new Question()
            //{
            //    QuestionText = questionDialog.questionTreeView.Nodes["Question"].Nodes["QuestionText"].Text,
            //    AnswerOptions = answerOptions.ToArray()
            //};

            //return question;

            return questionDialog.questionTreeView.Nodes;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
