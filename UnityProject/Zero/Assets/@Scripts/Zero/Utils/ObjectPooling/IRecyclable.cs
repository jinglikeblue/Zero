namespace ZeroHot
{
    public interface IRecyclable
    {
        /// <summary>
        /// 被回收了
        /// </summary>
        void Recycled();

        /// <summary>
        /// 被遗弃了
        /// </summary>
        void Discarded();
    }
}
