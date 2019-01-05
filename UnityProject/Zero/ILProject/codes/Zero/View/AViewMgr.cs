using System;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace IL.Zero
{
    public abstract class AViewMgr
    {
        /// <summary>
        /// 销毁视图对象，同时会回收对应的资源（包含依赖）
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="isGC">是否执行垃圾回收</param>
        static public void DestroyView(AView view)
        {
            //var type = view.GetType();
            //var entry = GetEntry(type);
            //ResMgr.Ins.Unload(entry.abName, false, true);



            //if (null != view.ViewEntry && null != view.ViewEntry.abName)
            //{
            //    if (_ab2view2EntryDic.ContainsKey(view.ViewEntry.abName))
            //    {
            //        ResMgr.Ins.Unload(view.ViewEntry.abName, false, true);
            //    }
            //}
            //GameObject.Destroy(view.gameObject);
        }

        protected Transform _root;

        /// <summary>
        /// 是否已经初始化了
        /// </summary>
        public bool IsInited
        {
            get
            {
                return _root == null ? false : true;
            }
        }

        public virtual void Init(Transform root)
        {
            _root = root;
        }

        

      

        
    }
}
