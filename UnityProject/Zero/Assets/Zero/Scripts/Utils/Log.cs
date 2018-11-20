using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 日志打印
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 红色
        /// </summary>
        public const string COLOR_RED = "FF0000";

        /// <summary>
        /// 绿色
        /// </summary>
        public const string COLOR_GREEN = "00FF00";

        /// <summary>
        /// 蓝色 
        /// </summary>
        public const string COLOR_BLUE = "0000FF";

        /// <summary>
        /// 日志是否激活
        /// </summary>
        public static bool isActive = true;
        
        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="message"></param>
        public static void I(object message)
        {
            if(!isActive)
            {
                return;
            }
            Debug.Log(message);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        public static void I(string format, params object[] args)
        {
            if (!isActive)
            {
                return;
            }
            Debug.LogFormat(format, args);
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void CI(string color, string message)
        {
            if(null == message)
            {
                return;
            }

            message = string.Format("<color=#{0}>{1}</color>", color, message);
            I(message);
        }

        /// <summary>
        /// 打印彩色信息
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void CI(string color, string format, params object[] args)
        {
            if (null == format)
            {
                return;
            }

            var message = string.Format("<color=#{0}>{1}</color>", color, string.Format(format,args));
            I(message);
        }


        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(object message)
        {
            if (!isActive)
            {
                return;
            }
            Debug.LogWarning(message);
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(string format, params object[] args)
        {
            if (!isActive)
            {
                return;
            }
            Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(object message)
        {
            if (!isActive)
            {
                return;
            }
            Debug.LogError(message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(string format, params object[] args)
        {
            if (!isActive)
            {
                return;
            }
            Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        /// 在一个UI面板中显示一条日志消息
        /// </summary>
        /// <param name="content"></param>
        public static void Msg(string content)
        {
            if (!isActive)
            {
                return;
            }

            const string NAME = "LogMsg";
            GameObject logMsg = GameObject.Find(NAME);
            if (null == logMsg)
            {
                GameObject prefab = Resources.Load<GameObject>("zero/LogMsg");
                logMsg = GameObject.Instantiate(prefab);
                logMsg.name = NAME;
            }

            logMsg.GetComponent<LogMsg>().SetContent(content);
        }
    }
}