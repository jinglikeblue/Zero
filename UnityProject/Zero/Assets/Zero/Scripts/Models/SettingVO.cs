using System.Collections.Generic;

public struct SettingVO
{
    /// <summary>
    /// 客户端版本号数据
    /// </summary>
    public struct Client
    {
        /// <summary>
        /// 客户端版本
        /// </summary>
        public string version;

        /// <summary>
        /// 客户端地址
        /// </summary>
        public string url;
    }

    public Client client;

    /// <summary>
    /// 网络资源目录
    /// </summary>
    public string netResRoot;

    /// <summary>
    /// 启动资源组列表
    /// </summary>
    public string[] startupResGroups;

    /// <summary>
    /// 配置的参数
    /// </summary>
    public Dictionary<string,string> startupParams;

    /// <summary>
    /// 配置跳转，如果当前客户端版本号有匹配的资源，则使用对应的setting文件
    /// </summary>
    public Dictionary<string, string> settingJump;
}

