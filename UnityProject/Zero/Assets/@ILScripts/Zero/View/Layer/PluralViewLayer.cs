using System.Collections.Generic;
using UnityEngine;

namespace IL.Zero
{
    /// <summary>
    /// 复数的视图层，该层中的视图，可以同时存在
    /// </summary>
    public sealed class PluralViewLayer : AViewLayer
    {
        public List<AView> ViewList { get; }

        public PluralViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {
            ViewList = new List<AView>();
        }

        public override void ShowView(AView view)
        {            
            if (false == ViewList.Contains(view))
            {
                view.onDestroyHandler += RemoveView;
                ViewList.Add(view);
            }
        }

        void RemoveView(AView view)
        {
            view.onDestroyHandler -= RemoveView;
            ViewList.Remove(view);           
        }

        public void ChangeSiblingIndex(AView view, int index)
        {
            view.gameObject.transform.SetSiblingIndex(index);
        }

        /// <summary>
        /// 清理所有的视图
        /// </summary>
        public override void Clear()
        {
            var idx = ViewList.Count;
            while(--idx > -1)
            {
                var view = ViewList[idx];
                view.onDestroyHandler -= RemoveView;
                view.Destroy();
            }
            ViewList.Clear();
        }
    }
}
