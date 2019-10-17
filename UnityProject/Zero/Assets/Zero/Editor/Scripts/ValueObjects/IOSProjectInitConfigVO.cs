using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zero.Edit
{
    /// <summary>
    /// iOS项目初始化配置
    /// </summary>
    public class IOSProjectInitConfigVO
    {
        public const string CONFIG_NAME = "ios_project_config.json";

        /// <summary>
        /// framework库配置
        /// </summary>
        public string[] frameworkToProjectList = new string[0];

        /// <summary>
        /// lib库配置
        /// </summary>
        public Dictionary<string, string> file2BuildList = new Dictionary<string, string>();

        /// <summary>
        /// build参数配置
        /// </summary>
        public Dictionary<string, string> buildPropertyList = new Dictionary<string, string>();

        /// <summary>
        /// info.plist参数配置
        /// </summary>
        public Dictionary<string, string> pListDataList = new Dictionary<string, string>();

        /// <summary>
        /// urlscheme配置
        /// </summary>
        public string[] urlSchemeList = new string[0];

        /// <summary>
        /// 信任urlscheme配置
        /// </summary>
        public string[] appQueriesSchemeList = new string[0];
    }
}
