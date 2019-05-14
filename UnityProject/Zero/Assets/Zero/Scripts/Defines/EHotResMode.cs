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
        NET_ASSET_BUNDLE,
        /// <summary>
        /// 从本地资源目录获取资源
        /// </summary>            
        LOCAL_ASSET_BUNDLE,
        /// <summary>
        /// 使用AssetDataBase接口加载资源
        /// </summary>
        ASSET_DATA_BASE,
    }
}