using System.Collections.Generic;
using UnityEngine;

namespace ILZero
{
    /// <summary>
    /// 复数的视图层，该层中的视图，可以同时存在
    /// </summary>
    public sealed class PluralViewLayer : AViewLayer
    {
        List<AView> _viewList;
        public List<AView> ViewList
        {
            get
            {
                return _viewList;
            }
        }

        public PluralViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {
            _viewList = new List<AView>();
        }

        protected override void BeforeShow()
        {
            Clear();
        }

        protected override void ShowView(AView view)
        {
            if (false == _viewList.Contains(view))
            {
                view.onDestroyHandler += RemoveView;
                _viewList.Add(view);
            }
        }

        void RemoveView(AView view)
        {
            view.onDestroyHandler -= RemoveView;
            _viewList.Remove(view);
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
            var idx = _viewList.Count;
            while (--idx > -1)
            {
                var view = _viewList[idx];
                view.onDestroyHandler -= RemoveView;
                view.Destroy();
            }
            _viewList.Clear();
        }


    }
}
