using Sirenix.Utilities.Editor;
using System.Reflection;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// 配置面板
    /// </summary>
    class HotConfigEditorWin : AZeroMenuEditorWindow<HotConfigEditorWin>
    {
        protected override void OnEnable()
        {
            AddModule<HotConfigEditorSettingModule>("设置", EditorIcons.SettingsCog);

            var types = Assembly.GetAssembly(typeof(ZeroHot.Main)).GetTypes();
            foreach (var type in types)
            {
                var att = type.GetCustomAttribute<ZeroHotConfigAttribute>(false);
                if (null != att)
                {                    
                    menuTree.Add(att.label, new HotConfigModule(type, att.path, this), EditorIcons.File);
                }
            }
        }
    }


}
