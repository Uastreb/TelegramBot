
namespace TelegramBot.Forms.Forms.Dialogs
{
    partial class QuestionDialog
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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Вопрос", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Ответы");
            this.saveButton = new System.Windows.Forms.Button();
            this.deleteNodeButton = new System.Windows.Forms.Button();
            this.addNodeButton = new System.Windows.Forms.Button();
            this.editNodeButton = new System.Windows.Forms.Button();
            this.questionTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(582, 397);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(170, 34);
            this.saveButton.TabIndex = 12;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // deleteNodeButton
            // 
            this.deleteNodeButton.Location = new System.Drawing.Point(707, 12);
            this.deleteNodeButton.Name = "deleteNodeButton";
            this.deleteNodeButton.Size = new System.Drawing.Size(78, 35);
            this.deleteNodeButton.TabIndex = 11;
            this.deleteNodeButton.Text = "Удалить";
            this.deleteNodeButton.UseVisualStyleBackColor = true;
            // 
            // addNodeButton
            // 
            this.addNodeButton.Location = new System.Drawing.Point(623, 12);
            this.addNodeButton.Name = "addNodeButton";
            this.addNodeButton.Size = new System.Drawing.Size(78, 35);
            this.addNodeButton.TabIndex = 10;
            this.addNodeButton.Text = "Добавить";
            this.addNodeButton.UseVisualStyleBackColor = true;
            this.addNodeButton.Click += new System.EventHandler(this.addNodeButton_Click);
            // 
            // editNodeButton
            // 
            this.editNodeButton.Location = new System.Drawing.Point(539, 12);
            this.editNodeButton.Name = "editNodeButton";
            this.editNodeButton.Size = new System.Drawing.Size(78, 35);
            this.editNodeButton.TabIndex = 9;
            this.editNodeButton.Text = "Изменить";
            this.editNodeButton.UseVisualStyleBackColor = true;
            // 
            // questionTreeView
            // 
            this.questionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.questionTreeView.Location = new System.Drawing.Point(3, 3);
            this.questionTreeView.Name = "questionTreeView";
            treeNode1.Name = "QuestionText";
            treeNode1.Text = "";
            treeNode2.Name = "Question";
            treeNode2.Text = "Вопрос";
            treeNode3.Name = "AnswerOptions";
            treeNode3.Text = "Ответы";
            this.questionTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3});
            this.questionTreeView.Size = new System.Drawing.Size(523, 437);
            this.questionTreeView.TabIndex = 8;
            this.questionTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.questionTreeView_BeforeSelect);
            // 
            // QuestionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 443);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.deleteNodeButton);
            this.Controls.Add(this.addNodeButton);
            this.Controls.Add(this.editNodeButton);
            this.Controls.Add(this.questionTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuestionDialog";
            this.Text = "Создание вопроса";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button deleteNodeButton;
        private System.Windows.Forms.Button addNodeButton;
        private System.Windows.Forms.Button editNodeButton;
        private System.Windows.Forms.TreeView questionTreeView;
    }
}