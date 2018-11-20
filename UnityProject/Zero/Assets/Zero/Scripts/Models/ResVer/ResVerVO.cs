using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源版本号数据
    /// </summary>
    public struct ResVerVO
    {
        /// <summary>
        /// Manifest的AB文件资源名称
        /// </summary>
        public string manifestName;

        /// <summary>
        /// 资源项
        /// </summary>
        public struct Item
        {
            /// <summary>
            /// 资源名称
            /// </summary>
            public string name;

            /// <summary>
            /// 版本号
            /// </summary>
            public string version;

            /// <summary>
            /// 依赖的资源
            /// </summary>
            //public string[] depends;
        }

        public Item[] items;        
    }
}