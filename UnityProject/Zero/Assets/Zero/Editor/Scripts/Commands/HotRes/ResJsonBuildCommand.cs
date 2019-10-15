using Jing;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;

namespace Zero.Edit
{
    /// <summary>
    /// 版本号文件生成命令。针对目标目录中的所有文件生成一个res.json文件，描述其MD5以及文件大小
    /// </summary>
    class ResJsonBuildCommand
    {
        string _sourceDir;

        ResVerVO _res;

        List<string> _files;

        public ResJsonBuildCommand(string sourceDir)
        {
            _sourceDir = FileSystem.CombineDirs(true, sourceDir);
        }

        public void Execute()
        {            
            if (false == Directory.Exists(_sourceDir))
            {
                EditorUtility.DisplayDialog("错误", "目标目录并不存在: " + _sourceDir, "确定");
                return;
            }

            var filePath = FileSystem.CombinePaths(_sourceDir, ZeroConst.RES_JSON_FILE_NAME);

            //首先删除旧的
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            BuildResJson();          
            string jsonStr = LitJson.JsonMapper.ToPrettyJson(_res);
            File.WriteAllText(filePath, jsonStr);
        }

        void BuildResJson()
        {
            _files = new List<string>();

            EditorUtility.DisplayProgressBar("正在生成 " + ZeroConst.RES_JSON_FILE_NAME, "即将开始", 0f);

            _files.Clear();

            ScanningFiles(_sourceDir);

            List<ResVerVO.Item> items = new List<ResVerVO.Item>();
            for (int i = 0; i < _files.Count; i++)
            {
                var file = _files[i];

                EditorUtility.DisplayProgressBar("正在生成 " + ZeroConst.RES_JSON_FILE_NAME, string.Format("文件:{0}", file), ((float)i / items.Count));
                FileInfo fi = new FileInfo(file);
                //是文件
                ResVerVO.Item item = new ResVerVO.Item();
                item.name = file.Replace(_sourceDir, "").Replace("\\", "/");
                item.version = GetMD5(file);
                item.size = fi.Length;
                items.Add(item);
            }

            _res.items = items.ToArray();

            EditorUtility.ClearProgressBar();
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
