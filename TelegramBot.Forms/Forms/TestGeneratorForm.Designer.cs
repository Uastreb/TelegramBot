
namespace TelegramBot.Forms
{
    partial class TestGeneratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Вступление", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Даные о пользователе");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Группы вопросов");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Заключение", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            this.testTreeView = new System.Windows.Forms.TreeView();
            this.editNodeButton = new System.Windows.Forms.Button();
            this.addNodeButton = new System.Windows.Forms.Button();
            this.deleteNodeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // testTreeView
            // 
            this.testTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.testTreeView.Location = new System.Drawing.Point(12, 1);
            this.testTreeView.Name = "testTreeView";
            treeNode1.Name = "IntroductionText";
            treeNode1.Text = "";
            treeNode2.Name = "Introduction";
            treeNode2.Text = "Вступление";
            treeNode3.Name = "UserDatas";
            treeNode3.Text = "Даные о пользователе";
            treeNode4.Name = "QuestionGroups";
            treeNode4.Text = "Группы вопросов";
            treeNode5.Name = "ConclusionText";
            treeNode5.Text = "";
            treeNode6.Name = "Conclusion";
            treeNode6.Text = "Заключение";
            this.testTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode6});
            this.testTreeView.Size = new System.Drawing.Size(523, 378);
            this.testTreeView.TabIndex = 0;
            this.testTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.testTreeView_BeforeSelect);
            // 
            // editNodeButton
            // 
            this.editNodeButton.Location = new System.Drawing.Point(546, 12);
            this.editNodeButton.Name = "editNodeButton";
            this.editNodeButton.Size = new System.Drawing.Size(78, 35);
            this.editNodeButton.TabIndex = 1;
            this.editNodeButton.Text = "Изменить";
            this.editNodeButton.UseVisualStyleBackColor = true;
            this.editNodeButton.Click += new System.EventHandler(this.EditNode_Click);
            // 
            // addNodeButton
            // 
            this.addNodeButton.Location = new System.Drawing.Point(630, 12);
            this.addNodeButton.Name = "addNodeButton";
            this.addNodeButton.Size = new System.Drawing.Size(78, 35);
            this.addNodeButton.TabIndex = 2;
            this.addNodeButton.Text = "Добавить";
            this.addNodeButton.UseVisualStyleBackColor = true;
            this.addNodeButton.Click += new System.EventHandler(this.AddNode_Click);
            // 
            // deleteNodeButton
            // 
            this.deleteNodeButton.Location = new System.Drawing.Point(714, 12);
            this.deleteNodeButton.Name = "deleteNodeButton";
            this.deleteNodeButton.Size = new System.Drawing.Size(78, 35);
            this.deleteNodeButton.TabIndex = 3;
            this.deleteNodeButton.Text = "Удалить";
            this.deleteNodeButton.UseVisualStyleBackColor = true;
            this.deleteNodeButton.Click += new System.EventHandler(this.DeleteNode_Click);
            // 
            // TestGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(804, 381);
            this.Controls.Add(this.deleteNodeButton);
            this.Controls.Add(this.addNodeButton);
            this.Controls.Add(this.editNodeButton);
            this.Controls.Add(this.testTreeView);
            this.Name = "TestGeneratorForm";
            this.Text = "TestGeneratorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView testTreeView;
        private System.Windows.Forms.Button editNodeButton;
        private System.Windows.Forms.Button addNodeButton;
        private System.Windows.Forms.Button deleteNodeButton;
    }
}