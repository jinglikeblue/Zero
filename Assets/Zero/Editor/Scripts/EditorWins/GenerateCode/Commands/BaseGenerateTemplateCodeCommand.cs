namespace ZeroEditor
{
    abstract class BaseGenerateTemplateCodeCommand
    {
        /// <summary>
        /// 模板内容分隔符
        /// </summary>
        public const string TEMPLATE_SPLIT = "------------------------------Split--------------------------------";

        #region 替换标记
        public const string CLASS_NAME_FLAG = "[CLASS NAME]";
        public const string CLASS_LIST_FLAG = "[CLASS LIST]";
        public const string FIELD_LIST_FLAG = "[FIELD LIST]";
        public const string FIELD_NAME_FLAG = "[FIELD NAME]";
        public const string FIELD_VALUE_FLAG = "[FIELD VALUE]";
        public const string EXPLAIN_FLAG = "[EXPLAIN]";
        public const string NAMESPACE_FLAG = "[NAMESPACE]";
        #endregion

        public abstract void Excute();
    }


}
