using System;
using System.Windows.Forms;
using System.Linq;
using TelegramBot.Forms.Models;
using System.Collections.Generic;

namespace TelegramBot.Forms.Forms.Dialogs
{
    public partial class QuestionGroupDialog : Form
    {
        public QuestionGroupDialog()
        {
            InitializeComponent();
        }

        private void editNodeButton_Click(object sender, EventArgs e)
        {
            if (questionGroupTreeView.SelectedNode.Name == "GroupName" ||
                questionGroupTreeView.SelectedNode.Parent?.Name == "GroupName")
            {
                var groupNameTextNode = questionGroupTreeView.Nodes["GroupName"].Nodes["GroupNameText"];
                var groupNameText = TextDialog.Show("Добавление названия", groupNameTextNode.Text);

                groupNameTextNode.Text = groupNameText;
            }

            if (questionGroupTreeView.SelectedNode.Name == "Introduction" ||
                questionGroupTreeView.SelectedNode.Parent?.Name == "Introduction")
            {
                var introductionTextNode = questionGroupTreeView.Nodes["Introduction"].Nodes["IntroductionText"];
                var introductionText = TextDialog.Show("Добавление вступления", introductionTextNode.Text);

                introductionTextNode.Text = introductionText;
            }

            if (questionGroupTreeView.SelectedNode.Parent?.Name == "Questions")
            {
                var awnswers = new List<Answer>();
                foreach(TreeNode answersNode in questionGroupTreeView.SelectedNode.LastNode.Nodes)
                {
                    awnswers.Add(new Answer
                    {
                        AnswerOption = answersNode.Text,
                        AnswerPoints = double.Parse(answersNode.FirstNode.Text)
                    });
                }

                var question = QuestionDialog.Show(questionGroupTreeView.SelectedNode.FirstNode.Text, awnswers);

                questionGroupTreeView.SelectedNode.FirstNode.Text = question.QuestionText;
                questionGroupTreeView.SelectedNode.LastNode.Nodes.Clear();

                foreach(var answer in question.AnswerOptions)
                {
                    questionGroupTreeView.SelectedNode.LastNode.Nodes.Add(answer.AnswerOption);
                    questionGroupTreeView.SelectedNode.LastNode.LastNode.Nodes.Add(answer.AnswerPoints.ToString());
                }
            }
        }

        private void questionGroupTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Name == "GroupName" || e.Node.Parent?.Name == "GroupName")
            {
                addNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "Introduction" || e.Node.Parent?.Name == "Introduction")
            {
                addNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "Questions")
            {
                deleteNodeButton.Enabled = false;
                editNodeButton.Enabled = false;

                addNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Parent?.Name == "Questions")
            {
                deleteNodeButton.Enabled = true;
                addNodeButton.Enabled = true;
                editNodeButton.Enabled = true;

                return;
            }

            editNodeButton.Enabled = false;
            addNodeButton.Enabled = false;
            deleteNodeButton.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //public static GroupQuestions Show(TreeNode defaultQuestionGroupNode = default)
        //{
        //    var questionGroupDialog = new QuestionGroupDialog();

        //    if (defaultQuestionGroupNode != default)
        //    {
        //        foreach (TreeNode childQuestionGroupNode in defaultQuestionGroupNode.Nodes)
        //        {
        //            questionGroupDialog.questionGroupTreeView.Nodes.Add((TreeNode)childQuestionGroupNode.Clone());
        //        }
        //    }

        //    questionGroupDialog.ShowDialog();

        //    var questionGroupNode = new TreeNode();
        //    foreach (TreeNode childQuestionGroupNode in questionGroupDialog.questionGroupTreeView.Nodes)
        //    {
        //        questionGroupNode.Nodes.Add((TreeNode)childQuestionGroupNode.Clone());
        //    }

        //    return questionGroupNode;
        //}

        private void addNodeButton_Click(object sender, EventArgs e)
        {
            if (questionGroupTreeView.SelectedNode.Name == "Questions" ||
                questionGroupTreeView.SelectedNode.Parent?.Name == "Questions")
            {
                var newQuestion = QuestionDialog.Show();
                var questionGroupNode = questionGroupTreeView.Nodes["Questions"];

                var newQuestionNode = new TreeNode();
                newQuestionNode.Nodes.Add("Question", newQuestion.QuestionText);
                newQuestionNode.Nodes.Add("AnswerOptions", "Ответы");

                foreach (var answer in newQuestion.AnswerOptions)
                {
                    newQuestionNode.Nodes["AnswerOptions"].Nodes.Add(answer.AnswerOption);
                    newQuestionNode.Nodes["AnswerOptions"].LastNode.Nodes.Add(answer.AnswerPoints.ToString());
                }

                questionGroupNode.Nodes.Add(newQuestionNode);
                UpdateQuestionNames();
            }
        }

        private void deleteNodeButton_Click(object sender, EventArgs e)
        {
            if (questionGroupTreeView.SelectedNode.Parent?.Name == "Questions")
            {
                questionGroupTreeView.SelectedNode.Remove();
                UpdateQuestionNames();
            }
        }

        private void UpdateQuestionNames()
        {
            var counter = 1;
            foreach (TreeNode questionGroupChildNode in questionGroupTreeView.Nodes["Questions"].Nodes)
            {
                questionGroupChildNode.Text = $"Вопрос {counter++}";
            }
        }
    }
}
