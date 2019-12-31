using System;

namespace Zero
{
    /// <summary>
    /// @Configs自动配置标签，添加该标记的对象会显示在 「Zero -> 配置文件编辑」中
    /// </summary>    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ZeroHotConfigAttribute : Attribute
    {
        /// <summary>
        /// 标签的名称
        /// </summary>
        public string label { get; private set; }

        /// <summary>
        /// 配置存储的位置，相对于@Configs目录
        /// </summary>
        public string path { get; private set; }

        public ZeroHotConfigAttribute(string label, string path)
        {
            this.label = label;
            this.path = path;
        }
    }
}