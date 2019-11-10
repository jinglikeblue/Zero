using Sirenix.OdinInspector;
using System;
using UnityEditor;
using LitJson;
using System.Text.RegularExpressions;
using System.Text;
using Jing;
using System.IO;
using Zero;

namespace ZeroEditor
{
    class HotConfigModule : AEditorModule
    {
        string _path;

        public HotConfigModule(Type type, string path, EditorWindow editorWin) : base(editorWin)
        {
            _path = FileSystem.CombinePaths(ZeroConst.HOT_CONFIGS_ROOT_DIR, path);

            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path, Encoding.UTF8);
                vo = JsonMapper.ToObject(type, json);
            }
            else
            {
                vo = Activator.CreateInstance(type);
            }
        }

        [ShowInInspector]
        [HideReferenceObjectPicker]
        public object vo;

        [LabelText("保存"), Button(size: ButtonSizes.Large), PropertyOrder(-1)]
        public void Save()
        {
            string json = JsonMapper.ToPrettyJson(vo);
            json = Regex.Unescape(json);
            var dir = Directory.GetParent(_path);
            if (!dir.Exists)
            {
                dir.Create();
            }
            File.WriteAllText(_path, json, Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}
