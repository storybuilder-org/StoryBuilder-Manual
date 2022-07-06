using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;  

namespace MarkdownSplitter;

internal class Splitter
{
    private string compilerFolder = @"C:\Users\jaker\Desktop\everywhereattheendoftime.md";
    private string ghPagesFolder = @"C:\Users\jaker\Desktop\nik";
    private string MarkdownFile = @"Testeverywhereattheendoftime.md";
    private string splitMarker = "#";
    private Block[] level = new Block[7];
    private List<string> toc = new();

    public void Split()
    {
        //Empty the (output) gh-pages folder
        EmptyGitHubPagesFolder();

        //Process the compile folder's contents.
        ProcessCompileFolder();

        SplitMarkdownFile();

        //This creates the index.md file
        CreateIndexFile();

        //This creates the other md files.
        foreach (var child in level[0].Children) { WriteFile(child); }
    }

    void WriteFile(Block bloc)
    {
        StringBuilder sb = new();
        sb.AppendLine(bloc.Header); //This writes the header
        foreach (var text in bloc.Text) { sb.AppendLine(CleanupMarkdown(text + " <br/>")); } //This writes all the text 
        foreach (var child in bloc.Children) { sb.AppendLine(CleanupMarkdown($"[{child.Title}]({child.Filename}) <br/><br/>")); } //This writes any links
        File.WriteAllText(Path.Combine(ghPagesFolder, bloc.Filename),sb.ToString()); //This actually writes the file.
        foreach (var child in bloc.Children) { WriteFile(child); } //This causes it to recursively run this on its children
    }


    /// <summary>
    /// Creates an empty github pages folder
    /// </summary>
    private void EmptyGitHubPagesFolder()
    {
        DirectoryInfo di = new(ghPagesFolder);
        if (di.Exists) {di.Delete(true);}
        di.Create();
    }

    void ProcessCompileFolder()
    {
        try
        {
            var txtFiles = Directory.EnumerateFiles(compilerFolder, "*.*");
            foreach (string currentFile in txtFiles)
            {
                string fileName = currentFile.Substring(compilerFolder.Length + 1);
                if (fileName.EndsWith(".md"))
                    ProcessMarkdownFile(currentFile);
                else
                    File.Copy(currentFile, Path.Combine(ghPagesFolder, fileName));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void ProcessMarkdownFile(string currentFile)
    {
        string[] markdown = File.ReadAllLines(currentFile);
        // When a new markdown line (splitMarker) is detected, its
        // parent block is the level just above its level.
        Block current = new("");
        level[0] = current;
        Block parent = current;
          
        foreach (string line in markdown) 
        {
            if (line.StartsWith(splitMarker))
            {
                Console.WriteLine(line);
                current = new Block(line);
                int parentLevel = current.Level - 1;
                parent = level[parentLevel];
                parent.Children.Add(current);
                level[current.Level] = current;    
            }
            else 
            {
                if (current != null)
                    current.Text.Add(line);
            }
        }
    }

    private void SplitMarkdownFile()
    {
        Block root = level[0];
        foreach (Block child in root.Children)
            RecurseMarkdownBlocks(child);
    }

    private void RecurseMarkdownBlocks(Block block)
    {
        WriteMarkdownBlock(block);
        foreach (Block child in block.Children)
            RecurseMarkdownBlocks(child);
    }

    private void WriteMarkdownBlock(Block block)
    {
        string filepath = Path.Combine(ghPagesFolder, block.Filename);
        using (StreamWriter file = new(filepath))
        {
            file.WriteLine(block.Header);
            foreach (string line in block.Text) 
                file.WriteLine(CleanupMarkdown(line)); 
            file.Close();  
        }
    }

    private string CleanupMarkdown(string line)
    {  
        if (line.Contains("[Front Page (Image)](Front_Page_(Image).md)")){return "![](StoryBuilder.png)"; }
        if (line == " <br/>") {line ="";}
        if (line.IndexOf("![") > -1)
        {
            string[] tokens = line.Split(new char[] {'[',']' });
            string line2 = "![](" + tokens[3] + ".png)";
            return line2;
        }
        return line;
    }

    private void CreateIndexFile()
    {
        using (StreamWriter file = new(Path.Combine(ghPagesFolder, "index.md")))
        {
            file.WriteLine("# Table of Contents");
            file.WriteLine("");
            foreach (var block in level[0].Children) { file.WriteLine($"[{block.Title}]({block.Filename}) <br/><br/>"); }
            file.Close();
        }
    }
}