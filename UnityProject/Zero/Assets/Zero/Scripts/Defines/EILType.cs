namespace Zero
{
    /// <summary>
    /// 热更DLL的执行方式
    /// </summary>
    public enum EILType
    {
        /// <summary>
        /// ILRuntime框架
        /// </summary>
        IL_RUNTIME,
        /// <summary>
        /// 反射执行(IL2CPP下会自动切换为ILRuntime)
        /// </summary>
        REFLECTION
    }
}