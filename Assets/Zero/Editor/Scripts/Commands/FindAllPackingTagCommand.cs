using Jing;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    class FindAllPackingTagCommand
    {
        Dictionary<string, List<TextureImporter>> _ptData;
        public Dictionary<string, List<TextureImporter>> Excute()
        {          
            _ptData = new Dictionary<string, List<TextureImporter>>();
            ScanningDir(Application.dataPath);            
            return _ptData;
        }

        void ScanningDir(string dir)
        {
            var files = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            float total = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var title = string.Format("扫描中...({0}/{1})", i + 1, files.Length);
                EditorUtility.DisplayProgressBar(title, file, i / total);
                CheckFile(file);
            }
            EditorUtility.ClearProgressBar();
        }

        void CheckFile(string file)
        {
            file = FileSystem.StandardizeBackslashSeparator(file);
            file = file.Replace(Application.dataPath + "/", "");
            var ai = AssetImporter.GetAtPath(FileSystem.CombinePaths("Assets",file));
            if(null == ai)
            {
                return;
            }

            var ti = ai as TextureImporter;
            if(null == ti || ti.textureType != TextureImporterType.Sprite)
            {
                return;
            }

            var ptName = ti.spritePackingTag.Trim();
            if (ptName != string.Empty)
            {
                if(_ptData.ContainsKey(ptName) == false)
                {
                    _ptData[ptName] = new List<TextureImporter>();
                }
                _ptData[ptName].Add(ti);
            }
        }
    }
}
