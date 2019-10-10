using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jing
{    
    public class MessageCenter
    {
        struct CacherVO
        {
            public Type type;
            public MethodInfo methodInfo;

            public CacherVO(Type type, MethodInfo methodInfo)
            {
                this.type = type;
                this.methodInfo = methodInfo;
            }
        }

        Dictionary<string, CacherVO> _dic;

        Type _baseCacherType;

        public MessageCenter()
        {
            _dic = new Dictionary<string, CacherVO>();
            _baseCacherType = typeof(AMessageCacher);
        }

        public void Register<T>(string id)
        {
            Register(id, typeof(T));
        }

        public void Register(string id, Type cacherType)
        {
            if (false == cacherType.IsSubclassOf(_baseCacherType))
            {
                throw new Exception(string.Format("无法为[Id: {0}]注册捕获器，因为[Type: {1}]不是[Jing.AMessageCacher]的子类!", id, cacherType.FullName));
            }

            var mi = cacherType.GetMethod(AMessageCacher.ON_CACHE_METHOD_NAME, BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic);
            if (null == mi)
            {
                throw new Exception(string.Format("无法为[Id: {0}]注册捕获器，因为[Type: {1}]没有实现方法[{2}]!", id, cacherType.FullName, AMessageCacher.ON_CACHE_METHOD_NAME));
            }

            _dic[id] = new CacherVO(cacherType, mi);
        }

        public void Unregister(string id)
        {            
            if (_dic.ContainsKey(id))
            {
                _dic.Remove(id);
            }
        }

        public void Trigger(string id, params object[] datas)
        {
            if (false == _dic.ContainsKey(id))
            {
                return;
            }
            var obj = Activator.CreateInstance(_dic[id].type);            
            _dic[id].methodInfo.Invoke(obj, datas);            
        }
    }
}
