using System;
using System.IO;

namespace UnityCSProjectParser
{
    public class UnitySolution{

        public UnityCSProject AssemblyCSharp;
        public UnityCSProject AssemblyCSharpEditor;
        private UnitySolution(){

        }

        public static UnitySolution Parse(string unitypath){

            //check unity project;
            if(string.IsNullOrEmpty(unitypath)){
                return null;
            }

            DirectoryInfo dirinfo = new DirectoryInfo(unitypath);
            if(!dirinfo.Exists) return null;

            var assetpath = Path.Combine(dirinfo.FullName,"Assets");
            if(!Directory.Exists(assetpath)){
                Console.WriteLine("Invalid Unity project path");
                return null;
            }

            var csprojPath = Path.Combine(dirinfo.FullName,"Assembly-CSharp.csproj");
            var csprojPathEditor = Path.Combine(dirinfo.FullName,"Assembly-CSharp-Editor.csproj");

            if(!File.Exists(csprojPath)){
                Console.WriteLine("Missing Assembly-CSharp.csproj");
                return null;
            }
            if(!File.Exists(csprojPathEditor)){
                Console.WriteLine("Missing Assembly-CSharp-Editor.csproj");
            }

            var solution = new UnitySolution();
            solution.AssemblyCSharp = UnityCSProject.Parse(csprojPath);
            solution.AssemblyCSharpEditor = UnityCSProject.Parse(csprojPathEditor);
            
            return solution;
        }
    }
}