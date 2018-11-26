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
        protected override void OnInit()
        {            
            Physics2DEventListener.Get(this.gameObject).onTriggerEnter2D += OnTrigger;                      
        }        

        private void OnTrigger(Collider2D obj)
        {
            Destroy();
        }
    }
}
