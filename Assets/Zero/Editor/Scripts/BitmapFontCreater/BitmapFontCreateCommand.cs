using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 位图字体创建指令
    /// </summary>
    public class BitmapFontCreateCommand
    {
        public Texture2D[] Textures { get; private set; }
        public Rect[] Rects { get; private set; }
        public char[] Chars { get; private set; }
        public string OutputPath { get; private set; }
        public string FontName { get; private set; }

        public Texture TextureAtlas { get; private set; }

        public string FontSettingFile { get; private set; }
        public string TextureAtlasFile { get; private set; }
        public string MatFile { get; private set; }

        public BitmapFontCreateCommand(Texture2D[] textures, string charContent, string outputPath, string fontName)
        {
            Textures = textures;
            Chars = charContent.ToCharArray();
            OutputPath = outputPath;
            FontName = fontName;
            var outFileWithoutExt = Path.Combine(outputPath, fontName);
            FontSettingFile = outFileWithoutExt + ".fontsettings";
            TextureAtlasFile = outFileWithoutExt + ".png";
            MatFile = outFileWithoutExt + ".mat";
        }

        public BitmapFontCreateCommand(Texture2D[] textures, char[] chars, string outputPath, string fontName)
        {
            Textures = textures;
            Chars = chars;
            OutputPath = outputPath;
            FontName = fontName;
            var outFileWithoutExt = Path.Combine(outputPath, fontName);
            FontSettingFile = outFileWithoutExt + ".fontsettings";
            TextureAtlasFile = outFileWithoutExt + ".png";
            MatFile = outFileWithoutExt + ".mat";
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        public void Execute()
        {
            try
            {
                //删除旧文件
                DeleteOldFiles();
                //合并图集
                BuildTextureAtlas();
                //创建字体
                BuildFont();
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

            AssetDatabase.Refresh();
        }

        void DeleteOldFiles()
        {
            if (File.Exists(TextureAtlasFile))
            {
                File.Delete(TextureAtlasFile);
            }

            if (File.Exists(FontSettingFile))
            {
                File.Delete(FontSettingFile);
            }

            if (File.Exists(MatFile))
            {
                File.Delete(MatFile);
            }
        }

        void BuildFont()
        {
            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetTexture("_MainTex", TextureAtlas);
            Font font = new Font();
            font.material = mat;

            CharacterInfo[] characterInfos = new CharacterInfo[Rects.Length];

            float lineSpace = 0.1f;

            for (int i = 0; i < Rects.Length; i++)
            {
                if (Rects[i].height > lineSpace)
                {
                    lineSpace = Rects[i].height;
                }
            }

            for (int i = 0; i < Rects.Length; i++)
            {
                Rect rect = Rects[i];

                CharacterInfo info = new CharacterInfo();
                info.index = Chars[i];

                float pivot = -lineSpace / 2;
                //pivot = 0;
                int offsetY = (int)(pivot + (lineSpace - rect.height) / 2);
                info.uvBottomLeft = new Vector2((float)rect.x / TextureAtlas.width, (float)(rect.y) / TextureAtlas.height);
                info.uvBottomRight = new Vector2((float)(rect.x + rect.width) / TextureAtlas.width, (float)(rect.y) / TextureAtlas.height);
                info.uvTopLeft = new Vector2((float)rect.x / TextureAtlas.width, (float)(rect.y + rect.height) / TextureAtlas.height);
                info.uvTopRight = new Vector2((float)(rect.x + rect.width) / TextureAtlas.width, (float)(rect.y + rect.height) / TextureAtlas.height);
                info.minX = 0;
                info.minY = -(int)rect.height - offsetY;
                info.maxX = (int)rect.width;
                info.maxY = -offsetY;
                info.advance = (int)rect.width;
                characterInfos[i] = info;
            }

            font.characterInfo = characterInfos;            

            AssetDatabase.CreateAsset(mat, MatFile);
            AssetDatabase.CreateAsset(font, FontSettingFile);            
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
        }

        void BuildTextureAtlas()
        {
            foreach (var t in Textures)
            {
                var path = AssetDatabase.GetAssetPath(t);
                var ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (false == ti.isReadable)
                {
                    //修改为可读写，这样才能进行后面的打包
                    ti.isReadable = true;
                    ti.SaveAndReimport();
                }
            }

            var textureAtlas = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            Rects = textureAtlas.PackTextures(Textures, 0);

            //Debug.LogFormat("图集合并完成:");

            //将比例关系转换为像素值
            for (var i = 0; i < Rects.Length; i++)
            {
                var rect = Rects[i];
                rect.x = rect.x * textureAtlas.width;
                rect.width = rect.width * textureAtlas.width;
                rect.y = rect.y * textureAtlas.height;
                rect.height = rect.height * textureAtlas.height;
                Rects[i] = rect;

                //Debug.LogFormat("字符: {0}  区域: {1}", Chars[i], rect.ToString());
            }

            if (false == Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            //保存图
            var bytes = textureAtlas.EncodeToPNG();
            File.WriteAllBytes(TextureAtlasFile, bytes);
            AssetDatabase.Refresh();

            TextureAtlas = AssetDatabase.LoadAssetAtPath<Texture>(TextureAtlasFile);
        }
    }
}