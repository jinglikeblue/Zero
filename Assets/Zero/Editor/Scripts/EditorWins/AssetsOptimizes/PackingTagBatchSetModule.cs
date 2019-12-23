using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class PackingTagBatchSetModule : AEditorModule
    {
        public PackingTagBatchSetModule(EditorWindow editorWin) : base(editorWin)
        {

        }

        public void SetAssetPaths(List<string> assetPaths)
        {
            taList.Clear();
            foreach (var assetPath in assetPaths)
            {
                if (false == AddAssetPath(assetPath))
                {
                    if (Directory.Exists(assetPath))
                    {
                        //是个目录
                        string[] files;
                        files = Directory.GetFiles(assetPath, "*", isDeepScan ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            AddAssetPath(file);
                        }
                    }
                }
            }
        }

        bool AddAssetPath(string assetPath)
        {
            var ai = AssetImporter.GetAtPath(assetPath);
            if (ai is TextureImporter)
            {
                var ti = ai as TextureImporter;
                var ta = new TextureAssetVO();
                ta.assetPath = ti.assetPath;
                ta.ti = ti;
                taList.Add(ta);
                return true;
            }
            return false;
        }

        bool IsSomePackingTagDifferent()
        {
            if (taList.Count > 2)
            {
                string packingTag = taList[0].packingTag;
                foreach (var ta in taList)
                {
                    if (ta.packingTag != packingTag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HorizontalGroup("Scan", order:-999)]
        [Button(size:ButtonSizes.Medium), LabelText("显示选中内容的Packing Tag"), PropertyOrder(1)]
        void ScanPackingTags()
        {
            List<string> assetPaths = new List<string>();

            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                assetPaths.Add(AssetDatabase.GetAssetPath(obj));
            }

            SetAssetPaths(assetPaths);
        }

        [HorizontalGroup("Scan",MaxWidth =200)]
        [LabelText("是否包含子目录"),LabelWidth(100), PropertyOrder(0), ToggleLeft]
        public bool isDeepScan = false;
        

        [Title("设置Packing Tag")]
        [LabelText("批量设置"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void BatchSet()
        {
            foreach (var ta in taList)
            {
                ta.ti.spritePackingTag = packingTagName;
                ta.ti.SaveAndReimport();
            }

            AssetDatabase.Refresh();
        }

        [LabelText("要设置的PackingTag名称")]
        [Space(10)]
        public string packingTagName;

        [Space(10)]
        [InfoBox("当前列表中的资源，Packing Tag不是统一的", infoMessageType: InfoMessageType.Warning, VisibleIf = "IsSomePackingTagDifferent")]
        [PropertyOrder(999)]
        [HideReferenceObjectPicker]
        [TableList(IsReadOnly = true, ShowPaging = true, NumberOfItemsPerPage = 21, AlwaysExpanded = true)]
        [LabelText("可设置资源")]
        [ShowInInspector]
        public List<TextureAssetVO> taList = new List<TextureAssetVO>();

        public class TextureAssetVO
        {
            [TableColumnWidth(600, true)]
            [DisplayAsString]
            public string assetPath;

            [TableColumnWidth(100)]
            [ShowInInspector]
            [DisplayAsString]
            public string packingTag
            {
                get
                {
                    return ti.spritePackingTag;
                }
                set
                {
                    ti.spritePackingTag = value;
                }
            }

            [TableColumnWidth(100)]
            [PropertyOrder(100)]
            public TextureImporter ti;
        }
    }
}
