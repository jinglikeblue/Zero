using System;
using System.IO;

namespace Jing
{
    /// <summary>
    /// 文件处理工具
    /// </summary>
    public class FileSystem
    {       
        /// <summary>
        /// 标准化路径中的路径分隔符（统一使用“/”符号）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StandardizeBackslashSeparator(string path)
        {
            path = path.Replace("\\", "/");
            return path;
        }

        /// <summary>
        /// 标准化路径中的路径分隔符（统一使用“\”符号）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StandardizeSlashSeparator(string path)
        {
            path = path.Replace("/", "\\");
            return path;
        }

        /// <summary>
        /// 删除目录下使用指定扩展名的文件
        /// </summary>
        /// <param name="dirPath">目录地址</param>
        /// <param name="ext">扩展名 格式可以为[exe]或[.exe]</param>
        public static void DeleteFilesByExt(string dirPath, string ext)
        {
            if (false == ext.StartsWith("."))
            {
                ext = "." + ext;
            }

            string[] dirs = Directory.GetDirectories(dirPath);
            foreach (string dir in dirs)
            {
                DeleteFilesByExt(dir, ext);
            }

            string[] files = Directory.GetFiles(dirPath);
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    if (Path.GetExtension(file) == ext)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        /// <summary>
        /// 将给的路径合并起来
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CombinePaths(params string[] args)
        {
            if (args.Length == 0)
            {
                return "";
            }                              

            string path = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                var node = RemoveStartPathSeparator(args[i]);
                path = Path.Combine(path, node);
            }

            //为了好看
            path = StandardizeBackslashSeparator(path);

            return path;
        }

        /// <summary>
        /// 将给的目录路径合并起来
        /// </summary>
        /// <param name="endWithBackslash">路径最后是否以反斜杠结束</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string CombineDirs(bool isEndWithBackslash, params string[] args)
        {
            string path = CombinePaths(args);

            if(isEndWithBackslash)
            {
                if (false == path.EndsWith("/"))
                {
                    path += "/";
                }
            }
            else
            {
                if(path.EndsWith("/"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }
            
            return path;
        }

        /// <summary>
        /// 如果路径开头有文件分隔符，则移除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveStartPathSeparator(string path)
        {
            if(path.StartsWith("/"))
            {
                return path.Substring(1);
            }
            else if(path.StartsWith("\\"))
            {
                return path.Substring(2);
            }

            return path;
        }

        /// <summary>
        /// 如果路径结尾有文件分隔符，则移除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveEndPathSeparator(string path)
        {
            if (path.EndsWith("/"))
            {
                return path.Substring(1);
            }
            else if (path.EndsWith("\\"))
            {
                return path.Substring(2);
            }

            return path;
        }

        /// <summary>
        /// 将源目录或文件拷贝到目标地址。拷贝过程中如果目录不存在，则创建。
        /// </summary>
        /// <param name="source">源目录或文件</param>
        /// <param name="target">目标目录或文件</param>
        /// <param name="overwrite">有相同的文件是否覆盖</param>
        /// <param name="extFilters">要过滤(不拷贝)的文件后缀名</param>
        /// <returns></returns>
        public static void Copy(string source, string target, bool overwrite, string[] extFilters = null)
        {
            source = StandardizeBackslashSeparator(source);
            target = StandardizeBackslashSeparator(target);
            if (File.Exists(source))
            {
                //拷贝文件
                CopyFile(source, target, overwrite);
            }
            else
            {
                var subFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
                for (int i = 0; i < subFiles.Length; i++)
                {
                    var subFile = StandardizeBackslashSeparator(subFiles[i]);
                    var subFileRelativePath = subFile.Replace(source, "");
                    var targetSubFile = CombinePaths(target, subFileRelativePath);
                    CopyFile(subFile, targetSubFile, true, extFilters);
                }
            }
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="overwrite"></param>
        static void CopyFile(string source, string target, bool overwrite, string[] extFilters = null)
        {
            if (false == File.Exists(source))
            {
                throw new Exception(string.Format("文件不存在:[{0}]", source));
            }

            if (null != extFilters)
            {
                var ext = Path.GetExtension(source);
                foreach(var extFilter in extFilters)
                {
                    if (ext.Equals(extFilter))
                    {
                        return;
                    }
                }
            }

            var targetDir = Directory.GetParent(target);
            if (false == targetDir.Exists)
            {
                targetDir.Create();
            }

            File.Copy(source, target, overwrite);
        }        
    }
}