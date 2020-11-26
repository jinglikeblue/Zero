//该类由 [Zero → 自动生成代码 → Assets资源名生成] 工具自动创建
using System;
using UnityEngine;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 自动视图注册器
    /// </summary>
    class ViewAutoRegister
    {
        static ViewAutoRegister _ins;

        /// <summary>
        /// 是否已注册
        /// </summary>
        public static bool IsRegistered {
            get
            {
                return _ins == null ? false : true;
            }
        }

        /// <summary>
        /// 进行注册
        /// </summary>
        public static void Register()
        {
            if(null == _ins)
            {
                _ins = new ViewAutoRegister();                
            }
        }

        private ViewAutoRegister()
        {
            R("root_assets.ab", "ILContent");
            R("root_assets.ab", "MenuPanel");
            R("roushan.ab", "Block");
            R("roushan.ab", "GamePanel");
            R("roushan.ab", "GameStage");
            R("roushan.ab", "HelpWin");
            R("roushan.ab", "StartupPanel");
            R("cross_depend_test/a.ab", "A");
            R("cross_depend_test/a.ab", "C");
            R("cross_depend_test/b.ab", "B");
        }

        void R(string abName, string viewName)
        {
            var typeName = "ILDemo." + viewName;
            var t = Type.GetType(typeName);
            if (t != null)
            {
                ViewFactory.Register(abName, t.Name, t);
            }
            else
            {
                Debug.Log(Log.Orange("[{0}/{1}.prefab]没有找到匹配的类[{2}]", abName, viewName, typeName));
            }
        }
    }
}
