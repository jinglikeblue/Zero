namespace Jing
{
    public class StringUtil
    {
        /// <summary>
        /// 精简字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimUnusefulChar(string str)
        {
            char[] chars = str.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\0')
                {
                    str = new string(chars, 0, i);
                    break;
                }
            }
            return str;
        }
    }
}