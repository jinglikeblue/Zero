namespace ZeroHot
{
    /// <summary>
    /// 消息派送结果
    /// </summary>
    public enum EDispatchResult
    {
        /// <summary>
        /// 消息派送成功
        /// </summary>
        SUCCESS,

        /// <summary>
        /// 未注册的消息接受者
        /// </summary>
        UNREGISTERED,

        /// <summary>
        /// 消息数据类型不匹配
        /// </summary>
        WRONG_TYPE,

        /// <summary>
        /// 接受出错
        /// </summary>
        RECEIVE_ERROR,
    }
}
