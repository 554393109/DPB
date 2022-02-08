# DPB: Dynamic Project Builder

DPB is a tool that allows developers to automatically generate project code. 

You can add annotations to the code templates, and use DPB to automatically filter or generate code to build a complete new project.

DPB support for all files and all languages.

## Why I need DPB

We always build new customized projects and template projects. When new project requirements come, we have to copy the whole project files and do a lot of <i>washing</i> jobs: remove the files, change the key contents, modify configuration values and so on.

Also when we published an open source project, we always support Demo / Sample and Documents. But it's really not every developer wants to read all of the code and sample project and documents.

With DPB, you just need to put same marks into your code, such as `PDBMARK Keep` and 'PDBMARK_END', or configure a profile, then run DPB, it will build a new clearn-project into the Output Directory, just keep the code and files you want.

## How to use

### Method 1: Build App by your own

1. Install the PDB from Nuget into your project(or compile the source code): https://www.nuget.org/packages/DPB/

> Both your working prject or a new project are OK.

2. Create a new class, such as BuildProject.cs

3. Create a method such as Build() in to BuildBProject

4. Create a manifest entity, and set the SourceDir(your template peojrct_ and OutputDir:

``` C#

using DPB.Models;

namespace DPB.Tests
{
    public class BuildProject
    {
        public void Build()
        {
            Manifest manifest = new Manifest();
            manifest.SourceDir = "..\\..\\SourceDir";//or absolute address: e:\ThisProject\src
            manifest.OutputDir = "..\\..\\OutputDir";//or absolute address: e:\ThisProject\Output
        }
    }
}

```

5. set the rules, such as :

#### Remove content block

``` C#
            //keep content Condition - while all the code blocks in *.cs files with keywrod mark: PDBMARK MP
            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "*.cs" },
                KeepContentConiditions = new List<string>() { "MP" }
            });

```

it will keep the code in your source project with the certain code block:

``` C#
            //PDBMARK MP
            var tip = "this line will stay here in OutputDir while Conditions have MP keyword.";
            //PDBMARK_END
```
当文件区域中的另一个代码块带有' PDBMARK OTHER '而不是' PDBMARK MP '时，该代码块将被删除。

你也可以设置多个“KeepContentConiditions”关键字来保持代码留在新项目中。

代码(或任何文本文件内容)没有' PDBMARK '标记bolck将始终保持在新项目。

#### Delete whole file

如果你想删除一个文件，只需在文件的任何地方添加以下代码:

> PDBMARK_FILE RemoveFile



