using System.Collections.Generic;
using UnityEngine;

namespace IL.Zero
{
    /// <summary>
    /// 复数的视图层，该层中的视图，可以同时存在
    /// </summary>
    public sealed class PluralViewLayer : AViewLayer
    {
        List<AView> _viewList = new List<AView>();

        public PluralViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {
        }

        public override void ShowView(AView view)
        {            
            if (false == _viewList.Contains(view))
            {
                view.onDestroyHandler += OnViewDestroy;
                _viewList.Add(view);
            }
        }

        void OnViewDestroy(AView view)
        {
            view.onDestroyHandler -= OnViewDestroy;
            _viewList.Remove(view);           
        }

        public void ChangeSiblingIndex(AView view, int index)
        {
            view.gameObject.transform.SetSiblingIndex(index);
        }

        /// <summary>
        /// 清理所有的视图
        /// </summary>
        public void Clear()
        {
            foreach(var view in _viewList)
            {
                view.onDestroyHandler -= OnViewDestroy;
            }
            _viewList.Clear();
        }
    }
}
