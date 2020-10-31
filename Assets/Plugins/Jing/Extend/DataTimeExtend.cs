using System;

namespace Jing
{
    /// <summary>
    /// 时间对象扩展方法
    /// </summary>
    public static class DataTimeExtend
    {
        static readonly DateTime UNIX_EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 以毫秒为单位，获取对象的UTC时间戳
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static long ToMillisecondUTC(this DateTime dataTime)
        {
            TimeSpan tn = dataTime - UNIX_EPOCH_TIME;
            return Convert.ToInt64(tn.TotalMilliseconds);
        }
    }
}
