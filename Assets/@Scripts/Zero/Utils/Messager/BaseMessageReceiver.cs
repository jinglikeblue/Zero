namespace ZeroHot
{
    /// <summary>
    /// 消息接受者基类
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class BaseMessageReceiver<TMessage>: IMessageReceiver
    {
        /// <summary>
        /// 收到消息的抽象事件
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void OnReceive(TMessage m);
    }
}
