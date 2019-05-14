namespace Zero
{
    /// <summary>
    /// 资源使用模式
    /// </summary>
    public enum EHotResMode
    {
        /// <summary>
        /// 从网络资源目录获取资源
        /// </summary>            
        NET,
        /// <summary>
        /// 从本地资源目录获取资源
        /// </summary>            
        LOCAL,
        /// <summary>
        /// 从Resources下直接获取资源（开发阶段使用）
        /// </summary>
        RESOURCES,
        /// <summary>
        /// 使用AssetDataBase接口加载资源
        /// </summary>
        ASSET_DATA_BASE,
    }
}