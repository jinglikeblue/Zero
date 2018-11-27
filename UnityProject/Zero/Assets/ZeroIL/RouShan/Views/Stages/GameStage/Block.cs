using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zero;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class Block : AView
    {
        protected override void OnEnable()
        {
            //自动获取gameObject上的Physics2DEventListener组件（没有的话会自动添加的），并添加TriggerEnter的委托方法
            Physics2DEventListener.Get(this.gameObject).onTriggerEnter2D += OnTrigger;
        }

        protected override void OnDisable()
        {
            //在不需要时移除委托，提高代码效率
            Physics2DEventListener.Get(this.gameObject).onTriggerEnter2D -= OnTrigger;
        }

        private void OnTrigger(Collider2D obj)
        {
            Destroy();
        }
    }
}
