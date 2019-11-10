using Jing;
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
        public class VO
        {
            /// <summary>
            /// 是否APP已初始化
            /// </summary>
            public bool isInit = false;

            /// <summary>
            /// 是否更新Setting文件
            /// </summary>
            public bool isUpdateSetting = true;

            public Dictionary<string, string> localValueDic = new Dictionary<string, string>();
        }

        VO _vo;
        string _path;

        public LocalDataModel()
        {
            _path = FileSystem.CombinePaths(Runtime.Ins.generateFilesDir, FILE_NAME);

            if (File.Exists(_path))
            {
                //读取已有的数据
                _vo = LitJson.JsonMapper.ToObject<VO>(File.ReadAllText(_path));
            }
            else
            {
                //新数据初始化
                _vo = new VO();                
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
        /// 是否更新setting文件
        /// </summary>
        public bool IsUpdateSetting
        {
            get
            {
                return _vo.isUpdateSetting;
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
            return null;
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
