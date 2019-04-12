using System;
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
        public const string COLOR_RED = "A83131";

        /// <summary>
        /// 橙色 
        /// </summary>
        public const string COLOR_ORANGE = "DE4D08";

        /// <summary>
        /// 黄色
        /// </summary>
        public const string COLOR_YELLOW = "D5CB6C";

        /// <summary>
        /// 绿色
        /// </summary>
        public const string COLOR_GREEN = "33B1B0";

        /// <summary>
        /// 蓝色 
        /// </summary>
        public const string COLOR_BLUE = "2762BD";

        /// <summary>
        /// 紫色
        /// </summary>
        public const string COLOR_PURPLE = "865FC5";


        static bool _isActive = true;

        /// <summary>
        /// 日志是否激活
        /// </summary>
        public static bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                Debug.unityLogger.logEnabled = value;
            }
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="message"></param>
        public static void I(object message)
        {            
            if(!IsActive)
            {
                return;
            }
            Debug.Log(message);
        }

        public static void I(string message)
        {
            if (!IsActive)
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
            if (!IsActive)
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
        public static void CI(string color, object message)
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
            Debug.LogWarning(message);
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        public static void W(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        public static void E(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        public static void CGUI(string color, object content)
        {
            var message = string.Format("<color=#{0}>{1}</color>", color, content);
            GUI(message);
        }

        public static void CGUI(string color, string format, params object[] args)
        {
            var message = string.Format("<color=#{0}>{1}</color>", color, string.Format(format, args));
            GUI(message);
        }

        public static void GUI(string format, params object[] args)
        {
            GUI(string.Format(format, args));
        }

        /// <summary>
        /// 在一个UI面板中显示一条日志消息
        /// </summary>
        /// <param name="content"></param>
        public static void GUI(string content)
        {
            if (!IsActive)
            {
                return;
            }

            content = string.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss.fff"), content);
            I(content);
            GUILog.Show(content);
        }
    }
}