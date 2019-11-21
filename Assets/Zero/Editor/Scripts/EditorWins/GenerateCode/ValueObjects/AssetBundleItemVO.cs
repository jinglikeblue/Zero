using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace ZeroEditor
{
    public struct AssetBundleItemVO
    {
        /// <summary>
        /// 注释
        /// </summary>
        [LabelText("添加注释")]
        public string explain;

        /// <summary>
        /// assetbundle名称
        /// </summary>
        [LabelText("$GetFieldName")]
        [DisplayAsString]
        public string assetbundle;

        [DisplayAsString]        
        [ListDrawerSettings(HideRemoveButton = true, HideAddButton = true, DraggableItems = false)]
        public List<string> assetList;

        public string GetFieldName()
        {            
            return assetbundle.Replace("/", "_").Replace(".ab", "").ToUpper();
        }
    }
}
