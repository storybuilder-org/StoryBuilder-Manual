using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MarkdownSplitter
{
    /// <summary>
    /// Generate the StoryBuilder User Manual website by processing
    /// the output of a Scrivener Compile to MultiMarkdown.
    /// The output of the Compile command is written to compilerFolder.
    ///
    /// The .md markdown files are rewritten as a tree of nodes (blocks) each 
    /// of which contains links to its child nodes, according to its depth
    /// or indententation level in the Scrivener file.
    /// 
    /// The workflow is as follows:
    ///    Verify that the StoryBuilder-Manual folder browsed to is valid.
    ///    Empty the (output) gh-pages folder.
    ///    Process the compileFolder's contents into markdownFolder.
    ///    Create the index.md file into ghPagesFolder by reading markdownFolder.
    ///    Write the child .md files for each block as a link statement in its
    ///    parent node.
    /// </summary>
    public class Splitter
    {
        public string MarkdownFolder = @"manual.md";
        private string repositoryPath = Directory.GetCurrentDirectory();
        private string ghPagesFolder = @"gh-pages";
        private string splitMarker = "#";
        private Block[] level = new Block[7];
        private List<string> toc = new();

        /// <summary>
        /// Creates an empty github pages folder
        /// </summary>
        public void EmptyGitHubPagesFolder()
        {
            repositoryPath = Directory.GetParent(MarkdownFolder)!.Name;
            ghPagesFolder = Path.Join(repositoryPath, @"gh-pages");
            DirectoryInfo di = new(ghPagesFolder);
            if (di.Exists)
            {
                di.Delete(true);
            }

            di.Create();
        }
        public bool ProcessMarkdownFolder()
        {
            bool found = false;
            try
            {
                var txtFiles = Directory.EnumerateFiles(MarkdownFolder, "*.*");
                foreach (string currentFile in txtFiles)
                {
                    string fileName = currentFile.Substring(MarkdownFolder.Length + 1);
                    if (fileName.EndsWith(".md"))
                    {
                        ProcessMarkdownFile(currentFile);
                        found = true;
                    }
                    else
                        File.Copy(currentFile, Path.Combine(ghPagesFolder, fileName));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return found;
        }
        public void SplitMarkdownFile()
        {
            Block root = level[0];
            foreach (Block child in root.Children)
                RecurseMarkdownBlocks(child);
        }
        public void CreateIndexFile()
        {
            using (StreamWriter file = new(Path.Combine(ghPagesFolder, "index.md")))
            {
                file.WriteLine("# Table of Contents");
                file.WriteLine("");
                foreach (var block in level[0].Children)
                {
                    file.WriteLine($"[{block.Title}]({block.Filename}) <br/><br/>");
                }

                file.Close();
            }
        }
        public void CreateChildMarkdownFiles()
        {
            //This creates the other md files.
            foreach (var child in level[0].Children)
            {
                WriteChildFile(child);
            }
        }
        private void ProcessMarkdownFile(string currentFile)
        {
            string[] markdown = File.ReadAllLines(currentFile);
            // When a new markdown line (splitMarker) is detected, its
            // parent block is the level just above its level.
            Block current = new Block("");
            level[0] = current;
            Block parent;

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
        private void RecurseMarkdownBlocks(Block block)
        {
            WriteMarkdownBlock(block);
            foreach (Block child in block.Children)
                RecurseMarkdownBlocks(child);
        }
        private void WriteMarkdownBlock(Block block)
        {
            string filepath = Path.Combine(ghPagesFolder, block.Filename);
            using (StreamWriter file = new StreamWriter(filepath))
            {
                file.WriteLine(block.Header);
                foreach (string line in block.Text)
                    file.WriteLine(CleanupMarkdown(line));
                file.Close();
            }
        }
        private void WriteChildFile(Block bloc)
        {
            StringBuilder sb = new();
            sb.AppendLine(bloc.Header); //This writes the header
            foreach (var text in bloc.Text)
            {
                sb.AppendLine(CleanupMarkdown(text + " <br/>"));
            } //This writes all the text 

            foreach (var child in bloc.Children)
            {
                sb.AppendLine(CleanupMarkdown($"[{child.Title}]({child.Filename}) <br/><br/>"));
            } //This writes any links

            File.WriteAllText(Path.Combine(ghPagesFolder, bloc.Filename),
                sb.ToString()); //This actually writes the file.
            foreach (var child in bloc.Children)
            {
                WriteChildFile(child);
            } //This causes it to recursively run this on its children
        }
        private string CleanupMarkdown(string line)
        {
            if (line.Contains("[Front Page (Image)](Front_Page_(Image).md)"))
            {
                return "![](StoryBuilder.png)";
            }

            if (line == " <br/>")
            {
                line = "";
            }

            if (line.IndexOf("![") > -1)
            {
                string[] tokens = line.Split(new char[] { '[', ']' });
                string line2 = "![](" + tokens[3] + ".png)";
                return line2;
            }

            return line;
        }
    }
}