using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramBot.Forms.Forms.Dialogs;

namespace TelegramBot.Forms
{
    public partial class TestGeneratorForm : Form
    {
        public TestGeneratorForm()
        {
            InitializeComponent();
        }

        private void EditNode_Click(object sender, EventArgs e)
        {
            if (testTreeView.SelectedNode.Name == "Introduction" || testTreeView.SelectedNode.Parent?.Name == "Introduction")
            {
                var introductionTextNode = testTreeView.Nodes["Introduction"].Nodes["IntroductionText"];
                var introductionText = TextDialog.Show("Добавление заголовка", introductionTextNode.Text);

                introductionTextNode.Text = introductionText;
            }

            if (testTreeView.SelectedNode.Name == "Conclusion" || testTreeView.SelectedNode.Parent?.Name == "Conclusion")
            {
                var conclusionTextNode = testTreeView.Nodes["Conclusion"].Nodes["ConclusionText"];
                var conclusionText = TextDialog.Show("Добавление заключения", conclusionTextNode.Text);

                conclusionTextNode.Text = conclusionText;
            }
        }

        private void AddNode_Click(object sender, EventArgs e)
        {
            if (testTreeView.SelectedNode.Name == "UserDatas" || testTreeView.SelectedNode.Parent?.Name == "UserDatas")
            {
                var userData = UserDataDialog.Show();

                var selectedNode = testTreeView.SelectedNode.Name == "UserDatas" ? testTreeView.SelectedNode : testTreeView.SelectedNode.Parent;
                var userDataNode = selectedNode.Nodes.Add(userData.Name);
                userDataNode.Nodes.Add(userData.Text);
            }

            if (testTreeView.SelectedNode.Name == "QuestionGroups")
            {
                QuestionGroupDialog.Show();
            }
        }

        private void testTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Name == "Introduction" || e.Node.Parent?.Name == "Introduction")
            {
                addNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "Conclusion" || e.Node.Parent?.Name == "Conclusion")
            {
                addNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "UserDatas")
            {
                editNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                addNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Parent?.Name == "UserDatas")
            {
                addNodeButton.Enabled = false;

                deleteNodeButton.Enabled = true;
                editNodeButton.Enabled = true;

                return;
            }

            if (e.Node.Name == "QuestionGroups")
            {
                editNodeButton.Enabled = false;
                deleteNodeButton.Enabled = false;

                addNodeButton.Enabled = true;

                return;
            }

            editNodeButton.Enabled = false;
            addNodeButton.Enabled = false;
            deleteNodeButton.Enabled = false;
        }

        private void DeleteNode_Click(object sender, EventArgs e)
        {
            if (testTreeView.SelectedNode.Parent?.Name == "UserDatas")
            {
                testTreeView.SelectedNode.Remove();
            }
        }
    }
}
