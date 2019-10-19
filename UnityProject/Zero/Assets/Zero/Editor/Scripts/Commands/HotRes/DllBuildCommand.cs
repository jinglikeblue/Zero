using Jing;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Zero.Edit
{
    public class DllBuildCommand 
    {
        /// <summary>
        /// 打包完成，返回一个bool表示成功还是失败
        /// </summary>
        public event Action<DllBuildCommand, bool> onFinished;

        string _sourcesDir;

        string _outputDir;

        string _outputAssemblyPath;

        public DllBuildCommand(string sourcesDir, string outputDir)
        {
            _sourcesDir = sourcesDir;
            _outputDir = outputDir;
            if (false == Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            _outputAssemblyPath = FileSystem.CombinePaths(outputDir, ZeroConst.DLL_FILE_NAME + ".dll");
        }

        public void Execute()
        {
            var scriptPaths = Directory.GetFiles(_sourcesDir, "*.cs", SearchOption.AllDirectories);
            var ab = new AssemblyBuilder(_outputAssemblyPath, scriptPaths);
            ab.compilerOptions = new ScriptCompilerOptions();            
            ab.additionalReferences = GetDepends();            
            ab.buildFinished += OnFinished;
            if (false == ab.Build())
            {
                onFinished?.Invoke(this, false);
                onFinished = null;
            }
        }

        string[] GetDepends()
        {
            //依赖Assets下的DLL
            var assetDir = Application.dataPath;
            var dllList0 = Directory.GetFiles(assetDir, "*.dll", SearchOption.AllDirectories);

            //依赖Library/ScriptAssemblies下的DLL
            var projectDir = Directory.GetParent(assetDir).FullName;
            var dllList1 = Directory.GetFiles(FileSystem.CombineDirs(true, projectDir, "Library", "ScriptAssemblies"), "*.dll", SearchOption.AllDirectories);

            //依赖Unity安装目录下的DLL
            var dir = FileSystem.CombineDirs(true, EditorApplication.applicationContentsPath, "Managed", "UnityEngine");
            var dllList2 = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);

            string[] depends = new string[dllList0.Length + dllList1.Length + dllList2.Length];
            Array.Copy(dllList0, 0, depends, 0, dllList0.Length);
            Array.Copy(dllList1, 0, depends, dllList0.Length, dllList1.Length);
            Array.Copy(dllList2, 0, depends, dllList0.Length + dllList1.Length, dllList2.Length);
            return depends;
        }

        private void OnFinished(string path, CompilerMessage[] msgs)
        {            
            bool isFail = false;
            foreach (var msg in msgs)
            {
                if (msg.type == CompilerMessageType.Error)
                {
                    Debug.LogError(msg.message); 
                    isFail = true;
                }
            }

            if (isFail)
            {                
                onFinished?.Invoke(this, false);
            }
            else
            {               
                onFinished?.Invoke(this, true);
            }

            onFinished = null;
        }
    }
}