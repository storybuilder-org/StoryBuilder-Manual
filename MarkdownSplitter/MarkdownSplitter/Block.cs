using System.Text;  

namespace MarkdownSplitter;

public class Block
{
    public string Header;
    public string Title;
    public List<Block> Children;
    public string Filename;
    public string WebPage;
    public List<string> Text;
    public int Level;

    public Block(string header) 
    {
        Header = header;
        Children = new List<Block>();
        Text = new List<string>();
        ParseFilename(header);
        Level = header.IndexOf(" ");
        if (Level == -1) { Level = 0;}
    }

    private void ParseFilename(string header)
    {
        string temp = header.Replace("#", " ");
        Title = temp.Trim();
        StringBuilder filename = new(Title);
        filename = filename.Replace(" ", "_");

        // replace invalid filename characters
        filename.Replace('<', '_');
        filename.Replace('>', '_');
        filename.Replace(':', '_');
        filename.Replace('"', '_');
        filename.Replace('/', '_');
        filename.Replace('\\','_');
        filename.Replace('|', '_');
        filename.Replace('?', '_');
        filename.Replace('*', '_');

        filename.Replace("__", "_");
        filename.Append(".md");
        Filename = filename.ToString();
    }
}