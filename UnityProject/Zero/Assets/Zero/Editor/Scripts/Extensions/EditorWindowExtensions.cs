namespace ZeroEditor
{
    public static class EditorWindowExtensions
    {
        /// <summary>
        /// 显示一个Tip信息
        /// </summary>
        /// <param name="thisRef"></param>
        /// <param name="content"></param>
        public static void ShowTip(this UnityEditor.EditorWindow thisRef, string content)
        {            
            thisRef.ShowNotification(new UnityEngine.GUIContent(content));
        }
    }
}
