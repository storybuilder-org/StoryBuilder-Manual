# About StoryBuilder's User Manual

The StoryBuilder User Manual is the GitHub Pages website located
at https://storybuilder-org.github.io/StoryBuilder-2/ which is 
associated with the project's repository:
https://github.com/storybuilder-org/StoryBuilder-2

The User Manual is generated from a series of static pages in 
Markdown (.md) format, but StoryBuilder's user manual is stored
as s Scrivener project, and compiled as a MultiMarkdown file.

Scrivener is a popular WSIWYG ('what you see is what you get') text editor
which allows generating its output in multiple formats.Scrivener's 
perfect for large, complicated documents containing many sections, 
such as novels and non-fiction books, or in our case, the user manual. 
The output of the Scrivener compile is post-processed to produce
the GitHub Pages website.

## Testing the user manual

Because it's the production website,https://storybuilder-org.github.io/StoryBuilder-2/
can't be modified to test and review documentation changes. Accordingly,
this repository, https://github.com/storybuilder-org/StoryBuilder-Manual 
was created. It has its own GitHub Pages website,
https://storybuilder-org.github.io/StoryBuilder-Manual/, which is for
testing changes to the documentation. 

The StoryBuilder-Manual repo is **self-contained** and has everything needed
to update the User Manual:

- The Scrivener project (/manual.scriv folder)
- A folder to hold the Scrivener compile MultiMarkdown output (/manual.md)
- MarkdownSplitter, a utility to post-process the markdown output
- The GitHub Pages the website's built from (/docs folder)

## Setup for testing

## Scrivener 

You MUST have a Scrivener license and Scrivener 3.0 installed
to update the StoryBuilder User Manual.

NOTE: The Scrivener project can only be edited by one person at 
a time. If multiple people are working on documentation changes,
you must coordinate with each other.

## Edit the User Manual

1. Make sure you are in the 'manual' branch. 
2. Navigate to the manual.scriv folder in your StoryBUilder-Manual 
repository clone. Open the scriner-manual.scrivx file using Scrivener.
3. Make you text changes as per any large Scrivener document.
4. The StoryBuilder Scrivener project has a great many images. An 
image inserted in an .rtf file is 

## Build and test the StoryBuilder User Manual

## Update the production User Manual


