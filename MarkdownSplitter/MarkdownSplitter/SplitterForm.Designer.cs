namespace MarkdownSplitter
{
    partial class SplitterForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.BrowseFolderButton = new System.Windows.Forms.Button();
            this.ManualFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.RunButton = new System.Windows.Forms.Button();
            this.StatusMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BrowseFolderButton
            // 
            this.BrowseFolderButton.Location = new System.Drawing.Point(152, 44);
            this.BrowseFolderButton.Name = "BrowseFolderButton";
            this.BrowseFolderButton.Size = new System.Drawing.Size(295, 34);
            this.BrowseFolderButton.TabIndex = 0;
            this.BrowseFolderButton.Text = "Select manual.md folder";
            this.BrowseFolderButton.UseVisualStyleBackColor = true;
            this.BrowseFolderButton.Click += new System.EventHandler(this.BrowseFolderButton_Click);
            // 
            // ManualFolder
            // 
            this.ManualFolder.Location = new System.Drawing.Point(42, 93);
            this.ManualFolder.Name = "ManualFolder";
            this.ManualFolder.ReadOnly = true;
            this.ManualFolder.Size = new System.Drawing.Size(513, 31);
            this.ManualFolder.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(228, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 25);
            this.label1.TabIndex = 2;
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(228, 190);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(134, 34);
            this.RunButton.TabIndex = 9;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // StatusMessage
            // 
            this.StatusMessage.Enabled = false;
            this.StatusMessage.Location = new System.Drawing.Point(42, 300);
            this.StatusMessage.Name = "StatusMessage";
            this.StatusMessage.ReadOnly = true;
            this.StatusMessage.Size = new System.Drawing.Size(513, 31);
            this.StatusMessage.TabIndex = 10;
            // 
            // SplitterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 374);
            this.Controls.Add(this.StatusMessage);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ManualFolder);
            this.Controls.Add(this.BrowseFolderButton);
            this.Name = "SplitterForm";
            this.Text = "Markdown Splitter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FolderBrowserDialog FolderBrowserDialog;
        private Button BrowseFolderButton;
        private TextBox ManualFolder;
        private Label label1;
        private Button RunButton;
        private TextBox StatusMessage;
    }
}