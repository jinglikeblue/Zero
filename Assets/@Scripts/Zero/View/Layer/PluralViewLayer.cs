using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace ZeroHot
{
    /// <summary>
    /// 复数的视图层，该层中的视图，可以同时存在
    /// </summary>
    public sealed class PluralViewLayer : AViewLayer
    {
        /// <summary>
        /// 自动优化功能是否启动(默认:false)
        /// </summary>
        public bool isAutoOptimize { get; private set; }

        public List<AView> viewList { get; }

        public PluralViewLayer(GameObject rootGameObject) : base(rootGameObject)
        {
            viewList = new List<AView>();
            isAutoOptimize = false;
        }

        protected override void BeforeShow()
        {
            
        }

        protected override void ShowView(AView view)
        {
            if (false == viewList.Contains(view))
            {
                view.onDestroyed += RemoveView;
                viewList.Add(view);                
                AutoOptimize();
            }
        }

        void RemoveView(AView view)
        {
            view.onDestroyed -= RemoveView;
            viewList.Remove(view);
            AutoOptimize();
        }

        public void ChangeSiblingIndex(AView view, int index)
        {
            view.gameObject.transform.SetSiblingIndex(index);            
            AutoOptimize();
        }

        /// <summary>
        /// 将ViewList的数据，按照SiblingIndex进行排序
        /// </summary>
        void SortViewListBySiblingIndex()
        {
            viewList.Sort((x,y)=> {
                int xIdx = x.gameObject.transform.GetSiblingIndex();
                int yIdx = y.gameObject.transform.GetSiblingIndex();
                return xIdx - yIdx;
            });
        }

        /// <summary>
        /// 设置自动优化开关
        /// <para>自动优化开启下，界面的矩形区域大小会影响下方的其它界面是否显示。所以矩形的大小是否准确以及界面透明区域，可能会对自动优化产生影响。</para>
        /// </summary>
        /// <param name="isAutoOptimize"></param>
        public void SetAutoOptimize(bool isAutoOptimize)
        {
            if (this.isAutoOptimize != isAutoOptimize)
            {
                this.isAutoOptimize = isAutoOptimize;
                if (false == isAutoOptimize)
                {
                    foreach (var view in viewList)
                    {
                        view.SetActive(true);
                    }
                }
                else
                {
                    AutoOptimize();
                }
            }
        }
        
        /// <summary>
        /// 自动优化，每一个视图，如果被上层的任意视图矩形区域完全覆盖，则自动SetActive(false)，节省渲染开销
        /// </summary>
        void AutoOptimize()
        {
            SortViewListBySiblingIndex();

            var lastIndex = viewList.Count - 1;
            if (isAutoOptimize && lastIndex > -1)
            {
                viewList[lastIndex].SetActive(true);
                var lastRT = viewList[lastIndex].GetComponent<RectTransform>();
                //如果被它完全压住的窗口，则隐藏
                for (int i = 0; i < lastIndex; i++)
                {
                    var compareRT = viewList[i].GetComponent<RectTransform>();
                    if (RectUtility.Contains(lastRT.rect, compareRT.rect))
                    {
                        compareRT.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 清理所有的视图
        /// </summary>
        public override void Clear()
        {
            var idx = viewList.Count;
            while (--idx > -1)
            {
                var view = viewList[idx];
                view.onDestroyed -= RemoveView;
                view.Destroy();
            }
            viewList.Clear();
        }
    }
}
