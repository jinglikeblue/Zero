using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 位图字体创建，菜单
    /// </summary>
    public class BitmapFontCreaterMenu
    {
        /// <summary>
        /// 直接用当前目录下的资源创建字体
        /// 文件名
        /// </summary>
        //[MenuItem("Assets/Zero/Create Bitmap Font (Direct)/Use「PNG File Name」", false, 0)]
        public static void CreateBitmapFontUsePNGFileName()
        {
            if (Selection.objects.Length != 1)
            {
                Debug.LogError("仅针对文件夹进行操作!");
                return;
            }

            var dirObj = Selection.objects[0];
            var path = AssetDatabase.GetAssetPath(dirObj);
            if (false == Directory.Exists(path))
            {
                Debug.LogError("选中的并不是一个文件夹!");
                return;
            }

            //找到所有的图片
            var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);

            List<Texture2D> textures = new List<Texture2D>();
            List<char> chars = new List<char>();
            for (var i = 0; i < files.Length; i++)
            {
                var nameChars = Path.GetFileNameWithoutExtension(files[i]).ToCharArray();
                if (nameChars.Length != 1)
                {
                    Debug.LogErrorFormat("文件[{0}]被跳过，因为他的文件名不是单字符", files[i]);
                    continue;
                }

                textures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(files[i]));
                chars.Add(nameChars[0]);
            }

            if(textures.Count == 0)
            {
                return;
            }

            new BitmapFontCreateCommand(textures.ToArray(), chars.ToArray(), path, dirObj.name).Execute();
        }

        /// <summary>
        /// 直接用当前目录下的资源创建字体
        /// </summary>
        //[MenuItem("Assets/Zero/Create Bitmap Font (Direct)/Use「chars.txt」", false, 0)]
        public static void CreateBitmapFontUseCharsTxt()
        {
            if (Selection.objects.Length != 1)
            {
                Debug.LogError("仅针对文件夹进行操作!");
                return;
            }

            var dirObj = Selection.objects[0];
            var path = AssetDatabase.GetAssetPath(dirObj);
            if (false == Directory.Exists(path))
            {
                Debug.LogError("选中的并不是一个文件夹!");
                return;
            }

            var charsTxtFile = Path.Combine(path, "chars.txt");
            if (false == File.Exists(charsTxtFile))
            {
                Debug.LogErrorFormat("文件[{0}]不存在!", charsTxtFile);
                return;
            }

            string charsContent = File.ReadAllText(charsTxtFile);
            char[] chars = charsContent.ToCharArray();

            //找到所有的图片
            var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);

            if(chars.Length != files.Length)
            {
                Debug.LogErrorFormat("PNG文件数量({0})和字符数量({1})不一致，请确定两者一致避免出错!", files.Length, chars.Length);
                return;
            }

            Texture2D[] textures = new Texture2D[files.Length];

            for(var i = 0; i < files.Length; i++)
            {
                textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(files[i]);
            }       
            
            if(textures.Length == 0)
            {
                return;
            }

            new BitmapFontCreateCommand(textures, chars, path, dirObj.name).Execute();
        }

        /// <summary>
        /// 直接
        /// </summary>
        //[MenuItem("Assets/Zero/Create Bitmap Font (GUI)", false, 1)]
        public static void CreateBitmapFontGUI()
        {            
            var editorWin = BitmapFontCreateEditorWindow.Open();

            if (Selection.objects.Length > 1 || Selection.objects[0] is Texture2D)
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj is Texture2D)
                    {
                        editorWin.textures.Add(obj as Texture2D);
                    }
                }
            }
            else if(Selection.objects.Length == 1)
            {
                var obj = Selection.objects[0];
                var path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path))
                {
                    editorWin.outputPath = path;
                    editorWin.fontName = obj.name;
                    //找到所有的图片
                    var files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);                                        
                    List<char> chars = new List<char>();
                    for (var i = 0; i < files.Length; i++)
                    {
                        var nameChars = Path.GetFileNameWithoutExtension(files[i]).ToCharArray();
                        if (nameChars.Length == 1)
                        {
                            chars.Add(nameChars[0]);
                        }
                        editorWin.textures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(files[i]));
                    }


                    var charsTxtFile = Path.Combine(path, "chars.txt");
                    if (File.Exists(charsTxtFile))
                    {
                        //如果目录中有「chars.txt」文件，则提取字符填入   
                        string charsContent = File.ReadAllText(charsTxtFile);
                        editorWin.charContent = charsContent;                        
                    }
                    else
                    {
                        editorWin.charContent = new string(chars.ToArray());                                                
                    }
                }
            }          
        }

        //[MenuItem("Tools/Zero/Create Bitmap Font", false, 1)]
        public static void CreateBitmapFontGUITools()
        {
            BitmapFontCreateEditorWindow.Open();
        }
    }
}