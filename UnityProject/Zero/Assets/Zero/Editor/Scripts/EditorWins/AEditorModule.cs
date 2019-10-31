using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
