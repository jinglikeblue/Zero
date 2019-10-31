using Jing;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class SpritePackingTagOptimizesModule : AEditorModule
    {
        public SpritePackingTagOptimizesModule(EditorWindow editorWin) : base(editorWin)
        {
        }

        [Title("Sprite Packing Tag 管理", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("扫描项目中的 Packing Tag 标记"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        void RefreshPackingTags()
        {
            var ptData = new FindAllPackingTagCommand().Excute();
            list.Clear();
            foreach (var data in ptData)
            {
                ItemVO vo = new ItemVO();
                vo.packingTag = data.Key;
                foreach (var ti in data.Value)
                {
                    vo.Add(ti);
                }
                list.Add(vo);
            }
        }

        [LabelText("删除选中的 Packing Tag 标记"), Button(size: ButtonSizes.Medium)]
        public void DeleteSelected()
        {
            List<ItemVO> delList = new List<ItemVO>();
            foreach (var vo in list)
            {
                if (vo.selected)
                {
                    delList.Add(vo);
                }
            }


            var selectCount = delList.Count;
            var processIdx = 0;
            foreach (var item in delList)
            {
                processIdx++;

                float total = item.spriteList.Count - 1;
                for (int i = 0; i < item.spriteList.Count; i++)
                {
                    var ti = item.spriteList[i].ti;
                    var title = string.Format("正在删除Packing Tag[{0}/{1}]: {2}({3}/{4})", processIdx, selectCount, item.packingTag, i + 1, item.spriteList.Count);
                    EditorUtility.DisplayProgressBar(title, ti.assetPath, i / total);
                    ti.spritePackingTag = string.Empty;
                    ti.SaveAndReimport();
                    list.Remove(item);
                }

            }

            EditorUtility.ClearProgressBar();
        }

        [LabelText("Packing Tag 列表"), ListDrawerSettings(Expanded = true, ShowPaging = false, IsReadOnly = true)]
        public List<ItemVO> list = new List<ItemVO>();


        [Serializable]
        public class ItemVO
        {
            [HideInEditorMode]
            public string packingTag;

            string PackingTagTitle()
            {
                return string.Format("Packing Tag: [{0}]", packingTag);
            }

            [HorizontalGroup("ItemVO")]
            [HideLabel]
            public bool selected = false;

            [HorizontalGroup("ItemVO")]
            [TableColumnWidth(70)]
            [LabelText("$PackingTagTitle"), ListDrawerSettings(IsReadOnly = true)]
            public List<Sprite> spriteList = new List<Sprite>();

            [Serializable]
            [HideLabel]
            public class Sprite
            {
                [HorizontalGroup("Sprite")]
                [HideLabel, DisplayAsString]
                public string name;

                [HideInEditorMode]
                public TextureImporter ti;

                [HorizontalGroup("Sprite", width: 60)]
                [Button, LabelText("Select"), LabelWidth(60)]
                void Select()
                {
                    Selection.objects = new UnityEngine.Object[] { ti };
                }
            }

            public void Add(TextureImporter ti)
            {
                var spr = new Sprite();
                spr.name = ti.assetPath;
                spr.ti = ti;
                spriteList.Add(spr);
            }
        }
    }
}
