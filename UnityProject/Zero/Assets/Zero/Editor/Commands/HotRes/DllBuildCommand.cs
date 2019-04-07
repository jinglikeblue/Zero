using Jing;
using System;
using System.Diagnostics;
using System.IO;

namespace Zero.Edit
{
    class DllBuildCommand
    {
        public event Action<string> onComplete;

        string _resDir;
        string _devenvPath;
        string _csprojPath;

        string _projContent;
        string _releaseDir;

        public DllBuildCommand(string resDir, string devenvPath, string csprojPath)
        {
            _resDir = resDir;
            _devenvPath = devenvPath;
            _csprojPath = csprojPath;
        }


        public void Execute()
        {
            _projContent = File.ReadAllText(_csprojPath);

            SetDllReleseDir();

            Process p = new Process();
            p.StartInfo.FileName = _devenvPath;// @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv";  //确定程序名
            p.StartInfo.Arguments = string.Format("\"{0}\" /Rebuild \"Release|AnyCPU\"", _csprojPath);// @"""E:\projects\unity\Zero\UnityProject\ZeroIL\ZeroIL\ZeroIL.csproj"" /build";  //指定程式命令行
            //p.StartInfo.UseShellExecute = false;   //是否使用Shell
            //p.StartInfo.RedirectStandardInput = true;   //重定向输入
            //p.StartInfo.RedirectStandardOutput = true;   //重定向输出            
            //p.StartInfo.RedirectStandardError = true;    //重定向输出错误
            //p.StartInfo.CreateNoWindow = true;        //设置不显示窗口            
            p.Start();
            //锁死线程等待命名执行完毕
            p.WaitForExit();
            if (null != onComplete)
            {
                onComplete.Invoke(_releaseDir);
            }
        }

        /// <summary>
        /// 设置DLL文件发布的目录
        /// </summary>
        /// <param name="vo"></param>
        private void SetDllReleseDir()
        {
            _releaseDir = FileSystem.CombineDirs(true, _resDir, ZeroEditorUtil.PlatformDirName, ZeroEditorUtil.DLL_DIR);

            if (false == Directory.Exists(_releaseDir))
            {
                Directory.CreateDirectory(_releaseDir);
            }

            _releaseDir = new DirectoryInfo(_releaseDir).FullName;

            int searchIdx = 0;
            do
            {
                searchIdx = RepleaceOutputPath(_releaseDir, searchIdx);
            }
            while (searchIdx > -1);

            File.WriteAllText(_csprojPath, _projContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="releaseDir"></param>
        /// <param name="startIdx"></param>
        /// <returns></returns>
        int RepleaceOutputPath(string releaseDir, int searchIdx)
        {
            const string NODE_START = "<OutputPath>";
            const string NODE_END = "</OutputPath>";
            int startIdx = _projContent.IndexOf(NODE_START, searchIdx);
            if (startIdx == -1)
            {
                return -1;
            }
            startIdx += NODE_START.Length;
            int endIdx = _projContent.IndexOf(NODE_END, searchIdx);

            string part1 = _projContent.Substring(0, startIdx);
            string part2 = _projContent.Substring(endIdx);

            _projContent = part1 + releaseDir + part2;

            return endIdx + NODE_END.Length;
        }
    }
}
