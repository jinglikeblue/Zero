using System.Collections.Generic;
using System.IO;

namespace Zero
{
    /// <summary>
    /// 本地数据模块
    /// </summary>
    public class LocalDataModel
    {
        const string FILE_NAME = "local_data.zero.json";
        /// <summary>
        /// 本地数据对象
        /// </summary>
        public struct VO
        {
            public bool isInit;
            public ResVerVO localResVO;
            public Dictionary<string, string> localValueDic;
        }

        VO _vo;
        string _path;

        public LocalDataModel()
        {
            _path = Runtime.Ins.localResDir + FILE_NAME;
            if (File.Exists(_path))
            {
                //读取已有的数据
                _vo = LitJson.JsonMapper.ToObject<VO>(File.ReadAllText(_path));
            }
            else
            {
                //新数据初始化
                _vo.isInit = false;
                _vo.localValueDic = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 保存数据到本地
        /// </summary>
        void Save2Local()
        {            
            string json = LitJson.JsonMapper.ToJson(_vo);
            File.WriteAllText(_path, json);
        }

        /// <summary>
        /// 程序是否初始化
        /// </summary>
        public bool IsInit
        {
            get
            {
                return _vo.isInit;
            }
            set
            {
                _vo.isInit = value;
                Save2Local();
            }
        }

        /// <summary>
        /// 本地数据版本对象
        /// </summary>
        public ResVerVO LocalResVO
        {
            get
            {
                return _vo.localResVO;
            }
            set
            {
                _vo.localResVO = value;
                Save2Local();
            }
        }

        /// <summary>
        /// 添加一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, string value)
        {
            _vo.localValueDic[key] = value;
            Save2Local();
        }

        /// <summary>
        /// 读取一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadValue(string key)
        {
            if (_vo.localValueDic.ContainsKey(key))
            {
                return _vo.localValueDic[key];
            }
            return key;
        }

        /// <summary>
        /// 移除一个存储到本地的字段
        /// </summary>
        /// <param name="key"></param>
        public void RemoveValue(string key)
        {
            if(_vo.localValueDic.ContainsKey(key))
            {
                _vo.localValueDic.Remove(key);
                Save2Local();
            }
        }

        /// <summary>
        /// 清除所有存储到本地的字段
        /// </summary>
        public void ClearValues()
        {
            _vo.localValueDic.Clear();
            Save2Local();
        }
    }
}
