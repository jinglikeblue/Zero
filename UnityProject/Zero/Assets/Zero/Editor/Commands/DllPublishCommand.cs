using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Jing;

namespace Zero.Edit
{
    public class DllPublishCommand
    {
        public event Action<string> onComplete;
        string _projContent;
        string _releaseDir;

        public void Execute()
        {
            var cfg = new DllPublishConfigModel();

            _projContent = File.ReadAllText(cfg.VO.ilProjPath);

            SetDllReleseDir(cfg.VO);

            Process p = new Process();
            p.StartInfo.FileName = cfg.VO.devenvPath;// @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv";  //确定程序名
            p.StartInfo.Arguments = string.Format("\"{0}\" /Rebuild \"Release|AnyCPU\"", cfg.VO.ilProjPath);// @"""E:\projects\unity\Zero\UnityProject\ZeroIL\ZeroIL\ZeroIL.csproj"" /build";  //指定程式命令行
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
        private void SetDllReleseDir(DllPublishConfigVO vo)
        {
            _releaseDir = FileSystem.CombineDirs(true, vo.resDir, ZeroEditorUtil.PlatformDirName, "dll");
            _releaseDir = new DirectoryInfo(_releaseDir).FullName;                        

            int searchIdx = 0;
            do
            {
                searchIdx = RepleaceOutputPath(_releaseDir, searchIdx);
            }
            while (searchIdx > -1);

            File.WriteAllText(vo.ilProjPath, _projContent);
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
            if(startIdx == -1)
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