using Jing;
using System.IO;

namespace Zero
{
    /// <summary>
    /// 本地版本文件数据模型
    /// </summary>
    public class LocalResVerModel : ResVerModel
    {
        LocalDataModel _localDataModel;
        public LocalResVerModel(LocalDataModel localDataModel)
        {
            _localDataModel = localDataModel;
            _vo = _localDataModel.LocalResVO;
            if (_vo.items == null)
            {
                _vo.items = new ResVerVO.Item[0];
            }
            else
            {
                ConformingLocalRes();
            }
        }

        /// <summary>
        /// 检查本地的文件，是否存在或者版本号是否一致
        /// <para>PS：版本号一致需要MD5验证，开销较大，暂不加入</para>
        /// </summary>
        public void ConformingLocalRes()
        {
            foreach(var item in _vo.items)
            {
                var filePath = FileSystem.CombinePaths(Runtime.Ins.localResDir, item.name);
                if(!File.Exists(filePath))
                {
                    SetVer(item.name, "");
                }
            }
        }

        /// <summary>
        /// 设置文件版本号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public void SetVerAndSave(string name, string version)
        {
            SetVer(name, version);
            Save();
        }

        /// <summary>
        /// 移除指定文件的版本信息
        /// </summary>
        /// <returns>The ver.</returns>
        /// <param name="name">Name.</param>
        public void RemoveVerAndSave(string name)
        {
            RemoveVer(name);
            Save();
        }

        /// <summary>
        /// 清理所有版本信息
        /// </summary>
        public void ClearVerAndSave()
        {
            ClearVer();
            Save();
        }

        public void Save()
        {
            _localDataModel.LocalResVO = _vo;
        }

    }
}
