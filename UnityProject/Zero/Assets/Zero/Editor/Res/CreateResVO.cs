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
        const string EXT_FILTERS = ".manifest";

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

            List<ResVerVO.Item> items = new List<ResVerVO.Item>();            
            for(int i = 0; i < _files.Count; i++)
            {
                var file = _files[i];                

                EditorUtility.DisplayProgressBar("正在生成 res.json", string.Format("文件:{0}",file), ((float)i / items.Count));
                FileInfo fi = new FileInfo(file);
                if(fi.Extension == EXT_FILTERS)
                {
                    continue;
                }

                //是文件
                ResVerVO.Item item = new ResVerVO.Item();
                item.name = file.Replace(_dir, "").Replace("\\", "/");
                item.version = GetMD5(file);
                item.size = fi.Length;
                items.Add(item);
            }

            res.items = items.ToArray();

            EditorUtility.ClearProgressBar();
            return res;
        }

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