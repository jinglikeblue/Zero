using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace ZeroEditor
{
    public class AEditorModule
    {
        /// <summary>
        /// 关联的编辑器窗口
        /// </summary>
        [HideInEditorMode]
        public readonly EditorWindow editorWin;

        public AEditorModule(EditorWindow editorWin)
        {
            this.editorWin = editorWin;            
        }                

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="content"></param>
        public void ShowTip(string content)
        {
            editorWin.ShowTip(content);
        }

        public void ShowTip(string content, params object[] args) {
            ShowTip(string.Format(content, args));
        }
    }
}
