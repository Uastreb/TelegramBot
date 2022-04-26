using System;
using System.Windows.Forms;
using System.Linq;

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

            editNodeButton.Enabled = false;
            addNodeButton.Enabled = false;
            deleteNodeButton.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public static void Show()
        {
            var questionGroupDialog = new QuestionGroupDialog();

            //textWithNameDialog.nameTextBox.Text = defaultName;
            //textWithNameDialog.valueTextBox.Text = defaultValue;
            questionGroupDialog.ShowDialog();

            //return new UserData()
            //{
            //    Name = textWithNameDialog.nameTextBox.Text,
            //    Text = textWithNameDialog.valueTextBox.Text
            //};
        }

        private void addNodeButton_Click(object sender, EventArgs e)
        {
            if (questionGroupTreeView.SelectedNode.Name == "Questions" || 
                questionGroupTreeView.SelectedNode.Parent?.Name == "Questions")
            {
                var question = QuestionDialog.Show();
                var selectedNode = questionGroupTreeView.SelectedNode.Name == "Questions" ? questionGroupTreeView.SelectedNode : questionGroupTreeView.SelectedNode.Parent;

                var newQuestionNode = selectedNode.Nodes.Add($"Вопрос {selectedNode.Nodes.Count}");
                foreach(TreeNode questionNode in question)
                {
                    newQuestionNode.Nodes.Add((TreeNode)questionNode.Clone());
                }
                //var questionNode = selectedNode.Nodes.Add(question.QuestionText);
                //question.A

                //userDataNode.Nodes.Add(userData.Text);
            }
        }
    }
}
