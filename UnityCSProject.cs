using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace UnityCSProjectParser
{
    public class UnityCSProject
    {

        public PropertyGroup PropertyDebug;
        public PropertyGroup PropertyRelease;

        public string ProductVersion;
        public string SchemaVersion;
        public string AssemblyName;
        public string TargetFrameworkVersion;
        public string FileAlignment;
        public string BaseDirectory;
        public string OutputType;
        public string ProjectGuid;

        public List<CompileItem> CompileItems;
        public List<ReferenceItem> ReferenceItems;

        private UnityCSProject()
        {

        }

        public static UnityCSProject Parse(string path)
        {

            if (string.IsNullOrEmpty(path)) return null;
            if (!path.ToLower().EndsWith(".csproj")) return null;
            if (!File.Exists(path)) return null;

            XElement xproject = XElement.Load(path);
            string xns = xproject.Name.NamespaceName;

            UnityCSProject csproj = new UnityCSProject();

            try
            {
                //Compile files
                List<CompileItem> compileFiles = new List<CompileItem>();
                List<ReferenceItem> referenceItems = new List<ReferenceItem>();
                var itemgroups = xproject.Elements(XName.Get("ItemGroup", xns));
                foreach (var itemgroup in itemgroups)
                {
                    var compiles = itemgroup.Elements(XName.Get("Compile", xns));
                    foreach (var compile in compiles)
                    {
                        var compilePath = compile.Attribute(XName.Get("Include"));
                        if (compilePath == null) continue;
                        compileFiles.Add(new CompileItem(compilePath.Value));
                    }

                    var references = itemgroup.Elements(XName.Get("Reference", xns));
                    foreach (var reference in references)
                    {
                        var attrName = reference.Attribute(XName.Get("Include"))?.Value;
                        var hintPath = reference.Element(XName.Get("HintPath", xns))?.Value;
                        referenceItems.Add(new ReferenceItem(attrName, hintPath));
                    }
                }

                csproj.CompileItems = compileFiles;
                csproj.ReferenceItems = referenceItems;

                //PropettyGroup

                var propertyGroups = xproject.Elements(XName.Get("PropertyGroup", xns));
                foreach (var propertyGroup in propertyGroups)
                {
                    var condition = propertyGroup.Attribute(XName.Get("Condition"));
                    if (condition == null)
                    {
                        //parse main config
                        csproj.AssemblyName = propertyGroup.Element(XName.Get(nameof(AssemblyName), xns))?.Value;
                        csproj.BaseDirectory = propertyGroup.Element(XName.Get(nameof(BaseDirectory), xns))?.Value;
                        csproj.FileAlignment = propertyGroup.Element(XName.Get(nameof(FileAlignment), xns))?.Value;
                        csproj.OutputType = propertyGroup.Element(XName.Get(nameof(OutputType), xns))?.Value;
                        csproj.ProductVersion = propertyGroup.Element(XName.Get(nameof(ProductVersion), xns))?.Value;
                        csproj.ProjectGuid = propertyGroup.Element(XName.Get(nameof(ProjectGuid), xns))?.Value;
                        csproj.SchemaVersion = propertyGroup.Element(XName.Get(nameof(SchemaVersion), xns))?.Value;
                        csproj.TargetFrameworkVersion = propertyGroup.Element(XName.Get(nameof(TargetFrameworkVersion), xns))?.Value;
                    }
                    else
                    {
                        var conditionDesc = condition.Value;
                        if (conditionDesc.Contains("Debug"))
                        {
                            csproj.PropertyDebug = new PropertyGroup(propertyGroup, xns);
                        }
                        else if (conditionDesc.Contains("Release"))
                        {
                            csproj.PropertyRelease = new PropertyGroup(propertyGroup, xns);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }


            return csproj;
        }
    }

    public class CompileItem
    {
        public string Path;

        public CompileItem(string path)
        {
            this.Path = path;
        }
    }

    public class PropertyGroup
    {
        public string DebugSymbols;
        public string DebugType;
        public string Optimize;
        public string OutputPath;
        public string DefineConstants;
        public string ErrorReport;
        public string WarningLevel;
        public string AllowUnsafeBlocks;
        public PropertyGroup(XElement property, string xns)
        {
            DebugSymbols = property.Element(XName.Get(nameof(DebugSymbols), xns))?.Value;
            DebugType = property.Element(XName.Get(nameof(DebugType), xns))?.Value;
            Optimize = property.Element(XName.Get(nameof(Optimize), xns))?.Value;
            OutputPath = property.Element(XName.Get(nameof(OutputPath), xns))?.Value;
            DefineConstants = property.Element(XName.Get(nameof(DefineConstants), xns))?.Value;
            ErrorReport = property.Element(XName.Get(nameof(ErrorReport), xns))?.Value;
            WarningLevel = property.Element(XName.Get(nameof(WarningLevel), xns))?.Value;
            AllowUnsafeBlocks = property.Element(XName.Get(nameof(AllowUnsafeBlocks), xns))?.Value;
        }
    }

    public class ReferenceItem
    {
        public string Name;
        public string HintPath;

        public ReferenceItem(string name, string hintpath)
        {
            this.Name = name;
            this.HintPath = hintpath;
        }
    }
}
