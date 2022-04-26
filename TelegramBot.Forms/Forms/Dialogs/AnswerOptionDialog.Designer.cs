
namespace TelegramBot.Forms.Forms.Dialogs
{
    partial class AnswerOptionDialog
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
            this.saveButton = new System.Windows.Forms.Button();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.pointsLabel = new System.Windows.Forms.Label();
            this.pointsNumericUD = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pointsNumericUD)).BeginInit();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(152, 250);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(142, 42);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // valueTextBox
            // 
            this.valueTextBox.Location = new System.Drawing.Point(36, 130);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(394, 82);
            this.valueTextBox.TabIndex = 8;
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(36, 102);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(81, 15);
            this.valueLabel.TabIndex = 7;
            this.valueLabel.Text = "Введите текст";
            // 
            // pointsLabel
            // 
            this.pointsLabel.AutoSize = true;
            this.pointsLabel.Location = new System.Drawing.Point(36, 25);
            this.pointsLabel.Name = "pointsLabel";
            this.pointsLabel.Size = new System.Drawing.Size(299, 15);
            this.pointsLabel.TabIndex = 5;
            this.pointsLabel.Text = "Введите кол-во балов (получаемых за выбор ответа)";
            // 
            // pointsNumericUD
            // 
            this.pointsNumericUD.Location = new System.Drawing.Point(36, 52);
            this.pointsNumericUD.Name = "pointsNumericUD";
            this.pointsNumericUD.Size = new System.Drawing.Size(394, 23);
            this.pointsNumericUD.TabIndex = 10;
            // 
            // AnswerOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 304);
            this.Controls.Add(this.pointsNumericUD);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.pointsLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnswerOptionDialog";
            this.Text = "Создание варианта ответа";
            ((System.ComponentModel.ISupportInitialize)(this.pointsNumericUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label pointsLabel;
        private System.Windows.Forms.NumericUpDown pointsNumericUD;
    }
}