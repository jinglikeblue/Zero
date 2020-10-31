using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 位图字创建GUI界面
    /// </summary>
    public class BitmapFontCreateEditorWindow : OdinEditorWindow
    {
        /// <summary>
        /// 打开
        /// </summary>
        public static BitmapFontCreateEditorWindow Open()
        {
            return GetWindow<BitmapFontCreateEditorWindow>("Bitmap Font Creater");
        }

        [PropertySpace(10)]
        [PropertyOrder(-2)]
        [Button(ButtonSizes.Large), LabelText("生成"), GUIColor(0, 1, 0)]
        void Build()
        {
            var chars = charContent.Replace("\n", "");

            if (textures.Count == 0)
            {
                Debug.LogErrorFormat("PNG数量为0!");
                return;
            }

            if (chars.Length == 0)
            {
                Debug.LogErrorFormat("字符数量为0!");
                return;
            }

            if (textures.Count != chars.Length)
            {
                Debug.LogErrorFormat("PNG数量和字符数量不一致!");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Debug.LogErrorFormat("输出路径未选择!");
                return;
            }

            if (string.IsNullOrWhiteSpace(fontName))
            {
                Debug.LogErrorFormat("输出字体名称未选择!");
                return;
            }

            new BitmapFontCreateCommand(textures.ToArray(), chars.ToCharArray(), outputPath, fontName).Execute();
        }

        [HideInInspector]
        public string outputPath;

        [Title("输出路径")]
        [Button(ButtonSizes.Medium), LabelText("@GetOutputPath()")]
        [PropertyOrder(-1)]
        void SelectOutputPath()
        {
            outputPath = EditorUtility.OpenFolderPanel("字体输出路径", Application.dataPath, "");
            if (false == string.IsNullOrEmpty(outputPath))
            {
                outputPath = outputPath.Replace(Application.dataPath, "Assets") + "/";
            }
        }

        [Title("字符内容")]
        [HideLabel]
        [TextArea(1, 5)]
        public string charContent;

        [Title("字符名称")]
        [HideLabel]
        public string fontName;

        [Title("字体图片")]           
        public List<Texture2D> textures = new List<Texture2D>();

        string GetOutputPath()
        {
            return string.IsNullOrEmpty(outputPath) ? "选择输出路径" : outputPath;
        }
    }
}