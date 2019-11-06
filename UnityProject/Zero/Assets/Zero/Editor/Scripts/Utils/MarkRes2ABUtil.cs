using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace ZeroEditor
{
    /// <summary>
    /// 将资源打上AB标记
    /// </summary>
    class MarkRes2ABUtil
    {
        /// <summary>
        /// 将制定目录中的所有资源打上AB标记
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="assetBundleVariant"></param>
        public static void Mark(string dir, string assetBundleName, string assetBundleVariant)
        {
            if (null == assetBundleName)
            {
                assetBundleName = "";
            }

            if (null == assetBundleVariant)
            {
                assetBundleVariant = "";
            }

            MarkDeep(dir, assetBundleName, assetBundleVariant);
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        static void MarkDeep(string dir, string assetBundleName, string assetBundleVariant)
        {
            var path = FileSystem.StandardizeBackslashSeparator(dir);
            var ai = AssetImporter.GetAtPath(path);

            if (null == ai)
            {
                return;
            }

            if (Directory.Exists(dir))
            {
                //目录不设置AB名
                ai.SetAssetBundleNameAndVariant("", "");

                //是目录
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {                    
                    MarkDeep(file, assetBundleName, assetBundleVariant);
                }


                var dirs = Directory.GetDirectories(dir);
                foreach (var subDir in dirs)
                {
                    MarkDeep(subDir, assetBundleName, assetBundleVariant);
                }
            }
            else
            {
                ai.SetAssetBundleNameAndVariant(assetBundleName, assetBundleVariant);
            }
        }
    }
}
