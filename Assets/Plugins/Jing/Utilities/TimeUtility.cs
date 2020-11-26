using System;

namespace Jing
{
    public class TimeUtility
    {
        static readonly DateTime UNIX_EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 以毫秒为单位当前UTC时间
        /// </summary>
        /// <returns></returns>
        public static long NowUtcMilliseconds
        {
            get
            {
                TimeSpan tn = DateTime.UtcNow - UNIX_EPOCH_TIME;
                return Convert.ToInt64(tn.TotalMilliseconds);                
            }
        }

        /// <summary>
        /// 以秒为单位当前UTC时间
        /// </summary>
        public static long NowUtcSeconds
        {
            get
            {
                TimeSpan tn = DateTime.UtcNow - UNIX_EPOCH_TIME;
                return Convert.ToInt64(tn.TotalSeconds);
            }
        }
    }
}
