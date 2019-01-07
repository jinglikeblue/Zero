using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Zero.Edit
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
            var files = Directory.GetFiles(dir);
            foreach(var file in files)
            {
                CheckFile(file);
            }


            var subDirs = Directory.GetDirectories(dir);
            foreach(var subDir in subDirs)
            {
                ScanningDir(subDir);
            }
        }

        void CheckFile(string file)
        {
            file = FileSystem.StandardizeSeparator(file);
            file = file.Replace(Application.dataPath + "/", "");
            var ai = AssetImporter.GetAtPath(FileSystem.CombinePaths("Assets",file));
            if(null == ai)
            {
                return;
            }

            var ti = ai as TextureImporter;
            if(null == ti)
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
