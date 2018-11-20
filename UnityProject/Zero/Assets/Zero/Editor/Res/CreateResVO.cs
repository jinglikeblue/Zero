using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
{
    /// <summary>
    /// 创建资源文件
    /// </summary>
    public class CreateResVO 
    {
        string _dir;

        ResVerVO res;

        List<string> _files;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resDir"></param>
        /// <param name="fileName"></param>
        public CreateResVO(string resDir)
        {
            _dir = resDir;
            _files = new List<string>();
        }

        public ResVerVO Start()
        {
            EditorUtility.DisplayProgressBar("正在生成 res.json", "即将开始", 0f);

            _files.Clear();

            ScanningFiles(_dir);

            ResVerVO.Item[] items = new ResVerVO.Item[_files.Count];
            for(int i = 0; i < items.Length; i++)
            {
               var file = _files[i];

                EditorUtility.DisplayProgressBar("正在生成 res.json", string.Format("文件:{0}",file), ((float)i / items.Length));

                //是文件
                ResVerVO.Item item;
                item.name = file.Replace(_dir, "").Replace("\\", "/");
                item.version = GetMD5(file);
                //item.depends = GetDepends(item.name);
                items[i] = item;
            }

            res.items = items;

            EditorUtility.ClearProgressBar();
            return res;
        }

        //string[] GetDepends(string file)
        //{
        //    var abDir = ABEditorWin.AB_DIR + "/";
        //    if(!file.StartsWith(abDir))
        //    {
        //        return new string[0];
        //    }

        //    file = file.Replace(abDir, "");
        //    string[] depends = _manifest.GetAllDependencies(file);
        //    for(int i = 0; i < depends.Length; i++)
        //    {
        //        //给取到的依赖再加上AB资源目录前缀
        //        depends[i] = abDir + depends[i];
        //    }
        //    return depends;
        //}

        void ScanningFiles(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            _files.AddRange(files);

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (var subDir in subDirs)
            {
                //是目录
                ScanningFiles(subDir);
            }
        }


        string GetMD5(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(fs);            
            fs.Close();
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < bytes.Length; i++)
            {
                strbul.Append(bytes[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }
    }
}