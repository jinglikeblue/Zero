using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZeroHot
{
    /// <summary>
    /// 消息发送者
    /// </summary>
    public class MessageDispatcher<TCode>
    {
        Dictionary<TCode, Type> _receiverDic;

        public MessageDispatcher()
        {
            _receiverDic = new Dictionary<TCode, Type>();
        }

        /// <summary>
        /// 注册接受者
        /// </summary>
        /// <typeparam name="TReceiver"></typeparam>
        /// <param name="code"></param>
        public void RegisterReceiver<TReceiver>(TCode code) where TReceiver : IMessageReceiver
        {
            var receiverType = typeof(TReceiver);
            RegisterReceiver(code, receiverType);
        }

        public void RegisterReceiver(TCode code, Type receiverType)
        {
            _receiverDic[code] = receiverType;
        }

        /// <summary>
        /// 注销接受者
        /// </summary>
        /// <param name="code"></param>
        public void UnregisterReceiver(TCode code)
        {
            if (_receiverDic.ContainsKey(code))
            {
                _receiverDic.Remove(code);
            }
        }

        /// <summary>
        /// 派送消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public EDispatchResult DispatchMessage(TCode code, object message)
        {
            if (_receiverDic.ContainsKey(code))
            {
                var receiverType = _receiverDic[code];
                var receiver = Activator.CreateInstance(receiverType);

                try
                {
                    var receiveMethod = receiverType.GetMethod("OnReceive", BindingFlags.NonPublic | BindingFlags.Instance);
                    var parameter = receiveMethod.GetParameters()[0];
                    if (parameter.ParameterType != message.GetType())
                    {
                        return EDispatchResult.WRONG_TYPE;
                    }
                    receiveMethod.Invoke(receiver, new object[] { message });
                    return EDispatchResult.SUCCESS;
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                    return EDispatchResult.RECEIVE_ERROR;
                }
            }
            else
            {
                return EDispatchResult.UNREGISTERED;
            }
        }
    }
}
