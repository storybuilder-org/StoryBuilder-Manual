namespace MarkdownSplitter
{
    public partial class SplitterForm : Form
    {
        private readonly Splitter splitter;
        
        public SplitterForm(Splitter splitter)
        {
            InitializeComponent();
            this.splitter = splitter;
        }

        private void BrowseFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();  
            folderDlg.ShowNewFolderButton = true;  
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                ManualFolder.Text = folderDlg.SelectedPath;
                splitter.MarkdownFolder = folderDlg.SelectedPath;
                RunButton.Enabled = true;
            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            splitter.EmptyGitHubPagesFolder();
            if (!splitter.ProcessMarkdownFolder())
            {
                StatusMessage.Text = @"Markdown file not found. Try again.";
                RunButton.Text = @"Failed";
                RunButton.Enabled = false;
                return;
            }
            splitter.SplitMarkdownFile();
            splitter.CreateIndexFile();
            splitter.CreateChildMarkdownFiles();
            RunButton.Text = @"Done";
            RunButton.Enabled = false;
        }
    }
}