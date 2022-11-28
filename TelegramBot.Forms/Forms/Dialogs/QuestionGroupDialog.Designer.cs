
namespace TelegramBot.Forms.Forms.Dialogs
{
    partial class QuestionGroupDialog
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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Название группы", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Вступление", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Вопросы");
            this.questionGroupTreeView = new System.Windows.Forms.TreeView();
            this.deleteNodeButton = new System.Windows.Forms.Button();
            this.addNodeButton = new System.Windows.Forms.Button();
            this.editNodeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // questionGroupTreeView
            // 
            this.questionGroupTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.questionGroupTreeView.Location = new System.Drawing.Point(2, 1);
            this.questionGroupTreeView.Name = "questionGroupTreeView";
            treeNode1.Name = "GroupNameText";
            treeNode1.Text = "";
            treeNode2.Name = "GroupName";
            treeNode2.Text = "Название группы";
            treeNode3.Name = "IntroductionText";
            treeNode3.Text = "";
            treeNode4.Name = "Introduction";
            treeNode4.Text = "Вступление";
            treeNode5.Name = "Questions";
            treeNode5.Text = "Вопросы";
            this.questionGroupTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode5});
            this.questionGroupTreeView.Size = new System.Drawing.Size(523, 446);
            this.questionGroupTreeView.TabIndex = 1;
            this.questionGroupTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.questionGroupTreeView_BeforeSelect);
            // 
            // deleteNodeButton
            // 
            this.deleteNodeButton.Location = new System.Drawing.Point(710, 12);
            this.deleteNodeButton.Name = "deleteNodeButton";
            this.deleteNodeButton.Size = new System.Drawing.Size(78, 35);
            this.deleteNodeButton.TabIndex = 6;
            this.deleteNodeButton.Text = "Удалить";
            this.deleteNodeButton.UseVisualStyleBackColor = true;
            this.deleteNodeButton.Click += new System.EventHandler(this.deleteNodeButton_Click);
            // 
            // addNodeButton
            // 
            this.addNodeButton.Location = new System.Drawing.Point(626, 12);
            this.addNodeButton.Name = "addNodeButton";
            this.addNodeButton.Size = new System.Drawing.Size(78, 35);
            this.addNodeButton.TabIndex = 5;
            this.addNodeButton.Text = "Добавить";
            this.addNodeButton.UseVisualStyleBackColor = true;
            this.addNodeButton.Click += new System.EventHandler(this.addNodeButton_Click);
            // 
            // editNodeButton
            // 
            this.editNodeButton.Location = new System.Drawing.Point(542, 12);
            this.editNodeButton.Name = "editNodeButton";
            this.editNodeButton.Size = new System.Drawing.Size(78, 35);
            this.editNodeButton.TabIndex = 4;
            this.editNodeButton.Text = "Изменить";
            this.editNodeButton.UseVisualStyleBackColor = true;
            this.editNodeButton.Click += new System.EventHandler(this.editNodeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(585, 389);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(170, 34);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // QuestionGroupDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.deleteNodeButton);
            this.Controls.Add(this.addNodeButton);
            this.Controls.Add(this.editNodeButton);
            this.Controls.Add(this.questionGroupTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuestionGroupDialog";
            this.Text = "Создание групы вопросов";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView questionGroupTreeView;
        private System.Windows.Forms.Button deleteNodeButton;
        private System.Windows.Forms.Button addNodeButton;
        private System.Windows.Forms.Button editNodeButton;
        private System.Windows.Forms.Button saveButton;
    }
}