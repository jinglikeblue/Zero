using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero.Edit
{
    public class DllPublishConfigVO 
    {
        /// <summary>
        /// 发布目录
        /// </summary>
        public string resDir = "./";

        /// <summary>
        /// 编译工具
        /// </summary>
        public string devenvPath;

        /// <summary>
        /// 开发目录
        /// </summary>
        public string ilDevelopDir;

        /// <summary>
        /// 项目目录
        /// </summary>
        public string ilProjDir;

        /// <summary>
        /// 项目csproj路径
        /// </summary>
        public string ilProjPath;

        /// <summary>
        /// 是否在发布DLL的时候自动拷贝代码
        /// </summary>
        public bool isAudoCopy;
    }
}