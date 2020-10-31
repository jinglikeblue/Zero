using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 原生代码的桥接器
    /// </summary>
    public class NativeBridge : ASingletonMonoBehaviour<NativeBridge>
    {      
        /// <summary>
        /// 接收来自原生层的消息
        /// </summary>
        /// <param name="json"></param>
        public void OnMessage(string json)
        {
            Debug.Log(Log.Zero1("收到Native的消息:\n{0}", json));
        }        
    }
}