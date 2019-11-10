using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// @Configs自动配置标签
    /// </summary>
    public class ZeroHotConfigAttribute : Attribute
    {
        public string label;

        public string path;

        public ZeroHotConfigAttribute(string label, string path)
        {
            this.label = label;
            this.path = path;
        }
    }
}