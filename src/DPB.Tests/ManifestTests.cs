using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using DPB.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;

namespace DPB.Tests
{
    [TestClass]
    public class ManifestTests
    {
        [TestMethod]
        public void JsonTest()
        {
            var json = @"{
""SourceDir"":"""",
""OutputDir"":"""",
""ConfigGroup"":[{
    ""Files"":[],
    ""KeepFileConiditions"":[],
    ""KeepContentConiditions"":[],
    ""ReplaceContents"":[
        {""XmlContent"":{""TagName"":""<TargetFrameworks>"",""ReplaceContent"":""this is the new content""}}
    ]
}]
}";

            //测试配置JSON读取
            var dpbManifest = SerializerHelper.GetObject<Manifest>(json);
            Console.WriteLine(dpbManifest.ToJson());
        }

        [TestMethod]
        public void BuildTest()
        {
            var sourceDir = "..\\..\\SourceDir";//or absolute address: e:\ThisProject\src
            var outputDir = "..\\..\\OutputDir";//or absolute address: e:\ThisProject\Output
            Manifest manifest = new Manifest(sourceDir, outputDir);

            #region 内容保留或文件保留

            // 保留内容条件 - 当*.cs文件中使用注释关键字DPBMARK CYSOFT标记代码块时（大小写敏感）
            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "*.cs" },
                OmitChangeFiles = new List<string>() { "*-Keep.cs" },
                KeepContentConiditions = new List<string>() { "CYSOFT" }
            });

            //keep files Condition - Keep
            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "*.txt" },
                KeepFileConiditions = new List<string>() { "Keep" },
                //KeepContentConiditions= new List<string>() { "o","k"}
            });

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "KeepPartsOfContent.txt" },
                KeepFileConiditions = new List<string>() { "KeepPartsOfContent" },
                KeepContentConiditions = new List<string>() { "KeepPartsOfContent" },
            });

            #endregion 内容保留或文件保留

            #region 替换字符串内容

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "StringReplaceFile.txt", "RegexReplaceFile.txt" },
                ReplaceContents = new List<ReplaceContent>() {
                    // 匹配替换
                     new ReplaceContent(){
                             StringContent=new StringContent(){
                                 String="<This conent will be replaced by StringContent>",
                                  ReplaceContent="[This is new content, replaced by StringContent]"
                             }
                     },
                     // 正则替换
                     new ReplaceContent(){
                          RegexContent = new RegexContent(){
                                  Pattern = @"\<[^\>]*\>",
                                  ReplaceContent="[This is new content, replaced by ReplaceContent]",
                                   RegexOptions = RegexOptions.IgnoreCase
                          }
                     }
                 }
            });

            #endregion 替换字符串内容

            #region 修改xml节点值

            var pathConfigXml = new GroupConfig()
            {
                Files = new List<string>() { "*.xml" }
            };
            pathConfigXml.ReplaceContents.Add(new ReplaceContent()
            {
                XmlContent = new XmlContent()
                {
                    TagName = "ProjectName",
                    ReplaceContent = "This is the new value"
                }
            });
            pathConfigXml.ReplaceContents.Add(new ReplaceContent()
            {
                XmlContent = new XmlContent()
                {
                    TagName = "Count",
                    ReplaceContent = "666"
                }
            });
            manifest.ConfigGroup.Add(pathConfigXml);

            #endregion 修改xml节点值

            #region 修改json节点值

            var pathConfigJson = new GroupConfig()
            {
                Files = new List<string>() { "*.json" }
            };

            pathConfigJson.ReplaceContents.Add(new ReplaceContent()
            {
                JsonContent = new JsonContent()
                {
                    KeyName = "version",
                    ReplaceContent = "6.6.6.6"
                }
            });
            manifest.ConfigGroup.Add(pathConfigJson);

            #endregion 修改json节点值

            #region 删除文件

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "FileRemove*.txt" },
                OmitChangeFiles = new List<string>() { "FileRemoveOmit.txt" },
                RemoveFiles = true
            });

            #endregion 删除文件

            #region 删除目录

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                RemoveDictionaries = new List<string>() {
                    "ChildrenDirectoriesWillBeRemoved\\Remove1",
                    "ChildrenDirectoriesWillBeRemoved\\Remove2"
              }
            });

            #endregion 删除目录

            #region 自定义

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "CustomFunctionFile1.txt" },
                CustomFunc = (fileName, fileContent) => fileContent.ToUpper() + $"{Environment.NewLine}FileName:{fileName} - {DateTime.Now}"// all letters ToUpper(), or do anythiny you like
            });

            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "CustomFunctionFile2-net45-csproj.xml" },
                KeepContentConiditions = new List<string>() { "MP", "Redis" },
                CustomFunc = (fileName, fileContent) =>
                {
                    XDocument d = XDocument.Parse(fileContent);
                    XNamespace dc = d.Root.Name.Namespace;
                    var xmlNamespace = dc.ToString();

                    d.Root.Elements(dc + "ItemGroup").ToList()
                            .ForEach(z => z.Elements(dc + "ProjectReference")
                                           .Where(el => !el.ToString().Contains("CommonService"))
                                           .Remove());

                    //add each nuget packages
                    var newItemGroup = new XElement(dc + "ItemGroup");
                    d.Root.Add(newItemGroup);

                    var newElement = new XElement(dc + "PackageReference");
                    newElement.Add(new XAttribute("Include", "NEW_PACKAGE"));
                    newElement.Add(new XAttribute("Version", "NEW_PACKAGE_VERSION"));
                    newItemGroup.Add(newElement);
                    return d.ToString();
                }
            });

            #endregion 自定义

            //.net core file test
            manifest.ConfigGroup.Add(new GroupConfig()
            {
                Files = new List<string>() { "Startup.cs.txt" },
                KeepContentConiditions = new List<string>() { "MP", "Redis" }
            });

            var json_manifest = manifest.ToJson();
            Console.WriteLine(json_manifest);

            LetsGo letsGo = new LetsGo(manifest);
            letsGo.Build();
        }
    }
}
