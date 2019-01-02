using System;

namespace IL.Zero
{
    /// <summary>
    /// 视图单元的数据
    /// </summary>
    public class ViewEntry
    {
        /// <summary>
        /// AB包名字
        /// </summary>
        public string abName;

        /// <summary>
        /// 视图名字
        /// </summary>
        public string viewName;

        /// <summary>
        /// 类型
        /// </summary>
        public Type type;

        public ViewEntry(string abName, string viewName, Type type)
        {
            this.abName = abName;
            this.viewName = viewName;
            this.type = type;
        }
    }
}
