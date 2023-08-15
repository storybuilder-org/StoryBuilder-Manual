using System.ComponentModel;
using System.Diagnostics;
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
    ///    Empty the (output) /docs folder. This folder is where the final
    ///       GitHub Pages .md files (and .png) images go. When your local repository
    ///       changes (your manual edits processed by Compile and MarkdownSplitter)
    ///       are pushed to the online repository, the /docs folder will be processed
    ///       by YAML to generate the test website.
    ///    Process the Scrivener project's Compile output folder contents. This is
    ///       the .md folder (typically manual.md) and contains single MultiMarkdown
    ///       .md file and its .png image files.
    ///    Create the index.md file in the docs folder by reading the .md markdown
    ///       file and reformatting it as a series of blocks.
    ///    Write the child .md files for each block as a set of link statements. That
    ///       is, each child node is a markdown link in its parent's .md file.
    ///
    /// Note that top-level functions, called from SplitterForm, are public; subordinate
    /// functions, called from the top-level ones, have private accessors.
    /// </summary>
    public class Splitter
    {
        public string MarkdownFolder = @"manual.md";
        private string repositoryPath = Directory.GetCurrentDirectory();
        private string docsFolder = @"docs";
        private string splitMarker = "#";
        private readonly Block[] level = new Block[7];
        private string[] FilesInOrder;

        /// <summary>
        /// Creates an empty github pages (/docs) folder
        /// </summary>
        public void EmptyDocsFolder()
        {
            repositoryPath = Directory.GetParent(MarkdownFolder)!.FullName;
            docsFolder = Path.Join(repositoryPath, @"docs");
            DirectoryInfo di = new(docsFolder);
            if (di.Exists)
            {
                di.Delete(true);
            }

            di.Create();
        }
        
        /// <summary>
        /// Process the folder written by Scrivener Compile. This contains a single
        /// .md markdown file and a set of .png image files. Find the .md file and
        /// process it. All other files (the images) are copied to the /docs folder.
        ///
        /// If the .md file can't be found, the Compile wasn't performed properly.
        /// </summary>
        /// <returns></returns>
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
                        File.Copy(currentFile, Path.Combine(docsFolder, fileName));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return found;
        }
        /// <summary>
        /// Process the Compiler's .md MultiMarkdown file.
        ///
        /// It's parsed as a series of linked Block objects. A new block is
        /// created everytime a Markdown Header (#, ##, etc.) is found in the
        /// Compiler output file, and added to it's parent block. The level of
        /// nesting for a block is determined by its heading level.
        ///
        /// If a line doesn't start a new block, it's added to the current block.
        /// </summary>
        /// <param name="currentFile"></param>
        private void ProcessMarkdownFile(string currentFile)
        {
            // Read the .md file into memory.     
            string[] markdown = File.ReadAllLines(currentFile);
            // When a new markdown line (splitMarker) is detected, its
            // parent block is the level just above its level.
            Block current = new Block("");
            level[0] = current;   // level zero is the table of contents.

            FilesInOrder = markdown.Where(str => str.StartsWith("#")).ToArray();

            foreach (string line in markdown)
            {
                if (line.StartsWith(splitMarker))
                {
                    Debug.WriteLine(line);
                    current = new Block(line);
                    int parentLevel = current.Level - 1;
                    var parent = level[parentLevel];
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

        /// <summary>
        /// The output of the Scrivener Compile process is a single .md Markdown file.
        /// Having created a series of Block instances containing each chuck of text,
        /// write each block as a separate file, traversing the blocks via recursive
        /// descent, starting with the single top-level block.
        /// </summary>
        public void SplitMarkdownFile()
        {
            Block root = level[0];
            foreach (Block child in root.Children)
                RecurseMarkdownBlock(child);
        }

        /// <summary>
        /// Write the passed block of Markdown text as a separate .md file
        /// and then process each of its children.
        /// </summary>
        /// <param name="block"></param>
        private void RecurseMarkdownBlock(Block block)
        {
            WriteMarkdownBlock(block);
            foreach (Block child in block.Children)
                RecurseMarkdownBlock(child);
        }


        /// <summary>
        /// Write one block's text as a .md file in the /docs folder.
        /// </summary>
        /// <param name="block"></param>
        private void WriteMarkdownBlock(Block block)
        {
            string filepath = Path.Combine(docsFolder, block.Filename);
            using (StreamWriter file = new StreamWriter(filepath))
            {
                file.WriteLine(block.Header);
                foreach (string line in block.Text)
                    file.WriteLine(CleanupMarkdown(line));
                file.Close();
            }
        }

        /// <summary>
        /// Write the top-level index.md file by looping through and printing
        /// </summary>
        public void CreateIndexFile()
        {
            string filepath = Path.Combine(docsFolder, "index.md");
            using (StreamWriter file = new StreamWriter(filepath))
            {
                file.WriteLine("# Table of Contents #");
                file.WriteLine("");
                foreach (var block in level[0].Children)
                {
                    string line = $"[{block.Title}]({block.Filename}) <br/><br/>";
                    file.WriteLine(line);
                }
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

        private void WriteChildFile(Block bloc)
        {
            StringBuilder sb = new();
            sb.AppendLine(bloc.Header); //This writes the header
            foreach (var text in bloc.Text)
            {
                if (text.Contains("•")) //<br/> is broken on bullet pointed lists.
                {
                    sb.AppendLine(CleanupMarkdown(text));
                }
                else { sb.AppendLine(CleanupMarkdown(text + " <br/>")); }
            } //This writes all the text 

            foreach (var child in bloc.Children)
            {
                sb.AppendLine(CleanupMarkdown($"[{child.Title}]({child.Filename}) <br/><br/>"));
            } //This writes any links

            //Insert blank line before next/prev for clarity purpose.
            sb.AppendLine(" <br/><br/>");

            int index = Array.IndexOf(FilesInOrder, bloc.Header);
            if (index != 0)
            {
                Block bk = new(FilesInOrder[index - 1]);
                //We shouldn't link to the front page since its not useful to user.
                if (bk.Filename == "Front_Page_(Image).md")
                {
                    sb.AppendLine($"[Previous - Front matter](Front_Matter.md) <br/><br/>");
                }
                else { sb.AppendLine($"[Previous - {bk.Title}]({bk.Filename}) <br/><br/>"); }
            }

            //Next Link
            if (index < FilesInOrder.Length - 1)
            {
                Block bk = new(FilesInOrder[index + 1]);

                //We shouldn't link to the front page since its not useful to user.
                if (bk.Filename == "Front_Page_(Image).md")
                {
                    sb.AppendLine($"[Next up - Preface](Preface.md)");
                }
                else { sb.AppendLine($"[Next up - {bk.Title}]({bk.Filename})"); }
                
            }

            File.WriteAllText(Path.Combine(docsFolder, bloc.Filename),
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
                return "![](StoryCAD.png)";
            }

            //Tabs and multiple spaces don't work in markdown.
            line = line.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

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