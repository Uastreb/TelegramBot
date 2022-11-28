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

        private void editNodeButton_Click(object sender, EventArgs e)
        {
            TreeNode answerOptionNode;
            if (questionTreeView.SelectedNode.Parent?.Name == "AnswerOptions")
            {
                answerOptionNode = questionTreeView.SelectedNode;
            }
            else if (questionTreeView.SelectedNode.Parent?.Parent?.Name == "AnswerOptions")
            {
                answerOptionNode = questionTreeView.SelectedNode.Parent;
            }
            else if (questionTreeView.SelectedNode.Name == "Question")
            {
                var questionTextNode = questionTreeView.SelectedNode.FirstNode;
                var questionText = TextDialog.Show("Добавление вопроса", questionTextNode.Text);
                questionTextNode.Text = questionText;

                return;
            }
            else if (questionTreeView.SelectedNode.Parent.Name == "Question")
            {
                var questionTextNode = questionTreeView.SelectedNode.Parent.FirstNode;
                var questionText = TextDialog.Show("Добавление вопроса", questionTextNode.Text);
                questionTextNode.Text = questionText;

                return;
            }
            else
            {
                return;
            }

            var answerOptionText = answerOptionNode.Text;

            var answerPointsNode = answerOptionNode.FirstNode;
            var answerPoints = int.Parse(answerPointsNode.Text);

            var answerOption = AnswerOptionDialog.Show(answerPoints, answerOptionText);
            if (answerOption == default)
            {
                return;
            }

            answerOptionNode.Text = answerOption.AnswerOption;
            answerOptionNode.FirstNode.Text = answerOption.AnswerPoints.ToString();
        }

        private void deleteNodeButton_Click(object sender, EventArgs e)
        {
            TreeNode answerOptionNode;
            if (questionTreeView.SelectedNode.Parent?.Name == "AnswerOptions")
            {
                answerOptionNode = questionTreeView.SelectedNode;
            }
            else if (questionTreeView.SelectedNode.Parent?.Parent?.Name == "AnswerOptions")
            {
                answerOptionNode = questionTreeView.SelectedNode.Parent;
            }
            else
            {
                return;
            }

            answerOptionNode.Remove();
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

            if (e.Node.Parent.Name == "AnswerOptions" || e.Node.Parent.Parent.Name == "AnswerOptions")
            {
                deleteNodeButton.Enabled = true;
                editNodeButton.Enabled = true;
                addNodeButton.Enabled = true;

                return;
            }

            editNodeButton.Enabled = false;
            addNodeButton.Enabled = false;
            deleteNodeButton.Enabled = false;
        }

        public static Question Show(string defaultQuestionText = default, List<Answer> defaultAnswers = default)
        {
            var questionDialog = new QuestionDialog();

            if (defaultQuestionText != default && defaultAnswers != default)
            {
                questionDialog.questionTreeView.Nodes["Question"].FirstNode.Text = defaultQuestionText;

                foreach(var answer in defaultAnswers)
                {
                    var answerNode = new TreeNode(answer.AnswerOption);
                    answerNode.Nodes.Add(answer.AnswerPoints.ToString());
                    questionDialog.questionTreeView.Nodes["AnswerOptions"].Nodes.Add(answerNode);
                }
            }

            questionDialog.ShowDialog();

            var question = new Question
            {
                QuestionText = questionDialog.questionTreeView.Nodes["Question"].FirstNode.Text
            };

            var answers = new List<Answer>();
            foreach (TreeNode answerNode in questionDialog.questionTreeView.Nodes["AnswerOptions"].Nodes)
            {
                answers.Add(new Answer()
                {
                     AnswerOption = answerNode.Text,
                     AnswerPoints = int.Parse(answerNode.FirstNode.Text)
                });
            }

            question.AnswerOptions = answers.ToArray();

            return question;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
